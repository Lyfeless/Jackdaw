using Foster.Framework;

namespace Jackdaw;

/// <summary>
/// Storage for input bindings.
/// </summary>
public class Bindings() {
    readonly Dictionary<string, VirtualAction> Actions = [];

    /// <summary>
    /// Find a button input action from loaded bindings.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <returns>The button input binding.</returns>
    /// <exception cref="Exception"></exception>
    public VirtualAction GetAction(string name) => Actions.TryGetValue(name, out VirtualAction? value) ? value : throw new Exception("Attempting to access undefinined action binding");

    /// <summary>
    /// Remove a button input binding
    /// </summary>
    /// <param name="name">The binding name to remove.</param>
    /// <returns></returns>
    public Bindings RemoveAction(string name) {
        Actions.Remove(name);
        return this;
    }

    /// <summary>
    /// Add a new button input binding.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <param name="action">The button input action.</param>
    /// <returns>The binding manager.</returns>
    public Bindings Add(string name, VirtualAction action) {
        Actions.Add(name, action);
        return this;
    }

    /// <summary>
    /// Add a new button input binding using the built-in action name.
    /// </summary>
    /// <param name="action">The button input action.</param>
    /// <returns>The binding manager.</returns>
    public Bindings Add(VirtualAction action) {
        Actions.Add(action.Name, action);
        return this;
    }

    readonly Dictionary<string, VirtualStick> Sticks = [];

    /// <summary>
    /// Find a stick input action from loaded bindings.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <returns>The stick input binding.</returns>
    /// <exception cref="Exception"></exception>
    public VirtualStick GetStick(string name) => Sticks.TryGetValue(name, out VirtualStick? value) ? value : throw new Exception("Attempting to access undefinined stick binding");

    /// <summary>
    /// Remove a stick input binding
    /// </summary>
    /// <param name="name">The binding name to remove.</param>
    /// <returns></returns>
    public Bindings RemoveStick(string name) {
        Sticks.Remove(name);
        return this;
    }

    /// <summary>
    /// Add a new stick input binding.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <param name="action">The stick input action.</param>
    /// <returns></returns>
    public Bindings Add(string name, VirtualStick action) {
        Sticks.Add(name, action);
        return this;
    }

    /// <summary>
    /// Add a new stick input binding using the built-in action name.
    /// </summary>
    /// <param name="action">The stick input action.</param>
    /// <returns></returns>
    public Bindings Add(VirtualStick action) {
        Sticks.Add(action.Name, action);
        return this;
    }

    readonly Dictionary<string, VirtualAxis> Axis = [];

    /// <summary>
    /// Find a axis input action from loaded bindings.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <returns>The axis input binding.</returns>
    /// <exception cref="Exception"></exception>
    public VirtualAxis GetAxis(string name) => Axis.TryGetValue(name, out VirtualAxis? value) ? value : throw new Exception("Attempting to access undefinined axis binding");

    /// <summary>
    /// Remove an axis input binding
    /// </summary>
    /// <param name="name">The binding name to remove.</param>
    /// <returns></returns>
    public Bindings RemoveAxis(string name) {
        Axis.Remove(name);
        return this;
    }

    /// <summary>
    /// Add a new axis input binding.
    /// </summary>
    /// <param name="name">The binding name.</param>
    /// <param name="action">The axis input action.</param>
    /// <returns></returns>
    public Bindings Add(string name, VirtualAxis action) {
        Axis.Add(name, action);
        return this;
    }

    /// <summary>
    /// Add a new axis input binding using the built-in action name.
    /// </summary>
    /// <param name="action">The axis input action.</param>
    /// <returns></returns>
    public Bindings Add(VirtualAxis action) {
        Axis.Add(action.Name, action);
        return this;
    }
}