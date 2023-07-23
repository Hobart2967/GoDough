using Godot;
using GoDough.Runtime;

namespace GoDough.Visuals.Components;

internal interface IComponent
{
  void WireScene(PackedScene scene, Node sceneRoot, AppHost appHost);
}