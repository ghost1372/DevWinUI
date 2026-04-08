namespace DevWinUI;
internal partial class ParticleModel
{
    private static readonly Random rnd = new Random();

    private Vector3 _Container;
    private Vector3 _Offset;
    private Vector3 _Head;
    private int _Step;

    public int Step { get => _Step; }
    public Vector3 Container { get => _Container; internal set => _Container = value; }
    public Vector3 Offset { get => _Offset; internal set => _Offset = value; }
    public Vector3 Head { get => _Head; }

    public ParticleModel(Vector3 Container, Vector3 Head, Vector3 Offset)
    {
        _Step = 0;
        _Container = Container;
        _Head = Head;
        _Offset = Offset;
    }

    public int NextStep()
    {
        _Step++;
        var tmp = Offset + Head;
        if (tmp.X < 0)
        {
            tmp.X = 0;
            _Head.X = -_Head.X;
        }
        if (tmp.X > Container.X)
        {
            tmp.X = Container.X;
            _Head.X = -_Head.X;
        }
        if (tmp.Y < 0)
        {
            tmp.Y = 0;
            _Head.Y = -_Head.Y;
        }
        if (tmp.Y > Container.Y)
        {
            tmp.Y = Container.Y;
            _Head.Y = -_Head.Y;
        }
        if (tmp.Z < 25)
        {
            tmp.Z = 25;
            _Head.Z = -_Head.Z;
        }
        if (tmp.Z > Container.Z)
        {
            tmp.Z = Container.Z;
            _Head.Z = -_Head.Z;
        }
        _Offset = tmp;

        return _Step;
    }

    public float GetRange(Vector3 sourceOffset)
    {
        var X = Math.Abs(Offset.X - sourceOffset.X);
        var Y = Math.Abs(Offset.Y - sourceOffset.Y);
        var range = Convert.ToSingle(Math.Sqrt(X * X + Y * Y));
        return range;
    }

    public float GetRange(Vector2 sourceOffset)
    {
        var X = Math.Abs(Offset.X - sourceOffset.X);
        var Y = Math.Abs(Offset.Y - sourceOffset.Y);
        var range = Convert.ToSingle(Math.Sqrt(X * X + Y * Y));
        return range;
    }

    public void ResetOffset(Vector3 Container)
    {
        Offset = rnd.NextVector3(Container);
    }

    public void ResetOffset(Vector2 Container)
    {
        var _Container = new Vector3(Container.X, Container.Y, rnd.Next(30, 61));
        ResetOffset(_Container);
    }

    public static ParticleModel CreateParticle(Vector2 Container)
    {
        return CreateParticle(Container.X, Container.Y, rnd.Next(30, 61));
    }
    public static ParticleModel CreateParticle(double Width, double Height, double Depth)
    {
        var Container = new Vector3(Convert.ToSingle(Width), Convert.ToSingle(Height), Convert.ToSingle(Depth));
        return CreateParticle(Container);
    }
    public static ParticleModel CreateParticle(float Width, float Height, float Depth)
    {
        var Container = new Vector3(Width, Height, Depth);
        return CreateParticle(Container);
    }
    public static ParticleModel CreateParticle(Vector3 Container)
    {
        var head = new Vector3(Convert.ToSingle(rnd.NextDouble() / 2), Convert.ToSingle(rnd.NextDouble().RandomNegative()) / 2, Convert.ToSingle(rnd.NextDouble().RandomNegative()) / 2);
        var offset = rnd.NextVector3(Container);
        var particle = new ParticleModel(Container, head, offset);
        return particle;
    }
}
