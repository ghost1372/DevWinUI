// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public sealed partial class SidebarItem : Control
{
	public SidebarView? Owner
	{
		get { return (SidebarView?)GetValue(OwnerProperty); }
		set { SetValue(OwnerProperty, value); }
	}
	public static readonly DependencyProperty OwnerProperty =
		DependencyProperty.Register(nameof(Owner), typeof(SidebarView), typeof(SidebarItem), new PropertyMetadata(null));

	public bool IsSelected
	{
		get { return (bool)GetValue(IsSelectedProperty); }
		set { SetValue(IsSelectedProperty, value); }
	}
	public static readonly DependencyProperty IsSelectedProperty =
		DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SidebarItem), new PropertyMetadata(false, OnPropertyChanged));

	public bool IsExpanded
	{
		get { return (bool)GetValue(IsExpandedProperty); }
		set { SetValue(IsExpandedProperty, value); }
	}
	public static readonly DependencyProperty IsExpandedProperty =
		DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(SidebarItem), new PropertyMetadata(true, OnPropertyChanged));

	public bool IsInFlyout
	{
		get { return (bool)GetValue(IsInFlyoutProperty); }
		set { SetValue(IsInFlyoutProperty, value); }
	}
	public static readonly DependencyProperty IsInFlyoutProperty =
		DependencyProperty.Register(nameof(IsInFlyout), typeof(bool), typeof(SidebarItem), new PropertyMetadata(false));

	public double ChildrenPresenterHeight
	{
		get { return (double)GetValue(ChildrenPresenterHeightProperty); }
		set { SetValue(ChildrenPresenterHeightProperty, value); }
	}
	// Using 30 as a default in case something goes wrong
	public static readonly DependencyProperty ChildrenPresenterHeightProperty =
		DependencyProperty.Register(nameof(ChildrenPresenterHeight), typeof(double), typeof(SidebarItem), new PropertyMetadata(30d));

	public ISidebarItemModel? Item
	{
		get { return (ISidebarItemModel)GetValue(ItemProperty); }
		set { SetValue(ItemProperty, value); }
	}
	public static readonly DependencyProperty ItemProperty =
		DependencyProperty.Register(nameof(Item), typeof(ISidebarItemModel), typeof(SidebarItem), new PropertyMetadata(null));

	public bool UseReorderDrop
	{
		get { return (bool)GetValue(UseReorderDropProperty); }
		set { SetValue(UseReorderDropProperty, value); }
	}
	public static readonly DependencyProperty UseReorderDropProperty =
		DependencyProperty.Register(nameof(UseReorderDrop), typeof(bool), typeof(SidebarItem), new PropertyMetadata(false));

	public FrameworkElement? Icon
	{
		get { return (FrameworkElement?)GetValue(IconProperty); }
		set { SetValue(IconProperty, value); }
	}
	public static readonly DependencyProperty IconProperty =
		DependencyProperty.Register(nameof(Icon), typeof(FrameworkElement), typeof(SidebarItem), new PropertyMetadata(null));

	public FrameworkElement? Decorator
	{
		get { return (FrameworkElement?)GetValue(DecoratorProperty); }
		set { SetValue(DecoratorProperty, value); }
	}
	public static readonly DependencyProperty DecoratorProperty =
		DependencyProperty.Register(nameof(Decorator), typeof(FrameworkElement), typeof(SidebarItem), new PropertyMetadata(null));

	public SidebarDisplayMode DisplayMode
	{
		get { return (SidebarDisplayMode)GetValue(DisplayModeProperty); }
		set { SetValue(DisplayModeProperty, value); }
	}
	public static readonly DependencyProperty DisplayModeProperty =
		DependencyProperty.Register(nameof(DisplayMode), typeof(SidebarDisplayMode), typeof(SidebarItem), new PropertyMetadata(SidebarDisplayMode.Expanded, OnPropertyChanged));

	public static void SetTemplateRoot(DependencyObject target, FrameworkElement value)
	{
		target.SetValue(TemplateRootProperty, value);
	}
	public static FrameworkElement GetTemplateRoot(DependencyObject target)
	{
		return (FrameworkElement)target.GetValue(TemplateRootProperty);
	}
	public static readonly DependencyProperty TemplateRootProperty =
		DependencyProperty.Register("TemplateRoot", typeof(FrameworkElement), typeof(FrameworkElement), new PropertyMetadata(null));

    internal string TextValue
    {
        get => (string)GetValue(TextValueProperty);
        set => SetValue(TextValueProperty, value);
    }
    public static readonly DependencyProperty TextValueProperty =
        DependencyProperty.Register(nameof(TextValue), typeof(string), typeof(SidebarItem), new PropertyMetadata(""));

    internal object? ToolTipValue
    {
        get => GetValue(ToolTipValueProperty);
        set => SetValue(ToolTipValueProperty, value);
    }
    public static readonly DependencyProperty ToolTipValueProperty =
        DependencyProperty.Register(nameof(ToolTipValue), typeof(object), typeof(SidebarItem), new PropertyMetadata(null));

    internal object? ChildrenValue
    {
        get => GetValue(ChildrenValueProperty);
        set => SetValue(ChildrenValueProperty, value);
    }
    public static readonly DependencyProperty ChildrenValueProperty =
        DependencyProperty.Register(nameof(ChildrenValue), typeof(object), typeof(SidebarItem), new PropertyMetadata(null));

    private void UpdateTemplateBindings()
    {
        if (Item is not null)
        {
            TextValue = Item.Text;
            ToolTipValue = Item.ToolTip;
            ChildrenValue = Item.Children;
        }
        else
        {
            TextValue = "";
            ToolTipValue = null;
            ChildrenValue = null;
        }
    }

    public static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is not SidebarItem item) return;
		if (e.Property == DisplayModeProperty)
		{
			item.SidebarDisplayModeChanged((SidebarDisplayMode)e.OldValue);
        }
		else if (e.Property == IsSelectedProperty)
		{
			item.UpdateSelectionState();
		}
		else if (e.Property == IsExpandedProperty)
		{
			item.UpdateExpansionState();
		}
		else if (e.Property == ItemProperty)
		{
			item.HandleItemChange();
            item.UpdateTemplateBindings();
        }
    }
}
