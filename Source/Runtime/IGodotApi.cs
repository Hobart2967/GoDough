using Godot;

namespace GoDough.Runtime {
  public interface IGodotApi {
    Shader LoadShader(string fileName);
    PackedScene LoadScene(string fileName);
  }
}