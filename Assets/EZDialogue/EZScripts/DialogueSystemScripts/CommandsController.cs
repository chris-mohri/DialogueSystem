using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;

public partial class CommandsController : MonoBehaviour
{

    // [SerializeField] [Tooltip("Directory for where images are located")]
    // private string baseImgDirectory = "Assets/Images/"; 
    [Tooltip("The basic prefab for sprites")]
    public GameObject spritePrefab; 
    [Tooltip("The GameObject that houses all the character sprites")]
    public GameObject spriteAreaObj; 

    [SerializeField]  [Tooltip("The GameObject that has the image for the underline that appears when hovering over an option (can leave empty if not using the built-in choice menu)")]
    public GameObject UnderlineObj;

    //the dialogue system script (needed to access methods)
    private EZDialogueSystem ds;

    //saved information script
    private SavedInformation save;

    //all of these must be saved
    //the choice menu variables
    [HideInInspector]
    public string chosenOption;
    [HideInInspector]
    public List<string> choiceOptionIDs;
    [HideInInspector]
    public List<string> choiceOptions;
    [HideInInspector]
    public bool skip;

    [Tooltip("Add the function you want that displays the choice options. It must have no parameters (choiceOptions and choiceOptionIDs are public variables! Also, send the chosen option to this script with SendChosenOption(<option>)  )")]
    public UnityEvent DisplayChoices;

    //required variables to handle function execution 
    [HideInInspector]
    public JobQueue queue;
    private bool newJobWave = false;

    void Awake(){
        //setup variables
        save = GetComponent<SavedInformation>();
        ds = GetComponent<EZDialogueSystem>();

        //refresh variables 
        queue = new JobQueue();
        chosenOption = null;
        choiceOptionIDs=null;
        skip = false;
    }
    
    // Start is called before the first frame update
    void Start(){
        
    }

    public IEnumerator Test(){
        yield return new WaitForSeconds(.2f);
    }

    public void refreshVariables(){
        queue.Clear();
        chosenOption = null;
        choiceOptionIDs=null;
        skip = false;
    }

    // Update is called once per frame 
    void Update() {
        if (queue.remaining == 0){
            skip = false;
        }
        if (newJobWave == true && queue.remaining == 0){
            refreshVariables();
            newJobWave = false;
        }
    }

    // ==============================================================
    // INSERT CUSTOM SCRIPTS HERE 
    // MAKE THEM PUBLIC!!
    // ======================== Base Scripts ========================

    //move camera abd reset cam

    public IEnumerator Spawn(string objName, float xPercent, float yPercent, float tWait, float fadeIn=0.1f){
        //wait by tWait seconds to spawn the object
        yield return StartCoroutine(WaitTime(tWait));

        float xScreen = xPercent * Screen.width;
        float yScreen = yPercent * Screen.height;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(xScreen, yScreen, 0));
        worldPos.z=0;

        Instantiate(spritePrefab, worldPos, Quaternion.identity, spriteAreaObj.transform);
        
