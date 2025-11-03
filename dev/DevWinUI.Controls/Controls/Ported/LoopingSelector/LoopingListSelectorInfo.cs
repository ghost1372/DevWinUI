using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Input;

using Microsoft.UI.Input;


namespace DevWinUI;

internal partial class LoopingListSelectorInfo : ContentControl
{
    internal enum State
    {
        Normal,
        Expanded,
        Selected,
        PointerOver,
        Pressed,
    };
    internal void GetVisualIndex(out int idx)
    {
        idx = _visualIndex;
        return;
    }

    internal void SetVisualIndex(int idx)
    {
        _visualIndex = idx;
        return;
    }

    internal void SetParent(LoopingSelector pValue)
    {
        _pParentNoRef = pValue;
        return;
    }

    internal void GetParentNoRef(out LoopingSelector ppValue)
    {
        ppValue = _pParentNoRef;
        return;
    }

    private State _state;

    // The visual index of the data item this item is displaying.
    // Note: Due to the looping behavior, this is not equal to the index of the item in the collection.
    // e.g. consider the case of Minute 59 looping around to 0. The Item after 59 does not have an index of 0. It has an index of 60.
    private int _visualIndex;

    // The parent is used by the AutomationPeer for ScrollIntoView
    // and Selection. We don't keep a strong to prevent cycles.
    private LoopingSelector _pParentNoRef;

    // There's no way to know if an AP has been created except to
    // keep track with an internal boolean.
    private bool _hasPeerBeenCreated;
    public LoopingListSelectorInfo()
	{
		_state = State.Normal;
		_visualIndex = 0;
		_pParentNoRef = null;
		_hasPeerBeenCreated = false;

		InitializeImpl();
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		var measureOverride = base.MeasureOverride(availableSize);
		return measureOverride;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		var arrangeOverride = base.ArrangeOverride(finalSize);
		return arrangeOverride;
	}

	void InitializeImpl()
	{
		//wrl.ComPtr<xaml_controls.IContentControlFactory> spInnerFactory;
		//ContentControl spInnerInstance;
		//DependencyObject spInnerInspectable;

		//LoopingSelectorItemGenerated.InitializeImpl();
		//(wf.GetActivationFactory(
		//	wrl_wrappers.Hstring(RuntimeClass_Microsoft_UI_Xaml_Controls_ContentControl),
		//	&spInnerFactory));

		//(spInnerFactory.CreateInstance(
		//	(DependencyObject)((ILoopingSelectorItem)(this)),
		//	&spInnerInspectable,
		//	&spInnerInstance));

		//(SetComposableBasePointers(
		//		spInnerInspectable,
		//		spInnerFactory));

		//(Private.SetDefaultStyleKey(
		//		spInnerInspectable,
		//		"Microsoft.UI.Xaml.Controls.Primitives.LoopingSelectorItem"));

		DefaultStyleKey = typeof(LoopingListSelectorInfo);
	}

	protected override void OnPointerEntered(PointerRoutedEventArgs pEventArgs)
	{
		var pointerDeviceType = PointerDeviceType.Touch;
		PointerPoint spPointerPoint;
		global::Windows.Devices.Input.PointerDevice spPointerDevice;

		spPointerPoint = pEventArgs.GetCurrentPoint(null);
		if (spPointerPoint == null) { return; }

		pointerDeviceType = pEventArgs.Pointer.PointerDeviceType;

		if (pointerDeviceType == PointerDeviceType.Mouse)
		{
			if (_state != State.Selected)
			{
				SetState(State.PointerOver, false);
			}
		}
	}

	protected override void OnPointerPressed(PointerRoutedEventArgs args)
	{
		if (_state != State.Selected)
		{
			SetState(State.Pressed, false);
		}
	}

	protected override void OnPointerExited(PointerRoutedEventArgs args)
	{
		if (_state != State.Selected)
		{
			SetState(State.Normal, false);
		}
	}

	protected override void OnPointerCaptureLost(PointerRoutedEventArgs args)
	{
		if (_state != State.Selected)
		{
			SetState(State.Normal, false);
		}
	}

	#region UIElementOverrides

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		AutomationPeer returnValue;
		LoopingSelectorItemAutomationPeer spLoopingSelectorItemAutomationPeer;

		//(wrl.MakeAndInitialize<xaml_automation_peers.LoopingSelectorItemAutomationPeer>
		//		(spLoopingSelectorItemAutomationPeer, this));
		spLoopingSelectorItemAutomationPeer = new LoopingSelectorItemAutomationPeer(this);


		_hasPeerBeenCreated = true;

		//spLoopingSelectorItemAutomationPeer.CopyTo(returnValue);
		returnValue = spLoopingSelectorItemAutomationPeer;

