using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Godot;
using GoDough.Runtime;
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
    private readonly IGodotApi _godotApi;

    private readonly GoDough.Threading.Dispatcher _dispatcher;
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
      IGodotApi godotApi,
      GoDough.Threading.Dispatcher dispatcher,
      IAppHostNodeProvider appHostNodeProvider) =>
      (_dispatcher, _godotApi, _logger, _appHostNodeProvider) =
      (dispatcher, godotApi, logger, appHostNodeProvider);
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
    public TaskWithProgress<PackedScene> LoadPackedScene(TSceneEnum sceneKey, PackedScene loadingScreen = null, Task innerLoading = null, Progress<double> innerLoadingProgress = null) {
      if (!this._registeredScenes.ContainsKey(sceneKey)) {
        throw new KeyNotFoundException(
          String.Format(
            "Could not find Scene with key '{0}'",
            Enum.GetName(typeof(TSceneEnum), sceneKey)));
      }

      var fileName = this._registeredScenes[sceneKey];
      return this._godotApi.LoadSceneAsync(fileName);
    }

    public async Task<PackedScene> LoadScene(TSceneEnum sceneKey, PackedScene loadingScreen = null, Progress<double> innerLoadingProgress = null) {
      if (!this._registeredScenes.ContainsKey(sceneKey)) {
        throw new KeyNotFoundException(
          String.Format(
            "Could not find Scene with key '{0}'",
            Enum.GetName(typeof(TSceneEnum), sceneKey)));
      }

      var appHostNode = this._appHostNodeProvider.GetNode();
      ProgressBar progressBar = null;

      Action<double> progress = loadingProgress =>
        this._dispatcher.Invoke(() => {
          if (progressBar != null) {
            progressBar.Value = loadingProgress;
          }
        });

      if (loadingScreen != null) {
        appHostNode
          .GetTree()
          .ChangeSceneToPacked(loadingScreen);

        double progressValue = 0;

        while (progressBar == null) {
          await appHostNode.ToSignal(appHostNode.GetTree(), "process_frame");
          var sceneTree = appHostNode.GetTree().CurrentScene;
          if (sceneTree == null) {
            continue;
          }
          progressBar = sceneTree.GetNode<ProgressBar>("%ProgressBar");
          progressBar.Value = progressValue;
        }
      }

      var fileName = this._registeredScenes[sceneKey];
      this._logger.LogInformation("Loading Scene '{0}' from '{1}'",
        Enum.GetName(typeof(TSceneEnum), sceneKey),
        fileName);

      if (this._appHostNodeProvider == null) {
        this._logger.LogInformation("_appHostNodeProvider null");
      }

      var task = this._godotApi.LoadSceneAsync(fileName);
      if (innerLoadingProgress != null) {
        innerLoadingProgress.ProgressChanged += (s, e) => progress(e);
      }

      return await task;
    }

    public async Task LoadAndEnableScene(TSceneEnum sceneKey, PackedScene loadingScreen = null, Task innerLoading = null, Progress<double> innerLoadingProgress = null) {
      var scene = await this.LoadScene(sceneKey, loadingScreen, innerLoadingProgress);
      await this.EnableScene(scene, sceneKey, innerLoading);
    }

    public async Task EnableScene(PackedScene sceneToEnable, TSceneEnum sceneKey, Task innerLoading = null) {
      var appHostNode = this._appHostNodeProvider.GetNode();

      if (innerLoading != null) {
        await innerLoading;
      }

      appHostNode.GetTree().ChangeSceneToPacked(sceneToEnable);

      await this.WaitForNextFrame(appHostNode, () => {
        this.CurrentScene = sceneKey;

        if (this.OnSceneChanged != null) {
            this.OnSceneChanged.Invoke(
            this,
            new SceneChangeEventArgs<TSceneEnum>(sceneKey));
        }

        this._logger.LogInformation("Done Enabling Scene '{0}'",
          Enum.GetName(typeof(TSceneEnum), sceneKey));
      });
    }

    private async Task WaitForNextFrame(Node node, Action value) {
      await node.ToSignal(node.GetTree(), "process_frame");
      value();
    }

    public SceneManagementService<TSceneEnum> ConfigureInitialScene(TSceneEnum login) {
      this.CurrentScene = login;
      return this;
    }
    #endregion
  }
}