using CompositionNS = Microsoft.UI.Composition;

namespace DevWinUI;

internal partial class CompositionPropertySetProxy : CompositionObjectProxy, ICompositionPropertySet
{
    public CompositionPropertySetProxy(CompositionPropertySet propertySet)
        : base(propertySet) { }

    public void InsertColor(string propertyName, Color value)
    {
        ((CompositionPropertySet)RawObject).InsertColor(propertyName, value);
    }

    public void InsertMatrix3x2(string propertyName, Matrix3x2 value)
    {
        ((CompositionPropertySet)RawObject).InsertMatrix3x2(propertyName, value);
    }

    public void InsertMatrix4x4(string propertyName, Matrix4x4 value)
    {
        ((CompositionPropertySet)RawObject).InsertMatrix4x4(propertyName, value);
    }

    public void InsertQuaternion(string propertyName, Quaternion value)
    {
        ((CompositionPropertySet)RawObject).InsertQuaternion(propertyName, value);
    }

    public void InsertScalar(string propertyName, float value)
    {
        ((CompositionPropertySet)RawObject).InsertScalar(propertyName, value);
    }

    public void InsertVector2(string propertyName, Vector2 value)
    {
        ((CompositionPropertySet)RawObject).InsertVector2(propertyName, value);
    }

    public void InsertVector3(string propertyName, Vector3 value)
    {
        ((CompositionPropertySet)RawObject).InsertVector3(propertyName, value);
    }

    public void InsertVector4(string propertyName, Vector4 value)
    {
        ((CompositionPropertySet)RawObject).InsertVector4(propertyName, value);
    }

    public CompositionGetValueStatus TryGetColor(string propertyName, out Color value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetColor(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetMatrix3x2(string propertyName, out Matrix3x2 value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetMatrix3x2(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetMatrix4x4(string propertyName, out Matrix4x4 value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetMatrix4x4(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetQuaternion(string propertyName, out Quaternion value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetQuaternion(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetScalar(string propertyName, out float value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetScalar(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetVector2(string propertyName, out Vector2 value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetVector2(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetVector3(string propertyName, out Vector3 value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetVector3(propertyName, out value));
    }

    public CompositionGetValueStatus TryGetVector4(string propertyName, out Vector4 value)
    {
        return MapCompositionGetValueStatus(((CompositionPropertySet)RawObject).TryGetVector4(propertyName, out value));
    }

    private static CompositionGetValueStatus MapCompositionGetValueStatus(CompositionNS.CompositionGetValueStatus status)
    {
        switch (status)
        {
            case CompositionNS.CompositionGetValueStatus.Succeeded:
                return CompositionGetValueStatus.Succeeded;

            case CompositionNS.CompositionGetValueStatus.TypeMismatch:
                return CompositionGetValueStatus.TypeMismatch;

            case CompositionNS.CompositionGetValueStatus.NotFound:
            default:
                return CompositionGetValueStatus.NotFound;
        }
    }
}
