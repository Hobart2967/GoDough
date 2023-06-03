using Godot;

namespace GoDough.Runtime.LivecycleHooks {
  public delegate void ProcessEventHandler(object sender, double delta);
  public delegate void InputEventHandler(object sender, InputEvent ev);
}