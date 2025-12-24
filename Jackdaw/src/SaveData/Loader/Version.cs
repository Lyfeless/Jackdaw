using System.Text.Json.Nodes;

namespace Jackdaw;

internal interface ISaveDataFileVersion {
    public SaveData LoadBinary(SaveData savedata, BinaryReader reader, bool skippedVersion = true);
    public SaveData LoadJSON(SaveData savedata, JsonNode rootNode);

    public void SaveBinary(SaveData savedata, string savePath);
    public void SaveJson(SaveData savedata, string savePath);
}