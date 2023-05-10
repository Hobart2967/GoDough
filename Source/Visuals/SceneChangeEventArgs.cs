using System;

namespace GoDough.Visuals {
  public class SceneChangeEventArgs<TSceneEnum> : EventArgs
    where TSceneEnum : Enum {
    #region Properties
    public TSceneEnum SceneKey { get; }
    #endregion

    #region Ctor
    public SceneChangeEventArgs(TSceneEnum sceneKey) => SceneKey = sceneKey;
    #endregion
  }
}