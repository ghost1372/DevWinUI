using Microsoft.UI.Xaml.Shapes;
using System.Text;

namespace DevWinUI;

public sealed partial class BankCard : Hover3DCard
{
    private Border? _imageLayer, _chip;
    private Rectangle? _glare;
    private TextBlock? _number, _holder, _expiry, _cvv, _bankName;
    private Image? _logo;
    private readonly TranslateTransform _glareShift = new();

    public BankCard()
    {
        DefaultStyleKey = typeof(BankCard);
        RegisterPropertyChangedCallback(CornerRadiusProperty, (_, _) => UpdateVisuals());
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _imageLayer = GetTemplateChild("ImageLayer") as Border;
        _chip = GetTemplateChild("ChipPart") as Border;
        _glare = GetTemplateChild("GlareLayer") as Rectangle;
        _number = GetTemplateChild("NumberText") as TextBlock;
        _holder = GetTemplateChild("HolderText") as TextBlock;
        _expiry = GetTemplateChild("ExpiryText") as TextBlock;
        _cvv = GetTemplateChild("CvvText") as TextBlock;
        _logo = GetTemplateChild("LogoImage") as Image;
        _bankName = GetTemplateChild("BankNameText") as TextBlock;

        ApplyGlareBrush();
        UpdateVisuals();
    }

    private void ApplyGlareBrush()
    {
        if (_glare is null)
        {
            return;
        }

        _glare.Fill = GlareBrush is LinearGradientBrush source
            ? CreateShiftableBrush(source)
            : GlareBrush;
    }

    private LinearGradientBrush CreateShiftableBrush(LinearGradientBrush source)
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = source.StartPoint,
            EndPoint = source.EndPoint,
            MappingMode = source.MappingMode,
            SpreadMethod = source.SpreadMethod,
            ColorInterpolationMode = source.ColorInterpolationMode,
            RelativeTransform = _glareShift
        };

        foreach (GradientStop stop in source.GradientStops)
        {
            brush.GradientStops.Add(new GradientStop { Color = stop.Color, Offset = stop.Offset });
        }

        return brush;
    }

    private void UpdateVisuals()
    {
        if (_number is not null)
        {
            _number.Text = FormatNumber(CardNumber, MaskNumber);
        }

        if (_holder is not null)
        {
            _holder.Text = string.IsNullOrWhiteSpace(CardHolder) ? "CARD HOLDER" : CardHolder.ToUpperInvariant();
        }

        if (_expiry is not null)
        {
            _expiry.Text = string.IsNullOrWhiteSpace(Expiration) ? "MM/YY" : Expiration;
        }

        if (_cvv is not null)
        {
            _cvv.Text = string.IsNullOrEmpty(Cvv2) ? "•••" : (ShowCvv ? Cvv2 : new string('•', Cvv2.Length));
        }

        if (_logo is not null)
        {
            _logo.Source = CardLogo;
            _logo.Visibility = CardLogo is null ? Visibility.Collapsed : Visibility.Visible;
        }

        if (_chip is not null)
        {
            _chip.Visibility = ShowChip ? Visibility.Visible : Visibility.Collapsed;
        }

        if (_bankName is not null)
        {
            _bankName.Text = BankName;
            _bankName.Visibility = string.IsNullOrWhiteSpace(BankName) ? Visibility.Collapsed : Visibility.Visible;
        }

        if (_imageLayer is not null)
        {
            _imageLayer.Background = BackgroundImage is null
                ? null
                : new ImageBrush { ImageSource = BackgroundImage, Stretch = Stretch.UniformToFill };
        }

        if (_glare is not null)
        {
            _glare.RadiusX = _glare.RadiusY = CornerRadius.TopLeft;
        }
    }

    protected override void OnTiltUpdated(float nx, float ny, float hoverProgress)
    {
        if (_glare is null)
        {
            return;
        }

        _glareShift.X = -nx * 0.6;
        _glareShift.Y = -ny * 0.3;
        _glare.Opacity = hoverProgress;
    }

    private static string FormatNumber(string? raw, bool mask)
    {
        var digits = new string((raw ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
        {
            return "•••• •••• •••• ••••";
        }

        int[] groups = digits.Length == 15 && (digits.StartsWith("34") || digits.StartsWith("37"))
            ? new[] { 4, 6, 5 }
            : Enumerable.Repeat(4, (digits.Length + 3) / 4).ToArray();

        var sb = new StringBuilder();
        int pos = 0;
        foreach (int g in groups)
        {
            if (pos >= digits.Length)
            {
                break;
            }

            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            for (int i = 0; i < g && pos < digits.Length; i++, pos++)
            {
                sb.Append(mask && pos < digits.Length - 4 ? '•' : digits[pos]);
            }
        }
        return sb.ToString();
    }
}
