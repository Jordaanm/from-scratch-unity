using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FromScratch.Inventory
{
    [Serializable]
    public class ConsumptionData
    {
        public string consumptionEffect;
        public int calories;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ConsumeEffectAttribute : Attribute
    {
        public string name;
    }

    public class ConsumptionEffects
    {

        private static Dictionary<string, MethodInfo> effects;

        private static void BuildEffectDB()
        {
            effects = new Dictionary<string, MethodInfo>();
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .SelectMany(a => a.GetMethods())
                .Where(t => t.IsDefined(typeof(ConsumeEffectAttribute), false))
                .ToList()
                .ForEach(info =>
                {
                    var attr = info
                        .GetCustomAttributes(typeof(ConsumeEffectAttribute), false)
                        .Cast<ConsumeEffectAttribute>()
                        .First();

                    if (attr != null)
                    {
                        effects.Add(attr.name, info);
                    }
                });
        } 
        
        public static MethodInfo Get(string name)
        {
            if(effects == null) { BuildEffectDB(); }
            return effects[name];
        }

        [ConsumeEffect(name="food")]
        public static void EatFood(Character.Character character, ItemData item)
        {
            Debug.LogFormat("Nom Nom Nom {0}", item.name);
        }
        
    }
}