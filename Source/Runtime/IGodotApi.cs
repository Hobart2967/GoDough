using Godot;

namespace GoDough.Runtime {
  public interface IGodotApi {
    PackedScene LoadScene(string fileName);
  }
}