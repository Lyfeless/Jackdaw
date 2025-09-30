using System.Text.Json;
using Foster.Framework;

namespace Jackdaw.Loader.LDTK;

/// <summary>
/// Imports files created by the LDTK level editor, automatically converting them to usable actor node structures. <br/>
/// Relies on LDTK's option to split levels into seperate files to avoid loading everything upfront.
/// In the LDTK project settings select the options "Save levels to seperate files" to get a subfolder of all levels.
/// </summary>
public class LDTKLoader {
    const string EntityLayerDescriptor = "Entities";
    const string IntGridLayerDescriptor = "IntGrid";
    const string TileLayerDescriptor = "Tiles";
    const string AutoLayerDescriptor = "AutoLayer";

    record class LayerTileStack(List<TileSaveData> Tiles);

    readonly Game Game;
    readonly string LevelFolderPath;

    readonly Dictionary<int, LDTKTileset> Tilesets = [];
    readonly Dictionary<int, LayerSaveDefinition> LayerDefinitions = [];

    readonly Dictionary<string, LevelSaveReference> Levels = [];

    /// <summary>
    /// All valid loadable level names.
    /// </summary>
    public string[] LevelNames => [.. Levels.Keys];

    readonly Dictionary<string, Action<Actor, EntitySaveData>> ActorRegistry = [];

    /// <summary>
    /// Create a loader from an LDTK level save file.
    /// </summary>
    /// <param name="game">The current game instance.</param>
    /// <param name="path">The path to the root .ldtk file, relative to the program directory.</param>
    /// <param name="collisionTagFunc">A callback function to convert LDTK enum strings into collision tags for tile info.</param>
    public LDTKLoader(Game game, string path, Func<string, int?> collisionTagFunc) {
        Game = game;
        LevelFolderPath = Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

        if (!File.Exists(path)) { return; }
        WorldSaveData? data = JsonSerializer.Deserialize(File.ReadAllText(path), LDTKSourceGenerationContext.Default.WorldSaveData);
        if (data == null) { return; }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            Tilesets.Add(
                tileset.DefinitionID,
                new(
                    Game,
                    identifier: tileset.Identifier,
                    atlas: GetTilesetTexture(tileset),
                    tileCount: new(tileset.TileCountX, tileset.TileCountY),
                    tileSize: tileset.GridSize,
                    enumTags: tileset.TileTypes,
                    customData: tileset.CustomData,
                    collisionTagFunc: collisionTagFunc
                )
            );
        }

        foreach (LayerSaveDefinition layer in data.Definitions.Layers) {
            LayerDefinitions.Add(layer.DefinitionID, layer);
        }