		return returnValue;
	}

	#endregion

	void GoToState(State newState, bool useTransitions)
	{
		string strState = null;

		switch (newState)
		{
			case State.Normal:
				strState = "Normal";
				break;
			case State.Expanded:
				strState = "Expanded";
				break;
			case State.Selected:
				strState = "Selected";
				break;
			case State.PointerOver:
				strState = "PointerOver";
				break;
			case State.Pressed:
				strState = "Pressed";
				break;
		}

		//wrl.ComPtr<xaml.IVisualStateManagerStatics> spVSMStatics;
		//wrl.ComPtr<xaml_controls.IControl> spThisAsControl;

		//(wf.GetActivationFactory(
		//	wrl_wrappers.Hstring(RuntimeClass_Microsoft_UI_Xaml_VisualStateManager),
		//	&spVSMStatics));

		//QueryInterface(__uuidof(xaml_controls.IControl), &spThisAsControl);

		//boolean returnValue = false;
		//spVSMStatics.GoToState(spThisAsControl, wrl_wrappers.Hstring(strState), useTransitions, &returnValue);
		VisualStateManager.GoToState(this, strState, useTransitions);
	}

	internal void SetState(State newState, bool useTransitions)
	{
		// NOTE: Not calling GoToState when the LSI is already in the target
		// state allows us to keep animations looking smooth when the following
		// sequence of events happens:
		// LS starts closing . Items changes . LSIs are Recycled/Realized/Assigned New Content
		if (newState != _state)
		{
			GoToState(newState, useTransitions);
			_state = newState;
		}
	}

	internal void AutomationSelect()
	{
		LoopingSelector pLoopingSelectorNoRef = null;
		GetParentNoRef(out pLoopingSelectorNoRef);
		pLoopingSelectorNoRef.AutomationScrollToVisualIdx(_visualIndex, true /* ignoreScrollingState */);
	}

	internal void AutomationGetSelectionContainerUIAPeer(out AutomationPeer ppPeer)
	{
		//wrl.ComPtr<xaml.Automation.Peers.FrameworkElementAutomationPeerStatics> spAutomationPeerStatics;
		AutomationPeer spAutomationPeer;
		UIElement spLoopingSelectorAsUI;
		LoopingSelector pLoopingSelectorNoRef = null;

		GetParentNoRef(out pLoopingSelectorNoRef);
		//(pLoopingSelectorNoRef.QueryInterface(
		//	__uuidof(UIElement),
		//	&spLoopingSelectorAsUI));
		spLoopingSelectorAsUI = this;

		//(wf.GetActivationFactory(
		//	  wrl_wrappers.Hstring(RuntimeClass_Microsoft_UI_Xaml_Automation_Peers_FrameworkElementAutomationPeer),
		//	  &spAutomationPeerStatics));
		//spAutomationPeerStatics.CreatePeerForElement(spLoopingSelectorAsUI, &spAutomationPeer);
		spAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(spLoopingSelectorAsUI);
		//spAutomationPeer.CopyTo(ppPeer);
		ppPeer = spAutomationPeer;
	}

	internal void AutomationGetIsSelected(out bool value)
	{
		LoopingSelector loopingSelectorNoRef = null;
		int selectedIdx;

		GetParentNoRef(out loopingSelectorNoRef);
		selectedIdx = loopingSelectorNoRef.SelectedIndex;

		uint itemIndex = 0;
		loopingSelectorNoRef.VisualIndexToItemIndex((uint)_visualIndex, out itemIndex);

		value = selectedIdx == (int)itemIndex;
	}

	internal void AutomationUpdatePeerIfExists(int itemIndex)
	{
		if (_hasPeerBeenCreated)
		{
			AutomationPeer spAutomationPeer;
			LoopingSelectorItemAutomationPeer spLSIAP;
			LoopingSelectorItemAutomationPeer pLSIAP;
			UIElement spThisAsUI;
			//wrl.ComPtr<xaml_automation_peers.FrameworkElementAutomationPeerStatics> spAutomationPeerStatics;

			//QueryInterface(__uuidof(UIElement), &spThisAsUI);
			spThisAsUI = this;
			//(wf.GetActivationFactory(
			//	wrl_wrappers.Hstring(RuntimeClass_Microsoft_UI_Xaml_Automation_Peers_FrameworkElementAutomationPeer),
			//	&spAutomationPeerStatics));
			// CreatePeerForElement does not always create one - if there is one associated with this UIElement it will reuse that.
			// We do not want to end up creating a bunch of peers for the same element causing Narrator to get confused.
			//spAutomationPeerStatics.CreatePeerForElement(spThisAsUI, &spAutomationPeer);
			spAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(spThisAsUI);
			//spAutomationPeer.As(spLSIAP);
			spLSIAP = spAutomationPeer as LoopingSelectorItemAutomationPeer;
			pLSIAP = spLSIAP;

			pLSIAP.UpdateEventSource();
			pLSIAP.UpdateItemIndex(itemIndex);
		}
	}
}
