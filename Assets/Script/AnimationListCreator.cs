using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class AnimationListCreator : MonoBehaviour {
    public bool isTrainer = false;
    public GameObject prefab;
    public GameObject prefab2;
    public GameObject trainersButton;
    public Sprite pause;
    public Sprite pressedPause;
    public Sprite play;
    public Sprite pressedPlay;
    public PointManPlayer pmp;
    public UnityEvent OnAddButtonClick;
    RectTransform rTransform;
    VerticalLayoutGroup vlg;
    int buttonCount;
    Button prevButton;
    public void Start()
    {
        
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            isTrainer = false;
            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (s.Equals("/trainer"))
                {
                    isTrainer = false;
                }
            }
        }
        rTransform = GetComponent<RectTransform>();
        vlg = GetComponent<VerticalLayoutGroup>();
        Refresh();
    }

    public void Refresh()
    {
        if (!isTrainer)
            StartCoroutine( GenerateButtons());
        else
            GenerateTrainersButtons();
    }

    public IEnumerator GenerateButtons()
    {
        if (vlg == null) ;
        else {
            DeleteAllChilds();
            yield return StartCoroutine(Network.TodayExercisesRqst());
            string[] l_exercises = Network.PopExerciseData();
            if (l_exercises != null)
            {
                if (l_exercises.Length > 1)
                {
                    string l_datapath = Application.dataPath + "/../Records/tmp";
                    if (Directory.Exists(l_datapath))
                    {
                        try
                        {
                            Directory.Delete(l_datapath, true);
                            RefreshExerciseList(l_datapath, l_exercises);
                        }
                        catch(Exception e)
                        {
                            Debug.Log("!!!Couldn't delete existing tmp directory!!!" + e.Message);
                        }
                    }
                    else
                    {
                        RefreshExerciseList(l_datapath, l_exercises);
                    }
                }
            }
        }
    }

    void DownloadExercises(string a_directory, string[] a_exercises)
    {
        for(int i = 0; i != a_exercises.Length; i += 2)
        {
            Network.DownloadFileViaFTP(a_directory, a_exercises[i + 1], a_exercises[i]);
        }
    }

    void CreateList(string a_directory)
    {
        DirectoryInfo d = new DirectoryInfo(a_directory);
        FileInfo[] Files = d.GetFiles("*.nrb");
        buttonCount = 0;
        foreach (FileInfo file in Files)
        {
            CreateButton(file.Name, file.FullName);
            buttonCount++;
        }
        float height = (float)buttonCount * prefab.GetComponent<LayoutElement>().minHeight;
        height += (float)buttonCount * vlg.spacing;
        rTransform.sizeDelta = new Vector2(0, height);
    }

    void RefreshExerciseList(string a_directory, string[] a_exersizes)
    {
        Directory.CreateDirectory(a_directory);
        DownloadExercises(a_directory, a_exersizes);
        CreateList(a_directory);
    }
    /*
    public void GenerateButtons()
    {
        if (vlg == null)
            return;
        DeleteAllChilds();
        
        DirectoryInfo d = new DirectoryInfo(Application.dataPath+"/../Records");
        FileInfo[] Files = d.GetFiles("*.nrb");
        buttonCount = 0;
        foreach (FileInfo file in Files)
        {
            CreateButton(file.Name, file.FullName);
            buttonCount++;
        }
        float height = (float)buttonCount * prefab.GetComponent<LayoutElement>().minHeight;
        height += (float)buttonCount * vlg.spacing;
        
        rTransform.sizeDelta = new Vector2(0, height);
    }
    */

    public void GenerateTrainersButtons()
    {
        DeleteAllChilds();
        //CreateAddButton();
        DirectoryInfo d = new DirectoryInfo(Application.dataPath + "/../Records");
        FileInfo[] Files = d.GetFiles("*.*");
        buttonCount = 0;
        foreach (FileInfo file in Files)
        {
            CreateTrainersButton(file.Name, file.FullName);
            buttonCount++;
        }
        float height = (float)buttonCount * trainersButton.GetComponent<LayoutElement>().minHeight;
        height += (float)buttonCount * vlg.spacing;
        height += 20f;
        //height += prefab2.GetComponent<LayoutElement>().minHeight;
        rTransform.sizeDelta = new Vector2(0, height);
    }

    public void DeleteAllChilds()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void CreateButton(string name, string fullPath)
    {
        GameObject go = Instantiate(prefab);
        go.transform.GetChild(0).GetComponent<Text>().text = name;
        go.GetComponent<Button>().onClick.AddListener(()=>OpenRecord(name));
        go.transform.SetParent(transform);
    }

    public void CreateTrainersButton(string name, string fullPath)
    {
        GameObject go = Instantiate(trainersButton);
        InputField inputField = go.transform.GetChild(0).GetComponent<InputField>();
        inputField.text = name;
        inputField.onEndEdit.AddListener(delegate(string text) { RenameFile(fullPath, name, text); });
        go.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => RemoveFile(fullPath));
        Button b = go.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();
        b.onClick.AddListener(() => OpenRecord(name, b));
        go.transform.SetParent(transform);
    }

    public void CreateAddButton()
    {
        GameObject go = Instantiate(prefab2);
        go.transform.GetChild(0).GetComponent<Text>().text = "RECORD NEW";
        go.GetComponent<Button>().onClick.AddListener(()=>OnAddButtonClick.Invoke());
        go.transform.SetParent(transform);
    }

    public void OpenRecord(string s, Button b)
    {
        if (b.name == "Play")
        {

            pmp.StartCountDown(s);
            ChangeButtonToStop(b);
            if (prevButton!=null  && prevButton != b)
                ChangeButtonToPlay(prevButton);
            prevButton = b;
        }
        else
        {
            ChangeButtonToPlay(b);
            pmp.StopPlaying();
        }

    }

    void ChangeButtonToPlay(Button b)
    {
        SpriteState ss = new SpriteState();
        ss.pressedSprite = pressedPlay;
        if (b != null)
        {
            b.spriteState = ss;
            b.GetComponent<Image>().sprite = play;
            b.name = "Play";
        }
    }

    void ChangeButtonToStop(Button b)
    {
        SpriteState ss = new SpriteState();
        ss.pressedSprite = pressedPause;
        b.spriteState = ss;
        b.GetComponent<Image>().sprite = pause;
        b.name = "Pause";
    }

    public void OpenRecord(string s)
    {
        pmp.ActivateStopPlayButton();
        pmp.ChangeToggleToStop();
        pmp.StartCountDown(s);
    }

    public void RenameFile(string path, string oldname, string newName)
    {
        if (oldname != newName)
        {
            File.Move(path, Application.dataPath + "/../Records/" + newName);
            Refresh();
        }
    }

    public void RemoveFile(string fullPath)
    {
        File.Delete(fullPath);
        Refresh();
        pmp.StopAndRemoveClip();
        ChangeButtonToPlay(prevButton);
    }
}
