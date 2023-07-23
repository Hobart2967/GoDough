using GoDough.Runtime.LivecycleHooks;

namespace GoDough.Visuals.Components.LivecycleHooks; 
public interface IOnUnhandledInput {
public event InputEventHandler OnUnhandledInput;
}