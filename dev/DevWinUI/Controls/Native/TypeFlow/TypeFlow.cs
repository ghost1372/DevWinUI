using Microsoft.UI.Xaml.Documents;

namespace DevWinUI;

[TemplatePart(Name = PartBody, Type = typeof(TextBlock))]
public sealed partial class TypeFlow : Control
{
    private const string PartBody = "PART_Body";

    private readonly Random _rng = new();
    private readonly Run _typed = new();
    private readonly Run _pending = new();
    private TextBlock? _body;
    private CancellationTokenSource? _cts;

    public TypeFlow()
    {
        DefaultStyleKey = typeof(TypeFlow);

        _pending.Foreground = new SolidColorBrush(Colors.Transparent);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public event EventHandler? Completed;

    public bool IsTyping { get; private set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _body?.Inlines.Clear();
        _body = GetTemplateChild(PartBody) as TextBlock;

        if (_body is not null)
        {
            _body.Inlines.Add(_typed);
            _body.Inlines.Add(_pending);
            SetProgress(Text ?? string.Empty, 0);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_body is null) return;

        if (AutoStart) Restart();
        else SetProgress(Text ?? string.Empty, 0);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        CancelTyping();
        IsTyping = false;
    }

    public void Restart()
    {
        CancelTyping();
        _cts = new CancellationTokenSource();
        _ = TypeAsync(_cts.Token);
    }

    public void Skip()
    {
        string text = Text ?? string.Empty;
        bool finished = !IsTyping && _typed.Text == text;

        CancelTyping();
        IsTyping = false;
        SetProgress(text, text.Length);

        if (!finished) Completed?.Invoke(this, EventArgs.Empty);
    }

    private void CancelTyping()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task TypeAsync(CancellationToken token)
    {
        string text = Text ?? string.Empty;

        SetProgress(text, 0);
        IsTyping = true;

        try
        {
            for (int i = 1; i <= text.Length; i++)
            {
                await Task.Delay(GetDelay(text, i - 1), token);
                token.ThrowIfCancellationRequested();
                SetProgress(text, i);
            }

            Finish();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void Finish()
    {
        IsTyping = false;
        Completed?.Invoke(this, EventArgs.Empty);
    }

    private void SetProgress(string text, int count)
    {
        count = Math.Clamp(count, 0, text.Length);

        if (count > 0 && count < text.Length && char.IsHighSurrogate(text[count - 1]))
        {
            count++;
        }

        _typed.Text = text[..count];
        _pending.Text = text[count..];
    }

    private int GetDelay(string text, int index)
    {
        double baseMs = Math.Max(1, BaseDelay);
        double delay = baseMs * (0.6 + _rng.NextDouble() * 0.8);

        char prev = index > 0 ? text[index - 1] : '\0';

        if (prev == ' ')
        {
            delay = baseMs * (0.5 + _rng.NextDouble() * 1.5);
        }
        else if (prev is '.' or '!' or '?')
        {
            delay += PunctuationDelay * 2;
        }
        else if (prev is ',' or ';' or ':' or '—' or '–' or '\n')
        {
            delay += PunctuationDelay;
        }

        return Math.Max(1, (int)delay);
    }
}
