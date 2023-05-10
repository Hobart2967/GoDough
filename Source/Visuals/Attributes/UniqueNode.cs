using System;

namespace GoDough.Visuals.Attributes {
  public class UniqueNode : Attribute {
    public string SpecificName { get; }
    public bool IgnoreInexistent { get; private set; }

    public UniqueNode(string? specificName = null, bool ignoreInexistent = false) {
      this.SpecificName = specificName;
      this.IgnoreInexistent = ignoreInexistent;
    }
  }
}