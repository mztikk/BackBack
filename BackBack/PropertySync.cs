using System;
using System.Collections.Generic;
using System.Reflection;
using RFReborn.Internals;

namespace BackBack
{
    //[Obsolete("Use SourceGenerator", true)]
    internal static class PropertySync
    {
        internal static void Sync<sourceType, targetType>(sourceType source, targetType target, HashSet<string>? ignores)
        {
            Type targetT = typeof(targetType);

            foreach (PropertyInfo property in GetProperties<sourceType>())
            {
                if (ignores?.Contains(property.Name) == true)
                {
                    continue;
                }

                PropertyInfo targetProperty = targetT.GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
                if (targetProperty?.SetMethod is { })
                {
                    targetProperty?.SetValue(target, property.GetValue(source));
                }
            }
        }

        internal static void Sync<targetType>(Dictionary<string, string> source, targetType target, HashSet<string>? ignores)
        {
            Type targetT = typeof(targetType);

            foreach (string item in source.Keys)
            {
                if (ignores?.Contains(item) == true)
                {
                    continue;
                }

                PropertyInfo targetProperty = targetT.GetProperty(item, BindingFlags.Instance | BindingFlags.Public);
                if (targetProperty?.SetMethod is { })
                {
                    targetProperty?.SetValue(target, DynamicProperty.ConvertValue(targetProperty.PropertyType, source[item]));
                }
            }
        }

        internal static void Sync<sourceType>(sourceType source, Dictionary<string, string> target, HashSet<string>? ignores)
        {
            foreach (PropertyInfo property in GetProperties<sourceType>())
            {
                if (ignores?.Contains(property.Name) == true || !target.ContainsKey(property.Name))
                {
                    continue;
                }

                target[property.Name] = property.GetValue(source).ToString();
            }
        }

        private static PropertyInfo[] GetProperties(Type t) => t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        private static PropertyInfo[] GetProperties<T>() => GetProperties(typeof(T));
    }
}
