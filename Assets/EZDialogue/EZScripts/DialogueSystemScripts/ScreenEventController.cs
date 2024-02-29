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
    private CommandsController commandsController;

    [SerializeField]
    private GameObject UnderlineObject;
    [SerializeField]
    private GameObject DialogueObject;

    private Camera camera; 
    private RectTransform textRectTransform;
    private TMP_Text textObj;
    private Canvas canvas;

    private int previouslyHoveredLink;

    // Start is called before the first frame update
    void Awake()
    {
        previouslyHoveredLink = -1;

        ds = GetComponent<EZDialogueSystem>();
        textObj = DialogueObject.GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();
        textRectTransform = DialogueObject.GetComponent<RectTransform>();
        commandsController = GetComponent<CommandsController>();

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay){
            camera = null;
        } else {
            camera = canvas.worldCamera;
        }
    }

    public void OnPointerClick(PointerEventData eventData){
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);

        //only continue if the user hasn't chosen an option yet
        if (commandsController.HasChosenOption()==false){
            //check if a choice option was clicked
            int link = TMP_TextUtilities.FindIntersectingLink(textObj, mousePosition, camera);
            if(link!=-1){
                TMP_LinkInfo linkInfo = textObj.textInfo.linkInfo[link];
                // Debug.Log("Clicked: "+linkInfo.GetLinkID()); 
                commandsController.SendChosenOption(linkInfo.GetLinkID());
            }
        }

    }

    private void CheckHover(){
        //only check hover if there's a menu on screen
        if (ds.IsMenu()==false){
            previouslyHoveredLink = -1;
            return;
        }
        
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        //only continue if the user hasn't chosen an option yet
        if (commandsController.HasChosenOption()==false){
            //check if hovering over a choice option
            int link = TMP_TextUtilities.FindIntersectingLink(textObj, mousePosition, camera);
            if(link!=-1){     
                TMP_LinkInfo linkInfo = textObj.textInfo.linkInfo[link];
                string linkId = linkInfo.GetLinkID();

                //only change the color when necessary
                if (previouslyHoveredLink==-1){
                    ds.ChangeOptionColor(linkId, ds.hoverFont);
                    DisplayUnderline(linkId);
                    //add here
                } else if (previouslyHoveredLink!=link){
                    ds.ChangeOptionColor(textObj.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), ds.normalFont);
                    ds.ChangeOptionColor(linkId, ds.hoverFont);
                    DisplayUnderline(linkId);
                    //add here
                }
                previouslyHoveredLink = link;
            } else {
                //if not hovering over any option, make sure no option has hover effect
                if (previouslyHoveredLink!=-1){
                    ds.ChangeOptionColor(textObj.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), ds.normalFont);
                }
                previouslyHoveredLink=-1;
            }
        }
    }

/*
<font="VarelaRound1"><link=$opt_1a$></link><link=1a>1.   "Aoko, you speak too much"</link>
1. choice A
<font="VarelaRound2>2. choice B
*/

    private void DisplayUnderline(string id){
        textObj.ForceMeshUpdate();

        //finds the charInfo of the letter whose coordinates we need
        int tagIndex = textObj.text.IndexOf($"<link={id}>");
        int closeTagIndex = textObj.text.IndexOf("</link>", tagIndex);
        int letterIndex = closeTagIndex-1;
        TMP_CharacterInfo charInfo;
        // Debug.Log(letterIndex + " | " + textObj.text.Length);
        // Debug.Log(textObj.textInfo.characterInfo.Length + " | "+textObj.textInfo.characterCount);
        
        Vector3[] vertices = textObj.mesh.vertices;

        string line = "";
        foreach (TMP_CharacterInfo ch in textObj.textInfo.characterInfo){
            line+=ch.character;
        }
        letterIndex = line.IndexOf("1. ");
        Debug.Log(textObj.textInfo.characterInfo[letterIndex].character);
        charInfo = textObj.textInfo.characterInfo[letterIndex];
        Debug.Log(line.Length);
        Debug.Log(textObj.text.Length);

        Debug.Log(charInfo.bottomLeft);
        Debug.Log(charInfo.character);
  
        // Debug.Log(textObj.GetTextInfo(textObj.text).characterInfo[letterIndex].bottomLeft.y);
        Vector3 localPosition = new Vector3(charInfo.bottomLeft.x, charInfo.bottomLeft.y, 0f);
        Vector3 screenPosition = textObj.transform.TransformPoint(localPosition);
        // screenPosition /= canvas.scaleFactor;
        localPosition.z = -0.1f;
        screenPosition.z=-.1f;

        Debug.Log("Local Position " + localPosition);
        Debug.Log("Screen Position " + screenPosition);
        
        UnderlineObject.GetComponent<Transform>().position = screenPosition;
    }

    void Update(){
        CheckHover();
    }
}
