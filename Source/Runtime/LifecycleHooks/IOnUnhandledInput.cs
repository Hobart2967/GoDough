using Godot;

namespace GoDough.Runtime.LivecycleHooks;

public interface IOnUnhandledInput
{
  public void OnUnhandledInput(InputEvent ev);
}