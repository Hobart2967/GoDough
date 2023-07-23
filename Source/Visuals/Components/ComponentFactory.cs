using Godot;
using System.Reflection;
using System;
using GoDough.Runtime;

namespace GoDough.Visuals.Components; 
public class ComponentFactory {
private readonly IServiceProvider _serviceProvider;
private readonly IGodotApi _godotApi;
private readonly AppHost _appHost;

public ComponentFactory(
  IServiceProvider serviceProvider,
  AppHost appHost,
  IGodotApi godotApi) =>
  (_serviceProvider, _appHost, _godotApi) = (serviceProvider, appHost, godotApi);

public object Create(Type type) {
  if (!typeof(IComponent).IsAssignableFrom(type)) {
    throw new InvalidOperationException(String.Format(
      "Cannot instantiate component of type {0} because it is not inheriting Component type.",
      type.Name));
  }
  var binding = type.GetCustomAttribute<SceneBinding>();
  if (binding == null) {
    throw new InvalidOperationException(String.Format(
      "Cannot instantiate component of type {0} because itd is missing a SceneBinding attribute.",
      type.Name));
  }

  var component = this._serviceProvider.GetService(type) as IComponent;
  if (component == null) {
    throw new InvalidOperationException(String.Format(
      "Cannot instantiate component of type {0} because it is missing in IServiceCollection or cannot be resolved.",
      type.Name));
  }
  var scene = this._godotApi.LoadScene(binding.SceneFile);
  var sceneRoot = scene.Instantiate();
  sceneRoot.BindToViewModel(component);
  component.WireScene(scene, sceneRoot, this._appHost);

  return component;
}

public T Create<T, TNodeType>()
  where T : Component<TNodeType>
  where TNodeType : Node {
  return this.Create(typeof(T)) as T;
}
}