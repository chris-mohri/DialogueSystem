using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DialogueSystem : MonoBehaviour
{

    //controls for the dialogue system
    public PlayerControls controls;

    //display variables
    //true if the letters of the current entry are still being displayed 1 by 1, false if finished
    private bool stillDisplaying=false;
    private bool chapterEnd=false;
    //blinks at the end of the current entry
    private string endCharacter = "[]";

    //the dialogue object that has the text component
    [SerializeField]
    private GameObject DialogueObject;
    //the text component to the DialogueObject
    private TMP_Text textObj;

    //max number of lines that the text can occupy on screen
    //search by text.textInfo.lineCount

    //text display/animation variables
    public int maxLines = 20;
    private int currentCharIndex=0;
    private int currentShownCharacters=0;
    private int currentTotalCharacters=0;
    private int currentDeployedAlphas=0;
    private int maxDeployedAlphas=5;
    private ArrayList alphaIndex;
    private Dictionary<string, string> alphaDict;
    [SerializeField]
    private int alphaIncrement = 5;
    private string aTag1 = "<alpha=#10>"; //11 length
    private int aTagLength;

    //keeps track of time 
    private float currentTime = 0.0f;
    private float displayTimer = 0.0f;
    [SerializeField]
    private float displaySpeed = 0.02f; //adjust as needed
    private float fadeTimer = 0.0f;
    [SerializeField]
    private float fadeSpeed = 0.01f; //adjust as needed

    //the main data object that holds the dialogue information
    private Book book;
    private string endName = ".END";

    //================== CHAPTERS ===========================================================
    //ADD ALL CHAPTERS HERE (in the editor, place the .jsons here: chapter1.json should be placed here)
    public TextAsset chapter1;

    // =======================================================================================

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
        //gets the text component from the DialogueObject
        textObj = DialogueObject.GetComponent<TMP_Text>();

        alphaIndex = new ArrayList();
    }

    void Start(){
        //creates the book
        book = new Book();
        book.LoadChapter(chapter1);

        //gets the alpha dictionary (controls the opacity increments when fading in letters)
        alphaDict = getAlphaDict();
        aTagLength = aTag1.Length;

    }

    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }

    // Update is called once per frame
    void Update(){
        //if the player presses continue
        if (controls.Keyboard.Continue.triggered){
            //Debug.Log("continue to next entry");

            //if still displaying the previous entry when clicked, set maxVisibleCharacters to max
            if (currentShownCharacters < textObj.textInfo.characterCount) {
                currentShownCharacters = textObj.textInfo.characterCount;
                textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
            } else { //otherwise start displaying the next entry
                //adds words to the dialogue box (they start as invisible)
                string text = book.getCurrentEntryAndIncrement().dialogue;
                if (text.Length>=2){
                    //automatically adds a space between sentences when not on a new line
                    if (text.Substring(0,2)!="\n" && textObj.text.Length!=0){
                        text = " "+ text;
                    }
                }
                //add the text to the textObj
                textObj.text += text;
                // check if lines have exceeded maxLines
                // TODO

                //handle letters
                // currentTotalCharacters = textObj.textInfo.characterCount;
                textObj.maxVisibleCharacters=currentShownCharacters;

                // Debug.Log(textObj.textInfo.characterCount);
                // Debug.Log(currentTotalCharacters);
                // Debug.Log(textObj.maxVisibleCharacters);
                // Debug.Log(currentShownCharacters);
                // Debug.Log(textObj.text.Length);
                // Debug.Log("====");
            }

        }
        // Debug.Log(textObj.textInfo.characterCount);
        // textObj.maxVisibleCharacters=4;
        // Debug.Log(textObj.textInfo.characterCount);
        // Debug.Log(textObj.text.Length);

        //display the letters
        addTextToScreen();

        //update timer
        handleTimer();

    }

    void addTextToScreen(){
        
        //in a faster timer, update every alpha tag in the arraylist, removing tags that reach 100%
        if (fadeTimer == 0){
            foreach (int index in alphaIndex){
                string newTag = alphaDict[textObj.text.Substring(index, 11)];
                //if the new tag is 100% alpha (opacity), then just remove it
                
            }
        }

        if (displayTimer==0){
            if (currentShownCharacters<textObj.textInfo.characterCount){
                currentShownCharacters+=1;
                currentCharIndex+=1;
                textObj.maxVisibleCharacters=currentShownCharacters;

                //add this index to the list
                alphaIndex.Add(currentCharIndex);
                textObj.text=textObj.text.Insert(currentCharIndex-1, aTag1);
                currentCharIndex+=11;
            }
        }

        // also lower opacity of previous entries
        // check if lines have exceeded maxLines
        //text.maxVisibleCharacters=9;
        //Debug.Log(text.textInfo.lineCount);

        //0.2, .4, .6., .8, 1

        /*
        <color=#aaaaaa> 15
        <alpha=#b8> 11
        <alpha=20> 10
        */
    }

    void handleTimer(){
        displayTimer += Time.deltaTime;
        fadeTimer += Time.deltaTime;
        if (displayTimer >= displaySpeed) {
            displayTimer = 0;
        }
        if (fadeTimer >= fadeSpeed) {
            fadeTimer = 0;
        }
    }

    private Dictionary<string, string> getAlphaDict(){
        Dictionary<string, string> dict = new Dictionary<string, string>();
        for (int i = 10; i < 100; i += alphaIncrement){
            string key = $"<alpha=#{i}>";
            string value = $"<alpha=#{i + 5}>";
            dict[key] = value;
        }
        return dict;
    }

