using Windows.Media.Casting;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Image), Type = typeof(Image))]
public partial class GifImage : Control
{
    private const string PART_Image = "PART_Image";
    private Image _image = null;

    public event RoutedEventHandler ImageOpened;
    public event EventHandler<Exception> ImageFailed;

    private bool _isInitialized = false;
    private bool _isPlaying = false;

    public Uri Source
    {
        get { return (Uri)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(GifImage), new PropertyMetadata(null, SourceChanged));
    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as GifImage;
        control.SetSource(e.NewValue as Uri);
    }

    public async void SetSource(Uri uri)
    {
        if (_isInitialized)
        {
            if (uri != null)
            {
                await ProcessSourceAsync(uri);
                if (this.AutoPlay)
                {
                    Play();
                }
                else
                {
                    await PlayFrameAsync();
                }
            }
            else
            {
                _timer.Stop();
                _isPlaying = false;
            }
        }
    }

    public bool AutoPlay
    {
        get { return (bool)GetValue(AutoPlayProperty); }
        set { SetValue(AutoPlayProperty, value); }
    }

    public static readonly DependencyProperty AutoPlayProperty =
        DependencyProperty.Register(nameof(AutoPlay), typeof(bool), typeof(GifImage), new PropertyMetadata(true, AutoPlayChanged));

    private static void AutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as GifImage;
        control.SetAutoPlay((bool)e.NewValue);
    }

    private void SetAutoPlay(bool autoPlay)
    {
        if (_isInitialized)
        {
            if (autoPlay && !_timer.IsEnabled)
            {
                this.Play();
            }
        }
    }

    public bool IsLooping
    {
        get { return (bool)GetValue(IsLoopingProperty); }
        set { SetValue(IsLoopingProperty, value); }
    }

    public static readonly DependencyProperty IsLoopingProperty =
        DependencyProperty.Register(nameof(IsLooping), typeof(bool), typeof(GifImage), new PropertyMetadata(true));
    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(GifImage), new PropertyMetadata(Stretch.Uniform));

    public Thickness NineGrid
    {
        get { return (Thickness)GetValue(NineGridProperty); }
        set { SetValue(NineGridProperty, value); }
    }

    public static readonly DependencyProperty NineGridProperty =
        DependencyProperty.Register(nameof(NineGrid), typeof(Thickness), typeof(GifImage), new PropertyMetadata(new Thickness(), NineGridChanged));

    private static void NineGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as GifImage;
        control.SetNineGrid((Thickness)e.NewValue);
    }

    private void SetNineGrid(Thickness newValue)
    {
        if (_isInitialized)
        {
            _image.NineGrid = newValue;
        }
    }

    public GifImage()
    {
        this.DefaultStyleKey = typeof(GifImage);
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
        this.InitializeTimer();
    }

    protected override void OnApplyTemplate()
    {
        _image = base.GetTemplateChild(PART_Image) as Image;

        _isInitialized = true;

        this.SetSource(this.Source);
        this.SetNineGrid(this.NineGrid);

        base.OnApplyTemplate();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isPlaying)
        {
            _timer.Start();
        }
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_isPlaying)
        {
            _timer.Stop();
        }
    }
    public void Play()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Start();
        _isPlaying = true;
    }
    public void Pause()
    {
        _timer.Stop();
        _isPlaying = false;
    }

    public async void Stop()
    {
        _timer.Stop();
        _index = 0;
        await PlayFrameAsync();
        _isPlaying = false;
    }

    public CastingSource GetAsCastingSource()
    {
        if (_isInitialized)
        {
            return _image.GetAsCastingSource();
        }
        return null;
    }
}
