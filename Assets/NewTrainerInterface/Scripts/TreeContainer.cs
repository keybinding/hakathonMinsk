using UnityEngine;
using System.Collections.Generic;

public class TreeContainer : MonoBehaviour {

    RectTransform i_transform;
    TreeNode i_rootNode = null;
    TreeNode i_currentNode = null;
    List<TreeNode> i_selectedNodes = new List<TreeNode>();

    [SerializeField]
    TreeNode.TreeNodeEvent onNodeSelected;

	// Use this for initialization
	void Awake () {
        i_transform = (RectTransform)transform;
	}

    public void OnNodeExpanded(TreeNode a_sender, bool a_isExpanded)
    {
        if (a_isExpanded)
        {
            i_transform.offsetMin = new Vector2(i_transform.offsetMin.x, i_transform.offsetMin.y - a_sender.expandedChildrenCount * a_sender.nodeHeight);
        }
        else
        {
            i_transform.offsetMin = new Vector2(i_transform.offsetMin.x, i_transform.offsetMin.y + a_sender.expandedChildrenCount * a_sender.nodeHeight);
        }
    }

    public void OnNodeClicked(TreeNode a_clickedNode)
    {
        if(i_currentNode != a_clickedNode)
        {
            SelectionChanged(a_clickedNode, i_currentNode);
        }
    }

    private void SelectionChanged(TreeNode a_currentNode, TreeNode a_previousNode)
    {
        a_previousNode.HighlightNode(false);
        a_currentNode.HighlightNode(true);
        i_currentNode = a_currentNode;
        onNodeSelected.Invoke(i_currentNode);
    }

    public void OnTreePopulated(TreeNode a_rootNode)
    {
        i_rootNode = a_rootNode;
        i_currentNode = a_rootNode;
        SelectionChanged(i_currentNode, i_currentNode);
    }
}
