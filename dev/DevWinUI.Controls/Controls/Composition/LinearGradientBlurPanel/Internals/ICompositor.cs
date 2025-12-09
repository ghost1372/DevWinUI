using Windows.Graphics.Effects;

namespace DevWinUI;

internal interface ICompositor : ICompositionWrapper
{
    ISpriteVisual CreateSpriteVisual();

    ICompositionLinearGradientBrush CreateLinearGradientBrush();

    ICompositionColorGradientStop CreateColorGradientStop();

    ICompositionColorGradientStop CreateColorGradientStop(float offset, Color color);

    ICompositionEffectBrush CreateEffectBrush(IGraphicsEffect effect, string[] animatableProperties);

    ICompositionBrush CreateBackdropBrush();

    ICompositionColorBrush CreateColorBrush();

    ICompositionColorBrush CreateColorBrush(Color color);

    ICompositionPropertySet CreatePropertySet();

    IExpressionAnimation CreateExpressionAnimation();

    IExpressionAnimation CreateExpressionAnimation(string expression);
}
internal interface ICompositionWrapper
{
    object RawObject { get; }
}
internal interface ISpriteVisual : IVisual
{

    IVisualCollection Children { get; }

    ICompositionBrush? Brush { get; set; }
}
internal interface IVisual : ICompositionObject
{
    Vector2 RelativeSizeAdjustment { get; set; }
}
internal interface ICompositionObject : ICompositionWrapper, IDisposable
{
    ICompositionPropertySet Properties { get; }

    void StartAnimation(string propertyName, ICompositionAnimation animation);

    void StopAnimation(string propertyName);
}
internal interface ICompositionPropertySet : ICompositionObject
{
    void InsertColor(string propertyName, Color value);

    void InsertMatrix3x2(string propertyName, Matrix3x2 value);

    void InsertMatrix4x4(string propertyName, Matrix4x4 value);

    void InsertQuaternion(string propertyName, Quaternion value);

    void InsertScalar(string propertyName, float value);

    void InsertVector2(string propertyName, Vector2 value);

    void InsertVector3(string propertyName, Vector3 value);

    void InsertVector4(string propertyName, Vector4 value);

    CompositionGetValueStatus TryGetColor(string propertyName, out Color value);

    CompositionGetValueStatus TryGetMatrix3x2(string propertyName, out Matrix3x2 value);

    CompositionGetValueStatus TryGetMatrix4x4(string propertyName, out Matrix4x4 value);

    CompositionGetValueStatus TryGetQuaternion(string propertyName, out Quaternion value);

    CompositionGetValueStatus TryGetScalar(string propertyName, out float value);

    CompositionGetValueStatus TryGetVector2(string propertyName, out Vector2 value);

    CompositionGetValueStatus TryGetVector3(string propertyName, out Vector3 value);

    CompositionGetValueStatus TryGetVector4(string propertyName, out Vector4 value);
}
internal interface ICompositionAnimation : ICompositionObject
{
    void ClearAllParameters();

    void ClearParameter(string key);

    void SetColorParameter(string key, Color value);

    void SetMatrix3x2Parameter(string key, Matrix3x2 value);

    void SetMatrix4x4Parameter(string key, Matrix4x4 value);

    void SetQuaternionParameter(string key, Quaternion value);

    void SetReferenceParameter(string key, ICompositionWrapper compositionObject);

    void SetScalarParameter(string key, float value);

    void SetVector2Parameter(string key, Vector2 value);

    void SetVector3Parameter(string key, Vector3 value);

    void SetVector4Parameter(string key, Vector4 value);
}
internal interface IVisualCollection : ICompositionWrapper
{
    void InsertAtTop(IVisual visual);

    void RemoveAll();
}
internal interface ICompositionBrush : ICompositionObject
{
}
internal interface IExpressionAnimation : ICompositionAnimation
{
    string Expression { get; set; }
}
internal interface ICompositionColorBrush : ICompositionBrush
{
    Color Color { get; set; }
}
internal interface ICompositionEffectBrush : ICompositionBrush
{
    void SetSourceParameter(string name, ICompositionBrush source);
}
internal interface ICompositionColorGradientStop : ICompositionObject
{
    public float Offset { get; set; }

    public Color Color { get; set; }
}
internal interface ICompositionLinearGradientBrush : ICompositionBrush
{
    Vector2 StartPoint { get; set; }

    Vector2 EndPoint { get; set; }

    CompositionMappingMode MappingMode { get; set; }

    ICompositionColorGradientStopCollection ColorStops { get; }
}
internal enum CompositionMappingMode
{
    Absolute,
    Relative
}
internal interface ICompositionColorGradientStopCollection : ICompositionWrapper
{
    void Add(ICompositionColorGradientStop stop);

    void Remove(ICompositionColorGradientStop stop);

    void Clear();

    ICompositionColorGradientStop this[int index] { get; }
}
internal enum CompositionGetValueStatus
{
    Succeeded,
    TypeMismatch,
    NotFound
}
