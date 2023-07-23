using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GoDough.Visuals.Extensions;
using BoundingBox = Godot.Aabb;

namespace GoDough.Visuals; 
public partial class Model3D : Node3D {
#region Private Fields
private MeshInstance3D _visualBoundingBox;
#endregion

#region Properties
private Vector3 _size = Vector3.Zero;
public Vector3 Size {
  get {
    if (this._size == Vector3.Zero) {
      UpdateBoundingBox();
    }
    return this._size;
  }
}

private BoundingBox? _boundingBox = null;
public BoundingBox BoundingBox {
  get {
    if (this._size == Vector3.Zero) {
      UpdateBoundingBox();
    }

    return this._boundingBox.Value;
  }
}

public bool DrawBoundingBox {
  get {
    return this._visualBoundingBox.Visible;
  }
  set {
    this._visualBoundingBox.Visible = value;
  }
}

public Color DrawnBoundingBoxColor { get; set; } = Colors.Red;

private Node3D _model;
public Node3D Model {
  get {
    return this._model;
  }

  set {
    this._model = value;

    foreach (var child in this.GetChildren()) {
      if (child == this._visualBoundingBox) {
        continue;
      }

      this.RemoveChild(child);
    }

    this.AddChild(value);
    this._size = Vector3.Zero;
    this._boundingBox = null;
  }
}

private string _groupName = "Body";
public string GroupName {
  get {
    return this._groupName;
  }

  set {
    this._groupName = value;

    if (this._size != Vector3.Zero) {
      UpdateBoundingBox();
    }
  }
}

private SizeCalculationMode _calculcationMode = SizeCalculationMode.Group;
public SizeCalculationMode CalculationMode {
  get {
    return this._calculcationMode;
  }

  set {
    this._calculcationMode = value;
  }
}
#endregion

#region Ctor
public Model3D() {
  this._visualBoundingBox = new MeshInstance3D();
  this._visualBoundingBox.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
  this._visualBoundingBox.Visible = false;
  this.AddChild(this._visualBoundingBox);
}
#endregion

#region Callbacks
public override void _Ready() {
  this.UpdateBoundingBoxIfRequested();
}

public override void _Process(double delta) {
  if (!this._visualBoundingBox.Visible) {
    return;
  }

  if (this._boundingBox == null) {
    this.UpdateBoundingBox();
    return;
  }

  this._visualBoundingBox.Mesh = this.BoundingBox.ConvertToMesh(this.DrawnBoundingBoxColor);
}

public override void _EnterTree(){
  this.UpdateBoundingBoxIfRequested();
}

public override void _ExitTree(){
  this.UpdateBoundingBoxIfRequested();
}
#endregion

#region Public Methods
public void UpdateBoundingBox() {
  var nodes = new List<MeshInstance3D>();
  var children = this.GetChildren().OfType<MeshInstance3D>();
  if (this.CalculationMode == SizeCalculationMode.Children && children.Count() == 1) {
    nodes = children.ToList();
  } else {
    nodes = this
      .FindAllChildren(x => x.IsInGroup(this.GroupName))
      .OfType<MeshInstance3D>()
      .ToList();
  }

  nodes = nodes
    .Except(new MeshInstance3D[]{ this._visualBoundingBox })
    .ToList();

  if (nodes.Count() == 0) {
    return;
  }

  this._boundingBox = nodes.GetContentsBoundingBox();
  this._size = this._boundingBox.Value.Size;
}
#endregion

#region Private Methods
private void UpdateBoundingBoxIfRequested() {
  if (this._size == Vector3.Zero) {
    return;
  }

  UpdateBoundingBox();
}

public void ColorizeMeshGroup<T>(T group, Color color)
  where T : Enum {
  var groupName = Enum.GetName(typeof(T), group);
  var meshes = this
    .FindAllChildren(x => x.IsInGroup(groupName))
    .OfType<MeshInstance3D>();

  foreach (var meshNode in meshes) {
    // Mesh -> Surface -> Material
    var mesh = meshNode.Mesh;
    var surfaceMaterial = meshNode.Mesh.SurfaceGetMaterial(0);
    surfaceMaterial.Set("albedo_color", color);
  }
}

public void AddShaderOverlay<T>(T group, Shader shader, Dictionary<string, Variant> shaderParams = null)
  where T : Enum {

  var groupName = Enum.GetName(typeof(T), group);
  var meshes = this
    .FindAllChildren(x => x.IsInGroup(groupName))
    .OfType<MeshInstance3D>();

  var shaderMaterial = new ShaderMaterial();
  shaderMaterial.Shader = shader;
  if (shaderParams != null) {
    foreach (var pair in shaderParams) {
      shaderMaterial.SetShaderParameter(pair.Key, pair.Value);
    }
  }

  foreach (var meshNode in meshes) {
    meshNode.MaterialOverlay = shaderMaterial;
  }
}

public void RemoveShaderOverlay<T>(T group)
  where T : Enum {

  var groupName = Enum.GetName(typeof(T), group);
  var meshes = this
    .FindAllChildren(x => x.IsInGroup(groupName))
    .OfType<MeshInstance3D>();

  foreach (var meshNode in meshes) {
    meshNode.MaterialOverlay = null;
  }
}
#endregion
}