using Godot;
using System.Reflection;
using System;
using System.Linq;
using GoDough.Visuals.Attributes;

namespace GoDough.Visuals.Components {
  public static class NodeExtensions {
    public static Node BindToViewModel(this Node node, object boundInstance = null) {
      if (boundInstance == null) {
        boundInstance = node;
      }
      var properties = boundInstance.GetType().GetProperties();
      var nodeBindings = properties
        .Select(x => new {
          Property = x,
          NodeAttribute = x.GetCustomAttribute<UniqueNode>()
        })
        .Where(x => x.NodeAttribute != null);

      foreach(var nodeBinding in nodeBindings) {
        var nodeName = nodeBinding.NodeAttribute.SpecificName == null
          ? nodeBinding.Property.Name
          : nodeBinding.NodeAttribute.SpecificName;

        var uniqueNode = node.GetNode(String.Format("%{0}", nodeName));
        if (uniqueNode == null && !nodeBinding.NodeAttribute.IgnoreInexistent) {
          throw new NodeNotFoundException(
            "Node with name {0} could not be found in the current scene tree.",
            nodeName);
        }

        nodeBinding.Property.SetValue(
          boundInstance,
          uniqueNode);
      }

      return node;
    }
  }
}