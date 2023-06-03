using GoDough.Runtime.LivecycleHooks;

namespace GoDough.Visuals.Components.LivecycleHooks {
  public interface IOnPhysicsProcess {
    public event ProcessEventHandler OnPhysicsProcess;
  }
}