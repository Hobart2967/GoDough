using Godot;

namespace GoDough.Visuals.Extensions; 
public static class VisualInstance3DExtensions {
public static Aabb GetGlobalAabb(this MeshInstance3D visualInstance3D) {
  // From PR:
  //
  // https://github.com/godotengine/godot/pull/66940
  //
  // get_global_transform().xform(get_aabb());

  return visualInstance3D.GlobalTransform * visualInstance3D.GetAabb();
}

public static Aabb GetRotatedAndTransformedAabb(this MeshInstance3D visualInstance3D) {
  var source = visualInstance3D.Mesh.GetAabb();

  var vertices = new Vector3[]{
    source.Position, //bottomLeftFront,
    new Vector3(source.End.X, source.Position.Y, source.Position.Z), //bottomRightFront,
    new Vector3(source.End.X, source.End.Y, source.Position.Z), //topRightFront,
    new Vector3(source.Position.X, source.End.Y, source.Position.Z), //topLeftFront,
    new Vector3(source.Position.X, source.End.Y, source.End.Z), //topLeftBack,
    new Vector3(source.End.X, source.End.Y, source.End.Z), //topRightBack,
    new Vector3(source.End.X, source.Position.Y, source.End.Z), //bottomRightBack,
    new Vector3(source.Position.X, source.Position.Y, source.End.Z), //bottomLeftBack,
  };

  var min = Vector3.Inf;
  var max = -Vector3.Inf;

  var scale = visualInstance3D.GlobalTransform.Basis.Scale;
  var rotation = visualInstance3D.GlobalRotation;

  for (var vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
    var vertex = vertices[vertexIndex];

    var transformedVertex = scale * vertex;
    vertices[vertexIndex] = transformedVertex;

    if (transformedVertex < min) {
      min = transformedVertex;
    }

    if (transformedVertex > max) {
      max = transformedVertex;
    }
  }

  var size = -(max - min);

  return new Aabb(min, size);
}
}