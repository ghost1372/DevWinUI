using Microsoft.UI.Xaml.Automation;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = InactiveDotsPartName, Type = typeof(Path))]
[TemplatePart(Name = HaloPartName, Type = typeof(Path))]
[TemplatePart(Name = DotsPartName, Type = typeof(Path))]
public sealed partial class DotMatrixText : Control
{
    private const string InactiveDotsPartName = "PART_InactiveDots";
    private const string HaloPartName = "PART_Halo";
    private const string DotsPartName = "PART_Dots";
    private const int GlyphColumns = 5;
    private const int GlyphRows = 7;
    private const double HaloStrokeRatio = 0.9;

    private static readonly Dictionary<char, byte[]> Glyphs = BuildGlyphs();
    private static readonly byte[] BlankGlyph = new byte[GlyphRows];

    private Path? _inactiveDots;
    private Path? _halo;
    private Path? _dots;

    public DotMatrixText()
    {
        DefaultStyleKey = typeof(DotMatrixText);
    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(DotMatrixText), new PropertyMetadata(string.Empty, OnLayoutPropertyChanged));

    public double DotSize
    {
        get { return (double)GetValue(DotSizeProperty); }
        set { SetValue(DotSizeProperty, value); }
    }

    public static readonly DependencyProperty DotSizeProperty =
        DependencyProperty.Register(nameof(DotSize), typeof(double), typeof(DotMatrixText), new PropertyMetadata(5.5d, OnLayoutPropertyChanged));

    public double DotSpacing
    {
        get { return (double)GetValue(DotSpacingProperty); }
        set { SetValue(DotSpacingProperty, value); }
    }

    public static readonly DependencyProperty DotSpacingProperty =
        DependencyProperty.Register(nameof(DotSpacing), typeof(double), typeof(DotMatrixText), new PropertyMetadata(2.5d, OnLayoutPropertyChanged));

    public Brush InactiveForeground
    {
        get { return (Brush)GetValue(InactiveForegroundProperty); }
        set { SetValue(InactiveForegroundProperty, value); }
    }

    public static readonly DependencyProperty InactiveForegroundProperty =
        DependencyProperty.Register(nameof(InactiveForeground), typeof(Brush), typeof(DotMatrixText), new PropertyMetadata(null));

    public bool IsGlowing
    {
        get { return (bool)GetValue(IsGlowingProperty); }
        set { SetValue(IsGlowingProperty, value); }
    }

