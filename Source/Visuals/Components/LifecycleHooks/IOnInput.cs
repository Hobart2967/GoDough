using GoDough.Runtime.LivecycleHooks;

namespace GoDough.Visuals.Components.LivecycleHooks;

public interface IOnInput
{
  public event InputEventHandler OnInput;
}