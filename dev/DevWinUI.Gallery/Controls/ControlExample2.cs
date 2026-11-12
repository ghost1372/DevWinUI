using System.Numerics;
using CommunityToolkit.WinUI.Animations;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

namespace DevWinUIGallery;

[TemplatePart(Name = HeaderTintHostPart, Type = typeof(Grid))]
[TemplatePart(Name = HeaderPanelPart, Type = typeof(Border))]
[TemplatePart(Name = TitleTextPart, Type = typeof(TextBlock))]
[TemplatePart(Name = DescriptionTextPart, Type = typeof(TextBlock))]
[TemplatePart(Name = HeaderToolsHostPart, Type = typeof(Border))]
[TemplatePart(Name = PreviewSurfacePart, Type = typeof(Border))]
[TemplatePart(Name = BackgroundHostPart, Type = typeof(Grid))]
[TemplatePart(Name = PreviewScrollViewerPart, Type = typeof(ScrollViewer))]
[TemplatePart(Name = PreviewHostPart, Type = typeof(Grid))]
[TemplatePart(Name = SamplePresenterPart, Type = typeof(SwitchPresenter))]
[TemplatePart(Name = SampleSelectorPart, Type = typeof(SelectorBar))]
[TemplatePart(Name = RightPanelPart, Type = typeof(Border))]
[TemplatePart(Name = PanePanelPart, Type = typeof(Border))]
[TemplatePart(Name = PaneContentPart, Type = typeof(Grid))]
[TemplatePart(Name = PaneHeaderPart, Type = typeof(Grid))]
[TemplatePart(Name = OptionsSelectorPart, Type = typeof(SelectorBar))]
[TemplatePart(Name = OptionsOverflowButtonPart, Type = typeof(Button))]
[TemplatePart(Name = ResetButtonPart, Type = typeof(Button))]
[TemplatePart(Name = ResetTextPart, Type = typeof(TextBlock))]
[TemplatePart(Name = OptionsHostPart, Type = typeof(Grid))]
[TemplatePart(Name = SideToolsHostPart, Type = typeof(Border))]
[TemplatePart(Name = ToolsPanelPart, Type = typeof(StackPanel))]
[TemplatePart(Name = DocButtonPart, Type = typeof(Button))]
[TemplatePart(Name = BackgroundButtonPart, Type = typeof(Button))]
[TemplatePart(Name = BackgroundIconPart, Type = typeof(FontIcon))]
[TemplatePart(Name = GlowButtonPart, Type = typeof(Button))]
[TemplatePart(Name = GlowIconPart, Type = typeof(FontIcon))]
[TemplatePart(Name = PaneToggleButtonPart, Type = typeof(Button))]
[TemplatePart(Name = FooterExpanderPart, Type = typeof(Expander))]
[TemplatePart(Name = CodeSelectorPart, Type = typeof(SelectorBar))]
[TemplatePart(Name = XamlHostPart, Type = typeof(ContentControl))]
[TemplatePart(Name = CSharpHostPart, Type = typeof(ContentControl))]
[TemplatePart(Name = GlowHostPart, Type = typeof(Grid))]
[ContentProperty(Name = nameof(Example))]
public sealed partial class ControlExample2 : Control
{
    private const string HeaderTintHostPart = "PART_HeaderTintHost";
    private const string HeaderPanelPart = "PART_HeaderPanel";
    private const string TitleTextPart = "PART_TitleText";
    private const string DescriptionTextPart = "PART_DescriptionText";
    private const string HeaderToolsHostPart = "PART_HeaderToolsHost";
    private const string PreviewSurfacePart = "PART_PreviewSurface";
    private const string BackgroundHostPart = "PART_BackgroundHost";
    private const string PreviewScrollViewerPart = "PART_PreviewScrollViewer";
    private const string PreviewHostPart = "PART_PreviewHost";
    private const string SamplePresenterPart = "PART_SamplePresenter";
    private const string SampleSelectorPart = "PART_SampleSelector";
    private const string RightPanelPart = "PART_RightPanel";
    private const string PanePanelPart = "PART_PanePanel";
    private const string PaneContentPart = "PART_PaneContent";
    private const string PaneHeaderPart = "PART_PaneHeader";
    private const string OptionsSelectorPart = "PART_OptionsSelector";
    private const string OptionsOverflowButtonPart = "PART_OptionsOverflowButton";
    private const string ResetButtonPart = "PART_ResetButton";
    private const string ResetTextPart = "PART_ResetText";
    private const string OptionsHostPart = "PART_OptionsHost";
    private const string SideToolsHostPart = "PART_SideToolsHost";
    private const string ToolsPanelPart = "PART_ToolsPanel";
    private const string DocButtonPart = "PART_DocButton";
    private const string BackgroundButtonPart = "PART_BackgroundButton";
    private const string BackgroundIconPart = "PART_BackgroundIcon";
    private const string GlowButtonPart = "PART_GlowButton";
    private const string GlowIconPart = "PART_GlowIcon";
    private const string PaneToggleButtonPart = "PART_PaneToggleButton";
    private const string FooterExpanderPart = "PART_FooterExpander";
    private const string CodeSelectorPart = "PART_CodeSelector";
    private const string XamlHostPart = "PART_XamlHost";
    private const string CSharpHostPart = "PART_CSharpHost";
    private const string GlowHostPart = "PART_GlowHost";

    private const double CompactWidth = 660;
    private const float GlowInset = 12f;
    private const double SegmentChrome = 8;
    private const double SegmentPadding = 32;
    private const double OverflowButtonWidth = 34;
    private static readonly Dictionary<string, double> TextWidths = new();
    private static readonly Color DefaultTint = Color.FromArgb(255, 91, 125, 184);

