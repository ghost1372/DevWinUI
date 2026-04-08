// Copyright (c) Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
//
// This file is part of the CompositionProToolkit project: 
// https://github.com/ratishphilip/CompositionProToolkit
//
// CompositionProToolkit v1.0.1
//

using System.Reflection;

namespace DevWinUI;

/// <summary>
/// Extension methods for System.Type
/// </summary>
public static partial class TypeExtensions
{
    /// <summary>
    /// Returns an object that represents the specified public property declared by the
    /// current type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>An object that represents the specified property, if found; otherwise, null.</returns>
    public static PropertyInfo GetProperty(this Type type, string propertyName)
    {
        return type.GetTypeInfo()?.GetDeclaredProperty(propertyName);
    }

    /// <summary>
    /// Returns an object that represents the specified public method declared by the
    /// current type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns></returns>
    public static MethodInfo GetMethod(this Type type, string methodName)
    {
        return type.GetTypeInfo()?.GetDeclaredMethod(methodName);
    }

    /// <summary>
    /// Checks if the current type is a subclass of the given parentType
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="parentType">Type</param>
    /// <returns>True if it is a subclass, otherwise False</returns>
    public static bool IsSubclassOf(this Type type, Type parentType)
    {
        return type.GetTypeInfo()?.IsSubclassOf(parentType) ?? false;
    }

    /// <summary>
    /// Checks if the current type is a class.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is a class, otherwise False</returns>
    public static bool IsClass(this Type type)
    {
        return type.GetTypeInfo()?.IsClass ?? false;
    }

    /// <summary>
    /// Checks if the current type is an Enum.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is an Enum, otherwise False</returns>
    public static bool IsEnum(this Type type)
    {
        return type.GetTypeInfo()?.IsEnum ?? false;
    }

    /// <summary>
    /// Checks if the current type is a primitive type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is a primitive type, otherwise False</returns>
    public static bool IsPrimitive(this Type type)
    {
        return type.GetTypeInfo()?.IsPrimitive ?? false;
    }

    /// <summary>
    /// Returns an object from which the current type is derived.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>An object from which the current type is derived</returns>
    public static Type BaseType(this Type type)
    {
        return type.GetTypeInfo()?.BaseType;
    }

    /// <summary>
    /// Checks if the current type is a generic type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is a generic type, otherwise False</returns>
    public static bool IsGenericType(this Type type)
    {
        return type.GetTypeInfo()?.IsGenericType ?? false;
    }

    /// <summary>
    /// Checks if the current type is a public type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is a public type, otherwise False</returns>
    public static bool IsPublic(this Type type)
    {
        return type.GetTypeInfo()?.IsPublic ?? false;
    }

    /// <summary>
    /// Gets types of the generic arguments for the current type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>Array of types of the generic arguments for the current type</returns>
    public static Type[] GetGenericArguments(this Type type)
    {
        return type.GetTypeInfo()?.GenericTypeArguments;
    }

    /// <summary>
    /// Returns an object that represents the value of the specified public
    /// property declared by the current object.
    /// </summary>
    /// <param name="instance">Object</param>
    /// <param name="propertyValue">The name of the property.</param>
    /// <returns>The property value of the specified object.</returns>
    public static object GetPropertyValue(this object instance, string propertyValue)
    {
        return instance.GetType().GetTypeInfo()?.GetDeclaredProperty(propertyValue)?.GetValue(instance);
    }

    /// <summary>
    /// Returns an object that represents the value of the specified public
    /// property declared by the current object with optional index values for
    /// indexed properties.
    /// </summary>
    /// <param name="instance">Object</param>
    /// <param name="propertyValue">The name of the property.</param>
    /// <param name="indices">Optional index values for indexed properties. The indexes of indexed properties
    /// are zero-based. This value should be null for non-indexed properties.</param>
    /// <returns>The property value of the specified object.</returns>
    public static object GetPropertyValue(this object instance, string propertyValue, object[] indices)
    {
        return instance.GetType().GetTypeInfo()?.GetDeclaredProperty(propertyValue)?.GetValue(instance, indices);
    }

    /// <summary>
    /// Retrieves an object that represents this type.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>An object that represents this type</returns>
    public static TypeInfo GetTypeInfo(this Type type)
    {
        var reflectableType = type as IReflectableType;
        return reflectableType?.GetTypeInfo();
    }

    /// <summary>
    /// Checks if the current type is an anonymous class.
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>True if it is anonymous, otherwise False</returns>
    internal static bool IsAnonymous(this Type type)
    {
        if (!string.IsNullOrEmpty(type.Namespace) || !type.IsGenericType()) return false;
        return IsAnonymous(type.Name);
    }

    /// <summary>
    /// Checks if the current type name string contains characters which identify it
    /// as an anonymous class
    /// </summary>
    /// <param name="typeName">Type name in string format</param>
    /// <returns>True if it is anonymous, otherwise False</returns>
    internal static bool IsAnonymous(string typeName)
    {
        // Optimization to improve perf when called from UserCache
        return
            typeName.Length > 5 &&
            (typeName[0] == '<' && typeName[1] == '>' && (typeName[5] == 'A' && typeName[6] == 'n' || typeName.IndexOf("anon", StringComparison.OrdinalIgnoreCase) > -1) ||
             typeName[0] == 'V' && typeName[1] == 'B' && typeName[2] == '$' && typeName[3] == 'A' && typeName[4] == 'n');
    }

    /// <summary>
    /// Gets the formatted name for the current type
    /// </summary>
    /// <param name="t">Type</param>
    /// <param name="fullname">Flag to indicate whether the fullname of the
    /// type should be returned.</param>
    /// <returns>Formatted name string</returns>
    internal static string FormattedName(this Type t, bool fullname = false)
    {
        return fullname ? t.FullName : t.Name;
    }
}
