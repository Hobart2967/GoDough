using System;
using Godot;

namespace GoDough.Visuals {
  public class AppHostNodeProvider : NodeProvider, IAppHostNodeProvider {
    public AppHostNodeProvider(Func<Node> nodeResolver)
      : base(nodeResolver) { }
  }
}