    private readonly List<Sample> _samples = new();
    private readonly List<OptionGroup> _optionGroups = new();
    private readonly List<ContentControl> _optionHosts = new();
    private Grid _headerTintHost;
    private Border _headerPanel;
    private TextBlock _titleText;
    private TextBlock _descriptionText;
    private Border _headerToolsHost;
    private Border _previewSurface;
    private Grid _backgroundHost;
    private ScrollViewer _previewScrollViewer;
    private Grid _previewHost;
    private SwitchPresenter _samplePresenter;
    private SelectorBar _sampleSelector;
    private Border _rightPanel;
    private Border _panePanel;
    private Grid _paneContent;
    private Grid _paneHeader;
    private SelectorBar _optionsSelector;
    private Button _optionsOverflowButton;
    private MenuFlyout _optionsOverflowMenu;
    private Button _resetButton;
    private TextBlock _resetText;
    private Grid _optionsHost;
    private Border _sideToolsHost;
    private StackPanel _toolsPanel;
    private Button _docButton;
    private Button _backgroundButton;
    private FontIcon _backgroundIcon;
    private Button _glowButton;
    private FontIcon _glowIcon;
    private Button _paneToggleButton;
    private Expander _footerExpander;
    private SelectorBar _codeSelector;
    private ContentControl _xamlHost;
    private ContentControl _csharpHost;
    private Grid _glowHost;
    private Flyout _paneFlyout;
    private int _selectedOptionIndex;
    private int _optionSlotIndex;
    private bool _updatingOptions;
    private bool _resetInHeader;
    private int _codeSet = -1;
    private int _visibleCount;
    private bool _resetCompact;
    private double _previewHeight;
    private EventHandler _resetRequested;
    private CanvasControl _backgroundCanvas;
    private CanvasControl _glowCanvas;
    private CanvasControl _headerCanvas;
    private CornerRadius _previewCorner = new CornerRadius(8, 0, 0, 0);
    private bool _isLoaded;
    private bool _isTemplateApplied;
    private bool _isCompact;
    private int _selectedSampleIndex;
    private bool _updatingSamples;
    private bool _codeDirty;
    private (string Code, string File) _xaml;
    private (string Code, string File) _csharp;

    public event EventHandler ResetRequested
    {
        add
        {
            _resetRequested += value;
            UpdateResetButton();
        }
        remove
        {
            _resetRequested -= value;
            UpdateResetButton();
        }
    }

    private bool IsReady => _isLoaded && _isTemplateApplied;

    private bool HasOptions => Options != null || OptionGroups?.Count > 0;

    private Sample CurrentSample => _selectedSampleIndex >= 0 && _selectedSampleIndex < _samples.Count ? _samples[_selectedSampleIndex] : null;

    public ControlExample2()
    {
        DefaultStyleKey = typeof(ControlExample2);

        Samples = new DependencyObjectCollection();
        Samples.VectorChanged += (_, _) =>
        {
            if (IsReady)
            {
                RebuildSamples();
            }
        };

        OptionGroups = new DependencyObjectCollection();
        OptionGroups.VectorChanged += (_, _) =>
        {
            if (IsReady)
            {
                RebuildOptions();
            }
        };

        Loaded += ControlExample_Loaded;
        Unloaded += ControlExample_Unloaded;
        SizeChanged += ControlExample_SizeChanged;
        ActualThemeChanged += (_, _) => InvalidateCanvases();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DestroyCanvases();

        _headerTintHost = GetTemplateChild(HeaderTintHostPart) as Grid;
        _headerPanel = GetTemplateChild(HeaderPanelPart) as Border;
        _titleText = GetTemplateChild(TitleTextPart) as TextBlock;
        _descriptionText = GetTemplateChild(DescriptionTextPart) as TextBlock;
        _headerToolsHost = GetTemplateChild(HeaderToolsHostPart) as Border;
        _previewSurface = GetTemplateChild(PreviewSurfacePart) as Border;
        _backgroundHost = GetTemplateChild(BackgroundHostPart) as Grid;
        _previewScrollViewer = GetTemplateChild(PreviewScrollViewerPart) as ScrollViewer;
        _previewHost = GetTemplateChild(PreviewHostPart) as Grid;
        _samplePresenter = GetTemplateChild(SamplePresenterPart) as SwitchPresenter;
        _sampleSelector = GetTemplateChild(SampleSelectorPart) as SelectorBar;
        _rightPanel = GetTemplateChild(RightPanelPart) as Border;
        _panePanel = GetTemplateChild(PanePanelPart) as Border;
        _paneContent = GetTemplateChild(PaneContentPart) as Grid;
        _paneHeader = GetTemplateChild(PaneHeaderPart) as Grid;
        _optionsSelector = GetTemplateChild(OptionsSelectorPart) as SelectorBar;
        _optionsOverflowButton = GetTemplateChild(OptionsOverflowButtonPart) as Button;
        _optionsOverflowMenu = _optionsOverflowButton?.Flyout as MenuFlyout;
        _resetButton = GetTemplateChild(ResetButtonPart) as Button;
        _resetText = GetTemplateChild(ResetTextPart) as TextBlock;
        _optionsHost = GetTemplateChild(OptionsHostPart) as Grid;
        _sideToolsHost = GetTemplateChild(SideToolsHostPart) as Border;
        _toolsPanel = GetTemplateChild(ToolsPanelPart) as StackPanel;
        _docButton = GetTemplateChild(DocButtonPart) as Button;
        _backgroundButton = GetTemplateChild(BackgroundButtonPart) as Button;
        _backgroundIcon = GetTemplateChild(BackgroundIconPart) as FontIcon;
        _glowButton = GetTemplateChild(GlowButtonPart) as Button;
        _glowIcon = GetTemplateChild(GlowIconPart) as FontIcon;
        _paneToggleButton = GetTemplateChild(PaneToggleButtonPart) as Button;
        _footerExpander = GetTemplateChild(FooterExpanderPart) as Expander;
        _codeSelector = GetTemplateChild(CodeSelectorPart) as SelectorBar;
        _xamlHost = GetTemplateChild(XamlHostPart) as ContentControl;
        _csharpHost = GetTemplateChild(CSharpHostPart) as ContentControl;
        _glowHost = GetTemplateChild(GlowHostPart) as Grid;

        _glowHost.Margin = new Thickness(-GlowInset);
        _paneFlyout = new Flyout
        {
            Placement = FlyoutPlacementMode.BottomEdgeAlignedRight,
            FlyoutPresenterStyle = CreateFlyoutPresenterStyle()
        };

        _previewScrollViewer.SizeChanged += PreviewScrollViewer_SizeChanged;
        _samplePresenter.SizeChanged += SamplePresenter_SizeChanged;
        _sampleSelector.SelectionChanged += SampleSelector_SelectionChanged;
        _optionsSelector.SelectionChanged += OptionsSelector_SelectionChanged;
        _resetButton.Click += Reset_Click;
        _docButton.Click += GoToDoc_Click;
        _backgroundButton.Click += ToggleBackground_Click;
        _glowButton.Click += ToggleGlow_Click;
        _paneToggleButton.Click += TogglePane_Click;
        _codeSelector.SelectionChanged += CodeSelector_SelectionChanged;
        _footerExpander.Expanding += FooterExpander_Expanding;

        _isTemplateApplied = true;

        UpdateResetButton();
        UpdateGlowState();
        UpdateBackgroundIcon();
        UpdateDocButton();

        if (_isLoaded)
        {
            Initialize();
        }
    }

