using UnityEngine;
using System.Collections;

public class RunBrowserButton : MonoBehaviour {
    public void OnClick()
    {
        Network.RunDefaultBrowser();
    }
}
