using Godot;
using Godot.Collections;
using Ritas5100.Stats;

// This example is entirely written in code and does not make use of Resources.
public partial class StatsTestNoResource : Node
{

    // Initialize Stats
    private Stat MaxHealth = new(50f);
    private Stat Shield = new(30f);

    public override void _Ready()
    {
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

        // We remove all the modifiers from max health that were provided by helmet.
        // Here, the helmet still has an effect on Shield, but not anymore on MaxHealth.

        MaxHealth.RemoveModifierFromSource(Helmet);

        GD.Print($"Current MaxHealth: {MaxHealth.FinalValue}");
        GD.Print($"Expected: 50"); // Since MaxHealth has no modifiers now.
        GD.Print($"Current Shield: {Shield.FinalValue}");
        GD.Print("Expected: 26"); // Should remain unchanged

        // Reset all the modifiers
        Helmet.RemoveModifiers();
        Boots.RemoveModifiers();

        // Add
        Helmet.AddModifiers();
        Boots.AddModifiers();

        GD.Print(MaxHealth.ToString());
    }
}
