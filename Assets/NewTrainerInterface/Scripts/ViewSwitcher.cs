using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ViewSwitcher : MonoBehaviour {

    public PrimaryView firstView;
    public PrimaryView secondView;
    public PrimaryView thirdView;
    public PrimaryView fourthView;

    public PanelWithVerticalSplitter twoViewVerticalLayout;
    public PanelWithHorizontalSplitter twoViewHorizontalLayout;

    public PanelWithVerticalSplitter threeViewLayout23;
    public PanelWithHorizontalSplitter threeViewLayout34;
    public PanelWithVerticalSplitter threeViewLayout41;
    public PanelWithHorizontalSplitter threeViewLayout12;

    public PanelWithVerticalSplitter fourViewLayout;

    public PanelWithVerticalSplitter parentPanel;

    private List<GameObject> SplittedPanels = null;

    private GameObject currentView;

    void Start()
    {
        currentView = firstView.gameObject;
        SplittedPanels = new List<GameObject>()
           { twoViewVerticalLayout.gameObject, twoViewHorizontalLayout.gameObject, threeViewLayout23.gameObject,
             threeViewLayout34.gameObject, threeViewLayout41.gameObject, threeViewLayout12.gameObject};
    }

	public void OnSecondViewClicked()
    {
        if (secondView.isActiveAndEnabled)
        {
            DeactivateSecondView();
        }
        else
        {
            //activate
            ActivateSecondView();
        }
    }

    private void DeactivateSecondView()
    {
        if (firstView.isActiveAndEnabled && thirdView.isActiveAndEnabled)
        {
            thirdView.transform.SetParent(twoViewHorizontalLayout.topPanel.transform, false);
            firstView.transform.SetParent(twoViewHorizontalLayout.bottomPanel.transform, false);

            thirdView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewHorizontalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewHorizontalLayout, true);
            RemoveSubscribers(threeViewLayout12, true);
            RemoveSubscribers(threeViewLayout23, true);

            twoViewHorizontalLayout.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);
            twoViewHorizontalLayout.OnBottomPanelResize.AddListener(firstView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewHorizontalLayout.OnParentPanelResize);
            secondView.SetActiveAndSelect(false);
            twoViewHorizontalLayout.AllignPanelsWithSplitter();
        }
        else if (firstView.isActiveAndEnabled)
        {
            DeactivateSlittedPanels();
            RemoveSubscribers(twoViewVerticalLayout, true);
            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(firstView.OnParentPanelResize);
            firstView.transform.SetParent(parentPanel.rightPanel.transform, false);
            secondView.SetActiveAndSelect(false);
            firstView.SetActiveAndSelect(true);
            parentPanel.AllignPanelsWithSplitter();
        }
        else if (thirdView.isActiveAndEnabled)
        {
            DeactivateSlittedPanels();
            RemoveSubscribers(twoViewHorizontalLayout, true);
            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(thirdView.OnParentPanelResize);
            thirdView.transform.SetParent(parentPanel.rightPanel.transform, false);
            secondView.SetActiveAndSelect(false);
            thirdView.SetActiveAndSelect(true);
            parentPanel.AllignPanelsWithSplitter();
        }
    }

    public void OnThirdViewClicked()
    {
        if (thirdView.isActiveAndEnabled)
        {
            DeactivateThirdView();
        }
        else
        {
            //activate
            ActivateThirdView();
        }
    }

    private void DeactivateThirdView()
    {
        if (firstView.isActiveAndEnabled && secondView.isActiveAndEnabled)
        {
            firstView.transform.SetParent(twoViewVerticalLayout.leftPanel.transform, false);
            secondView.transform.SetParent(twoViewVerticalLayout.rightPanel.transform, false);

            secondView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewVerticalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewVerticalLayout, true);
            RemoveSubscribers(threeViewLayout12, true);
            RemoveSubscribers(threeViewLayout23, true);

            twoViewVerticalLayout.OnLeftPanelResize.AddListener(firstView.OnParentPanelResize);
            twoViewVerticalLayout.OnRightPanelResize.AddListener(secondView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewVerticalLayout.OnParentPanelResize);
            thirdView.SetActiveAndSelect(false);
            twoViewVerticalLayout.MoveSplitterToCenter();
            twoViewVerticalLayout.AllignPanelsWithSplitter();
        }
        else if (firstView.isActiveAndEnabled)
        {
            DeactivateSlittedPanels();
            RemoveSubscribers(twoViewHorizontalLayout, true);
            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(firstView.OnParentPanelResize);
            firstView.transform.SetParent(parentPanel.rightPanel.transform, false);
            thirdView.SetActiveAndSelect(false);
            firstView.SetActiveAndSelect(true);
            parentPanel.AllignPanelsWithSplitter();
        }
        else if (secondView.isActiveAndEnabled)
        {
            DeactivateSlittedPanels();
            RemoveSubscribers(twoViewHorizontalLayout, true);
            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(secondView.OnParentPanelResize);
            secondView.transform.SetParent(parentPanel.rightPanel.transform, false);
            thirdView.SetActiveAndSelect(false);
            secondView.SetActiveAndSelect(true);
            parentPanel.AllignPanelsWithSplitter();
        }
    }

    private void ActivateSecondView()
    {
        if(firstView.isActiveAndEnabled && thirdView.isActiveAndEnabled)
        {
            firstView.transform.SetParent(threeViewLayout23.leftPanel.transform, false);
            PanelWithHorizontalSplitter rightPanel = threeViewLayout23.rightPanel.GetComponentInChildren<PanelWithHorizontalSplitter>();
            secondView.transform.SetParent(rightPanel.bottomPanel.transform, false);
            thirdView.transform.SetParent(rightPanel.topPanel.transform, false);

            RemoveSubscribers(twoViewVerticalLayout, true);

            threeViewLayout23.OnLeftPanelResize.AddListener(firstView.OnParentPanelResize);
            threeViewLayout23.OnRightPanelResize.AddListener(rightPanel.OnParentPanelResize);
            rightPanel.OnBottomPanelResize.AddListener(secondView.OnParentPanelResize);
            rightPanel.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(threeViewLayout23.OnParentPanelResize);

            DeactivateSlittedPanels();

            secondView.SetActiveAndSelect(true);
            threeViewLayout23.gameObject.SetActive(true);

            //threeViewLayout23.splitter.transform.position = twoViewVerticalLayout.splitter.transform.position;
            threeViewLayout23.MoveSplitterToCenter();
            rightPanel.MoveSplitterToCenter();
            threeViewLayout23.AllignPanelsWithSplitter();
        }
        else if (firstView.isActiveAndEnabled)
        {
            firstView.transform.SetParent(twoViewVerticalLayout.leftPanel.transform, false);
            secondView.transform.SetParent(twoViewVerticalLayout.rightPanel.transform, false);

            secondView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewVerticalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewVerticalLayout, true);

            twoViewVerticalLayout.OnLeftPanelResize.AddListener(firstView.OnParentPanelResize);
            twoViewVerticalLayout.OnRightPanelResize.AddListener(secondView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewVerticalLayout.OnParentPanelResize);
            twoViewVerticalLayout.MoveSplitterToCenter();
            twoViewVerticalLayout.AllignPanelsWithSplitter();
        }
        else if (thirdView.isActiveAndEnabled)
        {

            thirdView.transform.SetParent(twoViewHorizontalLayout.topPanel.transform, false);
            secondView.transform.SetParent(twoViewHorizontalLayout.bottomPanel.transform, false);

            secondView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewHorizontalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewHorizontalLayout, true);

            twoViewHorizontalLayout.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);
            twoViewHorizontalLayout.OnBottomPanelResize.AddListener(secondView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewHorizontalLayout.OnParentPanelResize);

            twoViewHorizontalLayout.MoveSplitterToCenter();
            twoViewVerticalLayout.AllignPanelsWithSplitter();
        }
    }

    private void ActivateThirdView()
    {
        if (secondView.isActiveAndEnabled && firstView.isActiveAndEnabled)
        {
            thirdView.transform.SetParent(threeViewLayout12.topPanel.transform, false);
            PanelWithVerticalSplitter bottomPanel = threeViewLayout12.bottomPanel.GetComponentInChildren<PanelWithVerticalSplitter>();
            firstView.transform.SetParent(bottomPanel.leftPanel.transform, false);
            secondView.transform.SetParent(bottomPanel.rightPanel.transform, false);

            RemoveSubscribers(twoViewVerticalLayout, true);

            threeViewLayout12.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);
            threeViewLayout12.OnBottomPanelResize.AddListener(bottomPanel.OnParentPanelResize);
            bottomPanel.OnLeftPanelResize.AddListener(firstView.OnParentPanelResize);
            bottomPanel.OnRightPanelResize.AddListener(secondView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(threeViewLayout12.OnParentPanelResize);

            DeactivateSlittedPanels();

            thirdView.SetActiveAndSelect(true);
            threeViewLayout12.gameObject.SetActive(true);

            bottomPanel.splitter.transform.position = twoViewVerticalLayout.splitter.transform.position;
            threeViewLayout12.MoveSplitterToCenter();
            bottomPanel.MoveSplitterToCenter();
            
            threeViewLayout12.AllignPanelsWithSplitter();
        }
        else if (secondView.isActiveAndEnabled)
        {
            thirdView.transform.SetParent(twoViewHorizontalLayout.topPanel.transform, false);
            secondView.transform.SetParent(twoViewHorizontalLayout.bottomPanel.transform, false);

            thirdView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewHorizontalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewVerticalLayout, true);

            twoViewHorizontalLayout.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);
            twoViewHorizontalLayout.OnBottomPanelResize.AddListener(secondView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewHorizontalLayout.OnParentPanelResize);

            twoViewHorizontalLayout.MoveSplitterToCenter();
            twoViewHorizontalLayout.AllignPanelsWithSplitter();
        }
        else if (firstView.isActiveAndEnabled)
        {
            thirdView.transform.SetParent(twoViewHorizontalLayout.topPanel.transform, false);
            firstView.transform.SetParent(twoViewHorizontalLayout.bottomPanel.transform, false);

            thirdView.SetActiveAndSelect(true);
            DeactivateSlittedPanels();
            twoViewHorizontalLayout.gameObject.SetActive(true);

            RemoveSubscribers(twoViewHorizontalLayout, true);

            twoViewHorizontalLayout.OnTopPanelResize.AddListener(thirdView.OnParentPanelResize);
            twoViewHorizontalLayout.OnBottomPanelResize.AddListener(firstView.OnParentPanelResize);

            parentPanel.OnRightPanelResize.RemoveAllListeners();
            parentPanel.OnRightPanelResize.AddListener(twoViewHorizontalLayout.OnParentPanelResize);

            twoViewHorizontalLayout.MoveSplitterToCenter();
            twoViewHorizontalLayout.AllignPanelsWithSplitter();
        }
    }

    void DeactivateSlittedPanels()
    {
        foreach (var l_go in SplittedPanels) l_go.SetActive(false);
    }

    private void RemoveSubscribers(PanelWithVerticalSplitter a_parent, bool a_removeFromChildren)
    {
        a_parent.OnLeftPanelResize.RemoveAllListeners();
        a_parent.OnRightPanelResize.RemoveAllListeners();
        if (a_removeFromChildren)
        {
            foreach (var l_child in a_parent.GetComponentsInChildren<PanelWithHorizontalSplitter>())
            {
                RemoveSubscribers(l_child, false);
            }
            foreach (var l_child in a_parent.GetComponentsInChildren<PanelWithVerticalSplitter>())
            {
                RemoveSubscribers(l_child, false);
            }
        }
    }

    private void RemoveSubscribers(PanelWithHorizontalSplitter a_parent, bool a_removeFromChildren)
    {
        a_parent.OnBottomPanelResize.RemoveAllListeners();
        a_parent.OnTopPanelResize.RemoveAllListeners();
        if (a_removeFromChildren)
        {
            foreach (var l_child in a_parent.GetComponentsInChildren<PanelWithHorizontalSplitter>())
            {
                RemoveSubscribers(l_child, false);
            }
            foreach (var l_child in a_parent.GetComponentsInChildren<PanelWithVerticalSplitter>())
            {
                RemoveSubscribers(l_child, false);
            }
        }
    }
    
}