    private static Style CreateFlyoutPresenterStyle()
    {
        var style = new Style(typeof(FlyoutPresenter));
        style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0)));
        style.Setters.Add(new Setter(FrameworkElement.MaxWidthProperty, 640d));
        style.Setters.Add(new Setter(Control.CornerRadiusProperty, new CornerRadius(8)));
        return style;
    }

    private static void OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ControlExample2)d).UpdateChrome();
    }

    private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ControlExample2)d;
        if (ctl.IsReady)
        {
            ctl.RebuildOptions();
        }

        ctl.UpdateChrome();
    }

    private static void OnDocChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ControlExample2)d).UpdateDocButton();
    }

    private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ControlExample2)d;
        if (ctl.IsReady)
        {
            ctl.UpdateCode();
        }
    }

    private static void OnSamplesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ControlExample2)d;
        if (ctl.IsReady)
        {
            ctl.RebuildSamples();
        }
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ControlExample2)d).InvalidateCanvases();
    }

    private static void OnBackgroundPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ControlExample2)d;
        ctl.UpdateBackgroundIcon();
        ctl.InvalidateCanvases();
    }

    private static void OnIsGlowEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ControlExample2)d).UpdateGlowState();
    }

    private void ControlExample_Loaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;

        if (_isTemplateApplied)
        {
            Initialize();
        }
    }

    private void ControlExample_Unloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;
        DestroyCanvases();
    }

    private void Initialize()
    {
        _backgroundCanvas ??= CreateCanvas(_backgroundHost, BackgroundCanvas_Draw);
        _headerCanvas ??= CreateCanvas(_headerTintHost, HeaderCanvas_Draw);
        _glowCanvas ??= CreateCanvas(_glowHost, GlowCanvas_Draw);

        UpdateDocButton();
        RebuildOptions();
        UpdateChrome();
        RebuildSamples();
    }

    private static CanvasControl CreateCanvas(Grid host, TypedEventHandler<CanvasControl, CanvasDrawEventArgs> draw)
    {
        var canvas = new CanvasControl { IsHitTestVisible = false };
        canvas.Draw += draw;
        host.Children.Add(canvas);
        return canvas;
    }

    private static void DestroyCanvas(ref CanvasControl canvas, Grid host, TypedEventHandler<CanvasControl, CanvasDrawEventArgs> draw)
    {
        if (canvas == null)
        {
            return;
        }

        canvas.Draw -= draw;
        host?.Children.Remove(canvas);
        canvas.RemoveFromVisualTree();
        canvas = null;
    }

    private void DestroyCanvases()
    {
        DestroyCanvas(ref _backgroundCanvas, _backgroundHost, BackgroundCanvas_Draw);
        DestroyCanvas(ref _headerCanvas, _headerTintHost, HeaderCanvas_Draw);
        DestroyCanvas(ref _glowCanvas, _glowHost, GlowCanvas_Draw);
    }

    private void InvalidateCanvases()
    {
        _backgroundCanvas?.Invalidate();
        _headerCanvas?.Invalidate();
        _glowCanvas?.Invalidate();
    }

    private void ControlExample_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var compact = e.NewSize.Width < CompactWidth;
        if (compact == _isCompact)
        {
            return;
        }

        _isCompact = compact;
        UpdateChrome();
    }

    private void PreviewScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _previewHost.MinWidth = e.NewSize.Width;
    }

    private void SamplePresenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdatePreviewHeight();
    }

    private void FooterExpander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
    {
        LoadCode();
    }

    private void CodeSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        UpdateCodeHosts();
    }

    private void UpdateCodeHosts()
    {
        var tag = _codeSelector.SelectedItem?.Tag as string;
        _xamlHost.Visibility = tag == "XAML" ? Visibility.Visible : Visibility.Collapsed;
        _csharpHost.Visibility = tag == "C#" ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RebuildSamples()
    {
        _updatingSamples = true;
        _samplePresenter.Value = null;
        _sampleSelector.ClearItems();
        _samples.Clear();

        foreach (var item in Samples)
        {
            if (item is Sample sample)
            {
                _samples.Add(sample);
            }
        }

        if (_samples.Count == 0 && Example != null)
        {
            _samples.Add(new Sample { Content = Example });
        }

        var cases = new CaseCollection();
        for (var i = 0; i < _samples.Count; i++)
        {
            cases.Add(new Case { Value = i, Content = _samples[i].Content });
            _sampleSelector.AddItem(new SelectorBarItem
            {
                Text = string.IsNullOrEmpty(_samples[i].Title) ? $"Sample {i + 1}" : _samples[i].Title,
                Tag = i
            });
        }

        _samplePresenter.SwitchCases = cases;

        var count = _samples.Count;
        _sampleSelector.Visibility = count > 1 ? Visibility.Visible : Visibility.Collapsed;
        _previewHost.Padding = count > 1 ? new Thickness(24, 72, 24, 24) : new Thickness(24);

        if (count == 0)
        {
            _selectedSampleIndex = -1;
        }
        else
        {
            _selectedSampleIndex = Math.Clamp(_selectedSampleIndex, 0, count - 1);
            _sampleSelector.SelectedItem = _sampleSelector.Items[_selectedSampleIndex];
            _samplePresenter.Value = _selectedSampleIndex;
        }

        _updatingSamples = false;
        UpdateCode();
    }

    private void RebuildOptions()
    {
        foreach (var host in _optionHosts)
        {
            host.Content = null;
        }

        _optionHosts.Clear();
        _optionsHost.Children.Clear();
        _optionsSelector.ClearItems();
        _optionGroups.Clear();

        foreach (var item in OptionGroups)
        {
            if (item is OptionGroup group)
            {
                _optionGroups.Add(group);
            }
        }

        if (_optionGroups.Count == 0 && Options != null)
        {
            _optionGroups.Add(new OptionGroup { Content = Options });
        }

        foreach (var group in _optionGroups)
        {
            var host = new ContentControl
            {
                Content = group.Content,
                IsTabStop = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Top
            };

            _optionHosts.Add(host);
            _optionsHost.Children.Add(host);
        }

        _optionsSelector.Visibility = _optionGroups.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

        if (_optionGroups.Count == 0)
        {
            _selectedOptionIndex = -1;
            _optionsOverflowMenu.Items.Clear();
            _optionsOverflowButton.Visibility = Visibility.Collapsed;
            UpdatePaneHeader();
            UpdateChrome();
            return;
        }

        _selectedOptionIndex = Math.Clamp(_selectedOptionIndex, 0, _optionGroups.Count - 1);
        _optionSlotIndex = 0;
        UpdateOptionOverflow(true);
        UpdatePaneHeader();
        UpdateChrome();
    }

    private void ShowOptionGroup(int index, bool animate)
    {
        for (var i = 0; i < _optionHosts.Count; i++)
        {
            var host = _optionHosts[i];
            var active = i == index;
            var changed = animate && host.IsHitTestVisible != active;

            host.Opacity = active ? 1 : 0;
            host.Translation = active ? Vector3.Zero : new Vector3(0, 24, 0);

            if (changed)
            {
                AnimateOptionGroup(host, active);
            }

            host.IsHitTestVisible = active;
            host.IsEnabled = active;
            AutomationProperties.SetAccessibilityView(host, active ? AccessibilityView.Content : AccessibilityView.Raw);
        }
    }

    private static void AnimateOptionGroup(UIElement host, bool show)
    {
        var builder = AnimationBuilder.Create();

        if (show)
        {
            builder
                .Opacity(to: 1, from: 0, duration: TimeSpan.FromMilliseconds(200), easingMode: EasingMode.EaseOut)
                .Translation(Axis.Y, to: 0, from: 24, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut);
        }
        else
        {
            builder
                .Opacity(to: 0, from: 1, duration: TimeSpan.FromMilliseconds(100), easingMode: EasingMode.EaseOut)
                .Translation(Axis.Y, to: 24, from: 0, duration: TimeSpan.FromMilliseconds(200), easingMode: EasingMode.EaseOut);
        }

        builder.Start(host);
    }

    private void UpdateOptionOverflow(bool force = false)
    {
        if (_optionGroups.Count == 0)
        {
            return;
        }

        var fit = ComputeOptionFit();
        var changed = force || fit.Visible != _visibleCount || fit.ResetCompact != _resetCompact;
        _resetCompact = fit.ResetCompact;

        if (!changed)
        {
            return;
        }

        _visibleCount = fit.Visible;
        _optionSlotIndex = Math.Max(_optionSlotIndex, _visibleCount - 1);

        RefreshOptionSelector();
        _optionsOverflowButton.Visibility = _optionGroups.Count > _visibleCount ? Visibility.Visible : Visibility.Collapsed;
        SelectOptionGroup(Math.Clamp(_selectedOptionIndex, 0, _optionGroups.Count - 1), animate: false);
    }

    private (int Visible, bool ResetCompact) ComputeOptionFit()
    {
        var count = _optionGroups.Count;
        var resetInPane = _resetButton.Visibility == Visibility.Visible && !_resetInHeader;
        var resetFull = resetInPane ? MeasureText("Reset") + 36 : 0;
        var resetCompact = resetInPane ? 32 : 0;
        var paneWidth = double.IsNaN(PaneWidth) ? 300 : PaneWidth;
        var available = paneWidth - 32 + 6 - 4;

        var widths = new double[count];
        var total = SegmentChrome;
        for (var i = 0; i < count; i++)
        {
            widths[i] = MeasureText(GetOptionTitle(i)) + SegmentPadding;
            total += widths[i];
        }

        if (total <= available - resetFull)
        {
            return (count, false);
        }

        if (total <= available - resetCompact)
        {
            return (count, true);
        }

        var room = available - resetCompact - OverflowButtonWidth - SegmentChrome;
        var selected = Math.Max(_selectedOptionIndex, 0);

        for (var k = count - 1; k >= 1; k--)
        {
            var pinned = k - 1;
            var slot = selected >= pinned ? selected : Math.Clamp(_optionSlotIndex, pinned, count - 1);
            var used = widths[slot];
            for (var i = 0; i < pinned; i++)
            {
                used += widths[i];
            }

            if (used <= room)
            {
                return (k, true);
            }
        }

        return (1, true);
    }

    private static double MeasureText(string text)
    {
        if (TextWidths.TryGetValue(text, out var width))
        {
            return width;
        }

        var block = new TextBlock { Text = text, FontSize = 14 };
        block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return TextWidths[text] = block.DesiredSize.Width;
    }

    private string GetOptionTitle(int index)
    {
        var title = _optionGroups[index].Title;
        return string.IsNullOrEmpty(title) ? $"Group {index + 1}" : title;
    }

    private void SelectOptionGroup(int index, bool animate = true)
    {
        if (index < 0 || index >= _optionGroups.Count)
        {
            return;
        }

        _selectedOptionIndex = index;
        ShowOptionGroup(index, animate);

        var pinned = _visibleCount - 1;
        if (_optionGroups.Count > _visibleCount && index >= pinned && index != _optionSlotIndex)
        {
            _optionSlotIndex = index;
            RefreshOptionSelector();
        }

        _updatingOptions = true;
        _optionsSelector.SelectedItem = _optionsSelector.FindItem(x => x.Tag is int tag && tag == index);
        _updatingOptions = false;
    }

    private void RefreshOptionSelector()
    {
        var overflow = _optionGroups.Count > _visibleCount;
        var pinned = overflow ? _visibleCount - 1 : _optionGroups.Count;

        _updatingOptions = true;
        _optionsSelector.ClearItems();
        _optionsOverflowMenu.Items.Clear();

        for (var i = 0; i < pinned; i++)
        {
            _optionsSelector.AddItem(new SelectorBarItem { Text = GetOptionTitle(i), Tag = i });
        }

        if (overflow)
        {
            _optionsSelector.AddItem(new SelectorBarItem { Text = GetOptionTitle(_optionSlotIndex), Tag = _optionSlotIndex });

            for (var i = pinned; i < _optionGroups.Count; i++)
            {
                if (i == _optionSlotIndex)
                {
                    continue;
                }

                var menuItem = new MenuFlyoutItem { Text = GetOptionTitle(i), Tag = i };
                menuItem.Click += OptionsOverflowItem_Click;
                _optionsOverflowMenu.Items.Add(menuItem);
            }
        }

        _updatingOptions = false;
    }

    private void OptionsSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (_updatingOptions || sender.SelectedItem?.Tag is not int index)
        {
            return;
        }

        SelectOptionGroup(index);
    }

    private void OptionsOverflowItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { Tag: int index })
        {
            SelectOptionGroup(index);
        }
    }

    private void SampleSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (_updatingSamples || sender.SelectedItem?.Tag is not int index)
        {
            return;
        }

        _selectedSampleIndex = index;
        _samplePresenter.Value = index;
        UpdateCode();
    }

    private void UpdateCode()
    {
        var sample = CurrentSample;
        _xaml = Resolve(sample?.XamlCode, sample?.Xaml, XamlCode, Xaml);
        _csharp = Resolve(sample?.CSharpCode, sample?.CSharp, CSharpCode, CSharp);

        var hasXaml = HasCode(_xaml);
        var hasCSharp = HasCode(_csharp);

        var codeSet = (hasXaml ? 1 : 0) | (hasCSharp ? 2 : 0);
        var codeSetChanged = codeSet != _codeSet;
        _codeSet = codeSet;

        RebuildCodeItems(hasXaml, hasCSharp, codeSetChanged);
        _footerExpander.Visibility = codeSet != 0 ? Visibility.Visible : Visibility.Collapsed;

        _codeDirty = true;
        if (_footerExpander.IsExpanded)
        {
            LoadCode();
        }

        UpdateChrome();
    }

    private void RebuildCodeItems(bool hasXaml, bool hasCSharp, bool selectFirst)
    {
        var previous = selectFirst ? null : _codeSelector.SelectedItem?.Tag as string;

        _codeSelector.ClearItems();

        if (hasXaml)
        {
            _codeSelector.AddItem(new SelectorBarItem { Text = "XAML", Tag = "XAML" });
        }

        if (hasCSharp)
        {
            _codeSelector.AddItem(new SelectorBarItem { Text = "C#", Tag = "C#" });
        }

        var selected = _codeSelector.FindItem(x => Equals(x.Tag, previous)) ?? _codeSelector.FindItem(_ => true);
        if (selected != null)
        {
            _codeSelector.SelectedItem = selected;
        }

        UpdateCodeHosts();
    }

    private void LoadCode()
    {
        if (!_codeDirty)
        {
            return;
        }

        _codeDirty = false;
        _xamlHost.Content = CreatePresenter(SampleCodePresenterType.XAML, _xaml);
        _csharpHost.Content = CreatePresenter(SampleCodePresenterType.CSharp, _csharp);
    }

    private static (string Code, string File) Resolve(string sampleCode, string sampleFile, string code, string file)
    {
        if (!string.IsNullOrEmpty(sampleCode) || !string.IsNullOrEmpty(sampleFile))
        {
            return (sampleCode, sampleFile);
        }

        return (code, file);
    }

    private static bool HasCode((string Code, string File) source)
    {
        return !string.IsNullOrEmpty(source.Code) || !string.IsNullOrEmpty(source.File);
    }

    private static SampleCodePresenter CreatePresenter(SampleCodePresenterType type, (string Code, string File) source)
    {
        if (!HasCode(source))
        {
            return null;
        }

        var presenter = new SampleCodePresenter { SampleType = type };
        if (!string.IsNullOrEmpty(source.Code))
        {
            presenter.Code = source.Code;
        }
        else
        {
            presenter.CodeSourceFile = source.File;
        }

        return presenter;
    }

    private void UpdateChrome()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        var side = !_isCompact && ToolbarPlacement == ExampleToolbarPlacement.Side;
        var hasOptions = HasOptions;
        var paneVisible = hasOptions && IsPaneOpen && !_isCompact;
        var hasTitle = !string.IsNullOrEmpty(Title);
        var hasDescription = !string.IsNullOrEmpty(Description);
        var headerVisible = hasTitle || hasDescription || !side;
        var rightVisible = side || paneVisible;
        var footerVisible = _footerExpander.Visibility == Visibility.Visible;

        _headerPanel.Visibility = headerVisible ? Visibility.Visible : Visibility.Collapsed;
        _headerTintHost.Visibility = headerVisible ? Visibility.Visible : Visibility.Collapsed;
        _titleText.Visibility = hasTitle ? Visibility.Visible : Visibility.Collapsed;
        _descriptionText.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;
        _headerPanel.Padding = hasDescription ? new Thickness(16, 8, 8, 8) : new Thickness(16, 5, 8, 5);

        var target = side ? _sideToolsHost : _headerToolsHost;
        var other = side ? _headerToolsHost : _sideToolsHost;

        if (other.Child == _toolsPanel)
        {
            other.Child = null;
        }

        if (target.Child != _toolsPanel)
        {
            target.Child = _toolsPanel;
        }
        _toolsPanel.Orientation = side ? Orientation.Vertical : Orientation.Horizontal;

        _sideToolsHost.Visibility = side ? Visibility.Visible : Visibility.Collapsed;
        _panePanel.Visibility = paneVisible ? Visibility.Visible : Visibility.Collapsed;
        _panePanel.BorderThickness = side ? new Thickness(0, 0, 1, 0) : new Thickness(0);
        _rightPanel.Visibility = rightVisible ? Visibility.Visible : Visibility.Collapsed;
        _paneToggleButton.Visibility = hasOptions ? Visibility.Visible : Visibility.Collapsed;
        _resetInHeader = IsResetInHeader && !side;
        if (_resetInHeader)
        {
            if (!_toolsPanel.Children.Contains(_resetButton))
            {
                _paneHeader.Children.Remove(_resetButton);
                _toolsPanel.Children.Insert(0, _resetButton);
            }
        }
        else if (!_paneHeader.Children.Contains(_resetButton))
        {
            _toolsPanel.Children.Remove(_resetButton);
            _paneHeader.Children.Add(_resetButton);
        }

        UpdateOptionOverflow();

        UpdatePaneHost();
        UpdatePaneHeader();

        var top = headerVisible ? 0 : 8;
        var bottom = footerVisible ? 0 : 8;
        var corner = new CornerRadius(top, headerVisible || rightVisible ? 0 : 8, footerVisible || rightVisible ? 0 : 8, bottom);
        var cornerChanged = !corner.Equals(_previewCorner);
        _previewCorner = corner;
        _previewSurface.CornerRadius = _previewCorner;
        _rightPanel.CornerRadius = new CornerRadius(0, top, bottom, 0);

        if (cornerChanged)
        {
            InvalidateCanvases();
        }
    }

    private void UpdatePreviewHeight()
    {
        var content = _samplePresenter.ActualHeight;
        var padding = _previewHost.Padding.Top + _previewHost.Padding.Bottom;
        _previewHeight = Math.Max(_previewHost.MinHeight, content + padding);
        ApplyPaneMaxHeight();
    }

    private void ApplyPaneMaxHeight()
    {
        _paneContent.MaxHeight = _isCompact ? PaneMaxHeight : Math.Max(PaneMaxHeight, _previewHeight);
    }

    private void UpdatePaneHost()
    {
        ApplyPaneMaxHeight();
        _panePanel.Width = PaneWidth;

        if (_isCompact)
        {
            if (_panePanel.Child == _paneContent)
            {
                _panePanel.Child = null;
            }

            if (_paneFlyout.Content != _paneContent)
            {
                _paneFlyout.Content = _paneContent;
            }

            _paneContent.Width = PaneWidth;
            _paneToggleButton.Flyout = _paneFlyout;
            return;
        }

        if (_paneFlyout.Content == _paneContent)
        {
            _paneFlyout.Hide();
            _paneFlyout.Content = null;
        }

        if (_panePanel.Child != _paneContent)
        {
            _panePanel.Child = _paneContent;
        }

        _paneContent.Width = double.NaN;
        _paneToggleButton.Flyout = null;
    }

    private void UpdateGlowState()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        _glowHost.Visibility = IsGlowEnabled ? Visibility.Visible : Visibility.Collapsed;
        _glowIcon.Opacity = IsGlowEnabled ? 1 : 0.5;
        _glowCanvas?.Invalidate();
    }

    private void UpdateBackgroundIcon()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        _backgroundIcon.Glyph = BackgroundPattern == PreviewBackgroundPattern.Dotted ? "\uE80A" : "\uE7C3";
    }

    private void UpdateDocButton()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        _docButton.Visibility = string.IsNullOrEmpty(Doc) ? Visibility.Collapsed : Visibility.Visible;
    }

    private void UpdateResetButton()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        _resetButton.Visibility = _resetRequested != null ? Visibility.Visible : Visibility.Collapsed;
        UpdateOptionOverflow();
        UpdatePaneHeader();
    }

    private void UpdatePaneHeader()
    {
        if (!_isTemplateApplied)
        {
            return;
        }

        var hasSelector = _optionsSelector.Visibility == Visibility.Visible;
        var hasReset = _resetButton.Visibility == Visibility.Visible && !_resetInHeader;
        var compactReset = _resetInHeader || _resetCompact;

        _paneHeader.Visibility = hasSelector || hasReset ? Visibility.Visible : Visibility.Collapsed;
        _paneContent.RowSpacing = hasSelector ? 12 : hasReset ? 6 : 0;
        _optionsSelector.HorizontalAlignment = hasReset ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        _resetButton.Height = _resetInHeader || hasSelector ? 32 : 28;
        _resetButton.Width = _resetInHeader ? 32 : double.NaN;
        _resetButton.Padding = _resetInHeader ? new Thickness(4) : new Thickness(8, 0, 8, 0);
        _resetText.Visibility = compactReset ? Visibility.Collapsed : Visibility.Visible;
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _resetRequested?.Invoke(this, EventArgs.Empty);
    }

    private void TogglePane_Click(object sender, RoutedEventArgs e)
    {
        if (_isCompact)
        {
            return;
        }

        IsPaneOpen = !IsPaneOpen;
    }

    private void ToggleBackground_Click(object sender, RoutedEventArgs e)
    {
        BackgroundPattern = BackgroundPattern == PreviewBackgroundPattern.Dotted ? PreviewBackgroundPattern.Plain : PreviewBackgroundPattern.Dotted;
    }

    private void ToggleGlow_Click(object sender, RoutedEventArgs e)
    {
        IsGlowEnabled = !IsGlowEnabled;
    }

    private async void GoToDoc_Click(object sender, RoutedEventArgs e)
    {
        var docTypeValue = DocType switch
        {
            DocType.Core => "DevWinUIBase",
            DocType.Shader => "DevWinUIShader",
            DocType.ContextMenu => "DevWinUIContextMenu",
            _ => "DevWinUI"
        };

        await Launcher.LaunchUriAsync(new Uri($"https://Ghost1372.github.io/{docTypeValue}/{Doc}"));
    }

    private void BackgroundCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var w = (float)sender.ActualWidth;
        var h = (float)sender.ActualHeight;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        var ds = args.DrawingSession;
        using var clip = CreateClip(sender, w, h);
        using (ds.CreateLayer(1f, clip))
        {
            DrawBackground(sender, ds, w, h);
        }
    }

    private void DrawBackground(CanvasControl sender, CanvasDrawingSession ds, float w, float h)
    {
        if (BackgroundPattern != PreviewBackgroundPattern.Dotted)
        {
            return;
        }

        DrawDots(sender, ds, w, h);
    }

    private void DrawDots(CanvasControl sender, CanvasDrawingSession ds, float w, float h)
    {
        var dark = ActualTheme == ElementTheme.Dark;
        var intensity = Math.Clamp((float)GradientOpacity * (dark ? 1f : 0.8f), 0f, 1f);

        const float spacing = 18f;
        const float radius = 1.2f;

        using var dotBrush = CreateHorizontalBrush(sender, w, BuildDotStops(dark));
        using var mask = CreateHorizontalBrush(sender, w, BuildMaskStops(intensity));

        using (ds.CreateLayer(mask))
        {
            for (var y = spacing / 2; y < h; y += spacing)
            {
                for (var x = spacing / 2; x < w; x += spacing)
                {
                    ds.FillCircle(x, y, radius, dotBrush);
                }
            }
        }
    }

    private void HeaderCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var w = (float)sender.ActualWidth;
        var h = (float)sender.ActualHeight;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        var dark = ActualTheme == ElementTheme.Dark;
        var intensity = Math.Clamp((float)HeaderTintOpacity * (dark ? 1f : 0.8f), 0f, 1f);
        var ds = args.DrawingSession;

        using var clip = CanvasGeometry.CreateRoundedRectangle(sender, 0, 0, w, h + 8f, 7f, 7f);
        using var fill = CreateHorizontalBrush(sender, w, BuildToneStops());
        using var fade = CreateHorizontalBrush(sender, w, BuildFadeStops(intensity));

        using (ds.CreateLayer(1f, clip))
        {
            ds.FillRectangle(0, 0, w, h, fill, fade);

            if (BackgroundPattern == PreviewBackgroundPattern.Dotted)
            {
                DrawDots(sender, ds, w, h);
            }
        }
    }

    private void GlowCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var w = (float)sender.ActualWidth;
        var h = (float)sender.ActualHeight;
        if (w <= 2 || h <= 2)
        {
            return;
        }

        var dark = ActualTheme == ElementTheme.Dark;
        var ds = args.DrawingSession;

        using var shape = CanvasGeometry.CreateRoundedRectangle(sender, GlowInset, GlowInset, w - GlowInset * 2f, h - GlowInset * 2f, 8f, 8f);
        using var full = CanvasGeometry.CreateRectangle(sender, 0, 0, w, h);
        using var outside = full.CombineWith(shape, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
        using var brush = new CanvasLinearGradientBrush(sender, BuildStops(1f, 0.25f));
        brush.StartPoint = Vector2.Zero;
        brush.EndPoint = new Vector2(w, 0);

        using (ds.CreateLayer(1f, outside))
        {
            brush.Opacity = dark ? 0.06f : 0.045f;
            for (var i = 6; i >= 1; i--)
            {
                ds.DrawGeometry(shape, brush, i * 4f);
            }
        }

        brush.Opacity = dark ? 0.9f : 0.75f;
        ds.DrawGeometry(shape, brush, 1.5f);
    }

    private static CanvasLinearGradientBrush CreateHorizontalBrush(ICanvasResourceCreator creator, float width, CanvasGradientStop[] stops)
    {
        var brush = new CanvasLinearGradientBrush(creator, stops, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Premultiplied, CanvasColorSpace.Srgb, CanvasColorSpace.Srgb, CanvasBufferPrecision.Precision16Float)
        {
            StartPoint = Vector2.Zero,
            EndPoint = new Vector2(width, 0)
        };

        return brush;
    }

    private Color GetBaseTint()
    {
        return TintSource == ExampleTintSource.Accent ? GetAccentColor() : DefaultTint;
    }

    private static Color GetAccentColor()
    {
        if (Application.Current.Resources.TryGetValue("SystemAccentColor", out var value) && value is Color color)
        {
            return color;
        }

        return DefaultTint;
    }

    private Color GetTint()
    {
        return GradientBrush switch
        {
            LinearGradientBrush linear when linear.GradientStops.Count > 0 => linear.GradientStops.OrderBy(x => x.Offset).First().Color,
            SolidColorBrush solid => solid.Color,
            _ => GetBaseTint()
        };
    }

    private CanvasGradientStop[] BuildDotStops(bool dark)
    {
        var color = GetTint();
        var target = dark ? (byte)255 : (byte)0;

        return new[]
        {
            new CanvasGradientStop { Position = 0f, Color = Tone(color, target, 0.15f, 255) },
            new CanvasGradientStop { Position = 1f, Color = Tone(color, target, 0.5f, 255) }
        };
    }

    private CanvasGradientStop[] BuildToneStops()
    {
        var color = GetTint();

        return new[]
        {
            new CanvasGradientStop { Position = 0f, Color = Tone(color, 0, 0.35f, 220) },
            new CanvasGradientStop { Position = 0.5f, Color = Tone(color, 0, 0f, 180) },
            new CanvasGradientStop { Position = 1f, Color = Tone(color, 255, 0.55f, 128) }
        };
    }

    private static CanvasGradientStop[] BuildFadeStops(float intensity)
    {
        return new[]
        {
            new CanvasGradientStop { Position = 0f, Color = Color.FromArgb((byte)(255 * intensity), 255, 255, 255) },
            new CanvasGradientStop { Position = 1f, Color = Color.FromArgb((byte)(64 * intensity), 255, 255, 255) }
        };
    }

    private static CanvasGradientStop[] BuildMaskStops(float intensity)
    {
        return new[]
        {
            new CanvasGradientStop { Position = 0f, Color = Color.FromArgb((byte)(70 * intensity), 255, 255, 255) },
            new CanvasGradientStop { Position = 0.45f, Color = Color.FromArgb((byte)(160 * intensity), 255, 255, 255) },
            new CanvasGradientStop { Position = 1f, Color = Color.FromArgb((byte)(255 * intensity), 255, 255, 255) }
        };
    }

    private static Color Tone(Color color, byte target, float amount, byte alpha)
    {
        return Color.FromArgb(
            alpha,
            (byte)(color.R + (target - color.R) * amount),
            (byte)(color.G + (target - color.G) * amount),
            (byte)(color.B + (target - color.B) * amount));
    }

    private CanvasGradientStop[] BuildStops(float scale, float endAlpha)
    {
        var brush = GradientBrush;

        if (brush is LinearGradientBrush linear && linear.GradientStops.Count > 1)
        {
            return linear.GradientStops
                .OrderBy(x => x.Offset)
                .Select(x => new CanvasGradientStop { Position = (float)Math.Clamp(x.Offset, 0, 1), Color = Scale(x.Color, scale) })
                .ToArray();
        }

        var color = brush switch
        {
            SolidColorBrush solid => solid.Color,
            LinearGradientBrush single when single.GradientStops.Count == 1 => single.GradientStops[0].Color,
            _ => GetBaseTint()
        };

        return new[]
        {
            new CanvasGradientStop { Position = 0f, Color = Scale(color, scale) },
            new CanvasGradientStop { Position = 0.5f, Color = Scale(color, scale * (endAlpha + (1f - endAlpha) * 0.35f)) },
            new CanvasGradientStop { Position = 1f, Color = Scale(color, scale * endAlpha) }
        };
    }

    private CanvasGeometry CreateClip(ICanvasResourceCreator creator, float w, float h)
    {
        var tl = (float)_previewCorner.TopLeft;
        var tr = (float)_previewCorner.TopRight;
        var br = (float)_previewCorner.BottomRight;
        var bl = (float)_previewCorner.BottomLeft;

        using var path = new CanvasPathBuilder(creator);
        path.BeginFigure(tl, 0);
        path.AddLine(w - tr, 0);
        Corner(path, tr, w, tr);
        path.AddLine(w, h - br);
        Corner(path, br, w - br, h);
        path.AddLine(bl, h);
        Corner(path, bl, 0, h - bl);
        path.AddLine(0, tl);
        Corner(path, tl, tl, 0);
        path.EndFigure(CanvasFigureLoop.Closed);
        return CanvasGeometry.CreatePath(path);
    }

    private static void Corner(CanvasPathBuilder path, float radius, float x, float y)
    {
        if (radius > 0)
        {
            path.AddArc(new Vector2(x, y), radius, radius, 0, CanvasSweepDirection.Clockwise, CanvasArcSize.Small);
        }
        else
        {
            path.AddLine(x, y);
        }
    }

    private static Color Scale(Color color, float factor)
    {
        return Color.FromArgb((byte)Math.Clamp(color.A * factor, 0, 255), color.R, color.G, color.B);
    }

    private static Color WithAlpha(Color color, byte alpha)
    {
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }
}

