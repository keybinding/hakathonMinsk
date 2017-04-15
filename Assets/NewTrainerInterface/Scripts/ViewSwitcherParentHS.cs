using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ViewSwitcherParentHS : MonoBehaviour
{

    public PrimaryView firstView;
    public PrimaryView secondView;
    public PrimaryView thirdView;
    public PrimaryView fourthView;
    public PanelWithHorizontalSplitter parentPanel;
    //Prefabs
    public GameObject PanelWithVerticalSplitterPrefab = null;
    public GameObject PanelWithHorizontalSplitterPrefab = null;

    private List<GameObject> i_primaryPanels = null;

    // Use this for initialization
    void Start()
    {
        i_primaryPanels = new List<GameObject>() {
            firstView != null ? firstView.gameObject : null,
            secondView != null ? secondView.gameObject : null,
            thirdView != null ? thirdView.gameObject : null,
            fourthView != null ? fourthView.gameObject : null
        };
    }

    void CreateSingleView(PrimaryView a_view)
    {
        DeleteSecondaryPanels();
        a_view.transform.SetParent(parentPanel.topPanel, false);
        parentPanel.OnTopPanelResize.RemoveAllListeners();
        parentPanel.OnTopPanelResize.AddListener(a_view.OnParentPanelResize);
        parentPanel.AllignPanelsWithSplitter();
    }

    void CreateTwoVertical(PrimaryView a_left, PrimaryView a_right)
    {
        DeleteSecondaryPanels();
        parentPanel.OnTopPanelResize.RemoveAllListeners();
        PanelWithVerticalSplitter l_panelVS = Instantiate(PanelWithVerticalSplitterPrefab).GetComponent<PanelWithVerticalSplitter>();
        l_panelVS.transform.SetParent(parentPanel.topPanel, false);
        parentPanel.OnTopPanelResize.AddListener(l_panelVS.OnParentPanelResize);
        l_panelVS.MoveSplitterToCenter();
        a_left.transform.SetParent(l_panelVS.leftPanel.transform, false);
        a_right.transform.SetParent(l_panelVS.rightPanel.transform, false);
        l_panelVS.OnLeftPanelResize.AddListener(a_left.OnParentPanelResize);
        l_panelVS.OnRightPanelResize.AddListener(a_right.OnParentPanelResize);
        parentPanel.AllignPanelsWithSplitter();
    }

    void CreateTwoHorizontal(PrimaryView a_btm, PrimaryView a_top)
    {
        DeleteSecondaryPanels();
        parentPanel.OnTopPanelResize.RemoveAllListeners();
        PanelWithHorizontalSplitter l_panelHS = Instantiate(PanelWithHorizontalSplitterPrefab).GetComponent<PanelWithHorizontalSplitter>();
        l_panelHS.transform.SetParent(parentPanel.topPanel, false);
        parentPanel.OnTopPanelResize.AddListener(l_panelHS.OnParentPanelResize);
        l_panelHS.MoveSplitterToCenter();
        a_btm.transform.SetParent(l_panelHS.bottomPanel.transform, false);
        a_top.transform.SetParent(l_panelHS.topPanel.transform, false);
        l_panelHS.OnBottomPanelResize.AddListener(a_btm.OnParentPanelResize);
        l_panelHS.OnTopPanelResize.AddListener(a_top.OnParentPanelResize);
        parentPanel.AllignPanelsWithSplitter();
    }

    void CreateHorizontalWithVertTop(PrimaryView a_bot, PrimaryView a_topL, PrimaryView a_topR)
    {
        DeleteSecondaryPanels();

        parentPanel.OnTopPanelResize.RemoveAllListeners();
        PanelWithHorizontalSplitter l_panelHS = Instantiate(PanelWithHorizontalSplitterPrefab).GetComponent<PanelWithHorizontalSplitter>();
        l_panelHS.transform.SetParent(parentPanel.topPanel, false);
        parentPanel.OnTopPanelResize.AddListener(l_panelHS.OnParentPanelResize);
        l_panelHS.MoveSplitterToCenter();

        a_bot.transform.SetParent(l_panelHS.bottomPanel.transform, false);
        l_panelHS.OnBottomPanelResize.AddListener(a_bot.OnParentPanelResize);

        PanelWithVerticalSplitter l_panelVS = Instantiate(PanelWithVerticalSplitterPrefab).GetComponent<PanelWithVerticalSplitter>();
        l_panelVS.transform.SetParent(l_panelHS.topPanel.transform, false);
        l_panelVS.MoveSplitterToCenter();
        l_panelHS.OnTopPanelResize.AddListener(l_panelVS.OnParentPanelResize);

        a_topL.transform.SetParent(l_panelVS.leftPanel.transform, false);
        a_topR.transform.SetParent(l_panelVS.rightPanel.transform, false);
        l_panelVS.OnLeftPanelResize.AddListener(a_topL.OnParentPanelResize);
        l_panelVS.OnRightPanelResize.AddListener(a_topR.OnParentPanelResize);

        parentPanel.AllignPanelsWithSplitter();
    }

    void CreateHorizontalWithVertBot(PrimaryView a_top, PrimaryView a_botL, PrimaryView a_botR)
    {
        DeleteSecondaryPanels();
        parentPanel.OnTopPanelResize.RemoveAllListeners();

        PanelWithHorizontalSplitter l_panelHS = Instantiate(PanelWithHorizontalSplitterPrefab).GetComponent<PanelWithHorizontalSplitter>();
        l_panelHS.transform.SetParent(parentPanel.topPanel, false);
        parentPanel.OnTopPanelResize.AddListener(l_panelHS.OnParentPanelResize);
        l_panelHS.MoveSplitterToCenter();

        a_top.transform.SetParent(l_panelHS.topPanel.transform, false);
        l_panelHS.OnTopPanelResize.AddListener(a_top.OnParentPanelResize);

        PanelWithVerticalSplitter l_panelVS = Instantiate(PanelWithVerticalSplitterPrefab).GetComponent<PanelWithVerticalSplitter>();
        l_panelVS.transform.SetParent(l_panelHS.bottomPanel.transform, false);
        l_panelVS.MoveSplitterToCenter();
        l_panelHS.OnBottomPanelResize.AddListener(l_panelVS.OnParentPanelResize);

        a_botL.transform.SetParent(l_panelVS.leftPanel.transform, false);
        a_botR.transform.SetParent(l_panelVS.rightPanel.transform, false);
        l_panelVS.OnLeftPanelResize.AddListener(a_botL.OnParentPanelResize);
        l_panelVS.OnRightPanelResize.AddListener(a_botR.OnParentPanelResize);

        parentPanel.AllignPanelsWithSplitter();
    }

    void CreateFull()
    {

    }

    void DeleteSecondaryPanels()
    {
        Stack<Transform> l_stack = new Stack<Transform>();
        l_stack.Push(parentPanel.topPanel);
        while (l_stack.Count > 0)
        {
            Transform l_curTr = l_stack.Pop();
            if (l_curTr.childCount > 0)
            {
                foreach (Transform l_tr in l_curTr)
                {
                    if (i_primaryPanels.Contains(l_tr.gameObject)) l_tr.SetParent(parentPanel.topPanel.transform, false);
                    else l_stack.Push(l_tr);
                }
            }
            else DestroyImmediate(l_curTr.gameObject);
        }
    }

    void ReArrangeLayout(PrimaryView a_obj1, PrimaryView a_obj2, PrimaryView a_obj3, PrimaryView a_obj4)
    {
        if (a_obj1 != null && a_obj2 == null && a_obj3 == null && a_obj4 == null) CreateSingleView(a_obj1);
        else if (a_obj1 == null && a_obj2 != null && a_obj3 == null && a_obj4 == null) CreateSingleView(a_obj2);
        else if (a_obj1 == null && a_obj2 == null && a_obj3 != null && a_obj4 == null) CreateSingleView(a_obj3);
        else if (a_obj1 == null && a_obj2 == null && a_obj3 == null && a_obj4 != null) CreateSingleView(a_obj4);

        else if (a_obj1 != null && a_obj2 == null && a_obj3 != null && a_obj4 == null) CreateTwoVertical(a_obj1, a_obj3);
        else if (a_obj1 != null && a_obj2 != null && a_obj3 == null && a_obj4 == null) CreateTwoVertical(a_obj1, a_obj2);
        else if (a_obj1 == null && a_obj2 != null && a_obj3 == null && a_obj4 != null) CreateTwoVertical(a_obj4, a_obj2);
        else if (a_obj1 == null && a_obj2 == null && a_obj3 != null && a_obj4 != null) CreateTwoVertical(a_obj4, a_obj3);

        else if (a_obj1 != null && a_obj2 == null && a_obj3 != null && a_obj4 != null) CreateTwoHorizontal(a_obj1, a_obj4);
        else if (a_obj1 == null && a_obj2 != null && a_obj3 != null && a_obj4 == null) CreateTwoHorizontal(a_obj2, a_obj3);

        else if (a_obj1 != null && a_obj2 != null && a_obj3 != null && a_obj4 == null) CreateHorizontalWithVertBot(a_obj3, a_obj1, a_obj2);
        else if (a_obj1 != null && a_obj2 != null && a_obj3 == null && a_obj4 != null) CreateHorizontalWithVertBot(a_obj4, a_obj1, a_obj2);
        else if (a_obj1 != null && a_obj2 == null && a_obj3 != null && a_obj4 != null) CreateHorizontalWithVertTop(a_obj1, a_obj4, a_obj3);
        else if (a_obj1 == null && a_obj2 != null && a_obj3 != null && a_obj4 != null) CreateHorizontalWithVertTop(a_obj2, a_obj4, a_obj3);

        else CreateFull();
    }

    public void OnFirstViewButtonClick()
    {
        OnViewButtonClick(firstView);
    }

    void OnViewButtonClick(PrimaryView a_clickedView)
    {
        bool l_onlyAvailable = false;

        foreach (var l_view in i_primaryPanels)
        {
            if (l_view != null)
            {
                PrimaryView l_viewC = l_view.GetComponent<PrimaryView>();
                if (l_viewC == a_clickedView) continue;
                l_onlyAvailable = l_onlyAvailable || l_viewC.isActiveAndEnabled;
            }
        }

        if (!l_onlyAvailable) return;

        if (a_clickedView.isActiveAndEnabled)
        {
            a_clickedView.SetActiveAndSelect(false);
        }
        else
        {
            a_clickedView.SetActiveAndSelect(true);
        }
        ReArrangeLayout(firstView != null ? (firstView.isActiveAndEnabled ? firstView : null) : null,
                        secondView != null ? (secondView.isActiveAndEnabled ? secondView : null) : null,
                        thirdView != null ? (thirdView.isActiveAndEnabled ? thirdView : null) : null,
                        fourthView != null ? (fourthView.isActiveAndEnabled ? fourthView : null) : null);
    }

    public void OnSecondViewButtonClick()
    {
        OnViewButtonClick(secondView);
    }

    public void OnThirdViewButtonClick()
    {
        OnViewButtonClick(thirdView);
    }

    public void OnFourthViewButtonClick()
    {
        OnViewButtonClick(fourthView);
    }
}
