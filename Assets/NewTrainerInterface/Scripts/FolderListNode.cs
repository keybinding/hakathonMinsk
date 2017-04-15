using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;
using System.Collections;
using System;

public class FolderListNode : MonoBehaviour {

    public Image picture = null;
    public Text nodeName = null;
    public Sprite folderPic = null;
    public Sprite filePic = null;
    public GameObject NodeSelection = null;
    public Sprite playButtonPic = null;
    public Sprite stopButtonPic = null;
    public Button playButton = null;

    public string filePath { get { return i_path; } }

    [System.Serializable]
    public class FolderListNodeEvent : UnityEvent<FolderListNode> { }
    [SerializeField]
    public FolderListNodeEvent onNodeClicked;

    [System.Serializable]
    public class OnPlayerCtrlBtnClick : UnityEvent<string, FolderListNode> { }
    [SerializeField]
    private OnPlayerCtrlBtnClick onCtrlBtnClick;

    private string i_path;
    private string i_name;
    private ListContainer i_parent;

    public void InitializeFile(string a_path, ListContainer a_parent)
    {
        InitNode(a_path, a_parent);
        if (folderPic != null)
        {
            picture.sprite = folderPic;
        }
    }

    public void InitializeDirectory(string a_path, ListContainer a_parent)
    {
        InitNode(a_path, a_parent);
        if (filePic != null)
        {
            picture.sprite = filePic;
        }
    }

    private void InitNode(string a_path, ListContainer a_parent)
    {
        i_path = a_path;
        i_name = Path.GetFileNameWithoutExtension(a_path);
        i_parent = a_parent;
        nodeName.text = i_name;
    }

    public void HighlightNode(bool a_enabled)
    {
        NodeSelection.SetActive(a_enabled);
    }

    public void OnClick()
    {
        onNodeClicked.Invoke(this);
    }

    public void OnPlayButtonClick()
    {
        if(playButton.gameObject.GetComponent<Image>().sprite == playButtonPic)
        {
            playButton.gameObject.GetComponent<Image>().sprite = stopButtonPic;
            onCtrlBtnClick.Invoke("play", this);
        }
        else
        {
            playButton.gameObject.GetComponent<Image>().sprite = playButtonPic;
            onCtrlBtnClick.Invoke("stop", this);
        }
    }

    public void SubscribePlayer(VTPlayer a_player)
    {
        onCtrlBtnClick.AddListener(a_player.OnPlayerButtonPressed);
    }
}
