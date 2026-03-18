namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Custom attribute to specify sort order for enum or class members.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class OrderAttribute : Attribute
{
    public OrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}
