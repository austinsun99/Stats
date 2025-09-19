This is a small stats system made for Godot, written in C#.

### Overview

Each stat has a base value. Each stat has a list of modifiers that will have an effect on the base value. The final value will be the base value with the list of modifiers applied.

There are currently two types of modifiers (additive, and multiplicative), of which the additive will always be applied first.

This system currently supports the usage of resources for stats and modifiers, and also supports everything done in code.

Also support for [displaying data](#output) with ToString() override

### Basic usage

A simple example is as follows:

Include the namespace

```cs
using Ritas5100.Stats;
```

Then we can create a stat with a base value of $50$:

```cs
Stat MaxHealth = new Stat(50f);
```

To create an additive stat modifier, the following steps are needed:

```cs
StatModifierData modifierData = new StatModifierData(5f, StatModifierType.Add);
StatModifier modifier = new StatModifier(modifierData);
```

This may seem excessive, but certain features, such as [what object applied a certain modifier belongs to](#modifier-source) and the ability to use [resources for modifier data](#using-resources) require it.

Now finally, we apply the modifier

```cs
MaxHealth.AddModifier(modifier);
```

To access the final value of the modifier, we do

```cs
GD.Print(MaxHealth.FinalValue);
```

of which has an expected value of $55$.

\*For an example of how to implement an armour system, see the [Modifier Source and Armour Example](#modifier-source-and-example-of-armour-system)

### Modifier Source and Example of Armour System with ToString()

When creating a stat modifier, you can specify a source. This is best demonstrateted with an example of how to implement an example armour system. (Files with documentation can be found in [Examples](addons/Stats/Example)).

We first start with an Armour resource class, implemented as follows:

```cs
using System.Linq;
using Godot;
using Godot.Collections;

using Ritas5100.Stats;

[GlobalClass]
public partial class Armour : Resource, IStatModifierObject
{

    [Export] private Dictionary<Stat, Array<StatModifierData>> modifiersToApply;
    [Export] public string ItemName { get; set; }

    private Armour() : this([], "") { }
    public Armour(Dictionary<Stat, Array<StatModifierData>> _modifiersToApply, string _itemName)
    {
        modifiersToApply = _modifiersToApply;
        ItemName = _itemName;
    }

    public void AddModifiers()
    {
        foreach (var modifierToApply in modifiersToApply)
        {
            // Here, we create an array of StatModifiers. Notice the second value in the constructor, which
            // allows us to specify a source object. The source object must implement IStatModifierObject.
            // In this example, the source object is this piece of armour, since the armour is providing the stat boosts
            // This source object allows us to easily remove all stat modifiers associated with this IStatModifierObject.
            modifierToApply.Key.AddModifiers(
                [.. modifierToApply.Value.Select(modifier => new StatModifier(modifier, this))]);
        }
    }

    public void RemoveModifiers()
    {
        foreach (var modifierToApply in modifiersToApply)
        {
            modifierToApply.Key.RemoveModifierFromSource(this);
        }
    }

    public string GetSourceName()
    {
        return ItemName;
    }
}
```

In another script, we make use of this script as follows.

```cs
        // Initialize Armour (IStatModifierObject)s
        Armour Boots;
        Armour Helmet;

        // Armour stats to change;

        // The boots will add 5 to the max health.
        Dictionary<Stat, Array<StatModifierData>> bootsStatsToModify = new()
        {
            {MaxHealth, [new StatModifierData(StatModifierType.Add, 5f)]},
        };

        Dictionary<Stat, Array<StatModifierData>> helmetStatsToModify = new()
        {
            // Here, the helmet will add a modifier which adds 5 to the MaxHealth. It also adds a modifier which multiples the MaxHealth by 2.
            // StatModifiers of type Mult will always be computed last, no matter the order. In this example, even though Mult is added
            // before the Add modifier, it will be applied last.
            { MaxHealth,
            [
                new StatModifierData(StatModifierType.Mult, 2f),
                new StatModifierData(StatModifierType.Add, 5f)
            ]},

            // The helmet also affects the Shield, however, it decreases the shield by 4.
            { Shield, [new StatModifierData(StatModifierType.Add, -4f)] }
        };

        // Here we create the Armour objects with its statsToModify.
        Boots = new Armour(bootsStatsToModify, "Boots");
        Helmet = new Armour(helmetStatsToModify, "Helmet");

        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 50");
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print($"Expected: 30");

        Boots.AddModifiers(); // Add all modifiers associated with the boots.

        // Boots add 5 to the MaxHealth. MaxHealth should now be at 50 + 5 = 55
        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 55");

        // Shield should be unchanged
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print($"Expected: 30");

        Helmet.AddModifiers();

        // Helmets add 5 to the MaxHealth, and adds a 2x Mult to MaxHealth
        // ((50 + 5) + 5) * 2 = 120
        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 120");

        // Helmets also subtracts 4 from the Shield, hence Shield should be at 50 - 4 = 46
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print($"Expected: 26");

        Boots.RemoveModifiers();

        // Removing the modifier should make MaxHealth equal to (50 + 5) * 2 = 110. 10 is removed since the boots remove 5 but also the
        // 2x Mult from the helmet is also now weaker.
        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 110");
```

To make use of object sources, here is an example of removing the effect of the helmet from MaxHealth.

**Note: The expected values are a continuation of the above script.**

```cs
        // We remove all the modifiers from max health that were provided by helmet.
        // Here, the helmet still has an effect on Shield, but not anymore on MaxHealth.

        MaxHealth.RemoveModifierFromSource(Helmet);

        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 50"); // Since MaxHealth has no modifiers now.
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print("Expected: 26"); // Should remain unchanged
```

#### Here is an example of ToString():

**Note: This code is a continuation of the 2 above script.**

```cs
        // We remove all the modifiers from max health that were provided by helmet.
        // Here, the helmet still has an effect on Shield, but not anymore on MaxHealth.

        MaxHealth.RemoveModifierFromSource(Helmet);

        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 50"); // Since MaxHealth has no modifiers now.
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print("Expected: 26"); // Should remain unchanged
```

#### Output:

```
Base Value: 50
+5 from Helmet
+5 from Boots
x2 from Helmet
Final Value: 120
```

### Using Resources

Todo...

### Examples

See the Examples folder for an example without the use of resources(includes an implementation of an armour class that can be equipped or unequipped to modify certain stats)
