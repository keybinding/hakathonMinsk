using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class SlidingPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

	void Update () {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
        }
	}



    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Hidden", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Hidden", true);

    }
}
