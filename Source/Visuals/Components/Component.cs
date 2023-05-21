using Godot;
using GoDough.Runtime;

namespace GoDough.Visuals.Components {
  public abstract class Component : Component<Node> {

  }

  public abstract class Component<TSceneRoot> : IComponent where TSceneRoot : Node {
    public TSceneRoot SceneRoot { get; internal set; }
    public PackedScene Scene { get; internal set; }

    public void WireScene(PackedScene scene, Node sceneRoot, AppHost appHost) {
      if (this.Scene != null || this.SceneRoot != null) {
        return;
      }

      this.Scene = scene;
      this.SceneRoot = sceneRoot as TSceneRoot;

      appHost.OnProcess += (s, delta) => this.OnProcess(delta);
      appHost.OnPhysicsProcess += (s, delta) => this.OnPhysicsProcess(delta);
      appHost.OnInput += (s, ev) => this.OnInput(ev);

      this.OnReady();
    }

    public virtual void OnInput(InputEvent ev) { }

    public virtual void OnProcess(double delta) { }

    public virtual void OnReady() { }

    public virtual void OnPhysicsProcess(double delta) { }
  }
}