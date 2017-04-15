using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckButton : MonoBehaviour {
    public void EnableButton(Text s)
    {

        this.GetComponent<Button>().interactable = s.text.Length > 0;

    }
}
