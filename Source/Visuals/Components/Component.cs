using System;
using Godot;
using GoDough.Runtime;
using GoDough.Visuals.Components.LivecycleHooks;

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
      var hooks = new Type[] {
        typeof(IOnProcess),
        typeof(IOnPhysicsProcess),
        typeof(IOnUnhandledInput),
        typeof(IOnInput)
      };

      if (this.SceneRoot is IOnProcess) {
        (this.SceneRoot as IOnProcess).OnProcess += (s, delta) => this.OnProcess(delta);
      }

      if (this.SceneRoot is IOnPhysicsProcess) {
        (this.SceneRoot as IOnPhysicsProcess).OnPhysicsProcess += (s, delta) => this.OnPhysicsProcess(delta);
      }

      if (this.SceneRoot is IOnInput) {
        (this.SceneRoot as IOnInput).OnInput += (s, ev) => this.OnInput(ev);
      }

      if (this.SceneRoot is IOnUnhandledInput) {
        (this.SceneRoot as IOnUnhandledInput).OnUnhandledInput += (s, ev) => this.OnUnhandledInput(ev);
      }

      this.OnReady();
    }

    public virtual void OnInput(InputEvent ev) { }

    public virtual void OnUnhandledInput(InputEvent ev) { }

    public virtual void OnProcess(double delta) { }

    public virtual void OnReady() { }

    public virtual void OnPhysicsProcess(double delta) { }
  }
}