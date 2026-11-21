// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace DevWinUI;


internal partial class LoopingSelectorItemAutomationPeer
{

	internal LoopingSelectorItemAutomationPeer(LoopingListSelectorInfo pOwner) : base(pOwner)
	{
		InitializeImpl(pOwner);
	}

	void InitializeImpl(LoopingListSelectorInfo pOwner)
	{

		//FrameworkElementAutomationPeerFactory spInnerFactory;
		//FrameworkElementAutomationPeer spInnerInstance;
		//FrameworkElement spLoopingSelectorItemAsFE;
		//LoopingSelectorItem spOwner = pOwner;
		//DependencyObject spInnerInspectable;

		//LoopingSelectorItemAutomationPeerGenerated.InitializeImpl(pOwner);
		//(wf.GetActivationFactory(
		//	wrl_wrappers.Hstring(RuntimeClass_Microsoft_UI_Xaml_Automation_Peers_FrameworkElementAutomationPeer),
		//	&spInnerFactory));

		//(((DependencyObject)(pOwner)).QueryInterface<FrameworkElement>(
		//	&spLoopingSelectorItemAsFE));
		//spLoopingSelectorItemAsFE = pOwner;

		//(spInnerFactory.CreateInstanceWithOwner(
		//	spLoopingSelectorItemAsFE,
		//	(ILoopingSelectorItemAutomationPeer)(this),
		//	&spInnerInspectable,
		//	&spInnerInstance));

		//(SetComposableBasePointers(
		//	spInnerInspectable,
		//	spInnerFactory));

		UpdateEventSource();
	}

	internal void UpdateEventSource()
	{
		LoopingSelectorItemDataAutomationPeer? spLSIDAP;

		GetDataAutomationPeer(out spLSIDAP);

		if (spLSIDAP is { })
		{
			SetEventSource(spLSIDAP);
		}

		return;
	}

	internal void UpdateItemIndex(int itemIndex)
	{
		LoopingSelectorItemDataAutomationPeer? spLSIDAP;

		GetDataAutomationPeer(out spLSIDAP);

		if (spLSIDAP is { })
		{
			((LoopingSelectorItemDataAutomationPeer)spLSIDAP).SetItemIndex(itemIndex);
		}

		return;
	}

	internal void SetEventSource(LoopingSelectorItemDataAutomationPeer pLSIDAP)
	{
		AutomationPeer spLSIDAPAsAP;
		AutomationPeer spThisAsAP;

		//pLSIDAP.QueryInterface<AutomationPeer>(spLSIDAPAsAP);
		spLSIDAPAsAP = pLSIDAP;
		//(QueryInterface(
		//	__uuidof(AutomationPeer),
		//	&spThisAsAP));
		spThisAsAP = this;
		spThisAsAP.EventsSource = spLSIDAPAsAP;
	}

	void GetDataAutomationPeer(
		out LoopingSelectorItemDataAutomationPeer? ppLSIDAP)
	{
		ppLSIDAP = null;
		if (Owner is not LoopingListSelectorInfo owner || owner.Content is not DependencyObject item)
		{
			return;
		}

		owner.GetParentNoRef(out var loopingSelector);
		if (loopingSelector is null)
		{
			return;
		}

		if (FrameworkElementAutomationPeer.CreatePeerForElement(loopingSelector) is LoopingSelectorAutomationPeer parentPeer)
		{
			parentPeer.GetDataAutomationPeerForItem(item, out ppLSIDAP);
		}
	}

	void GetOwnerAsInternalPtrNoRef(
		out LoopingListSelectorInfo? ppOwnerNoRef)
	{
		ppOwnerNoRef = null;
		if (Owner is LoopingListSelectorInfo owner)
		{
			ppOwnerNoRef = owner;
		}

	}

	#region AutomationPeerOverrides

	protected override object? GetPatternCore(PatternInterface patternInterface)
	{

		DependencyObject? returnValue = default;
		if (patternInterface == PatternInterface.ScrollItem ||
			patternInterface == PatternInterface.SelectionItem)
		{
			returnValue = (LoopingSelectorItemAutomationPeer)(this);
			//AddRef();
		}
		else
		{
			//LoopingSelectorItemAutomationPeerGenerated.GetPatternCoreImpl(patternInterface, returnValue);
		}

		return returnValue;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		AutomationControlType returnValue = default;

		//if (returnValue == null) throw new ArgumentNullException();
		returnValue = AutomationControlType.ListItem;

		return returnValue;

	}

	protected override string GetClassNameCore()
	{

		//if (returnValue == null) throw new ArgumentNullException();
		//wrl_wrappers.Hstring("LoopingSelectorItem").CopyTo(returnValue);
		return "LoopingSelectorItem";
	}

