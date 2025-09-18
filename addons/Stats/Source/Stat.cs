using Godot;
using System.Collections.Generic;

namespace Ritas5100.Stats;

[GlobalClass]
/// <summary>
/// Holds a value representing the stat, which can be increased or decreased with modifiers
/// </summary>
public partial class Stat : Resource
{

    private float baseValue;
    /// <value>
    /// The base value of the stat, the value of which the stat modifiers is acted upon.
    /// </value>
    /// <remarks>
    /// The stat modifiers do not act on this value. Instead, to get the value with the modifiers applied, get the FinalValue variable.
    /// </remarks>
    [Export] public float BaseValue { get => baseValue; set => baseValue = value; }

    private List<StatModifier> statModifiers;

    /// <value>
    /// The modifiers that are currently held by the stat. These modifiers will affect the final value of the stat.
    /// </value>
    public List<StatModifier> StatModifiers { get => statModifiers; set => statModifiers = value; }

    private float finalValue;

    /// <summary>
    /// The final value of the stat. This is equal to the base value with all the modifiers applied.
    /// </summary>
    public float FinalValue
    {
        get
        {
            finalValue = GetValue();
            return finalValue;
        }
    }

    /// <summary> 
    /// This empty constructor should not be used and is here due to Godot Resource requiring empty constructors.
    /// </summary>
    private Stat() : this(0, []) { }

    /// <summary>
    /// This constructor creates a Stat with a initial base value of <paramref name="_baseValue"/> and no modifiers.
    /// </summary>
    /// <param name="_baseValue">The starting value of the stat.</param>
    public Stat(float _baseValue) : this(_baseValue, []) { }

    /// <summary> 
    /// This constructor creates a Stat with a initial base value of <paramref name="_baseValue"/> 
    /// starting with <paramref name="startingModifiers"/> modifiers.
    /// </summary>
    /// <param name="_baseValue"> The starting value of the stat </param>
    /// <param name="startingModifiers"> The modifiers the stat starts with </param>
    public Stat(float _baseValue, List<StatModifier> startingModifiers)
    {
        BaseValue = _baseValue;
        StatModifiers = startingModifiers;
    }

    /// <summary>
    /// Adds <paramref name="modifier"/> to the list of modifiers.
    /// </summary>
    /// <remarks>
    /// The order of adding modifiers does not matter. Additive modifiers will be calculated before multiplicative modifiers.
    /// </remarks>
    /// <param name="modifier">The modifier to add.</param>
    public void AddModifier(StatModifier modifier)
    {
        StatModifiers.Add(modifier);
    }

    /// <summary>
    /// Appends <paramref name="modifiers"/> to the list of modifiers.
    /// </summary>
    /// <remarks>
    /// The order of adding modifiers does not matter. Additive modifiers will be calculated before multiplicative modifiers.
    /// </remarks>
    /// <param name="modifiers"> The modifier to add </param>
    public void AddModifiers(StatModifier[] modifiers)
    {
        foreach (var modifier in modifiers) AddModifier(modifier);
    }

    /// <summary>
    /// Tries to remove <paramref name="modifier"/> from the list of modifiers.
    /// </summary>
    /// <returns>
    /// A bool for whether the modifier was removed.
    /// </returns>
    /// <param name="modifier"> The modifier to remove </param>
    public bool RemoveModifier(StatModifier modifier)
    {
        return StatModifiers.Remove(modifier);
    }

    /// <summary>
    /// Removes all modifiers that belong to <paramref name="source"/> from the list of modifiers.
    /// </summary>
    /// <remarks>
    /// If <paramref name="source"/> is null, nothing is done.
    /// </remarks>
    /// <param name="source"> The source of which should have all modifiers belonging to that source be removed </param>
    public void RemoveModifierFromSource(Armour source)
    {
        if (source == null) return;
        for (int i = StatModifiers.Count - 1; i >= 0; i--)
        {
            if (StatModifiers[i].Source == source)
            {
                StatModifiers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Applies all the modifiers to the base value. Then clears the list of modifiers.
    /// </summary>
    public void ApplyModifiers()
    {
        BaseValue = FinalValue;
        StatModifiers.Clear();
    }

    /// <summary>
    /// Lists elements in the following order, each seperated by a new line:
    /// <list type="bullet">
    /// <item>
    /// <description> The base value of the Stat. </description>
    /// </item>
    /// <item>
    /// <description> All modifiers along with its source; each modifier is seperated with a new line. </description>
    /// </item>
    /// <item>
    /// <description> The final value of the Stat. </description>
    /// </item>
    /// </list>
    /// <example>
    /// Base Value: 5 <br/>
    /// +15 from Helmet <br/>
    /// *1.5 from Boots <br/>
    /// Final Value: 30
    /// </example>
    /// </summary>
    public override string ToString()
    {
        StatModifiers.Sort((mod1, mod2) => mod1.ModifierData.ModifierType.CompareTo(mod2.ModifierData.ModifierType));

        string ret = $"Base Value: {baseValue}";
        foreach (var modifier in StatModifiers)
        {
            ret += "\n" + modifier.ToString();
        }
        ret += "\n" + $"Final Value: {FinalValue}";
        return ret;
    }

    private float GetValue()
    {
        StatModifiers.Sort((mod1, mod2) => mod1.ModifierData.ModifierType.CompareTo(mod2.ModifierData.ModifierType));

        float currentValue = baseValue;
        foreach (var modifier in StatModifiers)
        {
            switch (modifier.ModifierData.ModifierType)
            {
                case StatModifierType.Add:
                    currentValue += modifier.ModifierData.Value;
                    break;
                case StatModifierType.Mult:
                    currentValue *= modifier.ModifierData.Value;
                    break;
            }
        }

        return currentValue;
    }
}
