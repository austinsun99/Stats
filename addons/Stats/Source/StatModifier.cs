namespace Ritas5100.Stats;
/// <summary>
/// Represents a modifier that to be applied to a Stat.
/// </summary>
public class StatModifier
{

    /// <summary>
    /// The data for this stat modifiers; includes its type and value
    /// </summary>
    public StatModifierData ModifierData { get; set; }

    /// <summary>
    /// The source of the modifier. Is used to identify where a certain modifier came from. Can be null.
    /// </summary>
    public IStatModifierObject Source { get; set; }

    /// <summary>
    /// This constructor sets <paramref name="_modifierData"/> with a null source.
    /// </summary>
    public StatModifier(StatModifierData _modifierData) : this(_modifierData, null) { }

    /// <summary>
    /// This constructor sets <paramref name="_modifierData"/> with <paramref name="_source"/>.
    /// </summary>
    /// <param name="_source">The source of the modifier</param>
    public StatModifier(StatModifierData _modifierData, IStatModifierObject _source)
    {
        ModifierData = _modifierData;
        Source = _source;
    }

    /// <summary>
    /// Returns a string that includes the type of modifier, its value, and the source if not null
    /// </summary>
    /// <example>
    /// x1.5 from Helmet
    /// </example>
    public override string ToString()
    {
        string ret = ModifierData.ModifierType switch
        {
            StatModifierType.Add => ModifierData.Value >= 0 ? "+" : "",
            StatModifierType.Mult => "x",
            _ => "",
        };

        if (Source != null) ret += $"{ModifierData.Value} from {Source.GetSourceName()}";
        return ret;
    }

}
