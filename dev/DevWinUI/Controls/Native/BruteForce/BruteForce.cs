namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ItemsRepeater), Type = typeof(ItemsRepeater))]
public partial class BruteForce : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";
    private readonly ObservableCollection<string> _characters = [];
    private readonly Dictionary<int, BruteForceState> _characterStates = [];
    private ItemsRepeater _itemsRepeater;
    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(nameof(Password), typeof(string), typeof(BruteForce), new PropertyMetadata(string.Empty, OnPasswordChanged));
    private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BruteForce)d).Reset();
    }

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(BruteForce), new PropertyMetadata(0.0));

    public TimeSpan RenderDelay
    {
        get { return (TimeSpan)GetValue(RenderDelayProperty); }
        set { SetValue(RenderDelayProperty, value); }
    }

    public static readonly DependencyProperty RenderDelayProperty =
        DependencyProperty.Register(nameof(RenderDelay), typeof(TimeSpan), typeof(BruteForce), new PropertyMetadata(TimeSpan.FromMilliseconds(50), OnRenderDelayChanged));
    private static void OnRenderDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BruteForce)d;
        var delay = (TimeSpan)e.NewValue;
        if (control._itemsRepeater is null)
        {
            return;
        }

        for (var i = 0; i < control._characters.Count; i++)
        {
            if (control._itemsRepeater.TryGetElement(i) is BruteForceItem item)
            {
                item.RenderDelay = delay;
            }
        }
    }
    public string Characters
    {
        get { return (string)GetValue(CharactersProperty); }
        set { SetValue(CharactersProperty, value); }
    }

    public static readonly DependencyProperty CharactersProperty =
        DependencyProperty.Register(nameof(Characters), typeof(string), typeof(BruteForce), new PropertyMetadata("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{};:,.<>?/\\|~", OnCharactersChanged));

    private static void OnCharactersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BruteForce)d;
        if (control._itemsRepeater is null || e.NewValue == null || e.NewValue is not string characters)
        {
            return;
        }

        for (var i = 0; i < control._characters.Count; i++)
        {
            if (control._itemsRepeater.TryGetElement(i) is BruteForceItem item)
            {
                item.Characters = characters.ToCharArray();
            }
        }
    }
    public BruteForce()
    {
        DefaultStyleKey = typeof(BruteForce);
        Reset();
    }

    protected override void OnApplyTemplate()
    {
        if (_itemsRepeater is not null)
        {
            _itemsRepeater.ElementPrepared -= OnItemElementPrepared;
        }

        base.OnApplyTemplate();

        _itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;
        if (_itemsRepeater is null)
        {
            return;
        }

        _itemsRepeater.ItemsSource = _characters;
        _itemsRepeater.ElementPrepared += OnItemElementPrepared;
    }

    public void UpdateCharacterState(int index, BruteForceState state)
    {
        if (index < 0 || index >= _characters.Count)
        {
            return;
        }

        _characterStates[index] = state;

        if (_itemsRepeater?.TryGetElement(index) is BruteForceItem item)
        {
            item.SetState(state);
        }
    }

    public void Reset()
    {
        _characterStates.Clear();
        _characters.Clear();

        var password = Password ?? string.Empty;
        for (var i = 0; i < password.Length; i++)
        {
            _characters.Add(password[i].ToString());
            _characterStates[i] = BruteForceState.Pending;
        }

        if (_itemsRepeater is not null)
        {
            _itemsRepeater.ItemsSource = _characters;
        }
    }

    private void OnItemElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is not BruteForceItem item)
        {
            return;
        }

        item.Text = _characters[args.Index];
        item.RenderDelay = RenderDelay;
        item.Characters = Characters.ToCharArray();

        if (_characterStates.TryGetValue(args.Index, out var state))
        {
            item.SetState(state);
            return;
        }

        item.SetState(BruteForceState.Pending);
    }
}
