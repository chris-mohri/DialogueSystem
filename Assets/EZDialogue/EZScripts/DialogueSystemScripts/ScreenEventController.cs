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

    //controls for the dialogue system
    private PlayerControls controls;

    [SerializeField] [Tooltip("Empty parent object of the dialogue text objects")]
    private GameObject DialogueContainer;
    [SerializeField] [Tooltip("Empty parent object of the history Log objects")]
    private GameObject LogContainer;
    [SerializeField]
    private GameObject DialogueObject;
    [SerializeField] [Tooltip("Y offset for displaying the underline (e.g. +5 raises the image by 5px)")]
    private int yOffset=0;

    private Camera screenCamera; 
    private RectTransform textRectTransform;
    private TMP_Text textObj;
    private Canvas canvas;

    private int previouslyHoveredLink;

    ScreenState screenState; // current screen state
    public enum ScreenState {
        Default,
        Dialogue,
        Log,
        Menu
    }

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
            screenCamera = null;
        } else {
            screenCamera = canvas.worldCamera;
        }

        screenState = ScreenState.Dialogue;
        controls = new PlayerControls();
        controls.Keyboard.Log.performed += OnTabPressed;
    }
    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }

    //opens or closes the log
    private void OnTabPressed(InputAction.CallbackContext context)
    {
        //don't allow interaction if in a state that you can't call the tab
        if (screenState == ScreenState.Menu){ return; }

        if (screenState == ScreenState.Dialogue){
            screenState = ScreenState.Log;
            ds.DisableControls();
            DialogueContainer.SetActive(false);
            LogContainer.SetActive(true);

        } else if (screenState == ScreenState.Log){
            screenState = ScreenState.Dialogue;
            ds.EnableControls();
            DialogueContainer.SetActive(true);
            LogContainer.SetActive(false);
        }


        Debug.Log("uwu key pressed");
    }

    public void OnPointerClick(PointerEventData eventData){
        //only continue if the screen state is set to dialogue
        if (screenState != ScreenState.Dialogue) return;

        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        //only continue if the user hasn't chosen an option yet
        if (commandsController.HasChosenOption()==false){
            //check if a choice option was clicked
            int link = TMP_TextUtilities.FindIntersectingLink(textObj, mousePosition, screenCamera);
            if(link!=-1){
                TMP_LinkInfo linkInfo = textObj.textInfo.linkInfo[link];
                // Debug.Log("Clicked: "+linkInfo.GetLinkID()); 
                commandsController.SendChosenOption(linkInfo.GetLinkID());
            }
        }
    }

    private void CheckHover(){
        //only continue if the screen state is set to dialogue
        if (screenState != ScreenState.Dialogue) return;

        //only check hover if there's a menu on screen
        if (ds.IsMenu()==false){
            previouslyHoveredLink = -1;
            return;
        }
        
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        //only continue if the user hasn't chosen an option yet
        if (commandsController.HasChosenOption()==false){
            //check if hovering over a choice option
            int link = TMP_TextUtilities.FindIntersectingLink(textObj, mousePosition, screenCamera);
            if(link!=-1){     
                TMP_LinkInfo linkInfo = textObj.textInfo.linkInfo[link];
                string linkId = linkInfo.GetLinkID();

                //only change the color when necessary
                if (previouslyHoveredLink==-1){
                    ds.ChangeOptionColor(linkId, ds.hoverFont);
                    textObj.ForceMeshUpdate();
                    int letterIndex = linkInfo.linkTextfirstCharacterIndex;//+linkInfo.linkTextLength-1;
                    DisplayUnderline(letterIndex);
                } else if (previouslyHoveredLink!=link){
                    ds.ChangeOptionColor(textObj.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), ds.normalFont);
                    ds.ChangeOptionColor(linkId, ds.hoverFont);
                    textObj.ForceMeshUpdate();
                    int letterIndex = linkInfo.linkTextfirstCharacterIndex;//+linkInfo.linkTextLength-1;
                    DisplayUnderline(letterIndex);
                }
                previouslyHoveredLink = link;
            } else {
                //if not hovering over any option, make sure no option has hover effect
                if (previouslyHoveredLink!=-1){
                    ds.ChangeOptionColor(textObj.textInfo.linkInfo[previouslyHoveredLink].GetLinkID(), ds.normalFont);
                    commandsController.UnderlineObj.SetActive(false); //disable the underline
                }
                previouslyHoveredLink=-1;
            }
        }
    }

    private void DisplayUnderline(int letterIndex){
        //only continue if the screen state is set to dialogue
        if (screenState != ScreenState.Dialogue) return;
        
        //find the minimum y of the last 3 chars
        TMP_CharacterInfo charInfo = textObj.textInfo.characterInfo[letterIndex];
        Vector3 localPosition = new Vector3(charInfo.bottomLeft.x, charInfo.bottomLeft.y+yOffset, 0f);
        Vector3 screenPosition = textObj.transform.TransformPoint(localPosition);
        
        Vector3 oldPos = commandsController.UnderlineObj.GetComponent<Transform>().position;
        oldPos.y = screenPosition.y;
        commandsController.UnderlineObj.SetActive(true);
        commandsController.UnderlineObj.GetComponent<Transform>().position = oldPos;
    }

    void Update(){
        CheckHover();
    }
}
