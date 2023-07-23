using System;

namespace GoDough.Composition;

public class Factory<T>
{
  #region Private Fields

  private readonly Func<T> _initFunc;

  #endregion Private Fields

  #region Ctor

  public Factory(Func<T> initFunc) =>
    (_initFunc) = (initFunc);

  #endregion Ctor

  #region Public Methods

  public T Create() => _initFunc();

  #endregion Public Methods
}