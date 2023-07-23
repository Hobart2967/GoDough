using Godot;
using GoDough.Runtime;

namespace GoDough.Visuals.Components; 
interface IComponent {
void WireScene(PackedScene scene, Node sceneRoot, AppHost appHost);
}