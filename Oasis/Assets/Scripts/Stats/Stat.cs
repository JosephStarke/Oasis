using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jozzuph.EntityStats //Makes one type using Joseph.EntityStats
{

    [Serializable]
    public class Stat
    {
        public float BaseValue;

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers; //allows to be seen but not edited by other classes

        //retrieve final val
        public virtual float GetValue //current value after modifiers
        {
            get
            {
                if (isDirty || BaseValue != lastBaseValue)
                {
                    lastBaseValue = BaseValue;
                    recentVal = CalculateFinalValue();
                    isDirty = false;
                }
                return recentVal;
            }
        }

        //determine if we need to run to check for stat change
        protected bool isDirty = true;
        protected float recentVal;
        protected float lastBaseValue = float.MinValue; //Takes Base value into actuont in final value if it is changed

        public Stat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public Stat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true; //show is modifiers change
            statModifiers.Add(mod);
        }

        //Tell the order to aply stats
        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
            {
                return -1;
            }
            else if (a.Order > b.Order)
            {
                return 1;
            }
            return 0; //if they are equal
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true; //show is modifiers change
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

            if (numRemovals > 0)
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            statModifiers.Sort(CompareModifierOrder);

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;

                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }
            return (float)Math.Round(finalValue, 4); //rounds to 4 sig figs
        }

    }
}
