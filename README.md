# Godot Utilities

- [Setup Dependencies](#setup-dependencies)
  - [Through NuGet:](#through-nuget)
  - [Refer to project directly (With cloning first)](#refer-to-project-directly-with-cloning-first)
- [Setup Project](#setup-project)
- [Injection and Logging in Nodes](#injection-and-logging-in-nodes)
- [Scene Manager](#scene-manager)
  - [Registering Scenes](#registering-scenes)
  - [Load a Scene](#load-a-scene)
  - [Listening to Scene changes](#listening-to-scene-changes)
- [Registering Service Factories](#registering-service-factories)
- [Unique Node Bindings](#unique-node-bindings)
  - [Using property name as node name.](#using-property-name-as-node-name)
  - [Using a different property name than the node's name.](#using-a-different-property-name-than-the-nodes-name)
- [Components](#components)
  - [Configuration](#configuration)
  - [Create a component definition:](#create-a-component-definition)
  - [Registering the component](#registering-the-component)
  - [Creating an instance of the component](#creating-an-instance-of-the-component)
- [Binding a Node to another class](#binding-a-node-to-another-class)


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

        public override void _Process(double delta) {
          this.AppHost.Process(delta);
        }

        public override void _PhysicsProcess(double delta) {
          this.AppHost.PhysicsProcess(delta);
        }

        public override void _Input(InputEvent ev) {
          this.AppHost.PhysicsProcess(delta);
        }
        #endregion
      }
      ```

  4. Enjoy!

## Injection and Logging in Nodes

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


## Scene Manager
### Registering Scenes

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

### Load a Scene
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
### Listening to Scene changes

You can always request the current scene via the `CurrentScene` properties.

If you want to get notified for scene changes, you can subscribe to the `OnSceneChanged` event:

```cs
this._sceneManager.OnSceneChanged += (s, e) => {
  if (e.SceneKey == SceneNames.Main) {
    GD.Print("Yay!");
  }
};
```

## Registering Service Factories

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

## Unique Node Bindings

### Using property name as node name.
1. Declare a node as "Unique Node" inside the scene.
2. Attach script to scene
3. Inside the script, create a property with the exact same name as the unique node.
4. Add `[UniqueNode]` attribute to that property
5. Inside the `_Ready` callback method, add a call to `this.BindToViewModel()`.

    ```cs
    using Godot;
    using GoDough.Visuals;
    using GoDough.Visuals.Attributes;

    public partial class MyPopup : Panel
    {
      [UniqueNode]
      public LineEdit MyTextField { get; set; }

      public override void _Ready() {
        this.BindToViewModel();
      }
    }
    ```

### Using a different property name than the node's name.
1. Declare a node as "Unique Node" inside the scene.
2. Attach script to scene
3. Inside the script, create a property that should hold the desired node.
4. Add `[UniqueNode("MyUniqueNodeName")]` attribute to that property. The string should contain the name that you gave it in Godot editor.
5. Inside the `_Ready` callback method, add a call to `this.BindToViewModel()`.

    ```cs
    using Godot;
    using GoDough.Visuals;
    using GoDough.Visuals.Attributes;

    public partial class MyPopup : Panel
    {
      [UniqueNode("MyUniqueNodeName")]
      public LineEdit MyTextField { get; set; }

      public override void _Ready() {
        this.BindToViewModel();
      }
    }
    ```

## Components

### Configuration

Inside your `IServiceCollection` configuration, add:

```cs
  using GoDough.Visuals.Components;

    // ...
    public override void ConfigureServices(IServiceCollection services) {
      //...
      services
        .AddComponentFactory();
    }
```

### Create a component definition:

```cs
using Godot;
using GoDough.Visuals.Attributes;
using GoDough.Visuals.Components;
using Gwen.Network.Models.Character;

namespace Gwen.Client.Scenes.CharacterList {
  [SceneBinding("res://prefabs/my-component.tscn")]
  public class MyComponent : Component<Camera3D> { // Make sure to set the correct root type as generic, otherwise set "Node".
    [UniqueNode]
    public Button MyButton { get; set; }
  }
}
```

### Registering the component

Inside your `IServiceCollection` configuration, add:

```cs
public override void ConfigureServices(IServiceCollection services) {
  services.AddTransient<MyComponent>();
}
```

### Creating an instance of the component

Inject `ComponentFactory` into your class, then run:

```csharp
var myComponent = this.ComponentFactory.Create<CharacterComponent>();

var rootNode = characterComponent.SceneRoot; // Access Node
```


## Binding a Node to another class

You can use a node to bind it to another class, accessing unique nodes using the attribute.

1. Create class containing properties for child nodes with a unique name:

```csharp
using Godot;
using GoDough.Visuals;
using GoDough.Visuals.Attributes;

public class ViewBindingClass {
  [UniqueNode]
  public LineEdit MyTextBox { get; set; }
}
```

2. Use `BindToViewModel` extension method at a place where you have access to the desired node, while passing the `ViewBindingClass`  instance as a parameter:

```csharp
using Godot;
using GoDough.Visuals;
using GoDough.Visuals.Attributes;

public partial class MyPopup : Panel {

  public override void _Ready() {
    var viewBindings = new ViewBindingClass();

    this.BindToViewModel(viewBindings);

    viewBindings.MyTextBox.Text = "Blub";
  }

}
```
