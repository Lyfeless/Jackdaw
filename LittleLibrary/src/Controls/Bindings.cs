using Foster.Framework;

namespace LittleLib;

//! FIXME (Alex): Load bindings from savedata?
public class Bindings() {
    readonly Dictionary<string, VirtualAction> Actions = [];
    public VirtualAction GetAction(string name) => Actions.TryGetValue(name, out VirtualAction? value) ? value : throw new Exception("Attempting to access undefinined action binding");
    public Bindings Add(string name, VirtualAction action) {
        Actions.Add(name, action);
        return this;
    }

    readonly Dictionary<string, VirtualStick> Sticks = [];
    public VirtualStick GetStick(string name) => Sticks.TryGetValue(name, out VirtualStick? value) ? value : throw new Exception("Attempting to access undefinined stick binding");
    public Bindings Add(string name, VirtualStick action) {
        Sticks.Add(name, action);
        return this;
    }

    readonly Dictionary<string, VirtualAxis> Axis = [];
    public VirtualAxis GetAxis(string name) => Axis.TryGetValue(name, out VirtualAxis? value) ? value : throw new Exception("Attempting to access undefinined axis binding");
    public Bindings Add(string name, VirtualAxis action) {
        Axis.Add(name, action);
        return this;
    }
}