using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CommandsController : MonoBehaviour
{

    [SerializeField] [Tooltip("Directory for where images are located")]
    private string baseImgDirectory = "Assets/Images/";

    //the dialogue system script (needed to access methods)
    private EZDialogueSystem ds;

    //saved information script
    private SavedInformation save;

    //the choice menu variables
    private string chosenOption;

    //required variables to handle function execution
    private string currentFunc = "";
    State state;

    //enum to keep track of current state of the function
    public enum State {
        Inactive,
        Displaying,
        Waiting,
        Continuing
    }

    void Awake(){

    }
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(baseImgDirectory);

        //setup variables
        save = GetComponent<SavedInformation>();
        ds = GetComponent<EZDialogueSystem>();

        //refresh variables
        refreshVariables();
    }

    public void refreshVariables(){
        currentFunc  = "";
        chosenOption = "";
        state        = State.Inactive;
    }

    // Update is called once per frame
    void Update() {
        //if currently executing a function
        if (state != State.Inactive){
            //invoke the function
            AlisterInvoke(currentFunc);

            //once completed the function, set back to inactive and reset variables
            if (state == State.Continuing){
                ds.EnableControls(); // renable controls
                refreshVariables();
            }
        }
    }

    //uses reflection to be able to invoke methods
    public void AlisterInvoke(string line){
        
    }

    // INSERT CUSTOM SCRIPTS HERE 
    // MAKE THEM PUBLIC!!
    // ======================= STORY SCRIPTS =======================
    //chapter 1 func
    public void c1_1(){
        if (state == State.Displaying){
            List<string> options = new List<string>(){"Aoko, you speak too much", "Stay silent"};
            List<string> results = new List<string>(){"1a", "1b"};
            DisplayChoices(options, results);
        }
        else if (state == State.Waiting){
            // ==== Player Input Waiting Room ..zzzZZzZzz. (leave empty)
        }

        else if (state == State.Continuing){
            if (chosenOption == "1a"){
                // routeInfo.Add("1a");
                Jump("route 1");
            } else {
                // routeInfo.Add("1b");
                Jump("label happy");
            } 

        } else {
            Debug.Log("huh!? How'd it get here??");
        }
        
    }
    // ==============================================================

    // INSERT CUSTOM SCRIPTS HERE 
    // MAKE THEM PUBLIC!!
    // ======================== Base Scripts ========================

    //choices = what is displayed on screen
    //result = items to be added to inventory
    public void DisplayChoices(List<string> choices, List<string> result){
        ds.DisableControls(); //forbid continuation of story 

        //create the game object with choices 
    }

    public void Jump(string toJumpTo){
        
    }

    public void SendChosenOption(string ret){
        chosenOption = ret;
    }

    public void ExecuteFunction(string func){
        currentFunc = func;
    }

}
