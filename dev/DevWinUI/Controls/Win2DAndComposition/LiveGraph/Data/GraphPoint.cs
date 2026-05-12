namespace DevWinUI;

public partial class GraphPoint
{
    public float Value { get; set; }   // normalized Y value (0-100)
    public float Space { get; set; }   // optional horizontal spacing

    public GraphPoint()
    {
        
    }
    public GraphPoint(float value)
    {
        this.Value = value;
        this.Space = 20f;
    }
    public GraphPoint(float value, float space)
    {
        this.Value = value;
        this.Space = space;
    }
}
