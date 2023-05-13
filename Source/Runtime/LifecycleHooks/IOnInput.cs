using Godot;

namespace GoDough.Runtime.LivecycleHooks {
  public interface IOnInput {
    public void OnInput(InputEvent ev);
  }
}