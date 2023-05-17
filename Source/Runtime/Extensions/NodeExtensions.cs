using System;
using System.Threading.Tasks;
using Godot;

namespace GoDough.Runtime.Extensions {
  public static class NodeExtensions {
    public static async Task WaitForNextFrame(this Node node, Action value) {
      await node.ToSignal(node.GetTree(), "process_frame");
      value();
    }

    public static async Task WaitForNextFrame(this Node node) {
      await node.ToSignal(node.GetTree(), "process_frame");
    }
  }
}