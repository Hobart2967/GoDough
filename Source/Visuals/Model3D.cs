using System.Collections.Generic;
using System.Linq;
using Godot;
using GoDough.Visuals.Extensions;
using BoundingBox = Godot.Aabb;

namespace GoDough.Visuals {
  public partial class Model3D : Node3D {
    #region Private Fields
    private MeshInstance3D _visualBoundingBox;
    #endregion

    #region Properties
    private Vector3 _size = Vector3.Inf;
    public Vector3 Size {
      get {
        if (this._size == Vector3.Inf) {
          UpdateBoundingBox();
        }
        return this._size;
      }
    }

    private BoundingBox? _boundingBox = null;
    public BoundingBox BoundingBox {
      get {
        if (this._size == Vector3.Inf) {
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
      }
    }


    public string GroupName { get; set; } = "Mesh";

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
      if (this._size == Vector3.Inf) {
        return;
      }

      UpdateBoundingBox();
    }
    #endregion
  }
}