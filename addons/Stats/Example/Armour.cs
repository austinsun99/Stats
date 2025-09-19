using System.Linq;
using Godot;
using Godot.Collections;

using Ritas5100.Stats;

[GlobalClass]

/// <summary>
/// This is an example of an item that changes stats.
/// All entities that change stats should implement IStatModifierObject
/// </summary>
public partial class Armour : Resource, IStatModifierObject
{

    [Export] private Dictionary<Stat, Array<StatModifierData>> modifiersToApply;
    [Export] public string ItemName { get; set; }

    private Armour() : this([], "") { }
    /// <summary>
    /// Constructor that sets the modifiers to apply, and also sets the name of the source object (to be used for ToString() in the StatModifier class).
    /// </summary>
    /// <remarks>
    /// The modifiers to apply are not applied immediately. The user must call AddModifiers() to apply the modifiers.
    /// </remarks>
    /// <param name="_modifiersToApply">The modifiers to be applied when AddModifiers() is called. The key represents Stat to apply the
    /// modifiers to, the Array of StatModifiers represent the modifiers to apply to that Stat.</param>
    /// <param name="_sourceName">The name of the source object.</param>
    public Armour(Dictionary<Stat, Array<StatModifierData>> _modifiersToApply, string _itemName)
    {
        modifiersToApply = _modifiersToApply;
        ItemName = _itemName;
    }

    /// <summary>
    /// Applies all the modifiers to all the stats
    /// </summary>
    public void AddModifiers()
    {
        foreach (var modifierToApply in modifiersToApply)
        {
            // Here, we create an array of StatModifiers. Notice the second value in the constructor, which
            // allows us to specify a source object. The source object must implement IStatModifierObject.
            // This source object allows us to easily remove all stat modifiers associated with this IStatModifierObject.
            modifierToApply.Key.AddModifiers([.. modifierToApply.Value.Select(modifier => new StatModifier(modifier, this))]);
        }
    }

    /// <summary>
    /// Removes all the modifiers from all the stats
    /// </summary>
    public void RemoveModifiers()
    {
        foreach (var modifierToApply in modifiersToApply)
        {
            modifierToApply.Key.RemoveModifierFromSource(this);
        }
    }

    // This function is required when implementing IStatModifierObject and is used solely for ToString().
    public string GetSourceName()
    {
        return ItemName;
    }
}
