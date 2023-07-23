using System;
using Godot;

namespace GoDough.Visuals.Extensions; 
public static class Vector3Extensions {

    public static Vector3 CopyWith(
      this Vector3 source,
      Nullable<float> x,
      Nullable<float> y = null,
      Nullable<float> z = null) => new Vector3(
        x ?? source.X,
        y ?? source.Y,
        z ?? source.Z);

    public static Vector3 Copy(
     this Vector3 source) => source.CopyWith(null);
}