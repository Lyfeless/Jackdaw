using System.Text.Json;
using Foster.Framework;

namespace LittleLib.Loader.LDTK;

public class LDTKLoader {
    static readonly Dictionary<int, TilesetSaveDefinition> tilesets = [];

    static readonly Dictionary<string, LevelSaveReference> levels = [];
    public static string[] LevelNames => [.. levels.Keys];

    readonly Dictionary<string, Func<EntitySaveData, Actor>> ActorRegistry = [];

    public LDTKLoader(string path) {
        if (!File.Exists(path)) { return; }
        WorldSaveData? data = JsonSerializer.Deserialize(File.ReadAllText(path), LDTKSourceGenerationContext.Default.WorldSaveData);
        if (data == null) { return; }

        foreach (TilesetSaveDefinition tileset in data.Definitions.Tilesets) {
            tilesets.Add(tileset.NameID, tileset);
        }

        foreach (LevelSaveReference levelRef in data.Levels) {
            levels.Add(levelRef.NameID, levelRef);
        }
    }

    public Actor Load(LittleGame game, string name) {
        if (!levels.TryGetValue(name, out LevelSaveReference? levelRef)) { throw new Exception("Attempt to access unidentified level"); }
        LevelSaveData levelData = JsonSerializer.Deserialize(File.ReadAllText($"./Content/Levels/{levelRef.NameID}.ldtkl"), LDTKSourceGenerationContext.Default.LevelSaveData) ?? throw new Exception("Failed to load level");

        Actor levelRoot = new(game);
        levelRoot.Match.Name = $"Level_{levelRef.NameID}";

        Point2 levelPosition = new(levelRef.X, levelRef.Y);
        Point2 levelSize = new(levelRef.Width, levelRef.Height);

        foreach (LayerSaveData layerData in levelData.Layers) {
            switch (layerData.Type) {
                case "Entities": {
                        Actor newEntityLayer = new(game);

                        foreach (EntitySaveData entityData in layerData.Entities) {
                            if (!ActorRegistry.ContainsKey(entityData.NameID)) {
                                Console.WriteLine($"LDTKLoader: Unhandled entity creation for {entityData.NameID}, no generator defined");
                            }
                            Actor newActor = ActorRegistry[entityData.NameID](entityData);
                            newActor.Match.Name = entityData.InstanceID;
                            //! FIXME (Alex): Set Guid?

                            newEntityLayer.Children.Add(newActor);
                        }

                        levelRoot.Children.Add(newEntityLayer);
                    }
                    break;
                case "IntGrid":
                case "Tiles":
                case "AutoLayer": {
                        //! FIXME (Alex): Handle Tile Layer
                    }
                    break;
            }
        }

        return Actor.Invalid;
    }

    public LDTKLoader RegisterActor(string id, Func<EntitySaveData, Actor> func) {
        if (ActorRegistry.ContainsKey(id)) { throw new Exception($"LDTKLoader: Attempting to re-define actor id {id}"); }
        ActorRegistry.Add(id, func);
        return this;
    }
}