        foreach (LevelSaveReference levelRef in data.Levels) {
            Levels.Add(levelRef.NameID, levelRef);
        }
    }

    /// <summary>
    /// Create a new instance of save data.
    /// </summary>
    /// <param name="name">Name ID of level to load.</param>
    /// <returns>Level instance.</returns>
    public Actor? Load(string name) {
        if (!Levels.TryGetValue(name, out LevelSaveReference? levelRef)) {
            Log.Warning($"LDTKLoader: Level name {name} not found, load unsuccessful");
            return null;
        }

        LevelSaveData? levelData;
        try {
            levelData = JsonSerializer.Deserialize(File.ReadAllText(Path.Join(LevelFolderPath, $"{levelRef.NameID}.ldtkl")), LDTKSourceGenerationContext.Default.LevelSaveData);
        } catch {
            Log.Warning($"LDTKLoader: An error occured while trying to load {name}, load unsuccessful");
            return null;
        }

        if (levelData == null) {
            Log.Warning($"LDTKLoader: An error occured while trying to load {name}, load unsuccessful");
            return null;
        }

        Actor levelRoot = new(Game);
        levelRoot.Match.Name = levelRef.NameID;
        levelRoot.Position.LocalPosition = new(levelRef.X, levelRef.Y);
        // Point2 levelSize = new(levelRef.Width, levelRef.Height);

        List<Actor> layers = [];

        foreach (LayerSaveData layerData in levelData.Layers) {
            LayerSaveDefinition layerDefinition = LayerDefinitions[layerData.LayerDefinitionID];

            Actor newLayer = new(Game);
            newLayer.Match.Name = layerDefinition.Identifier ?? layerData.InstanceID;
            newLayer.Match.Guid = new Guid(layerData.InstanceID);
            newLayer.Position.LocalPosition = new(layerData.OffsetX, layerData.OffsetY);
            if (!layerData.Visible) {
                newLayer.Ticking = false;
                newLayer.Visible = false;
            }

            switch (layerData.Type) {
                case EntityLayerDescriptor: {
                        foreach (EntitySaveData entityData in layerData.Entities) {
                            if (!ActorRegistry.ContainsKey(entityData.NameID)) {
                                Log.Warning($"LDTKLoader: Unhandled entity creation for {entityData.NameID}, no generator defined");
                                continue;
                            }
                            Actor newEntity = new(Game);
                            newEntity.Match.Name = entityData.InstanceID;
                            newEntity.Match.Guid = new Guid(entityData.InstanceID);
                            newEntity.Position.LocalPosition = new(entityData.Position[0], entityData.Position[1]);
                            ActorRegistry[entityData.NameID](newEntity, entityData);

                            newLayer.Children.Add(newEntity);
                        }
                    }
                    break;
                case IntGridLayerDescriptor:
                case TileLayerDescriptor:
                case AutoLayerDescriptor: {
                        if (layerData.Tileset == null) {
                            Log.Warning($"LDTKLoader: Unable to load layer {newLayer.Match.Name}, no tileset assigned");
                            continue;
                        }
                        LDTKTileset tileset = Tilesets[(int)layerData.Tileset];
                        LDTKTileLayer tiles = new(Game, tileset, new(layerData.Width, layerData.Height), layerData.TileSize, new(layerData.OffsetX, layerData.OffsetY));
                        foreach (TileSaveData tile in layerData.Tiles) {
                            tiles.AddTileStackLocal(tileset.GetTileCoord(new Point2(tile.Source[0], tile.Source[1])), new(tile.Position[0], tile.Position[1]));
                        }
                        newLayer.Components.Add(tiles);
                    }
                    break;
            }

            layers.Add(newLayer);
        }

        // Add in reverse order, ldtk renders backwards compared to the engine
        for (int i = layers.Count - 1; i >= 0; --i) {
            levelRoot.Children.Add(layers[i]);
        }

        return levelRoot;
    }

    /// <summary>
    /// Register a conversion function to create new actors out of LDTK entity data. </br>
    /// Actor registrations pass in a premade actor set up with its position and other base information.
    /// </summary>
    /// <param name="id">The LDTK entity identifier.</param>
    /// <param name="func">A conversion function to create an actor from the LDTK data.</param>
    /// <returns>The loader instance</returns>
    public LDTKLoader RegisterActor(string id, Action<Actor, EntitySaveData> func) {
        if (ActorRegistry.ContainsKey(id)) {
            Log.Warning($"LDTKLoader: Attempting to re-define actor id {id}, skipping.");
            return this;
        }
        ActorRegistry.Add(id, func);
        return this;
    }

    Subtexture GetTilesetTexture(TilesetSaveDefinition tileset) {
        string path = Path.Join(
                Path.GetDirectoryName(tileset.TexturePath[(Game.Assets.Config.TextureFolder.Length + 1)..]),
                Path.GetFileNameWithoutExtension(tileset.TexturePath)
            ).Replace("\\", "/");

        return Game.Assets.GetTexture(path);
    }

    /// <summary>
    /// Get the LDTK field data of a level.
    /// </summary>
    /// <param name="name">Name ID of level to load.</param>
    /// <returns>An array of data fields for the level.</returns>
    public FieldSaveData[] GetLevelFieldData(string name) {
        if (!Levels.TryGetValue(name, out LevelSaveReference? levelRef)) {
            Log.Warning($"Unable to find level {name}, no field data to return");
            return [];
        }
        return levelRef.Fields;
    }
}