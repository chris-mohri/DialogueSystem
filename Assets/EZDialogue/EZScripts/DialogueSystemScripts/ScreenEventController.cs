using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(TMP_Text))]
public class ScreenEventController : MonoBehaviour, IPointerClickHandler
{
    //the dialogue system script (needed to access methods)
    private EZDialogueSystem ds;

    [SerializeField]
    private GameObject DialogueObject;

    private Camera camera; 

    private RectTransform textRectTransform;

    private TMP_Text textbox;
    private Canvas canvas;

    private int previouslyHoveredLink;

    // Start is called before the first frame update
    void Awake()
    {
        previouslyHoveredLink = -1;

        ds = GetComponent<EZDialogueSystem>();
        textbox = DialogueObject.GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();
        textRectTransform = DialogueObject.GetComponent<RectTransform>();

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

    private void CheckHover(){
        //only check hover if there's a menu on screen
        if (ds.IsMenu()==false){
            previouslyHoveredLink = -1;
            return;
        }
        
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // //checks if mouse collides with text object 
        // bool hasIntersectionWithChoice = TMP_TextUtilities.IsIntersectingRectTransform(textRectTransform, mousePosition, camera);
        // Debug.Log(hasIntersectionWithChoice);
        // if(!hasIntersectionWithChoice){
        //     return;
        // }

        //check if hovering over a choice option
        int link = TMP_TextUtilities.FindIntersectingLink(textbox, mousePosition, camera);
        if(link!=-1){     
            TMP_LinkInfo linkInfo = textbox.textInfo.linkInfo[link];
            string linkId = linkInfo.GetLinkID();
            //only change the color when necessary
            if (previouslyHoveredLink==-1){
                ds.ChangeOptionColor(linkId, ds.hoverColor);
            } else if (previouslyHoveredLink!=link){
                ds.ChangeOptionColor(textbox.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), "ffffff");
                ds.ChangeOptionColor(linkId, ds.hoverColor);
            }
            previouslyHoveredLink = link;
        } else {
            if (previouslyHoveredLink!=-1){
                ds.ChangeOptionColor(textbox.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), "ffffff");
            }
            previouslyHoveredLink=-1;
        }
    }

    void Update(){
        CheckHover();
    }
}
