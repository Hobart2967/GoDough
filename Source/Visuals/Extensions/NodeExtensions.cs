using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using BoundingBox = Godot.Aabb;

namespace GoDough.Visuals.Extensions; 
public static class NodeExtensions {
public static BoundingBox GetContentsBoundingBox(this List<MeshInstance3D> nodes) {
  var aabb = nodes
    .Select(x => x
      .GetRotatedAndTransformedAabb())
    .Aggregate((a, b) => a.Merge(b));

  return aabb.Abs();
}

public static T ClearAllChildren<T>(this T node) where T : Node {
  var children = node.GetChildren();

  foreach(var child in children) {
    node.RemoveChild(child);
  }

  return node;
}

private static IEnumerable<Node> FindAllChildren(Node node, Func<Node, bool> predicate, List<Node> nodeList) {
  foreach(var child in node.GetChildren()) {
    if (predicate(child)) {
      nodeList.Add(child);
    }

    FindAllChildren(child, predicate, nodeList);
  }

  return nodeList;
}

public static IEnumerable<Node> FindAllChildren(this Node node, Func<Node, bool> predicate) {
  var matchingNodes = new List<Node>();

  return FindAllChildren(node, predicate, matchingNodes);
}
}