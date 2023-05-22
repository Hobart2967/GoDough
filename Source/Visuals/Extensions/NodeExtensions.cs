using System;
using System.Collections.Generic;
using Godot;

namespace GoDough.Visuals.Extensions {
  public static class NodeExtensions {
    private static IEnumerable<Node> FindAllChildren(Node node,  Func<Node, bool> predicate, List<Node> nodeList) {
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