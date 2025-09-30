using FMOD.Studio;
using Foster.Framework;

namespace Jackdaw.Audio.FMODAudio;

/// <summary>
/// A component responsible for loading and playing sound files from FMOD bank data.
/// Runs setup and update once it's added to the main actor tree.
/// </summary>
/// <param name="game">The current game instance.</param>
/// <param name="directory">The file path to load the bank files from, relative to the game's content root folder.</param>
public class AudioManager(Game game, string directory = "") : Component(game) {
    const string BANK_EXTENSION = ".bank";
    const string STRINGS_EXTENSION = ".strings.bank";

    readonly string BankPath = directory;

    FMOD.Studio.System FMODInstance;
    readonly List<Bank> banks = [];
    readonly Dictionary<string, EventDescription> events = [];
    readonly Dictionary<string, Bus> buses = [];

    protected override void EnterTreeFirst() {
        FMOD.Studio.System.create(out FMODInstance);
        FMODInstance.initialize(1024, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);

        string path = Path.Join(Game.Assets.Config.RootFolder, BankPath);
        if (Path.Exists(path)) {
            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(STRINGS_EXTENSION))) {
                LoadBank(file);
            }

            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(BANK_EXTENSION) && !e.EndsWith(STRINGS_EXTENSION))) {
                LoadBank(file);
            }
        }
    }

    void LoadBank(string bank) {
        FMODInstance.loadBankFile(bank, LOAD_BANK_FLAGS.NORMAL, out Bank bankData);
        banks.Add(bankData);

        bankData.getEventList(out EventDescription[] bankEvents);
        foreach (EventDescription bankEvent in bankEvents) {
            bankEvent.getPath(out string eventPath);
            events.Add(eventPath, bankEvent);
        }

        bankData.getBusList(out Bus[] bankBuses);
        foreach (Bus bankBus in bankBuses) {
            bankBus.getPath(out string busPath);
            buses.Add(busPath, bankBus);
        }
    }

    protected override void Invalidated() {
        FMODInstance.release();
        banks.Clear();
        events.Clear();
        buses.Clear();
    }

    protected override void Update() {
        FMODInstance.update();
    }

    /// <summary>
    /// Get the volume of a sound bus.
    /// </summary>
    /// <param name="bus">The bus to query.</param>
    /// <returns>The volume of the bus, or 1 if no matching bus is found.</returns>
    public float GetBusVolume(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return 1; }
        value.getVolume(out float volume);
        return volume;
    }

    /// <summary>
    /// Set the volume of a sound bus.
    /// </summary>
    /// <param name="bus">The bus to set.</param>
    /// <param name="volume">The volume the bus should be set to.</param>
    public void SetBusVolume(string bus, float volume) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setVolume(Calc.Clamp(volume, 0, 1));
    }

    /// <summary>
    /// Get the current paused state of the bus.
    /// </summary>
    /// <param name="bus">The bus to query.</param>
    /// <returns>If the bus is currently paused, or false if no matching bus is found.</returns>
    public bool GetBusPause(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return false; }
        value.getPaused(out bool paused);
        return paused;
    }

    /// <summary>
    /// Pause a sound bus if it's current playing.
    /// </summary>
    /// <param name="bus">The bus to pause.</param>
    public void PauseBus(string bus) => SetBusPause(bus, true);

    /// <summary>
    /// Unpause a sound bus if it's currently paused.
    /// </summary>
    /// <param name="bus">The bus to unpause.</param>
    public void UnpauseBus(string bus) => SetBusPause(bus, false);

    /// <summary>
    /// Set the pause state of a sound bus.
    /// </summary>
    /// <param name="bus">The bus to set.</param>
    /// <param name="paused">The pause state.</param>
    public void SetBusPause(string bus, bool paused) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setPaused(paused);
    }

    /// <summary>
    /// Get the current mute state of a sound bus.
    /// </summary>
    /// <param name="bus">The bus to query.</param>
    /// <returns>If the bus is muted.</returns>
    public bool GetBusMute(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return false; }
        value.getMute(out bool muted);
        return muted;
    }

    /// <summary>
    /// Mute a sound bus.
    /// </summary>
    /// <param name="bus">The bus to mute.</param>
    public void MuteBus(string bus) => SetBusMute(bus, true);

    /// <summary>
    /// Unmute a sound bus.
    /// </summary>
    /// <param name="bus">The bus to unmute.</param>
    public void UnMuteBus(string bus) => SetBusMute(bus, false);

    /// <summary>
    /// Set the mute state of a sound bus.
    /// </summary>
    /// <param name="bus">The bus to set.</param>
    /// <param name="muted">The mute state.</param>
    public void SetBusMute(string bus, bool muted) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setMute(muted);
    }

    /// <summary>
    /// Stop all currently playing sounds on all sound buses.
    /// </summary>
    /// <param name="immediate">If the sounds should be allowed to fade out.</param>
    public void StopAll(bool immediate = false) {
        if (!FMODInstance.isValid()) { return; }
        foreach (Bus bus in buses.Values) { StopBus(bus, immediate); }
    }

    /// <summary>
    /// Stop all sounds on a single sound bus.
    /// </summary>
    /// <param name="name">The bus to stop all sounds on.</param>
    /// <param name="immediate">If the sounds should be allowed to fade out.</param>
    public void StopBus(string name, bool immediate = false) {
        if (!buses.TryGetValue(name, out Bus bus)) { return; }
        StopBus(bus, immediate);
    }

    /// <summary>
    /// Stop all sounds on a single sound bus.
    /// </summary>
    /// <param name="bus">The bus to stop all sounds on.</param>
    /// <param name="immediate">If the sounds should be allowed to fade out.</param>
    public void StopBus(Bus bus, bool immediate = false) {
        if (!FMODInstance.isValid() || !bus.isValid()) { return; }
        bus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    }

    /// <summary>
    /// Play a sound event.
    /// </summary>
    /// <param name="eventName">The sound event to play.</param>
    /// <returns>A reference to the playing sound event.</returns>
    public SoundEvent Play(string eventName) {
        SoundEvent sound = Get(eventName);
        sound.Play();
        return sound;
    }

    /// <summary>
    /// Get a new handle for a sound effect without playing it.
    /// </summary>
    /// <param name="eventName">The sound event name.</param>
    /// <returns>A reference to the sound event.</returns>
    public SoundEvent Get(string eventName) {
        if (!events.TryGetValue(eventName, out EventDescription desc)) {
            Log.Warning($"FMOD: No event found with name {eventName}");
            return new(new());
        }

        return new(desc);
    }
}