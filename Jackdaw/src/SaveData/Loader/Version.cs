using System.Text.Json.Nodes;

namespace Jackdaw;

internal interface ISaveDataFileVersion {
    public SaveData LoadBinary(SaveData savedata, BinaryReader reader, bool skippedVersion = true);
    public SaveData LoadJson(SaveData savedata, JsonNode rootNode);

    public void SaveBinary(SaveData savedata);
    public void SaveJson(SaveData savedata);
}