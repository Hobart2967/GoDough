using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Godot;
using GoDough.Runtime.LivecycleHooks;
using GoDough.Diagnostics.Logging;
using GoDough.Visuals;
using System;

namespace GoDough.Runtime; 
public class AppHost {
#region Private Fields
public Node AutoLoadNode { get; private set; }
#endregion

public const string NodePath = "/root/DependencyInjection";

#region Properties
public IHost Application { get; private set; }
public delegate void ProcessEventHandler(object sender, double delta);
public delegate void InputEventHandler(object sender, InputEvent ev);

public event ProcessEventHandler OnProcess;
public event ProcessEventHandler OnPhysicsProcess;
public event InputEventHandler OnInput;
public event InputEventHandler OnUnhandledInput;

private static AppHost? _instance = null;
public static AppHost? Instance {
  get { return AppHost._instance; }
}
#endregion

#region Ctor
public AppHost(Node autoLoadNode) {
  this.AutoLoadNode = autoLoadNode;

  AppHost._instance = AppHost.Instance ?? this;
}
#endregion

#region Public Methods
public void Start() {
  GD.Print("[GoDough] Building AppHost");
  var builder = Host.CreateDefaultBuilder(null);

  GD.Print("[GoDough] Booting Dependency Container");
  builder
    .ConfigureLogging(loggingBuilder => this.ConfigureLogging(loggingBuilder))
    .ConfigureServices(services => this.ConfigureServices(services));

  GD.Print("[GoDough] Sealing AppHost");
  this.Application = builder.Build();

  this.Boot();
}

public virtual void ConfigureLogging(ILoggingBuilder loggingBuilder) {
  loggingBuilder.AddGodotLogger();

  if (!OS.IsDebugBuild()) {
    return;
  }

  loggingBuilder.SetMinimumLevel(LogLevel.Trace);
}

    public virtual void ConfigureServices(IServiceCollection services) => services
        .AddSingleton<IAppHostNodeProvider, AppHostNodeProvider>(x =>
          new AppHostNodeProvider(() => this.AutoLoadNode))
        .AddSingleton(typeof(SceneManagementService<>));

    private void Boot() {
  var logger = this.Application.Services.GetService<ILogger<AppHost>>();

  var bootstrappers = this.Application.Services.GetServices<IBootstrapper>();
  logger.LogInformation("Booting App with {0} Bootstrappers", bootstrappers.Count());

  foreach(var service in bootstrappers) {
    service.Run();
  }
}

public void PhysicsProcess(double delta) {
  if (this.OnPhysicsProcess != null) {
    this.OnPhysicsProcess.Invoke(this, delta);
  }

  this.InvokeLifeCycleHooks<IOnPhysicsProcess>(x =>
    x.OnPhysicsProcess(delta));
}



public void UnhandledInput(InputEvent ev) {
  if (this.OnUnhandledInput != null) {
    this.OnUnhandledInput.Invoke(this, ev);
  }

  this.InvokeLifeCycleHooks<IOnUnhandledInput>(x =>
    x.OnUnhandledInput(ev));
}

public void Input(InputEvent ev) {
  if (this.OnInput != null) {
    this.OnInput.Invoke(this, ev);
  }

  this.InvokeLifeCycleHooks<IOnInput>(x =>
    x.OnInput(ev));
}


public void Process(double delta) {
  if (this.OnProcess != null) {
    this.OnProcess.Invoke(this, delta);
  }

  this.InvokeLifeCycleHooks<IOnProcess>(x =>
    x.OnProcess(delta));
}
#endregion

#region Private Methods
private void InvokeLifeCycleHooks<T>(Action<T> action) {
  var framebasedServices = this.Application.Services.GetServices<T>();
  foreach(var service in framebasedServices) {
    action(service);
  }
}
#endregion
}
