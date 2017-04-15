using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VanishingAnimation : MonoBehaviour {
    public float speed = 5;
    Image img;
	void Start () {
        img = this.GetComponent<Image>();
	}

	void Update () {
        Color c = img.color;
        c.a -= speed * Time.deltaTime;
        img.color = c;
        if (c.a <= 0f)
            Destroy(this.gameObject);
	}
}
