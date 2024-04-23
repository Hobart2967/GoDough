using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace GoDough.Runtime {
  internal class GodotApi : IGodotApi {
    private readonly Dictionary<string, List<Action<double>>> _threadedResourceLoadProgress
      = new Dictionary<string, List<Action<double>>>();
    private readonly Dictionary<string, object> _resourceLoadTasks
      = new Dictionary<string, object>();
    private readonly Dictionary<string, PackedScene> _sceneLoadTasks
      = new Dictionary<string, PackedScene>();
    private readonly Dictionary<string, Shader> _shaderLoadTasks
      = new Dictionary<string, Shader>();
    private readonly Timer _timer;

    public GodotApi(AppHost appHost) {
      this._timer = new Timer();
      appHost.AutoLoadNode.AddChild(this._timer);

      this._timer.WaitTime = 0.1;
      this._timer.Name = "GodotApiTimer";
      this._timer.Timeout += () => this.OnProcess();
      this._timer.Start();
    }
    public PackedScene LoadScene(string fileName) {
      return this.LoadResource<PackedScene>(fileName);
    }

    public TaskWithProgress<PackedScene> LoadSceneAsync(string fileName) {
      return this.LoadResourceAsync<PackedScene>(fileName);
    }

    public TaskWithProgress<Shader> LoadShaderAsync(string fileName) {
      return this.LoadResourceAsync<Shader>(fileName);
    }

    public T LoadResource<T>(string fileName) where T : Resource {
      return (T)this.LoadResourceAsync<T>(fileName).Result;
    }

    public TaskWithProgress<T> LoadResourceAsync<T>(string fileName) where T : Resource {
      if (!this._resourceLoadTasks.ContainsKey(fileName)) {
        var progress = new Progress<double>();
        var progressReporter = progress as IProgress<double>;
        if(!this._threadedResourceLoadProgress.ContainsKey(fileName)) {
          this._threadedResourceLoadProgress[fileName] = new List<Action<double>>();
        }
        this._threadedResourceLoadProgress[fileName].Add(progressValue => progressReporter.Report(progressValue));

        var resourceLoadTask = new TaskWithProgress<T>(progress, () => {
            var error = ResourceLoader.LoadThreadedRequest(fileName);
            var loadStatus = ResourceLoader.LoadThreadedGetStatus(fileName);
            var resourceLoaded = ResourceLoader.LoadThreadedGet(fileName) as T;

            return resourceLoaded;
        });

        this._resourceLoadTasks[fileName] = resourceLoadTask;

        resourceLoadTask.Start();
      }

      return (TaskWithProgress<T>)this._resourceLoadTasks[fileName];
    }

    public Shader LoadShader(string fileName) {
      return this.LoadResource<Shader>(fileName);
    }

    public void OnProcess() {
      var keysToRemove = new List<string>();
      foreach (var progressInfo in this._threadedResourceLoadProgress) {
        var progressArray = new Godot.Collections.Array();
        var loadStatus = ResourceLoader.LoadThreadedGetStatus(progressInfo.Key, progressArray);
        if (loadStatus != ResourceLoader.ThreadLoadStatus.InProgress) {
          if (loadStatus == ResourceLoader.ThreadLoadStatus.Loaded) {
            progressInfo.Value.ForEach(x => x(1));
          }

          keysToRemove.Add(progressInfo.Key);

          continue;
        }

        var progress = progressArray.FirstOrDefault().AsSingle();
        progressInfo.Value.ForEach(x => x(progress));
      }

      foreach (var key in keysToRemove) {
        this._threadedResourceLoadProgress.Remove(key);
      }
    }
  }
}