This is a small stats system made for Godot, written in C#.  

### Overview

Each stat has a base value. Each stat has a list of modifiers that will have an effect on the base value. The final value will be the base value with the list of modifiers applied.  

There are currently two types of modifiers (additive, and multiplicative), of which the additive will always be applied first.  

This system currently supports the usage of resources for stats and modifiers, and also supports the everything done in code.

### Usage

A simple example is as follows:

Include the namespace
~~~cs
using Ritas5100.Stats;
~~~

Then we can create a stat with a base value of $50$:
~~~cs
Stat MaxHealth = new Stat(50f);
~~~

To create an additive stat modifier, the following steps are needed:
~~~cs
StatModifierData modifierData = new StatModifierData(5f, StatModifierType.Add);
StatModifier modifier = new StatModifier(modifierData);
~~~
This may seem excessive, but certain features, such as [what object applied a certain modifier belongs to](#modifier-source) and the ability to use [resources for modifier data](using-resources) require it.

Now finally, we apply the modifier
~~~cs
MaxHealth.AddModifier(modifier);
~~~

To access the final value of the modifier, we do
~~~cs
GD.Print(MaxHealth.FinalValue);
~~~
of which has an expected value of $55$.

---

For a more complete example, including an implementation of equipping and unequipping armour, see the Examples folder.

---

#### Modifier Source

Todo...

#### Using Resources

Todo...

### Examples
See the Examples folder for an example without the use of resources(includes an implementation of an armour class that can be equipped or unequipped to modify certain stats)
