using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ScriptLib;

public static class Util
{
    public static bool IsBetween<T>(this T item, T start, T end)
    {
        return Comparer<T>.Default.Compare(item, start) >= 0
            && Comparer<T>.Default.Compare(item, end) <= 0;
    }
}

namespace ScriptViz.Util
{
    public static class EnumUtil
    {
        public static string GetEnumDescription(Enum e)
        {
            var fi = e.GetType().GetField(e.ToString());

            var attr = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attr.Length > 0) ? ((DescriptionAttribute[])attr)[0].Description : e.ToString();
        }

        public static int GetEnumValueFromDescription(string desc)
        {
            foreach (var field in typeof(HitboxEffectTypeEnum).GetFields())
            {

                if (field.GetCustomAttribute(typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
                {
                    if (attr.Description == desc)
                        return (int)field.GetValue(null);
                }
                else
                {
                    if (field.Name == desc)
                        return (int)field.GetValue(null);
                }
            }
            throw new ArgumentException("Bad value.", "Description of the Enum.");
        }
    }
}