        //instantiate prefab
    }

    //waits <twait> seconds, then moves the in-game object named <obj> by x and y, and
    //will take <tTravel> seconds to fully move by x and y
    public IEnumerator Move(string objName, float xPercent, float yPercent, float tWait, float tTravel){
        yield return new WaitForSeconds(tWait); //don't do this cuz can't skip
        Debug.Log("placeholder");
    }

    //same as above but continues movement while player is continuing dialogue
    public IEnumerator PersistMove(string objName, float xPercent, float yPercent, float tWait, float tTravel){
        yield return StartCoroutine(Move(objName, xPercent, yPercent, tWait, tTravel));
    }

    //waits <twait> seconds, then moves the in-game object named <obj> to x and y, and
    //will take <tTravel> seconds to fully move to x and y 
    public IEnumerator MoveTo(string objName, int x, int y, float tWait, float tTravel){
        yield return new WaitForSeconds(tWait);
        Debug.Log("placeholder");
    }

    //clears the screen of all text
    public void Clear(){
        ds.ClearTextThenAdd("");
    }

    //displays the dialogue from the same dialogue block on a new page
    public void OnNewPage(){
        ds.ClearTextThenRestateEntry();
    }

    //e.g. https://www.youtube.com/watch?v=qVUhV_pHig0
    //displays the choices and waits for user input
    private IEnumerator DisplayAndWaitForChoices(List<string> options) {
        List<string> results = new List<string>();
        for (int i = 0; i<options.Count; i++){
            results.Add($"option{i+1}");
        }

        choiceOptionIDs = results;
        choiceOptions = options;

        chosenOption = null;
        StartCoroutine(DisplayChoicesAfter(options, results));

        //wait for user input
        while(chosenOption == null){
            yield return null;
        }

        //tell the dialogue system script to handle the text on screen
        ds.PlayerChoseAnOption(chosenOption);

        //play an animation
        yield return StartCoroutine(PlayChosenOptionAnimation());
        
        ds.ReturnControlAfterChoice(); //allow the player to continue
        UnderlineObj.SetActive(false); //disable underline
    }

    //displays the choices only when the question has been fully displayed
    private IEnumerator DisplayChoicesAfter(List<string> options, List<string> results) {
        //wait until question has been fully displayed, then display the choices
        while(ds.LettersStillDisplaying() == true){
            yield return null;
        }
        //invoke the function that displays the choices
        DisplayChoices.Invoke();
    }

    //jumps to the given route or label
    public void Jump(string dest){
        ds.book.Jump(dest);
    }

    public IEnumerator WaitTime(float tWait, bool skippable=true){
        float timer = 0;
        while (timer < tWait){
            if (skippable==true && skip==true){
                break;
            }

            yield return null;
            timer = Time.deltaTime;
        }
    }

    // ====================================================================
    // ================ BELOW ARE JUST HELPER FUNCTIONS =======================
    // ========= YOU DONT NEED TO EDIT THEM (unless to fix stuff) =================
    // ===================== PROCEED WITH CAUTION ! ===========================
    // ====================================================================

    public IEnumerator PlayChosenOptionAnimation(){
        yield return new WaitForSeconds(0.5f);
        Debug.Log("finished playing the animation");
        // UnderlineObj.SetActive(false);
    }

    //use reflection to invoke methods
    public void AlisterInvoke(Job job){
        //fetch the job's function
        string funcName = job.name;
        MethodInfo func = this.GetType().GetMethod(funcName);

        //fetch the job's arguments (if any)
        object[] args = null;
        if (job.args.Length != 0){
            args = job.args;
        }

        //if the function doesn't exist, then do not attempt to execute
        if (func==null){
            throw new Exception("Function does not exist: "+ funcName+"()");
        } else {
            //invoke coroutine if the func is a coroutine
            if (typeof(System.Collections.IEnumerator).IsAssignableFrom(func.ReturnType)){
                StartCoroutine(InvokedCoroutine(func, args));
            } else { //else invoke normal func
                // func.Invoke(this, args);
                InvokedNormalFunction(func, args);
            }
        }
    }

    //invokes the coroutine and manages queue counter
    private IEnumerator InvokedCoroutine(MethodInfo func, object[] args) {
        //check if it is a persist function or not.
        //persist function = a type of func that allows the player to start new dialogue while this func is still executing
        string funcName = func.Name;

        if (funcName.IndexOf("Persist")==0){
            queue.remaining--;
            yield return StartCoroutine((IEnumerator)func.Invoke(this, args));
        } else {
            yield return StartCoroutine((IEnumerator)func.Invoke(this, args)); //invoke it
            queue.remaining--; //decrement active remaining jobs in queue
        }
    }

    //invokes the normal function and manages queue counter
    private void InvokedNormalFunction(MethodInfo func, object[] args) {
        //check if it is a persist function or not.
        //persist function = a type of func that allows the player to start new dialogue while this func is still executing
        string funcName = func.Name;

        if (funcName.IndexOf("Persist")==0){
            queue.remaining--;
            func.Invoke(this, args);
        } else {
            func.Invoke(this, args);//invoke it
            queue.remaining--;//decrement active remaining jobs in queue
        }
    }

    //external option menu will send the response back to here with this func
    public void SendChosenOption(string opt){
        chosenOption = opt;
    }

    //tells all commands to skip to the end
    public void Skip(){
        skip = true;
    }

    public void ExecuteFunction(string commands){
        newJobWave = true;
        commands = commands.Trim();
        string[] listOfFuncs = commands.Split(".func");

        List<Job> jobs = new List<Job>();
        //loops through all functions and parses the func name and args
        foreach (string func in listOfFuncs){
            string tempFunc = func.Trim();
            if (tempFunc!=""){
                string name = tempFunc;
                name = name.Substring(0, name.IndexOf("("));
                string stringArgs = tempFunc.Substring(tempFunc.IndexOf("(")+1);
                stringArgs = stringArgs.Substring(0, stringArgs.Length-1);
                object[] args = ParseArgs(name, stringArgs);
                if (args==null){
                    throw new Exception("Func argument(s) not supported or not formed correctly. Skipping this func");
                } else {
                    jobs.Add(new Job(name, args));
                }
            }
        }
        queue = new JobQueue(jobs);

        //invoke all jobs
        foreach (Job job in queue.jobs){
            AlisterInvoke(job);
        }
    }

    public bool ReadyToContinue(){
        return queue.remaining == 0;
    }

    public bool HasChosenOption(){
        return chosenOption!=null;
    }

    private object[] ParseArgs(string func, string argsToParse){
        //if empty, just return empty list
        if (argsToParse.Length==0){
            return new object[0];
        }
        List<string> tokens = SplitParams(argsToParse);

        //finds all the argument types of the given function
        MethodInfo methodInfo = this.GetType().GetMethod(func);
        ParameterInfo[] parameters = methodInfo.GetParameters();

        object[] args = new object[parameters.Length];

        //check if the argument count is correct
        if (tokens.Count > parameters.Length){
            throw new Exception("Too many arguments given to function: "+func+"()\nPlease revise your dialogue script or edit the function");
        } else if (tokens.Count < parameters.Length){
            throw new Exception("Not enough arguments given to function: "+func+"()\nPlease revise your dialogue script or edit the function");
        }

        //parses each argument as its designated type (according to the function)
        for(int i = 0; i<parameters.Length; i++) {
            object arg = ParseArg(tokens[i], parameters[i].ParameterType);
            if (arg == null){
                return null;
            }
            args[i]=arg;
        }

        return args;
    }

    //parses the individual argument into the given type
    private object ParseArg(string arg, System.Type parameterType)
    {
        if (parameterType == typeof(string)) {
            return arg;
        } else if (parameterType == typeof(int)) {
            return int.Parse(arg);
        } else if (parameterType == typeof(float)) {
            return float.Parse(arg);
        } else if (parameterType == typeof(bool)){
            return bool.Parse(arg);
        }
        return null;
    }


    private static List<string> SplitParams(string input)
    {
        List<string> tokens = new List<string>();
        bool inQuotes = false;
        int start = 0;

        for (int i = 0; i < input.Length; i++){
            char c = input[i];

            if (c == '"'){
                inQuotes = !inQuotes;
            } else if (c == ',' && !inQuotes) {
                tokens.Add(input.Substring(start, i-start).Trim());
                start = i+1;
            }
        }

        // Add the last token
        tokens.Add(input.Substring(start).Trim());

        return tokens;
    }

    public class JobQueue {
        public List<Job> jobs;
        public int remaining;

        public JobQueue(){
            jobs = new List<Job>();
            remaining = 0;
        }

        public JobQueue(List<Job> in_jobs){
            jobs = in_jobs;
            remaining = in_jobs.Count;
        }

        public void AddNewJobs(List<Job> in_jobs){
            if (remaining != 0){
                Debug.Log("Previous commands haven't finished");
            }
            jobs = in_jobs;
            remaining = in_jobs.Count;
            
        }

        public void Clear(){
            jobs.Clear();
            remaining = 0;
        }
    }

    //funcs from the commands 
    public class Job{
        public string name;
        public object[] args;

        public Job(string in_name, object[] in_args){
            name=in_name;
            args=in_args;
        }

    }

}
