using Godot;
using System;

namespace GoDough.Visuals;

public class NodeProvider : INodeProvider
{
  #region Private Fields

  private readonly Func<Node> _nodeResolver;

  #endregion Private Fields

  #region Ctor

  public NodeProvider(Func<Node> nodeResolver) =>
    (_nodeResolver) = (nodeResolver);

  #endregion Ctor

  #region Public Methods

  public Node GetNode() => this._nodeResolver();

  #endregion Public Methods
}