    public static readonly DependencyProperty IsGlowingProperty =
        DependencyProperty.Register(nameof(IsGlowing), typeof(bool), typeof(DotMatrixText), new PropertyMetadata(true, OnLayoutPropertyChanged));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _inactiveDots = GetTemplateChild(InactiveDotsPartName) as Path;
        _halo = GetTemplateChild(HaloPartName) as Path;
        _dots = GetTemplateChild(DotsPartName) as Path;
        Rebuild();
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (DotMatrixText)d;
        AutomationProperties.SetName(control, control.Text);
        control.Rebuild();
    }

    private static void Apply(Path path, Geometry? data, double width, double height)
    {
        path.Data = data;
        path.Width = width;
        path.Height = height;
    }

    private static EllipseGeometry CreateDot(Point center, double radius)
    {
        return new EllipseGeometry { Center = center, RadiusX = radius, RadiusY = radius };
    }

    private void Rebuild()
    {
        if (_dots is null || _inactiveDots is null || _halo is null)
        {
            return;
        }

        string text = Text?.ToUpperInvariant() ?? string.Empty;
        double size = DotSize;

        if (text.Length == 0 || size <= 0)
        {
            Apply(_inactiveDots, null, 0, 0);
            Apply(_halo, null, 0, 0);
            Apply(_dots, null, 0, 0);
            return;
        }

        double pitch = size + DotSpacing;
        double radius = size / 2;

        var lit = new GeometryGroup { FillRule = FillRule.Nonzero };
        var inactive = new GeometryGroup { FillRule = FillRule.Nonzero };
        var halo = IsGlowing ? new GeometryGroup { FillRule = FillRule.Nonzero } : null;
        int columnOffset = 0;

        foreach (char c in text)
        {
            byte[] rows = Glyphs.TryGetValue(c, out byte[]? glyph) ? glyph : BlankGlyph;

            for (int row = 0; row < GlyphRows; row++)
            {
                byte bits = rows[row];
                for (int col = 0; col < GlyphColumns; col++)
                {
                    var center = new Point((columnOffset + col) * pitch + radius, row * pitch + radius);

                    if ((bits & (1 << (GlyphColumns - 1 - col))) == 0)
                    {
                        inactive.Children.Add(CreateDot(center, radius));
                        continue;
                    }

                    lit.Children.Add(CreateDot(center, radius));
                    halo?.Children.Add(CreateDot(center, radius));
                }
            }

            columnOffset += GlyphColumns + 1;
        }

        double width = (columnOffset - 1) * pitch - DotSpacing;
        double height = GlyphRows * pitch - DotSpacing;

        Apply(_inactiveDots, inactive, width, height);
        Apply(_dots, lit, width, height);
        Apply(_halo, halo, width, height);
        _halo.StrokeThickness = size * HaloStrokeRatio;
    }

    private static Dictionary<char, byte[]> BuildGlyphs()
    {
        var map = new Dictionary<char, byte[]>(40);

        void Add(char c, string rows)
        {
            string[] parts = rows.Split(' ');
            var bytes = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                bytes[i] = System.Convert.ToByte(parts[i], 2);
            }
            map[c] = bytes;
        }

        Add('A', "01110 10001 10001 11111 10001 10001 10001");
        Add('B', "11110 10001 10001 11110 10001 10001 11110");
        Add('C', "01110 10001 10000 10000 10000 10001 01110");
        Add('D', "11110 10001 10001 10001 10001 10001 11110");
        Add('E', "11111 10000 10000 11110 10000 10000 11111");
        Add('F', "11111 10000 10000 11110 10000 10000 10000");
        Add('G', "01110 10001 10000 10111 10001 10001 01111");
        Add('H', "10001 10001 10001 11111 10001 10001 10001");
        Add('I', "01110 00100 00100 00100 00100 00100 01110");
        Add('J', "00111 00010 00010 00010 00010 10010 01100");
        Add('K', "10001 10010 10100 11000 10100 10010 10001");
        Add('L', "10000 10000 10000 10000 10000 10000 11111");
        Add('M', "10001 11011 10101 10101 10001 10001 10001");
        Add('N', "10001 11001 10101 10011 10001 10001 10001");
        Add('O', "01110 10001 10001 10001 10001 10001 01110");
        Add('P', "11110 10001 10001 11110 10000 10000 10000");
        Add('Q', "01110 10001 10001 10001 10101 10010 01101");
        Add('R', "11110 10001 10001 11110 10100 10010 10001");
        Add('S', "01111 10000 10000 01110 00001 00001 11110");
        Add('T', "11111 00100 00100 00100 00100 00100 00100");
        Add('U', "10001 10001 10001 10001 10001 10001 01110");
        Add('V', "10001 10001 10001 10001 10001 01010 00100");
        Add('W', "10001 10001 10001 10101 10101 11011 10001");
        Add('X', "10001 10001 01010 00100 01010 10001 10001");
        Add('Y', "10001 10001 01010 00100 00100 00100 00100");
        Add('Z', "11111 00001 00010 00100 01000 10000 11111");
        Add('0', "01110 10001 10011 10101 11001 10001 01110");
        Add('1', "00100 01100 00100 00100 00100 00100 01110");
        Add('2', "01110 10001 00001 00010 00100 01000 11111");
        Add('3', "11110 00001 00001 01110 00001 00001 11110");
        Add('4', "00010 00110 01010 10010 11111 00010 00010");
        Add('5', "11111 10000 11110 00001 00001 10001 01110");
        Add('6', "00110 01000 10000 11110 10001 10001 01110");
        Add('7', "11111 00001 00010 00100 01000 01000 01000");
        Add('8', "01110 10001 10001 01110 10001 10001 01110");
        Add('9', "01110 10001 10001 01111 00001 00010 01100");
        Add('-', "00000 00000 00000 11111 00000 00000 00000");
        Add(' ', "00000 00000 00000 00000 00000 00000 00000");

        return map;
    }
}
