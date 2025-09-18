using Godot;

namespace Ritas5100.Stats;

/// <summary>
/// An enum representing the method of which the stat modifier is applied to a Stat.
/// </summary>
public enum StatModifierType
{
    /// <summary> Adds the value directly to the Stat. Always applied first </summary>
    Add,

    /// <summary> Multiples the value directly to the Stat. Always applied last </summary>
    Mult
}

[GlobalClass]
/// <summary> Stores data for a stat modifier </summary>
public partial class StatModifierData : Resource
{
    /// <summary>
    /// The type of the modifier. Affects calculation done on the Stat from the Stat modifier.
    /// </summary>
    [Export] public StatModifierType ModifierType { get; set; }

    /// <summary>
    /// The value of the modifier.
    /// </summary>
    [Export] public float Value { get; set; }

    private StatModifierData() : this(StatModifierType.Add, 0f) { }
    public StatModifierData(StatModifierType _statModifierType, float _value)
    {
        ModifierType = _statModifierType;
        Value = _value;
    }

}
