using Godot;

namespace GoDough.Visuals.Components {
  public class Component {
    public Node SceneRoot { get; internal set; }
    public PackedScene Scene { get; internal set; }

    internal void WireScene(PackedScene scene, Node sceneRoot) {
      this.Scene = scene;
      this.SceneRoot = sceneRoot;
    }
  }
}