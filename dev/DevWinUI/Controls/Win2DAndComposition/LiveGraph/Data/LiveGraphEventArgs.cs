namespace DevWinUI;

public partial class LiveGraphEventArgs : EventArgs
{
    public ICanvasAnimatedControl CanvasAnimatedControl { get;}
    public CanvasAnimatedDrawEventArgs DrawEventArgs { get; }

    public LiveGraphEventArgs(ICanvasAnimatedControl canvasAnimatedControl, CanvasAnimatedDrawEventArgs drawEventArgs)
    {
        this.CanvasAnimatedControl = canvasAnimatedControl;
        this.DrawEventArgs = drawEventArgs;
    }
}
