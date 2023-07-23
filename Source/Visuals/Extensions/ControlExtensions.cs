using System;
using Godot;

namespace GoDough.Visuals.Extensions; 
public static class ResourceExtensions {
public static T DuplicateWith<T>(this Resource resource, Action<T> modifier)
  where T : Resource {
  var styleBox = resource.Duplicate() as T;
  modifier(styleBox);
  return styleBox;
}
}


