namespace Jozzuph.EntityStats
{
    //Make two types of stat mod
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200, //doesn't adds percent vs multipying 100% 100% = 400%
        PercentMult = 300,
    }
    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType Type;
        public readonly int Order;
        public readonly object Source; //can hold and type

        public StatModifier(float value, StatModType type, int order, object source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { } //auto calls other constructor, flat mod have order 0 and % have order 1

        public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }

    }
}