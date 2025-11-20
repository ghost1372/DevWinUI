namespace DevWinUI;

internal struct ConfettiParticle
{
    public Vector2 Position;
    public Vector2 Velocity2D;

    public double Wobble;
    public double WobbleSpeed;

    public float Velocity;
    public float Angle2D;

    public float TiltAngle;
    public float TiltSin;
    public float TiltCos;

    public float WobbleX;
    public float WobbleY;

    public Color Color;
    public string Shape;

    public int Tick;
    public int TotalTicks;

    public float Decay;
    public float Drift;
    public float RandomValue;
    public float Gravity;
    public float Scalar;
    public float OvalScalar;

    public float Opacity;

    public bool Flat;
    public bool Initialized;

    public Point Origin;

    public bool IsCompleted => Tick >= TotalTicks;
}
