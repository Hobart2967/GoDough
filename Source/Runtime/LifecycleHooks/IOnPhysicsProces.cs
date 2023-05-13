using Godot;

namespace GoDough.Runtime.LivecycleHooks {
  public interface IOnPhysicsProcess {
    public void OnPhysicsProcess(double delta);
  }
}