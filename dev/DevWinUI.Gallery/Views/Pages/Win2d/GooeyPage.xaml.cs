using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DevWinUIGallery.Views;

public sealed partial class GooeyPage : Page
{
    private readonly Random rnd = new();
    private ObservableCollection<Symbol> strings { get; }

    public GooeyPage()
    {
        this.InitializeComponent();
        strings = new ObservableCollection<Symbol>
        {
            Symbol.AddFriend,
            Symbol.Forward,
            Symbol.Share
        };
    }


    private void gooeyButton_Invoked(object sender, GooeyButton.GooeyButtonInvokedEventArgs args)
    {
        Debug.WriteLine("Invoked");
    }

    private void gooeyButton_ItemInvoked(object sender, GooeyButton.GooeyButtonItemInvokedEventArgs args)
    {
        if (args.Item is Symbol symbol)
        {
            if (symbol == Symbol.AddFriend)
            {
                if (strings.Count == 3)
                {
                    strings.Add(Symbol.Home);
                    gooeyButton.Distance += 20;
                }
                else
                {
                    strings.RemoveAt(3);
                    gooeyButton.Distance -= 20;
                }
            }
            else if (symbol == Symbol.Forward)
            {
                var pos = (int)gooeyButton.ItemsPosition + 1;
                if (pos == 4) pos = 0;
                gooeyButton.ItemsPosition = (GooeyButtonItemsPosition)pos;
            }
            else if (symbol == Symbol.Share)
            {
                gooeyButton.Distance = rnd.Next(80, 300);
            }
        }

        Debug.WriteLine(args.Item.ToString());
    }

    private void nbBubble_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (nbBubble != null && gooeyFooter != null)
        {
            gooeyFooter.Bubble = (int)args.NewValue;
        }
    }
}
