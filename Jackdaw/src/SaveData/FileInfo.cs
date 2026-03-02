namespace Jackdaw;

/// <summary>
/// Information about a save's options for loading from file.
/// </summary>
/// <param name="hasSingle">If the save has a file with an exact matching name.</param>
/// <param name="backupCount">The number of backup files a save has.</param>
public readonly struct SaveFileInfo(bool hasSingle, int backupCount) {
    /// <summary>
    /// If the save has a file with an exact matching name.
    /// </summary>
    public readonly bool HasSingle = hasSingle;

    /// <summary>
    /// If the save has timestamped backup files with the same name.
    /// </summary>
    public readonly bool HasBackups = backupCount > 0;

    /// <summary>
    /// How many timestamped backup files exist for the save.
    /// </summary>
    public readonly int BackupCount = backupCount;
}