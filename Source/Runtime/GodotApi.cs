using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoDough.Runtime;

internal class GodotApi : IGodotApi
{
  private readonly Dictionary<string, List<Action<double>>> _threadedResourceLoadProgress = new();
  private readonly Dictionary<string, Task<Resource>> _resourceLoadTasks = new();
  private readonly Timer _timer;

  public GodotApi(AppHost appHost)
  {
    this._timer = new Timer();
    appHost.AutoLoadNode.AddChild(this._timer);

    this._timer.WaitTime = 0.1;
    this._timer.Name = "GodotApiTimer";
    this._timer.Timeout += () => this.OnProcess();
    this._timer.Start();
  }

  public PackedScene LoadScene(string fileName) => (PackedScene)this.LoadResource(fileName);

  public async Task<PackedScene> LoadSceneAsync(string fileName, Action<double> progress = null) => (PackedScene)(await this.LoadResourceAsync(fileName, progress));

  public async Task<Shader> LoadShaderAsync(string fileName, Action<double> progress = null) => (Shader)(await this.LoadResourceAsync(fileName, progress));

  public Resource LoadResource(string fileName) => this.LoadResourceAsync(fileName).Result;

  public Task<Resource> LoadResourceAsync(string fileName, Action<double> progress = null)
  {
    if (progress != null)
    {
      if (!this._threadedResourceLoadProgress.ContainsKey(fileName))
      {
        this._threadedResourceLoadProgress[fileName] = new List<Action<double>>();
      }

      this._threadedResourceLoadProgress[fileName].Add(progress);
    }

    if (!this._resourceLoadTasks.ContainsKey(fileName))
    {
      this._resourceLoadTasks[fileName] = Task.Run(() =>
      {
        var error = ResourceLoader.LoadThreadedRequest(fileName);
        var loadStatus = ResourceLoader.LoadThreadedGetStatus(fileName);
        var resourceLoaded = ResourceLoader.LoadThreadedGet(fileName);

        return resourceLoaded;
      });
    }

    return this._resourceLoadTasks[fileName];
  }

  public Shader LoadShader(string fileName) => (Shader)this.LoadResource(fileName);

  public void OnProcess()
  {
    var keysToRemove = new List<string>();
    foreach (var progressInfo in this._threadedResourceLoadProgress)
    {
      var progressArray = new Godot.Collections.Array();
      var loadStatus = ResourceLoader.LoadThreadedGetStatus(progressInfo.Key, progressArray);
      if (loadStatus != ResourceLoader.ThreadLoadStatus.InProgress)
      {
        if (loadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
        {
          progressInfo.Value.ForEach(x => x(1));
        }

        keysToRemove.Add(progressInfo.Key);

        continue;
      }

      var progress = progressArray.FirstOrDefault().AsSingle();
      progressInfo.Value.ForEach(x => x(progress));
    }

    foreach (var key in keysToRemove)
    {
      this._threadedResourceLoadProgress.Remove(key);
    }
  }
}