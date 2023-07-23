using Godot;

namespace GoDough.Visuals.Extensions;

public static class ColorExtensions
{
  public static Color WithAlpha(this Color color, float alpha) => new(color.R, color.G, color.B, alpha);
}