using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoDough.Runtime {
  public class TaskWithProgress<T> : Task<T> {
    public Progress<double> Progress { get; private set; }

    public TaskWithProgress(Progress<double> progress, Func<T> action) : base(action) {
      this.Progress = progress;
    }
    public TaskWithProgress(Progress<double> progress, Func<T> action, CancellationToken cancellationToken) : base(action, cancellationToken) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<T> action, TaskCreationOptions creationOptions) : base(action, creationOptions) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<object?, T> action, object? state) : base(action, state) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<T> action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, cancellationToken, creationOptions) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<object?, T> action, object? state, CancellationToken cancellationToken) : base(action, state, cancellationToken) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<object?, T> action, object? state, TaskCreationOptions creationOptions) : base(action, state, creationOptions) {
      this.Progress = progress;
    }

    public TaskWithProgress(Progress<double> progress, Func<object?, T> action, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions) {
      this.Progress = progress;
    }
  }
}