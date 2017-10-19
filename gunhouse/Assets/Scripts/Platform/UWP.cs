using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWP : MonoBehaviour {

}

#if UNITY_WSA || UNITY_WSA_10_0

namespace System.Reflection
{
    public static class TypeExtendedMethods
    {
        public static FieldInfo GetField(this Type type, string fieldName)
        {
            if (type == null || string.IsNullOrEmpty(fieldName)) return null;

            var currentType = type;
            do {
                var typeInfo = currentType.GetTypeInfo();
                var declaredField = typeInfo.GetDeclaredField(fieldName);
                if (declaredField != null) return declaredField;
                currentType = typeInfo.BaseType;
            } while (currentType != null);
            return null;
        }
    }
}

#endif