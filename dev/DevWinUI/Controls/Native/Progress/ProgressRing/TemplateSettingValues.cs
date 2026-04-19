namespace DevWinUI;

public partial class ProgressRing
{
    public partial class TemplateSettingValues : DependencyObject
    {
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(TemplateSettingValues),
                new PropertyMetadata(0D));

        public static readonly DependencyProperty EllipseOffsetProperty =
            DependencyProperty.Register("EllipseOffset", typeof(Thickness), typeof(TemplateSettingValues),
                new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty MaxSideLengthProperty =
            DependencyProperty.Register("MaxSideLength", typeof(double), typeof(TemplateSettingValues),
                new PropertyMetadata(0D));

        public TemplateSettingValues(double width)
        {
            if (width <= 40)
            {
                EllipseDiameter = width / 10 + 1;
            }
            else
            {
                EllipseDiameter = width / 10;
            }

            MaxSideLength = width - EllipseDiameter;
            EllipseOffset = new Thickness(0, EllipseDiameter * 2.5, 0, 0);
        }

        public double EllipseDiameter
        {
            get => (double)GetValue(EllipseDiameterProperty);
            set => SetValue(EllipseDiameterProperty, value);
        }

        public Thickness EllipseOffset
        {
            get => (Thickness)GetValue(EllipseOffsetProperty);
            set => SetValue(EllipseOffsetProperty, value);
        }

        public double MaxSideLength
        {
            get => (double)GetValue(MaxSideLengthProperty);
            set => SetValue(MaxSideLengthProperty, value);
        }
    }
}
