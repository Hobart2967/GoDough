using System;

namespace GoDough.Visuals.Components; 
public class SceneBinding : Attribute {
public string SceneFile { get; }

public SceneBinding(string sceneFile) =>
  this.SceneFile = sceneFile;
}