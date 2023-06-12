using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GoDough.Runtime.LivecycleHooks;

namespace GoDough.Threading {
  public class Dispatcher : IOnProcess {
    private class ActionOrFunc {
      public Action Action { get; set; }
      public Func<object> Func { get; set; }
      public TaskCompletionSource<object> Completion { get; set; }
    }

    private ConcurrentDictionary<string, ActionOrFunc> _actions = new ConcurrentDictionary<string, ActionOrFunc>();

    public void Invoke(Action action) {
      this._actions[Guid.NewGuid().ToString()] = new ActionOrFunc {
        Action = action
      };
    }

    public Task<T> Invoke<T>(Func<T> func) where T : class {
      var completionSource = new TaskCompletionSource<object>();
      this._actions[Guid.NewGuid().ToString()] = new ActionOrFunc {
        Func = func as Func<object>,
        Completion = completionSource
      };

      return completionSource.Task.ContinueWith<T>((Task<object> t) => {
        return t.Result as T;
      });
    }

    public void OnProcess(double delta) {
      while (this._actions.Count > 0) {
        var action = this._actions.FirstOrDefault();

        this._actions.TryRemove(action);

        if (action.Value.Action != null) {
          action.Value.Action();
        } else {
          var result = action.Value.Func();
          action.Value.Completion.SetResult(result);
        }
      }
    }
  }
}