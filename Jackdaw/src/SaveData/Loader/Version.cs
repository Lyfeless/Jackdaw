using System.Text.Json;
using System.Text.Json.Nodes;

namespace Jackdaw;

internal interface ISaveDataFileVersion {
    public SaveData LoadBinary(SaveData savedata, BinaryReader reader, bool skippedVersion = true);
    public SaveData LoadJSON(SaveData savedata, JsonNode rootNode);

    public void SaveBinary(SaveData savedata, BinaryWriter writer);
    public void SaveJson(SaveData savedata, Utf8JsonWriter writer);
}