[ContentProperty(Name = nameof(Content))]
public sealed partial class Sample : DependencyObject
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(Sample), new PropertyMetadata(null));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(Sample), new PropertyMetadata(null));

    public static readonly DependencyProperty XamlProperty =
        DependencyProperty.Register(nameof(Xaml), typeof(string), typeof(Sample), new PropertyMetadata(null));

    public static readonly DependencyProperty XamlCodeProperty =
        DependencyProperty.Register(nameof(XamlCode), typeof(string), typeof(Sample), new PropertyMetadata(null));

    public static readonly DependencyProperty CSharpProperty =
        DependencyProperty.Register(nameof(CSharp), typeof(string), typeof(Sample), new PropertyMetadata(null));

    public static readonly DependencyProperty CSharpCodeProperty =
        DependencyProperty.Register(nameof(CSharpCode), typeof(string), typeof(Sample), new PropertyMetadata(null));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public object Content
    {
        get { return GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public string Xaml
    {
        get { return (string)GetValue(XamlProperty); }
        set { SetValue(XamlProperty, value); }
    }

    public string XamlCode
    {
        get { return (string)GetValue(XamlCodeProperty); }
        set { SetValue(XamlCodeProperty, value); }
    }

    public string CSharp
    {
        get { return (string)GetValue(CSharpProperty); }
        set { SetValue(CSharpProperty, value); }
    }

    public string CSharpCode
    {
        get { return (string)GetValue(CSharpCodeProperty); }
        set { SetValue(CSharpCodeProperty, value); }
    }
}

[ContentProperty(Name = nameof(Content))]
public sealed partial class OptionGroup : DependencyObject
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(OptionGroup), new PropertyMetadata(null));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(OptionGroup), new PropertyMetadata(null));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public object Content
    {
        get { return GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }
}

public enum ExampleToolbarPlacement
{
    Side,
    Header
}

public enum ExampleTintSource
{
    Default,
    Accent
}

public enum PreviewBackgroundPattern
{
    Dotted,
    Plain
}

public enum DocType
{
    Core,
    Controls,
    Shader,
    ContextMenu
}
