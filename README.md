# Godot Utilities

Utility library for implement state of the art application development in Godot 4. Includes:

- Using .NET Standards:
  - Logging Setup using ILogger interface (Microsoft.Extensions.Logging)
  - Dependency Injection (Microsoft.Extensions.DependencyInjection)

- General utilities for Godot:
  - Node Extension Methods

## Setup Dependencies

Add project reference to your Godot *.csproj project:


### Through NuGet:
```xml
<Project Sdk="Godot.NET.Sdk/4.0.2">
  <!-- ... -->
  <ItemGroup>
    <PackageReference Include="Hobart2967.GoDough" Version="1.0.0" />

    <PackageReference Include="Microsoft.Extensions.Hosting" Version="6.0.0" />
    <!-- ... -->
```

### Refer to project directly (With cloning first)

```xml
<Project Sdk="Godot.NET.Sdk/4.0.2">
  <!-- ... -->
  <ItemGroup>
    <ProjectReference Include="../../GoDough/GoDough.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="6.0.0" />
  </ItemGroup>
```

## Setup Project

1. Create an autoload script in Godot
   1. Visit `Project`  Menu, then `Project Settings..`
   2. Select `Autoload Tab`
   3. Add a new script here.
2. Create specialized `AppHost` class which is for tailoring to your needs:

    ```cs
    using Godot;
    using GoDough.Composition.Extensions;
    using GoDough.Runtime;
    using GoDough.Runtime.LivecycleHooks;
    using Microsoft.Extensions.DependencyInjection;

    public class GwenAppHost : AppHost {
      #region Ctor
      public GwenAppHost(Node autoLoadNode)
        : base(autoLoadNode) { }
      #endregion

      #region Public Methods

      public override void ConfigureServices(IServiceCollection services) {
        base.ConfigureServices(services);

        services
          .AddSingleton<MyService>()

          .AddSingleton<IOnProcess, ServiceDoingSomethingOnProcess>() // Called on each _Process

          .AddSingleton<IBootstrapper, BootstrapperService>(); // Called on Startup (_Ready)
      }
      #endregion
    }
    ```

  3. Call AppHost from your AutoloadScript

      ```cs
      using Godot;

      public partial class AppHostLoader : Node {
        #region Properties
        public GwenAppHost AppHost { get; private set; }
        #endregion

        #region Ctor
        public AppHostLoader() {
          this.AppHost = new GwenAppHost(this);
        }
        #endregion

        #region Public Methods
        public override void _Ready() {
          this.AppHost.Start();
        }
        #endregion
      }
      ```

  4. Enjoy!

## Usage - Injection and Logging in Nodes

```cs
using GoDough.Composition.Attributes;
using GoDough.Composition.Extensions;

public partial class MyNode : Node {
  // Add [Inject] property to any injected service
	[Inject]
	public ILogger<MyNode> Logger { get; set; }

	public override void _Ready() {
		this.WireNode(); // Always call WireNode to make sure that the dependencies are loaded.

		this.Logger.LogDebug("Node is ready.");
	}
}
```

This will result in:

```
[07.05.2023 18:14:34] [Debug] [MyNode] Node is ready.
```


## Usage - Scene Manager
1. Create a Scenes Enum with all your scene names.
    ```cs
    public enum Scenes {
      MainMenu,
      Game,
      Settings
    }
    ```

2. Add a Bootstrapper to register your scenes on startup

    ```cs
    public class SceneRegistrationBootstrapper : IBootstrapper {
      #region Private Fields
      private readonly SceneManagementService<Scenes> _sceneManagementService;
      #endregion

      #region Ctor
      public SceneRegistrationBootstrapper(
        ILogger<SceneRegistrationBootstrapper> logger) =>
        (_sceneManagementService) = (sceneManagementService);
      #endregion

      #region Public Methods
      public void Run() {
        this._sceneManagementService
          .RegisterSceneFile(Scenes.MainMenu, "res://scenes/main-menu/main-menu.tscn")
          .RegisterSceneFile(Scenes.World, "res://scenes/world/world.tscn")
          .RegisterSceneFile(Scenes.Settings, "res://scenes/settings/settings.tscn");
      }
      #endregion
    }
    ```

    See above for registering the bootstrapper.

  3. Load a Scene
      ```cs
      using GoDough.Composition.Attributes;
      using GoDough.Composition.Extensions;

      public partial class MyNode : Node {
        // Add [Inject] property to any injected service
        [Inject]
        public SceneManagementService<Scenes> SceneManager { get; set; }

        public override void _Ready() {
          this.WireNode();

          this.SceneManager.LoadScene(Scenes.World);
        }
      }
      ```

## Usage - Registering Factories

1. Call `services.AddFactory` with the Class Type to created inside `AppHost.ConfigureServices`
    ```cs
    services.AddFactory<ListItem>()
    ```
2. Inject the factory and use it
    ```cs
    using GoDough.Composition.Attributes;
    using GoDough.Composition.Extensions;

    public partial class MyNode : Node {
      // Add [Inject] property to any injected service
      [Inject]
      public Factory<ListItem> ListItemFactory { get; set; }

      public override void _Ready() {
        this.WireNode(); // Always call WireNode to make sure that the dependencies are loaded.

        var listItem = this.ListItemFactory.Create();
      }
    }
    ```