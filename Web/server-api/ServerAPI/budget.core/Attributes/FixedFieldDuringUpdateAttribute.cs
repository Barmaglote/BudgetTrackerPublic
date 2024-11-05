namespace budget.core.Attributes {

  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  sealed class FixedFieldDuringUpdateAttribute : Attribute {

    public FixedFieldDuringUpdateAttribute() {
    }
  }
}
