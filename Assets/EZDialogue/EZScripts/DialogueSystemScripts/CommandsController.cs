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
    private List<Job> jobs;
    private double timer;

    private State state;
    
    //enum to keep track of current state of the function
    public enum State {
        Inactive,   //no functions running
        Active,     //a function is running (might require multiple cycles)
        Displaying, //a func should run and execute a display func once
        Waiting,    //a func is running and is waiting for player input
        Continuing  //a func is running and should continue for 1 more cycle
    }

    void Awake(){

    }
    
    // Start is called before the first frame update
    void Start()
    {
        //setup variables
        save = GetComponent<SavedInformation>();
        ds = GetComponent<EZDialogueSystem>();

        //refresh variables
        refreshVariables();
    }

    public void refreshVariables(){
        jobs = new List<Job>();
        timer = 0f;
        chosenOption = "";
    }

    // Update is called once per frame
    void Update() {
        //if currently executing a function
        int numActive = 0;
        foreach (Job job in jobs){
            if (state != State.Inactive){
                numActive+=1;
                state = job.state;
                //invoke the job
                AlisterInvoke(job);
            } // else if (state == State.Continuing || state == State.Inactive) {
            //     //once completed the function, set back to inactive and reset variables
            //     ds.EnableControls(); // renable controls
            //     refreshVariables();
            // }
            timer += Time.deltaTime;

            //clear job list if all are inactive
            if (numActive==0){
                jobs = new List<Job>();
            }
        }
        
    }

    //uses reflection to be able to invoke methods
    public void AlisterInvoke(Job job){
        string funcName = job.name;
        MethodInfo func = this.GetType().GetMethod(funcName);
        //invokes the func
        func.Invoke(this, null);
    }

    // INSERT CUSTOM SCRIPTS HERE 
    // MAKE THEM PUBLIC!!
    // ======================= STORY SCRIPTS =======================
    //chapter 1 func
    public void func1(){
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

    public void Clear(){
        ds.ClearTextThenAdd("");
        state = State.Inactive;
    }

    //choices = what is displayed on screen
    //result = items to be added to inventory
    public void DisplayChoices(List<string> choices, List<string> result){
        ds.DisableControls(); //forbid continuation of story 

        //create the game object with choices 
    }

    public void Jump(string toJumpTo){
        state = State.Inactive;
    }

    public void SendChosenOption(string ret){
        chosenOption = ret;
    }

    public void ExecuteFunction(string commands){
        commands = commands.Trim();
        string[] listOfFuncs = commands.Split(".func");

        foreach (string func in listOfFuncs){
            if (func.Trim()!=""){
                string name = func.Trim();
                name = name.Substring(0, name.IndexOf("("));
                jobs.Add(new Job(name, null));
            }
        }

        foreach (Job job in jobs){
            AlisterInvoke(job);
        }

        //try to parse via objects[]
    }

    public object[] ParseArgs(string func, string argsToParse){
        string args = argsToParse;

        return null;
    }

    //funcs from the commands 
    public class Job{
        public string name;
        public object[] args;
        public State state;
        public double timer;

        public Job(string in_name, object[] in_args){
            name=in_name;
            args=in_args;
            state = State.Active;
            timer=0;
        }

        public void UpdateTimer(){
            timer += Time.deltaTime;
        }
    }

}
