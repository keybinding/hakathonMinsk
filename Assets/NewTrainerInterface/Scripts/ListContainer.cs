using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class ListContainer : MonoBehaviour {

    public GameObject listNodePrefab;
    public GameObject recordPrefab;

    RectTransform i_transform;

    List<FolderListNode> i_childrenNodes = new List<FolderListNode>();

    FolderListNode i_currentNode = null;
    TreeNode i_currentTreeNode = null;

    // Use this for initialization

    void Awake ()
    {
        i_transform = (RectTransform)transform;
    }

    public void PopulateList(string a_directoryPath)
    {
        List<string> l_directories = new List<string>(Directory.GetDirectories(a_directoryPath));
        List<string> l_files = new List<string>( Directory.GetFiles(a_directoryPath, "*.nrb"));
        foreach(string l_file in l_files)
        {
            FolderListNode l_newNode = Instantiate(recordPrefab).GetComponent<FolderListNode>();
            l_newNode.gameObject.transform.SetParent(gameObject.transform, false);
            i_childrenNodes.Add(l_newNode);
            l_newNode.InitializeFile(l_file, this);
        }

        foreach (string l_dir in l_directories)
        {
            FolderListNode l_newNode = Instantiate(listNodePrefab).GetComponent<FolderListNode>();
            l_newNode.gameObject.transform.SetParent(gameObject.transform, false);
            i_childrenNodes.Add(l_newNode);
            l_newNode.InitializeFile(l_dir, this);
        }

        AlignNodes();
        if (i_childrenNodes.Count > 0)
        {
            onListPopulated.Invoke(i_childrenNodes);
        }
    }

    private void AlignNodes()
    {
        float l_curOffset = 0.0f;
        float l_nodeHeight = GetNodePrefabHeight();
        foreach(FolderListNode l_node in i_childrenNodes)
        {
            l_node.transform.Translate(new Vector3(0, -l_curOffset, 0));
            l_curOffset += l_nodeHeight;
        }
        SetContainerHeight(-Mathf.Abs(l_curOffset));
    }

    private void SetContainerHeight(float l_curOffset)
    {
        i_transform.offsetMin = new Vector2(i_transform.offsetMin.x, l_curOffset);
    }

    float GetNodePrefabHeight()
    {
        RectTransform l_prefabTransform = (RectTransform)listNodePrefab.transform;
        return l_prefabTransform.rect.height;
    }

    public void OnFolderNodeSelected(TreeNode a_treeNode)
    {
        Flush();
        i_currentTreeNode = a_treeNode;
        PopulateList(a_treeNode.path);
        if (i_childrenNodes.Count > 0)
        {
            i_currentNode = i_childrenNodes[0];
            SelectNode(i_currentNode, true);
        }
        SubscribeToChildren();
    }

    public void OnRecordSaved()
    {
        if (i_currentTreeNode != null)
        {
            Flush();
            PopulateList(i_currentTreeNode.path);
            if (i_childrenNodes.Count > 0)
            {
                i_currentNode = i_childrenNodes[0];
                SelectNode(i_currentNode, true);
            }
            SubscribeToChildren();
        }
    }

    private void Flush()
    {
        foreach(var l_child in i_childrenNodes)
        {
            DestroyImmediate(l_child.gameObject);
        }
        i_childrenNodes.Clear();
    }

    void SelectNode(FolderListNode a_node, bool a_enabled)
    {
        a_node.HighlightNode(a_enabled);
    }

    public void OnNodeClicked(FolderListNode a_clickedNode)
    {
        if (i_currentNode != a_clickedNode)
        {
            SelectionChanged(a_clickedNode, i_currentNode);
        }
    }

    private void SelectionChanged(FolderListNode a_currentNode, FolderListNode a_previousNode)
    {
        a_previousNode.HighlightNode(false);
        a_currentNode.HighlightNode(true);
        i_currentNode = a_currentNode;
        //onNodeSelected.Invoke(i_currentNode);
    }

    void SubscribeToChildren()
    {
        foreach(var l_child in i_childrenNodes)
        {
            l_child.onNodeClicked.AddListener(OnNodeClicked);
        }
    }

    [System.Serializable]
    public class Populated : UnityEvent<List<FolderListNode>> { }
    [SerializeField]
    public Populated onListPopulated;
}
