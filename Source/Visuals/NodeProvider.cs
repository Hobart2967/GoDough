using System;
using Godot;

namespace GoDough.Visuals; 
public class NodeProvider : INodeProvider {
#region Private Fields
private readonly Func<Node> _nodeResolver;
#endregion

#region Ctor
public NodeProvider(Func<Node> nodeResolver) =>
  (_nodeResolver) = (nodeResolver);
    #endregion

    #region Public Methods
    public Node GetNode() => this._nodeResolver();
    #endregion
}