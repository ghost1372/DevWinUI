namespace DevWinUI;

internal sealed partial class MotionWallCardSet : IDisposable
{
    private readonly List<MotionWallCard> _owned;

    public MotionWallCardSet(MotionWallCard[] cards, MotionWallItemInfo[] infos, List<MotionWallCard> owned, float width, float height)
    {
        Cards = cards;
        Infos = infos;
        Width = width;
        Height = height;
        _owned = owned;
    }

    public MotionWallCard[] Cards { get; }

    public MotionWallItemInfo[] Infos { get; }

    public float Width { get; }

    public float Height { get; }

    public void Dispose()
    {
        foreach (MotionWallCard card in _owned)
        {
            card.Dispose();
        }
    }
}

