using GoDough.Runtime.LivecycleHooks;

namespace GoDough.Visuals.Components.LivecycleHooks {
  public interface IOnProcess {
    public event ProcessEventHandler OnProcess;
  }
}