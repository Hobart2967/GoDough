using Godot;

namespace GoDough.Visuals.Extensions {
  public static class VisualInstance3DExtensions {
    public static Aabb GetGlobalAabb(this VisualInstance3D visualInstance3D) {
      return visualInstance3D.GlobalTransform * visualInstance3D.GetAabb();
    }
  }
}