using Godot;

namespace GoDough.Visuals.Extensions {
  public static class ColorExtensions {
    public static Color WithAlpha(this Color color, float alpha) {
      return new Color(color.R, color.G, color.B, alpha);
    }
  }
}