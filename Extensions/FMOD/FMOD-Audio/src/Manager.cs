using System;
using FMOD.Studio;
using Foster.Framework;

namespace LittleLib.Audio.FMODAudio;

public class AudioManager(LittleGame game, string directory = "") : Component(game) {
    const string BANK_EXTENSION = ".bank";
    const string STRINGS_EXTENSION = ".strings.bank";

    readonly string BankPath = directory;

    FMOD.Studio.System FMODInstance;
    readonly List<Bank> banks = [];
    readonly Dictionary<string, EventDescription> events = [];
    readonly Dictionary<string, Bus> buses = [];

    protected override void EnterTree() {
        FMOD.Studio.System.create(out FMODInstance);
        FMODInstance.initialize(1024, INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);

        string path = Path.Join(Game.Assets.Config.RootFolder, BankPath);
        if (Path.Exists(path)) {
            Console.WriteLine("Load FMOD Data");

            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(STRINGS_EXTENSION))) {
                LoadBank(file);
            }

            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(e => e.EndsWith(BANK_EXTENSION) && !e.EndsWith(STRINGS_EXTENSION))) {
                LoadBank(file);
            }
        }
    }

    void LoadBank(string bank) {
        Console.WriteLine($"Load bank {bank}");

        FMODInstance.loadBankFile(bank, LOAD_BANK_FLAGS.NORMAL, out Bank bankData);
        banks.Add(bankData);

        bankData.getEventList(out EventDescription[] bankEvents);
        foreach (EventDescription bankEvent in bankEvents) {
            bankEvent.getPath(out string eventPath);
            Console.WriteLine($"Event {eventPath}");
            events.Add(eventPath, bankEvent);
        }

        bankData.getBusList(out Bus[] bankBuses);
        foreach (Bus bankBus in bankBuses) {
            bankBus.getPath(out string busPath);
            Console.WriteLine($"Bus {busPath}");
            buses.Add(busPath, bankBus);
        }
    }

    protected override void ExitTree() {
        FMODInstance.release();
        banks.Clear();
        events.Clear();
        buses.Clear();
    }

    protected override void Update() {
        FMODInstance.update();
    }

    public float GetBusVolume(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return 1; }
        value.getVolume(out float volume);
        return volume;
    }

    public void SetBusVolume(string bus, float volume) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setVolume(Calc.Clamp(volume, 0, 1));
    }

    public bool GetBusPause(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return false; }
        value.getPaused(out bool paused);
        return paused;
    }

    public void PauseBus(string bus) => SetBusPause(bus, true);
    public void UnpauseBus(string bus) => SetBusPause(bus, false);
    public void SetBusPause(string bus, bool paused) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setPaused(paused);
    }

    public bool GetBusMute(string bus) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return false; }
        value.getMute(out bool muted);
        return muted;
    }

    public void MuteBus(string bus) => SetBusMute(bus, true);
    public void UnMuteBus(string bus) => SetBusMute(bus, false);
    public void SetBusMute(string bus, bool muted) {
        if (!FMODInstance.isValid() || !buses.TryGetValue(bus, out Bus value)) { return; }
        value.setMute(muted);
    }

    public void StopAll(bool immediate = false) {
        if (!FMODInstance.isValid()) { return; }
        foreach (Bus bus in buses.Values) { StopBus(bus, immediate); }
    }

    public void StopBus(string name, bool immediate = false) {
        if (!buses.TryGetValue(name, out Bus bus)) { return; }
        StopBus(bus, immediate);
    }

    public void StopBus(Bus bus, bool immediate = false) {
        if (!FMODInstance.isValid() || !bus.isValid()) { return; }
        bus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    }

    public SoundEvent Play(string eventName) {
        SoundEvent sound = Get(eventName);
        sound.Play();
        return sound;
    }

    public SoundEvent Get(string eventName) {
        if (!events.TryGetValue(eventName, out EventDescription desc)) {
            Console.WriteLine($"FMOD: No event found with name {eventName}");
            return new(new());
        }

        return new(desc);
    }
}