using System.Reflection;

namespace DevWinUI;

/// <summary>
/// Helper class for KeyFrameAnimation&lt;T&gt;
/// </summary>
internal static partial class KeyFrameAnimationHelper
{
    #region Fields

    private static readonly Dictionary<Type, Type> AnimationTypes;
    private static readonly Dictionary<Type, MethodInfo> InitMethods;

    #endregion

    #region Public Field(s)

    public static readonly Dictionary<Type, MethodInfo> InsertMethods;

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    static KeyFrameAnimationHelper()
    {
        AnimationTypes = new Dictionary<Type, Type>()
        {
            [typeof(Color)] = typeof(ColorKeyFrameAnimation),
            [typeof(Quaternion)] = typeof(QuaternionKeyFrameAnimation),
            [typeof(float)] = typeof(ScalarKeyFrameAnimation),
            [typeof(Vector2)] = typeof(Vector2KeyFrameAnimation),
            [typeof(Vector3)] = typeof(Vector3KeyFrameAnimation),
            [typeof(Vector4)] = typeof(Vector4KeyFrameAnimation),
            [typeof(CompositionPath)] = typeof(PathKeyFrameAnimation)
        };

        // Get all CreateXXXKeyFrameAnimation methods
        InitMethods = typeof(Compositor).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.Name.StartsWith("Create") && m.Name.EndsWith("KeyFrameAnimation"))
            .ToDictionary(m => m.ReturnType, m => m);

        // Get all InsertKeyFrame methods for supported types
        InsertMethods = new Dictionary<Type, MethodInfo>();

        foreach (var animationType in AnimationTypes)
        {
            // Get the InsertKeyFrame method (of the KeyFrameAnimation matching 
            // the animationType.Key) which take 3 parameters
            var methodInfo = animationType.Value.GetMethods(BindingFlags.Public |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Static)
                .FirstOrDefault(m => m.Name.Equals("InsertKeyFrame") &&
                                     (m.GetParameters().Length == 3));

            InsertMethods[animationType.Key] = methodInfo;
        }
    }

    #endregion

    #region Internal APIs

    /// <summary>
    /// Creates the XXXKeyFrameAnimation based on the specified type
    /// </summary>
    /// <typeparam name="T">Type of property being animated by the KeyFrameAnimation</typeparam>
    /// <param name="compositor">Compositor</param>
    /// <returns>KeyFrameAnimation</returns>
    internal static KeyFrameAnimation CreateAnimation<T>(Compositor compositor)
    {
        if (compositor == null)
        {
            throw new ArgumentNullException(nameof(compositor));
        }

        var animationType = AnimationTypes[typeof(T)];
        if (InitMethods.ContainsKey(animationType))
        {
            return (KeyFrameAnimation)InitMethods[animationType].Invoke(compositor, null);
        }

        return null;
    }

    #endregion
}