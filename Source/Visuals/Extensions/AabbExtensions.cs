using System;
using Godot;

namespace GoDough.Visuals.Extensions {
  public static class AabbExtensions {
    public static Mesh ConvertToMesh(this Aabb input, Color color) {
      var source = new {
        Position = new Vector3(-(input.Size.X / 2), 0, -(input.Size.Z / 2)),
        End = input.Size.CopyWith(input.Size.X / 2, null, input.Size.Z / 2)
      };

      var bottomLeftFront = source.Position;
      var bottomRightFront = new Vector3(source.End.X, source.Position.Y, source.Position.Z);
      var topRightFront = new Vector3(source.End.X, source.End.Y, source.Position.Z);
      var topLeftFront = new Vector3(source.Position.X, source.End.Y, source.Position.Z);
      var topLeftBack = new Vector3(source.Position.X, source.End.Y, source.End.Z);
      var topRightBack = new Vector3(source.End.X, source.End.Y, source.End.Z);
      var bottomRightBack = new Vector3(source.End.X, source.Position.Y, source.End.Z);
      var bottomLeftBack = new Vector3(source.Position.X, source.Position.Y, source.End.Z);

      var vertices = new Vector3[]{
        topRightFront,    topLeftFront,
        topLeftBack,      topRightBack,

        bottomRightBack,  bottomLeftBack,
        bottomLeftFront,  bottomRightFront,

        topLeftFront,     topLeftBack,
        topRightFront,    topRightBack,

        bottomLeftFront,  bottomLeftBack,
        bottomRightFront, bottomRightBack,

        topLeftFront,     bottomLeftFront,
        topLeftBack,      bottomLeftBack,

        topRightFront,    bottomRightFront,
        topRightBack,     bottomRightBack,
      };

      var mesh = new ImmediateMesh();
      var material = new OrmMaterial3D();
      material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
      material.AlbedoColor = color;

      mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
      for (var i = 0; i < vertices.Length; i++) {
        mesh.SurfaceSetColor(color);
        mesh.SurfaceAddVertex(vertices[i]);
      }
      mesh.SurfaceEnd();

      return mesh;
    }
  }
}