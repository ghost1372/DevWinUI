using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class TextBox : Microsoft.UI.Xaml.Controls.TextBox
{
    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(TextBox), new PropertyMetadata(null, OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextBox)d;
        if (ctl != null)
        {
            ctl.UpdateCursor();
        }
    }

    private void UpdateCursor()
    {
        if (Content != null)
        {
            if (Content is Button button)
            {
                GeneralHelper.ChangeCursor(button, InputSystemCursor.Create(InputSystemCursorShape.Arrow));
            }
            else if (Content is Panel panel)
            {
                foreach (var item in panel.Children)
                {
                    GeneralHelper.ChangeCursor(item, InputSystemCursor.Create(InputSystemCursorShape.Arrow));
                }
            }
        }
    }
}
