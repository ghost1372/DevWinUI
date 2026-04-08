namespace DevWinUI;

internal partial class CompositionAnimationProxy : CompositionObjectProxy, ICompositionAnimation
{
    public CompositionAnimationProxy(CompositionAnimation animation)
        : base(animation) { }

    public void ClearAllParameters()
    {
        ((CompositionAnimation)RawObject).ClearAllParameters();
    }

    public void ClearParameter(string key)
    {
        ((CompositionAnimation)RawObject).ClearParameter(key);
    }

    public void SetColorParameter(string key, Color value)
    {
        ((CompositionAnimation)RawObject).SetColorParameter(key, value);
    }

    public void SetMatrix3x2Parameter(string key, Matrix3x2 value)
    {
        ((CompositionAnimation)RawObject).SetMatrix3x2Parameter(key, value);
    }

    public void SetMatrix4x4Parameter(string key, Matrix4x4 value)
    {
        ((CompositionAnimation)RawObject).SetMatrix4x4Parameter(key, value);
    }

    public void SetQuaternionParameter(string key, Quaternion value)
    {
        ((CompositionAnimation)RawObject).SetQuaternionParameter(key, value);
    }

    public void SetReferenceParameter(string key, ICompositionWrapper compositionObject)
    {
        ((CompositionAnimation)RawObject).SetReferenceParameter(key, (CompositionObject)compositionObject.RawObject);
    }

    public void SetScalarParameter(string key, float value)
    {
        ((CompositionAnimation)RawObject).SetScalarParameter(key, value);
    }

    public void SetVector2Parameter(string key, Vector2 value)
    {
        ((CompositionAnimation)RawObject).SetVector2Parameter(key, value);
    }

    public void SetVector3Parameter(string key, Vector3 value)
    {
        ((CompositionAnimation)RawObject).SetVector3Parameter(key, value);
    }

    public void SetVector4Parameter(string key, Vector4 value)
    {
        ((CompositionAnimation)RawObject).SetVector4Parameter(key, value);
    }
}
