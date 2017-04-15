using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TreeNode : MonoBehaviour {
    public bool isRoot = false;
    public GameObject treeNodePrefab;
    public GameObject treeNodeSelection;
    public float levelOffset = 25.0f;
    public TreeContainer viewContainer = null;
    // Use this for initialization
    public string path { get { return i_fullPath; } set { i_fullPath = value; } }

    public string folderName { get { return i_folderName; } set { i_folderName = value; } }

    public TreeNode parent { get { return i_parent; } set { i_parent = value; } }

    public List<TreeNode> children { get { return new List<TreeNode>(i_children); } }

    public bool isExpanded { get { return i_isExpanded; } set { if (value != i_isExpanded) { i_isExpanded = Expand(value); } } }

    public int childrenCount { get { return i_children.Count; } }

    public int expandedChildrenCount {
        get {
            int l_childrenCount = i_children.Count;
            foreach(var l_child in i_children)
            {
                if (l_child.i_isExpanded) l_childrenCount += l_child.expandedChildrenCount;
            }
            return l_childrenCount;
        }
    }

    public float nodeHeight { get { return i_transform.offsetMax.y - i_transform.offsetMin.y; } }

    [System.Serializable]
    public class TreeNodeExpanded : UnityEvent<TreeNode, bool> { }
    [SerializeField]
    public TreeNodeExpanded onNodeExpanded;

    [System.Serializable]
    public class TreeNodeEvent : UnityEvent<TreeNode> { }
    [SerializeField]
    public TreeNodeEvent onNodeClicked;
    public TreeNodeEvent onTreePopulated;
    

    private TreeNode i_parent = null;
    private string i_fullPath = null;
    private string i_folderName = "";
    private List<TreeNode> i_children = null;
    private RectTransform i_transform;
    private Text i_text = null;
    private const string c_rootDirectory = "Records";
    private bool i_isExpanded = false;
    private int i_level = 0;

    void Awake()
    {
        i_transform = (RectTransform)transform;
        i_text = GetComponent<Text>();
    }

	void Start () {
        
	    if(isRoot)
        {
            //Populate tree;
            i_fullPath = c_rootDirectory;
            if (!Directory.Exists(i_fullPath))
            {
                Directory.CreateDirectory(i_fullPath);
            }

            if (viewContainer != null)
            {
                onTreePopulated.AddListener(viewContainer.OnTreePopulated);
                onNodeClicked.AddListener(viewContainer.OnNodeClicked);
            }

            i_children = PopulateNode();

            onTreePopulated.Invoke(this);

            isExpanded = true;
            
            
        }
	}

    public List<TreeNode> PopulateNode()
    {
        List<TreeNode> l_children = new List<TreeNode>();
        List<string> l_childrenDirs = new List<string>(Directory.GetDirectories(i_fullPath));
        foreach(string dir in l_childrenDirs)
        {
            GameObject l_newObj = Instantiate(treeNodePrefab);
            TreeNode l_newNode = l_newObj.GetComponent<TreeNode>();
            l_newNode.viewContainer = viewContainer;
            l_newNode.onNodeExpanded.AddListener(viewContainer.OnNodeExpanded);
            l_newNode.onNodeClicked.AddListener(viewContainer.OnNodeClicked);
            l_children.Add(l_newNode);
            l_newNode.parent = this;
            l_newNode.path = dir;
            RectTransform l_nodeTr = (RectTransform)l_newNode.transform;
            l_nodeTr.SetParent( i_transform.parent, false);
            l_newNode.PopulateNode();
            l_newObj.SetActive(false);
        }
        i_text.text = Path.GetFileNameWithoutExtension(i_fullPath);
        i_level = GetLevel();
        i_children = l_children;
        i_folderName = Path.GetDirectoryName(i_fullPath);
        return l_children;
    }

    int GetLevel()
    {
        if (parent == null) return 0;
        return 1 + i_parent.GetLevel();
    }

    bool Expand(bool a_expand)
    {
        //expand node

        if (a_expand == true)
        {
            ExpandNode();
        }
        //collapse node
        else
        {
            CollapseChildren();
        }
        onNodeExpanded.Invoke(this, a_expand);
        return a_expand;
    }

    void ExpandNode()
    {
        AllignChildren();
        MoveDownNighbours(i_children.Count);
        foreach(var l_child in i_children)
        {
            if (l_child.i_isExpanded) l_child.ExpandNode();
        }
    }

    private void MoveDownNighbours(int a_count)
    {
        moveUpNeighbours(-a_count);
    }

    private void AllignChildren()
    {
        float l_height = i_transform.offsetMax.y - i_transform.offsetMin.y;
        float l_verticalOffset = l_height;
        for(int i = 0; i != i_children.Count; ++i)
        {
            TreeNode l_child = i_children[i];
            l_child.i_transform.offsetMin = new Vector2(i_transform.offsetMin.x + levelOffset, i_transform.offsetMin.y - l_verticalOffset);
            l_child.i_transform.offsetMax = new Vector2(l_child.i_transform.offsetMax.x, i_transform.offsetMax.y - l_verticalOffset);
            l_child.gameObject.SetActive(true);
            l_verticalOffset += l_height;
        }
    }

    void CollapseChildren()
    {
        foreach(var l_child in i_children)
        {
            if(l_child.childrenCount > 0)
            {
                if (l_child.isExpanded)
                {
                    l_child.CollapseChildren();
                }
            }
            l_child.gameObject.SetActive(false);
        }
        if (i_children.Count > 0)
        {
            moveUpNeighbours(i_children.Count);
        }
    }

    private void moveUpNeighbours(int a_count)
    {
        if (parent == null) return;
        int l_idx = 0;
        int l_parentChildrenCount = parent.i_children.Count;
        for (int i = 0; i != l_parentChildrenCount; ++i)
        {
            if(parent.i_children[i] == this) { l_idx = i; break; }
        }
        for(int i = ++l_idx; i!= l_parentChildrenCount; ++i)
        {
            parent.i_children[i].MoveUp(a_count);
        }
        parent.moveUpNeighbours(a_count);
    }

    void MoveUp(int a_count)
    {
        float l_height = i_transform.offsetMax.y - i_transform.offsetMin.y;
        i_transform.offsetMin = new Vector2(i_transform.offsetMin.x, i_transform.offsetMin.y + a_count*l_height);
        i_transform.offsetMax = new Vector2(i_transform.offsetMax.x, i_transform.offsetMax.y + a_count*l_height);
        foreach (var child in children) child.MoveUp(a_count);
    }

    void SetActiveSubtree(bool a_isActive)
    {
        foreach(var child in i_children)
        {
            child.SetActiveSubtree(a_isActive);
        }
        gameObject.SetActive(a_isActive);
    }

    private void ParentMove(TreeNode a_moveAfter, int a_linesNum)
    {
        int l_childrenCount = i_children.Count;
        int l_idx = 0;
        for (int i = 0; i != l_childrenCount; ++i)
        {
            if(a_moveAfter == i_children[i]) { l_idx = i; } 
        }
        for(int i = l_idx + 1; i != l_childrenCount; ++i)
        {
            children[i].MoveByLinesNum(a_linesNum);
        }
        if ( i_parent != null)
        {
            i_parent.ParentMove(this, a_linesNum);
        }
    }

    

    private void MoveByLinesNum(int a_linesNum)
    {
        float l_height = i_transform.offsetMax.y - i_transform.offsetMin.y;
        i_transform.offsetMin = new Vector2(i_transform.offsetMin.x, i_transform.offsetMin.y - a_linesNum * l_height);
        i_transform.offsetMax = new Vector2(i_transform.offsetMax.x, i_transform.offsetMax.y - a_linesNum * l_height);
        /*if (i_isExpanded)*/ foreach (var child in children) child.MoveByLinesNum(a_linesNum);
    }
    
    public void onFolderIconClick()
    {
        i_isExpanded = Expand(!i_isExpanded);
    }

    public void HighlightNode(bool a_enabled)
    {
        treeNodeSelection.SetActive(a_enabled);
    }

    public void OnClick()
    {
        onNodeClicked.Invoke(this);
    }
}
