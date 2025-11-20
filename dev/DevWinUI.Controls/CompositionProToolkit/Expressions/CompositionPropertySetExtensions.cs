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
/// Extension methods for CompositionPropertySet
/// </summary>
public static partial class CompositionPropertySetExtensions
{
    #region Fields

    private static readonly Dictionary<Type, MethodInfo> InsertMethods;
    private static readonly Dictionary<Type, MethodInfo> TryGetMethods;

    #endregion

    #region Static Constructor

    /// <summary>
    /// Static Ctor
    /// </summary>
    static CompositionPropertySetExtensions()
    {
        // Get all the InsertXXX methods
        InsertMethods = typeof(CompositionPropertySet)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("Insert"))
            .ToDictionary(m => m.GetParameters()[1].ParameterType,
                m => m);
        // Get all the TryGetXXX methods
        TryGetMethods = typeof(CompositionPropertySet)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("TryGet"))
            .ToDictionary(m => m.GetParameters()[1].ParameterType.GetElementType(),
                m => m);
    }

    #endregion

    #region Extension methods

    /// <summary>
    /// Inserts a key and its corresponding value in the CompositionPropertySet
    /// </summary>
    /// <typeparam name="T">Type of the object to be inserted in the CompositionPropertySet</typeparam>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key for the object to be inserted in the CompositionPropertySet</param>
    /// <param name="input">Object to be inserted in the CompositionPropertySet</param>
    public static void Insert<T>(this CompositionPropertySet propertySet, string key, object input)
    {
        var type = typeof(T);
        var parameter = input;

        // Can the type be converted to float?
        if (CompositionExpressionEngine.Floatables.Contains(type))
        {
            type = typeof(float);
            parameter = Convert.ToSingle(parameter);
        }

        // Find matching InsertXXX method for the given type or if there
        // is no InsertXXX method directly matching the parameter type, then
        // find if the parameter type derives from any of the types which are the 
        // keys in the InsertMethods dictionary
        var methodKey = InsertMethods.Keys.FirstOrDefault(t => (t == type) || t.IsAssignableFrom(type));

        // If no matching method is found, then raise an exception
        if (methodKey == null)
        {
            throw new ArgumentException($"No suitable method was found to set the key '{key}' " +
                                        $"for the value of type '{type}' in the CompositionPropertySet!");
        }

        // Once a matching Insertxxx method is found, Invoke it!
        InsertMethods[methodKey].Invoke(propertySet, new[] { key, parameter });
    }

    #region GetXXX Methods

    /// <summary>
    /// Retrieves an object for the given key, from the CompositionPropertySet
    /// </summary>
    /// <typeparam name="T">Type of the object to retrieve</typeparam>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>The retrieved object</returns>
    public static T Get<T>(this CompositionPropertySet propertySet, string key)
    {
        var type = typeof(T);
        while (!type.IsPublic())
        {
            type = type.BaseType();
        }

        // Find matching TryGetXXX method for the given type or if there
        // is no TryGetXXX method directly matching the parameter type, then
        // find if the parameter type derives from any of the types which are the 
        // keys in the TryGetMethods dictionary
        var methodKey = TryGetMethods.Keys.FirstOrDefault(t => (t == type) || t.IsAssignableFrom(type));

        // If no matching method is found, then raise an exception
        if (methodKey == null)
        {
            throw new ArgumentException($"No suitable method was found to obtain the value of type '{type}' " +
                                        $"for the key '{key}' in the CompositionPropertySet!");
        }

        var result = default(T);
        // Once a matching TryGetXXX method is found, Invoke it!
        var methodResult =
            (CompositionGetValueStatus)TryGetMethods[methodKey].Invoke(propertySet, new object[] { key, result });

        switch (methodResult)
        {
            case CompositionGetValueStatus.Succeeded:
                return result;
            case CompositionGetValueStatus.TypeMismatch:
                throw new ArgumentException($"The key \'{key}\' does not return data of type "
                                            + $"\'{type.FullName}\' in the CompositionPropertySet!");
            case CompositionGetValueStatus.NotFound:
                throw new ArgumentException($"The key \'{key}\' was not found in the CompositionPropertySet!");
        }

        throw new ArgumentException($"The key \'{key}\' was not found in the CompositionPropertySet!");
    }

    /// <summary>
    /// Retrieves a Boolean value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Boolean</returns>
    public static bool GetBoolean(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<bool>(key);
    }

    /// <summary>
    /// Retrieves a Color value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Color</returns>
    public static Color GetColor(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Color>(key);
    }

    /// <summary>
    /// Retrieves a Matrix3x2 value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Matrix3x2</returns>
    public static Matrix3x2 GetMatrix3x2(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Matrix3x2>(key);
    }

    /// <summary>
    /// Retrieves a Matrix4x4 value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Matrix4x4</returns>
    public static Matrix4x4 GetMatrix4x4(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Matrix4x4>(key);
    }

    /// <summary>
    /// Retrieves a Quaternion value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Quaternion</returns>
    public static Quaternion GetQuaternion(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Quaternion>(key);
    }

    /// <summary>
    /// Retrieves a Scalar value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Scalar</returns>
    public static float GetScalar(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<float>(key);
    }

    /// <summary>
    /// Retrieves a Vector2 value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Vector2</returns>
    public static Vector2 GetVector2(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Vector2>(key);
    }

    /// <summary>
    /// Retrieves a Vector3 value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Vector3</returns>
    public static Vector3 GetVector3(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Vector3>(key);
    }

    /// <summary>
    /// Retrieves a Vector4 value of the property defined in 
    /// the CompositionPropertySet using the given key
    /// </summary>
    /// <param name="propertySet">CompositionPropertySet</param>
    /// <param name="key">Key of the object to retrieve</param>
    /// <returns>Vector4</returns>
    public static Vector4 GetVector4(this CompositionPropertySet propertySet, string key)
    {
        return propertySet.Get<Vector4>(key);
    }

    #endregion

    #endregion

    #region APIs

    /// <summary>
    /// Converts given object into a CompositionPropertySet
    /// </summary>
    /// <param name="input">Object to convert</param>
    /// <param name="compositor">Compositor</param>
    /// <returns>CompositionPropertySet</returns>
    public static CompositionPropertySet ToPropertySet(object input, Compositor compositor)
    {
        var propertySet = compositor.CreatePropertySet();

        foreach (var property in input.GetType().GetTypeInfo().DeclaredProperties)
        {
            var type = property.PropertyType;
            var parameter = property.GetValue(input);

            // Can the type be converted to float?
            if (CompositionExpressionEngine.Floatables.Contains(type))
            {
                type = typeof(float);
                parameter = Convert.ToSingle(parameter);
            }

            // Find matching InsertXXX method for the given type or if there
            // is no InsertXXX method directly matching the parameter type, then
            // find if the parameter type derives from any of the types which are the 
            // keys in the InsertMethods dictionary
            var methodKey = InsertMethods.Keys.FirstOrDefault(t => (t == type) || t.IsAssignableFrom(type));

            // If no matching method is found, then raise an exception
            if (methodKey == null)
            {
                throw new ArgumentException($"No suitable method was found to set the key '{property.Name}' " +
                                            $"for the value of type '{type}' in the CompositionPropertySet!");
            }

            // Once a matching Insertxxx method is found, Invoke it!
            InsertMethods[methodKey].Invoke(propertySet, new[] { property.Name, parameter });
        }

        return propertySet;
    }

    #endregion
}
