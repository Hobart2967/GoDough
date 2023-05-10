using System;

namespace GoDough.Visuals.Attributes {
  public class UniqueNode : Attribute {
    public string SpecificName { get; }

    public UniqueNode(string? specificName = null) {
      this.SpecificName = specificName;
    }
  }
}