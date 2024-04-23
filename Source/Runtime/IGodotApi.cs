using System;
using System.Threading.Tasks;
using Godot;

namespace GoDough.Runtime {
  public interface IGodotApi {
    Shader LoadShader(string fileName);
    PackedScene LoadScene(string fileName);

    TaskWithProgress<Shader> LoadShaderAsync(string fileName);
    TaskWithProgress<PackedScene> LoadSceneAsync(string fileName);
  }
}