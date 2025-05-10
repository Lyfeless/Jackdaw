using System.Text.Json.Serialization;

namespace LittleLib.Loader.LDTK;

public class TilesetSaveDefinition {
    [JsonPropertyName("uid")]
    public int NameID { get; set; }

    [JsonPropertyName("relPath")]
    public string TexturePath { get; set; } = string.Empty;

    public string TextureID {
        get {
            string newPath = Path.Join(
                //! FIXME (Alex): This is a bit jank, look into a better way to handle this
                Path.GetDirectoryName(TexturePath.Remove(0, Assets.TextureFolder.Length + 1)),
                Path.GetFileNameWithoutExtension(TexturePath)
            ).Replace("\\", "/");

            return newPath;
        }
    }

    [JsonPropertyName("__cWid")]
    public int TileCountX { get; set; }

    [JsonPropertyName("__cHei")]
    public int TileCountY { get; set; }

    [JsonPropertyName("tileGridSize")]
    public int GridSize { get; set; }

    [JsonPropertyName("padding")]
    public int Padding { get; set; }

    [JsonPropertyName("enumTags")]
    public TileTypeSaveTag[] TileTypes { get; set; } = [];

    public bool HasAnyTags(int tileID, string[] tags) {
        foreach (TileTypeSaveTag tag in TileTypes) {
            if (tags.Contains(tag.Value) && tag.tileIDs.Contains(tileID)) {
                return true;
            }
        }

        return false;
    }
}