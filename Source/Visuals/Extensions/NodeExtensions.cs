using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GoDough.Visuals.Extensions {
  public static class NodeExtensions {
    public static Vector3 GetContentsSize(this Node3D node, string groupName = "Mesh") {
      var nodes = new List<MeshInstance3D>();
      if (node is MeshInstance3D) {
        nodes.Add(node as MeshInstance3D);
      } else {
        nodes = node
          .FindAllChildren(x => x.IsInGroup(groupName))
          .OfType<MeshInstance3D>()
          .ToList();
      }

      var aabbs = nodes
        .Select(x => x
          .GetRotatedAndTransformedAabb())
        .Aggregate((a, b) => a.Merge(b));

      return aabbs.Abs().Size;
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
}