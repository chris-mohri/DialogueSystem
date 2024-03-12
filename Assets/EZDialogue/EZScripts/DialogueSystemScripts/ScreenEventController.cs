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

    [SerializeField] [Tooltip("The GameObject that has the TextMeshPro component for displaying dialogue")]
    private GameObject DialogueObject;
    [SerializeField] [Tooltip("Empty parent object of the dialogue text objects")]
    private GameObject DialogueContainer;
    [SerializeField] [Tooltip("Empty parent object of the history Log objects")]
    private GameObject LogContainer;
    [SerializeField] [Tooltip("Number of seconds it takes to fade in/out the log")] [Range(0, 5)]
    private float logFadeDuration=.3f;
    [SerializeField] [Tooltip("Y offset for displaying the underline (e.g. +5 raises the image by 5px)")]
    private int yOffset=0;

    private Camera screenCamera; 
    private RectTransform textRectTransform;
    private TMP_Text textObj;
    private Canvas canvas;
    private CanvasGroup logAlphaComponent;
    private CanvasGroup dialogAlphaComponent;

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
        logAlphaComponent = LogContainer.GetComponent<CanvasGroup>();
        dialogAlphaComponent = DialogueContainer.GetComponent<CanvasGroup>();

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay){
            screenCamera = null;
        } else {
            screenCamera = canvas.worldCamera;
        }

        screenState = ScreenState.Dialogue;
        controls = new PlayerControls();
        controls.Keyboard.Log.performed += ToggleLog;
    }
    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }

    //opens or closes the log
    private void ToggleLog(InputAction.CallbackContext context){
        StartCoroutine(StartToggleLog());
    }

    private bool stillToggling=false;
    public IEnumerator StartToggleLog(){
        //don't allow interaction if in a state that you can't call the tab
        if (stillToggling==true || screenState == ScreenState.Menu){

        }
        else {
            stillToggling = true;
            if (screenState == ScreenState.Dialogue){
                StartCoroutine(FadeInObj(logAlphaComponent, true));
                Vector3 newPosition = new Vector3(0, 0, 0);
                LogContainer.GetComponent<Transform>().position = newPosition;
                screenState = ScreenState.Log;
                ds.DisableControls();
                yield return StartCoroutine(FadeInObj(dialogAlphaComponent, false));
                DialogueContainer.SetActive(false);
                stillToggling=false;
                

            } else if (screenState == ScreenState.Log){
                StartCoroutine(FadeInObj(dialogAlphaComponent, true));
                screenState = ScreenState.Dialogue;
                ds.EnableControls();
                DialogueContainer.SetActive(true);
                //wait until it is faded out before moving it
                yield return StartCoroutine(FadeInObj(logAlphaComponent, false));
                Vector3 newPosition = new Vector3(0, 3000, 0);
                LogContainer.GetComponent<Transform>().position = newPosition;
                stillToggling=false;
            }
        }
    }

    public IEnumerator FadeInObj(CanvasGroup obj, bool fadeIn){
        float counter = 0;
        float opacity = 1.0f;
        if (fadeIn==true){
            opacity=0.0f;
        }

        while (counter < logFadeDuration)
        {
            counter += Time.deltaTime;
            //fade from 1 to 0
            if (fadeIn==true){
                opacity = Mathf.Lerp(0, 1, counter / logFadeDuration);
            } else {
                opacity = Mathf.Lerp(1, 0, counter / logFadeDuration);
            }

            //adjust alpha
            obj.alpha = opacity;

            //wit for next frame
            yield return null;
        }

        if (logFadeDuration==0){
            opacity = 1-opacity;
            obj.alpha = opacity;
        }
    }


    public void OnPointerClick(PointerEventData eventData){
        //only continue if the screen state is set to dialogue
        if (screenState != ScreenState.Dialogue) return;
        //only continue if using the built-in choice menu
        if (ds.UseBuiltInPlayerChoiceMenu == false) return;

        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        //only continue if the user hasn't chosen an option yet
        if (commandsController.HasChosenOption()==false){
            //check if a choice option was clicked
            int link = TMP_TextUtilities.FindIntersectingLink(textObj, mousePosition, screenCamera);
            if(link!=-1){
                TMP_LinkInfo linkInfo = textObj.textInfo.linkInfo[link];
                // Debug.Log("Clicked: "+linkInfo.GetLinkID()); 
                commandsController.SendChosenOption(linkInfo.GetLinkID());
                ds.AddChoiceToLog(linkInfo.GetLinkText());
            }
        }
    }

    private void CheckHover(){
        //only continue if the screen state is set to dialogue
        if (screenState != ScreenState.Dialogue) return;
        //only continue if using the built-in choice menu
        if (ds.UseBuiltInPlayerChoiceMenu == false) return;

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
        //return if not using the underline obj
        if (commandsController.UnderlineObj == null) return;
        //only continue if using the built-in choice menu
        if (ds.UseBuiltInPlayerChoiceMenu == false) return;

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