// ============== INNER CLASSES =========================================

//required for deserializing json
//In the .json file, must look like  { dialogueEntries: [ {entry1}, {entry2}, {...} ] }
    [System.Serializable]
    public class DialogueWrapper{
        public List<DialogueEntry> dialogueEntries;
    }
    //required for deserializing json as described above
    //each entry must have these fields
    [System.Serializable]
    public class DialogueEntry{
        public string name;
        public string dialogue;
        public string route;
        public string voiceFile;

        public string commands; //might be beyond scope of dialogue system
        //example 
        /*
        .swap Aoko Aoko_happy
            swaps the sprite of the gameObject with the name "Aoko" to "Aoko_happy"
        .move Aoko left 100 2
            moves gameObject with the name "Aoko" to the left 100 pixels within 2 seconds
        */
    }

    [System.Serializable]
    public class Book{
        //the container for all the entries in the loaded chapter. Each route corresponds to a list of entries
        private Dictionary<string, List<DialogueEntry>> currentChapter;
        //the list of entries of the current route
        private List<DialogueEntry> currentRoute;
        //saves the index for the current dialogueEntry in currentRoute
        private int bookmark=0;
        //marker that marks the end of the chapter (must be set as the name in a dialogue entry)
        private string endName = ".END";

        public Book(){
            currentChapter = new Dictionary<string, List<DialogueEntry>>();
        }

        //reads in a chapter file and returns a list of name/text/.../command entries
        public void LoadChapter(TextAsset file){
            string jsonText = file.text;
            DialogueWrapper wrapper = JsonUtility.FromJson<DialogueWrapper>(jsonText);

            //for debugging
            // foreach (DialogueEntry entry in wrapper.dialogueEntries)
            // {
            //     Debug.Log(entry.name+": " + entry.dialogue);
            //     Debug.Log(entry.voiceFile+": " + entry.commands);
            // }

            LoadIntoDictionary(wrapper);
        }

        public void LoadIntoDictionary(DialogueWrapper wrapper){
            //create dictionary based on the different routes of each entry
            foreach (DialogueEntry entry in wrapper.dialogueEntries)
            {
                //if route is not in the dictionary yet, add it
                if (!currentChapter.ContainsKey(entry.route)){
                currentChapter[entry.route] = new List<DialogueEntry>();
                }
                //add the entry to that route
                currentChapter[entry.route].Add(entry);
                    
            }

            //sets the current route of this chapter to the 1st (default) route
            currentRoute = currentChapter["1"];
        }

        //moves the current route to another route or another entry (part of the route)
        public void JumpTo(string route, int part=0){
            currentRoute = currentChapter[route];
            bookmark=part;
        }

        public List<DialogueEntry> getCurrentRoute(){
            return currentRoute;
        }

        public DialogueEntry getCurrentEntry(){
            return currentRoute[bookmark];
        }

        public DialogueEntry getCurrentEntryAndIncrement(){
            DialogueEntry entry = currentRoute[bookmark];

            //if the current entry was NOT the end of the chapter, then move the bookmark to the next entry
            if (entry.name!=endName){
                bookmark++;
            }

            return entry;
        }

        public DialogueEntry getEntry(int i){
            return currentRoute[i];
        }

        public void setBookmark(int num){
            bookmark = num;
        }
        public int getBookmark(){
            return bookmark;
        }

        public bool isEnd(){
            return false;
        }

    }

    /*
    1. get text to appear on screen + lower alpha of previous entries + add blinking marker at end
    2. log system
    3. choice system
    4. hide button 
    */

}