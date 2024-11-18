using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

public partial class PagerControlAutomationPeer : FrameworkElementAutomationPeer
{
	public PagerControlAutomationPeer(PagerControl owner) : base(owner)
	{
	}

	// IAutomationPeerOverrides
	public new object GetPatternCore(PatternInterface patternInterface)
	{
		if (patternInterface == PatternInterface.Selection)
		{
			return this;
		}

		return base.GetPatternCore(patternInterface);
	}

	public new string GetClassNameCore()
	{
		return nameof(PagerControl);
	}

	public new string GetNameCore()
	{
		var name = base.GetNameCore();

		if (string.IsNullOrEmpty(name))
		{
			if (Owner is PagerControl pagerControl)

			{
				name = pagerControl.GetValue(AutomationProperties.NameProperty).ToString();
			}
		}

		return name!;
	}

	public new AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Menu;
	}

	PagerControl? GetImpl()
	{
		PagerControl? impl = null;

		if (Owner is PagerControl pagerControl)
		{
			impl = pagerControl;
		}

		return impl;
	}

	public object[] GetSelection()
	{
		if (GetImpl() is PagerControl pagerControl)
		{
			return new object[] { pagerControl.SelectedPageIndex };
		}
		return Array.Empty<object>();
	}

	public void RaiseSelectionChanged(double oldIndex, double newIndex)
	{
		if (ListenerExists(AutomationEvents.PropertyChanged))
		{
			RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty,
				oldIndex,
				newIndex);
		}
	}
}
