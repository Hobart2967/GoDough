using Godot;
using GoDough.Visuals;
using System.Reflection;
using System;
using GoDough.Runtime;

namespace GoDough.Visuals.Components {
  public class ComponentFactory {
    private readonly IServiceProvider _serviceProvider;
    private readonly IGodotApi _godotApi;

    public ComponentFactory(
      IServiceProvider serviceProvider,
      IGodotApi godotApi) =>
      (_serviceProvider, _godotApi) = (serviceProvider, godotApi);

    public object Create(Type type) {
      if (!typeof(Component).IsAssignableFrom(type)) {
        throw new InvalidOperationException(String.Format(
          "Cannot instantiate component of type {0} because it is not inheriting Component type.",
          type.Name));
      }

      var binding = type.GetCustomAttribute<SceneBinding>();
      if (binding == null) {
        throw new InvalidOperationException(String.Format(
          "Cannot instantiate component of type {0} because it is missing a SceneBinding attribute.",
          type.Name));
      }

      var component = this._serviceProvider.GetService(type) as Component;
      if (component == null) {
        throw new InvalidOperationException(String.Format(
          "Cannot instantiate component of type {0} because it is missing in IServiceCollection or cannot be resolved.",
          type.Name));
      }
      var scene = this._godotApi.LoadScene(binding.SceneFile);
      var sceneRoot = scene.Instantiate();
      sceneRoot.BindToViewModel(component);
      component.WireScene(scene, sceneRoot);

      return component;
    }

    public T Create<T>(Action<T> configureComponent = null) where T : Component {
      return this.Create(typeof(T)) as T;
    }
  }
}