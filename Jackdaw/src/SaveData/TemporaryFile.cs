using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Creates a file in the system temp folder that gets automatically deleted when disposed. <br/>
/// Has support for auto-copying to a permanent file when closed.
/// </summary>
public sealed class TemporaryFile : IDisposable {
    /// <summary>
    /// The file that will be copied to when disposed. If empty, the data will not be copied.
    /// </summary>
    public readonly string OutPath = string.Empty;

    /// <summary>
    /// The temporary file's path.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// If the file will copy its data when disposed.
    /// </summary>
    public bool HasOutput => OutPath != string.Empty;

    /// <summary>
    /// Create a temporary file.
    /// </summary>
    public TemporaryFile() {
        Name = Create();
    }

    /// <summary>
    /// Create a temporary file that saves to a permanent file.
    /// </summary>
    /// <param name="path">The path to save to when closed.</param>
    /// <param name="copyCurrent">If the temp file should copy the current data from the output file, if any exists.</param>
    public TemporaryFile(string path, bool copyCurrent = false) : this() {
        OutPath = path;
        if (copyCurrent) { CopyFromOutPath(); }
    }

    void CopyFromOutPath() {
        if (!File.Exists(OutPath)) { return; }
        using FileStream from = File.Open(OutPath, FileMode.Open);
        using FileStream to = File.Open(Name, FileMode.Open);
        from.CopyTo(to);
    }

    static string Create() => Path.GetTempFileName();

    public void Dispose() {
        try {
            if (HasOutput) { File.Move(Name, OutPath, true); }
            else { File.Delete(Name); }
        } catch (Exception e) {
            Log.Warning($"TemporaryFile: Unable to delete temporary file for {OutPath}, exception {e.Message}");
        }
    }
}