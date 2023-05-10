using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

// TODO: OnSceneChanging Event - Scene Change blocker?

namespace GoDough.Visuals {
  public class SceneManagementService<TSceneEnum>
    where TSceneEnum : System.Enum {

    public delegate void SceneChangeEventHandler(object sender, SceneChangeEventArgs<TSceneEnum> e);

    #region Private Fields
    private readonly Dictionary<TSceneEnum, string> _registeredScenes = new Dictionary<TSceneEnum, string>();
    private readonly ILogger<SceneManagementService<TSceneEnum>> _logger;
    private readonly IAppHostNodeProvider _appHostNodeProvider;
    #endregion

    #region Properties
    public TSceneEnum CurrentScene { get; set; }

    public ReadOnlyDictionary<TSceneEnum, string> RegisteredSceneFiles {
      get {
        return new ReadOnlyDictionary<TSceneEnum, string>(this._registeredScenes);
      }
    }
    #endregion

    #region Ctor
    public SceneManagementService(
      ILogger<SceneManagementService<TSceneEnum>> logger,
      IAppHostNodeProvider appHostNodeProvider) => (_logger, _appHostNodeProvider) = (logger, appHostNodeProvider);
    #endregion

    #region Events
    public event SceneChangeEventHandler OnSceneChanged;
    #endregion

    #region Public Methods
    public SceneManagementService<TSceneEnum> RegisterSceneFile(TSceneEnum sceneKey, string fileName) {
      if (this._registeredScenes.ContainsKey(sceneKey)) {
        throw new InvalidOperationException(
          String.Format(
            "Scene with key '{0}' already registered",
            Enum.GetName(typeof(TSceneEnum), sceneKey)));
      }

      this._logger.LogInformation("Registering Scene '{0}' to load from '{1}'",
        Enum.GetName(typeof(TSceneEnum), sceneKey),
        fileName);

      this._registeredScenes
        .Add(sceneKey, fileName);

      return this;
    }

    public void LoadScene(TSceneEnum sceneKey) {
      if (!this._registeredScenes.ContainsKey(sceneKey)) {
        throw new KeyNotFoundException(
          String.Format(
            "Could not find Scene with key '{0}'",
            Enum.GetName(typeof(TSceneEnum), sceneKey)));
      }

      var fileName = this._registeredScenes[sceneKey];
      this._logger.LogInformation("Loading Scene '{0}' from '{1}'",
        Enum.GetName(typeof(TSceneEnum), sceneKey),
        fileName);

      if (this._appHostNodeProvider == null) {
        this._logger.LogInformation("_appHostNodeProvider null");
      }

      var appHostNode = this._appHostNodeProvider.GetNode();
      appHostNode.GetTree().ChangeSceneToFile(fileName);

      this.CurrentScene = sceneKey;

      if (this.OnSceneChanged != null) {
        this.OnSceneChanged.Invoke(
          this,
          new SceneChangeEventArgs<TSceneEnum>(sceneKey));
      }

      this._logger.LogInformation("Done Loading Scene '{0}'",
        Enum.GetName(typeof(TSceneEnum), sceneKey),
        fileName);

    }

    public object ConfigureInitialScene(TSceneEnum login) {
      this.CurrentScene = login;
    }
    #endregion
  }
}