using Godot;
using System;
using System.Threading.Tasks;

namespace GoDough.Runtime;

public interface IGodotApi
{
  Shader LoadShader(string fileName);

  PackedScene LoadScene(string fileName);

  Task<Shader> LoadShaderAsync(string fileName, Action<double> progress = null);

  Task<PackedScene> LoadSceneAsync(string fileName, Action<double> progress = null);
}