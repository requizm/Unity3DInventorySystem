using System;
using System.Reflection;

namespace Tests
{
    public static class AssemblyUtils
    {
        public static void SetField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new Exception($"Field {fieldName} not found in {obj.GetType()}");
            }
            field.SetValue(obj, value);
        }
        
        public static T GetField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new Exception($"Field {fieldName} not found in {obj.GetType()}");
            }
            return (T)field.GetValue(obj);
        }
    }
}