	protected override bool IsKeyboardFocusableCore()
	{
		bool returnValue = default;
		AutomationPeer? loopingSelectorAP;

		returnValue = false;

		// LoopingSelectorItems aren't actually keyboard focusable,
		// but we need to give them automation focus in order to have
		// UIA clients like Narrator read their contents when they're
		// selected, so we'll act as though they're keyboard focusable
		// to enable that to be possible.  In order to do this,
		// for the keyboard focus status of a LoopingSelectorItem,
		// we'll just report the keyboard focus status of its parent LoopingSelector.
		GetParentAutomationPeer(out loopingSelectorAP);

		if (loopingSelectorAP is { })
		{
			returnValue = loopingSelectorAP.IsKeyboardFocusable();
		}

		return returnValue;
	}

	protected override bool HasKeyboardFocusCore()
	{
		AutomationPeer? loopingSelectorAP;

		bool returnValue = false;

		// In order to support giving automation focus to selected LoopingSelectorItem
		// automation peers, we'll report that a LoopingSelectorItem has keyboard focus
		// if its parent LoopingSelector has keyboard focus, and if this LoopingSelectorItem
		// is selected.
		GetParentAutomationPeer(out loopingSelectorAP);

		if (loopingSelectorAP is { })
		{
			bool hasKeyboardFocus = false;

			hasKeyboardFocus = loopingSelectorAP.HasKeyboardFocus();

			if (hasKeyboardFocus)
			{
				returnValue = IsSelected;
			}
		}

		return returnValue;
	}


	#endregion

	#region IScrollItemProvider

	public void ScrollIntoView()
	{
		LoopingListSelectorInfo? pOwnerNoRef = null;

		GetOwnerAsInternalPtrNoRef(out pOwnerNoRef);

		if (pOwnerNoRef is { })
		{
			LoopingSelector? pOwnerParentNoRef = null;

			pOwnerNoRef.GetParentNoRef(out pOwnerParentNoRef);

			if (pOwnerParentNoRef is { })
			{
				if (pOwnerNoRef.Content is DependencyObject content)
				{
					pOwnerParentNoRef.AutomationTryScrollItemIntoView(content);
				}
			}
		}

		return;
	}

	#endregion

	#region ISelectionItemProvider

	public bool IsSelected
	{
		get
		{
			bool value = default;
			LoopingListSelectorInfo? pOwnerNoRef = null;
			GetOwnerAsInternalPtrNoRef(out pOwnerNoRef);
			if (pOwnerNoRef is { })
			{
				pOwnerNoRef.AutomationGetIsSelected(out value);
			}

			return value;
		}
	}

	public IRawElementProviderSimple SelectionContainer
	{
		get
		{
			LoopingListSelectorInfo? pOwnerNoRef = null;
			GetOwnerAsInternalPtrNoRef(out pOwnerNoRef);
			if (pOwnerNoRef is null)
			{
				throw new InvalidOperationException("The owning LoopingSelector is unavailable.");
			}

			pOwnerNoRef.AutomationGetSelectionContainerUIAPeer(out AutomationPeer? spAutomationPeer);
			if (spAutomationPeer is null)
			{
				throw new InvalidOperationException("The selection container automation peer is unavailable.");
			}

			return ProviderFromPeer(spAutomationPeer);
		}
	}

	public void AddToSelection()
	{
		//return UIA_E_INVALIDOPERATION;
		throw new InvalidOperationException();
	}

	public void RemoveFromSelection()
	{
		//return UIA_E_INVALIDOPERATION;
		throw new InvalidOperationException();
	}

	public void Select()
	{

		LoopingListSelectorInfo? pOwnerNoRef = null;
		GetOwnerAsInternalPtrNoRef(out pOwnerNoRef);
		if (pOwnerNoRef is { })
		{
			pOwnerNoRef.AutomationSelect();
		}



	}

	#endregion

	void
		GetParentAutomationPeer(
			out AutomationPeer? parentAutomationPeer)
	{
		LoopingListSelectorInfo? pOwnerNoRef = null;

		parentAutomationPeer = null;

		GetOwnerAsInternalPtrNoRef(out pOwnerNoRef);

		if (pOwnerNoRef is { })
		{
			LoopingSelector? pLoopingSelectorNoRef = null;

			pOwnerNoRef.GetParentNoRef(out pLoopingSelectorNoRef);

			if (pLoopingSelectorNoRef is { })
			{
				LoopingSelector loopingSelector = pLoopingSelectorNoRef;
				UIElement loopingSelectorAsUIE;

				//IGNOREHR(loopingSelector.As(loopingSelectorAsUIE));
				loopingSelectorAsUIE = loopingSelector;

				if (loopingSelectorAsUIE is { })
				{
		AutomationPeer? loopingSelectorAP;

					//(Private.AutomationHelper.CreatePeerForElement(
					//	loopingSelectorAsUIE,
					//	&loopingSelectorAP));
					loopingSelectorAP = AutomationHelper.CreatePeerForElement(loopingSelectorAsUIE);

					parentAutomationPeer = loopingSelectorAP;
				}
			}
		}

		return;
	}

}
