using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(TMP_Text))]
public class ScreenEventController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject DialogueObject;

    [SerializeField]
    private Camera camera;

    private TMP_Text textbox;
    private Canvas canvas;

    // Start is called before the first frame update
    void Awake()
    {
        textbox = DialogueObject.GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay){
            camera = null;
        } else {
            camera = canvas.worldCamera;
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);

        //check if a choice option was clicked
        int link = TMP_TextUtilities.FindIntersectingLink(textbox, mousePosition, camera);
        if(link!=-1){
            TMP_LinkInfo linkInfo = textbox.textInfo.linkInfo[link];
            Debug.Log("Clicked: "+linkInfo.GetLinkID());
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);

        //check if a choice option was entered
        int link = TMP_TextUtilities.FindIntersectingLink(textbox, mousePosition, camera);
        if(link!=-1){
            TMP_LinkInfo linkInfo = textbox.textInfo.linkInfo[link];
            Debug.Log("Entered: "+linkInfo.GetLinkID());
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);

        //check if a choice option was exited
        int link = TMP_TextUtilities.FindIntersectingLink(textbox, mousePosition, camera);
        if(link!=-1){
            TMP_LinkInfo linkInfo = textbox.textInfo.linkInfo[link];
            Debug.Log("Exited: "+linkInfo.GetLinkID());
        }
    }
}
