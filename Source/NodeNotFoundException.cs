using System;

namespace GoDough; 
public class NodeNotFoundException : Exception {
public string NodeName { get; }

public NodeNotFoundException(string message, string nodeName)
  : base(message) => this.NodeName = nodeName;
}