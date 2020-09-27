using System;
using System.Collections.Generic;
using System.Reflection;

namespace BackBack
{
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
                targetProperty?.SetValue(target, property.GetValue(source));
            }
        }

        private static PropertyInfo[] GetProperties(Type t) => t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        private static PropertyInfo[] GetProperties<T>() => GetProperties(typeof(T));
    }
}
