using Godot;
using System.Reflection;
using System;
using System.Linq;
using GoDough.Visuals.Attributes;

namespace GoDough.Visuals {
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

        nodeBinding.Property.SetValue(
          boundInstance,
          node.GetNode(String.Format("%{0}", nodeName)));
      }

      return node;
    }
  }
}