using System.Text.Json;
using Foster.Framework;

namespace LittleLib.Loader.LDTK;

public class LDTKLoader {
    const string EntityLayerDescriptor = "Entities";
    const string IntGridLayerDescriptor = "IntGrid";
    const string TileLayerDescriptor = "Tiles";
    const string AutoLayerDescriptor = "AutoLayer";

    record class LayerTileStack(List<TileSaveData> Tiles);

    readonly LittleGame Game;
    readonly string LevelFolderPath;

    readonly Dictionary<int, LDTKTileset> Tilesets = [];
    readonly Dictionary<int, LayerSaveDefinition> LayerDefinitions = [];

    readonly Dictionary<string, LevelSaveReference> Levels = [];
    public string[] LevelNames => [.. Levels.Keys];

    readonly Dictionary<string, Action<Actor, EntitySaveData>> ActorRegistry = [];

    public LDTKLoader(LittleGame game, string path) {
        Game = game;
        LevelFolderPath = Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

        if (!File.Exists(path)) { return; }
        WorldSaveData? data = JsonSerializer.Deserialize(File.ReadAllText(path), LDTKSourceGenerationContext.Default.WorldSaveData);
        if (data == null) { return; }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            Tilesets.Add(
                tileset.DefinitionID,
                new(
                    identifier: tileset.Identifier,
                    atlas: GetTilesetTexture(tileset),
                    tileCount: new(tileset.TileCountX, tileset.TileCountY),
                    tileSize: tileset.GridSize,
                    customData: tileset.CustomData
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

    public Actor Load(string name) {
        if (!Levels.TryGetValue(name, out LevelSaveReference? levelRef)) { throw new Exception("Attempt to access unidentified level"); }
        LevelSaveData levelData = JsonSerializer.Deserialize(File.ReadAllText(Path.Join(LevelFolderPath, $"{levelRef.NameID}.ldtkl")), LDTKSourceGenerationContext.Default.LevelSaveData) ?? throw new Exception("Failed to load level");

        Actor levelRoot = new(Game);
        levelRoot.Match.Name = $"Level_{levelRef.NameID}";

        Point2 levelPosition = new(levelRef.X, levelRef.Y);
        Point2 levelSize = new(levelRef.Width, levelRef.Height);

        List<Actor> layers = [];

        foreach (LayerSaveData layerData in levelData.Layers) {
            LayerSaveDefinition layerDefinition = LayerDefinitions[layerData.LayerDefinitionID];

            Actor newLayer = new(Game);
            newLayer.Match.Name = layerDefinition.Identifier ?? layerData.InstanceID;
            newLayer.Match.Guid = new Guid(layerData.InstanceID);
            newLayer.Position.Set(layerData.OffsetX, layerData.OffsetY);
            if (!layerData.Visible) {
                newLayer.Ticking = false;
                newLayer.Visible = false;
            }

            switch (layerData.Type) {
                case EntityLayerDescriptor: {
                        foreach (EntitySaveData entityData in layerData.Entities) {
                            if (!ActorRegistry.ContainsKey(entityData.NameID)) {
                                Console.WriteLine($"LDTKLoader: Unhandled entity creation for {entityData.NameID}, no generator defined");
                                continue;
                            }
                            Actor newEntity = new(Game);
                            newEntity.Match.Name = entityData.InstanceID;
                            newEntity.Match.Guid = new Guid(entityData.InstanceID);
                            newEntity.Position.Set(new(entityData.Position[0], entityData.Position[1]));
                            ActorRegistry[entityData.NameID](newEntity, entityData);

                            newLayer.Children.Add(newEntity);
                        }
                    }
                    break;
                case IntGridLayerDescriptor:
                case TileLayerDescriptor:
                case AutoLayerDescriptor: {
                        if (layerData.Tileset == null) {
                            Console.WriteLine($"LDTKLoader: Unable to load layer {newLayer.Match.Name}, no tileset assigned");
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

    public LDTKLoader RegisterActor(string id, Action<Actor, EntitySaveData> func) {
        if (ActorRegistry.ContainsKey(id)) { throw new Exception($"LDTKLoader: Attempting to re-define actor id {id}"); }
        ActorRegistry.Add(id, func);
        return this;
    }

    Subtexture GetTilesetTexture(TilesetSaveDefinition tileset) {
        string path = Path.Join(
                //! FIXME (Alex): This is a bit jank, look into a better way to handle this
                Path.GetDirectoryName(tileset.TexturePath[(Game.Assets.Config.TextureFolder.Length + 1)..]),
                Path.GetFileNameWithoutExtension(tileset.TexturePath)
            ).Replace("\\", "/");

        return Game.Assets.GetTexture(path);
    }
}