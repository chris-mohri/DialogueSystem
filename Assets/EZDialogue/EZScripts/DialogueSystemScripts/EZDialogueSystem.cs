using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using TMPro;
using System.Text.RegularExpressions;

public class EZDialogueSystem : MonoBehaviour
{

    //controls for the dialogue system
    private PlayerControls controls;

    //saved information script
    private SavedInformation save;
    private CommandsController commandController;

    
    [SerializeField] [Tooltip("The GameObject that has the TextMeshPro component for displaying dialogue")]
    private GameObject DialogueObject; //the dialogue object that has the main text component
    private TMP_Text textObj; //the text component to the DialogueObject

    //for the log (dialogue history)
    [SerializeField] [Tooltip("The GameObject that has the TextMeshPro component for displaying the dialogue in the Log")]
    private GameObject LogTextObject; 
    private TMP_Text logTextTMP;
    [SerializeField] [Tooltip("The GameObject that has the TextMeshPro component for displaying the names in the Log")]
    private GameObject LogNamesObject; 
    private TMP_Text logNamesTMP;

    // ------------------- text display/animation variables -------------------
    //variables here need to be saved into save file
    private bool displayingChoices = false;
    private bool canContinue = true; //if the player can continue to the next entry
    private bool moreLettersToDisplay=false;//true if the letters of the current entry are still being displayed 1 by 1, false if finished
    private int undimTagIndex=-1; //keeps track of the un-dim tags. 
    [HideInInspector]
    public Book book; //the main data object that holds the dialogue information. must be accessible to CommandsController
    [HideInInspector]
    public string logNames;
    [HideInInspector]
    public string logText;


    // === variables below here are not needed to be saved in save files ===
    private bool forceContinue = false;
    //indentation spaces for new lines. adjust as needed
    private string newLineSpace = "  ";
    //tracks the index of the newest letter shown on screen from the raw text
    //  (raw text also counts any of TMPro's alpha or color tags that don't show up
    //  when using textObj.textInfo.characterCount)
    private int currentCharIndex=0;
    //the increment amount for the alpha (opacity) of letters
    [SerializeField] [Tooltip("Increment amount (percent) for the fade-in effect of letters (default: 5)")] [Range(1, 100)]
    private int alphaIncrement = 5;
    //starting alpha value of the newest letter that is displayed (keep at 10 since
    //  since the length must stay as 11)
    private string aTag1 = "<alpha=#10><link=$fade$></link>"; //11 length
    //dim tag (previously displayed sentences are dimmed)
    private string dimTag = "<color=#aaaaaa><link=$dim$></link><alpha=#b8><link=$dim$></link>";
    private string undimTag = "<color=#ffffff><link=$undim$></link><alpha=#ff><link=$undim$></link>";
    private int dimTagLength;
    private int aTagLength; //set as 11
    [SerializeField] [Tooltip("Font for hovering over choice options")]
    public string hoverFont = "VarelaRound2";
    [SerializeField] [Tooltip("Normal font")]
    public string normalFont = "VarelaRound1";

    // ------------------- keeps track of time -------------------
    private double currentTime = 0.0f;
    private double displayTimer = 0.0f;
    [SerializeField] [Tooltip("Time (in seconds) to display the next letter (default: 0.03)")] [Range(0.001f, 10f)]
    private double displaySpeed = 0.03f; //adjust as needed 
    private double fadeTimer = 0.0f;
    [SerializeField] [Tooltip("Time (in seconds) to increment the opacity of letters (default: 0.004)")] [Range(0.001f, 10f)]
    private double fadeSpeed = 0.004f; //adjust as needed
    [SerializeField] [Tooltip("(default: 1) Default Route name in the story")]
    private static string defaultRouteName = "1";

    //[SerializeField] [Tooltip("(default: Assets/EZDialogue/) File Path for Dialogue files. Make sure to also have subfolders here called 'JSONs' and 'TSVs'")]
    private static string dialogueFolderPath = "Assets/EZDialogue/DialogueFiles/";
    [SerializeField] [Tooltip("Toggle to allow script to automatically create necessary directories")]
    private bool autoCreateDirectories = true;

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
        //gets the text component from the DialogueObject
        textObj = DialogueObject.GetComponent<TMP_Text>();

        textObj.maxVisibleCharacters=0;
        aTagLength = aTag1.Length;
        dimTagLength = dimTag.Length;

        //log variables
        logText="";
        logNames="";
        logTextTMP = null;
        logNamesTMP = null;
    
        logTextTMP = LogTextObject.GetComponent<TMP_Text>();
        logNamesTMP = LogNamesObject.GetComponent<TMP_Text>();
     
    }

    void Start(){
        //verify that the necessary folders exist
        if (autoCreateDirectories){
            SetupFolders();
        }

        //setup connection with other components
        save = GetComponent<SavedInformation>();
        commandController = GetComponent<CommandsController>();

        //make sure the counters are counted correctly at the start
        textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
        currentCharIndex = textObj.text.Length;

        book = new Textbook();
        book.allowDynamicReading = true;
        book.LoadChapter("chapter1.txt");
        // book.Export();
    }

    //sets up all the necessary directories
    private void SetupFolders(){
        try {
            //create directory if doesn't exist 
            if (!Directory.Exists(dialogueFolderPath)){
                Directory.CreateDirectory(dialogueFolderPath);
            }
            //create directory if doesn't exist
            if (!Directory.Exists(dialogueFolderPath+"JSONs")){
                Directory.CreateDirectory(dialogueFolderPath+"JSONs");
                Debug.Log("Created "+dialogueFolderPath+"JSONs");
            }
            //create directory if doesn't exist
            if (!Directory.Exists(dialogueFolderPath+"TEXTs")){
                Directory.CreateDirectory(dialogueFolderPath+"TEXTs");
                Debug.Log("Created "+dialogueFolderPath+"TEXTs");
            }
            //create directory if doesn't exist
            if (!Directory.Exists(dialogueFolderPath+"JSONtoTEXT")){
                Directory.CreateDirectory(dialogueFolderPath+"JSONtoTEXT");
                Debug.Log("Created "+dialogueFolderPath+"JSONtoTEXT");
            }
            //create directory if doesn't exist
            if (!Directory.Exists(dialogueFolderPath+"TEXTtoJSON")){
                Directory.CreateDirectory(dialogueFolderPath+"TEXTtoJSON");
                Debug.Log("Created "+dialogueFolderPath+"TEXTtoJSON");
            }

        } catch (System.Exception ex){
            Debug.LogError("Error : " + ex.Message);
        }
    }

    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }

    public void DisableControls(){
        controls.Disable();
    }

    public void EnableControls(){
        controls.Enable();
    }

    public void DisableContinue(){
        canContinue = false;
    }

    public void EnableContinue(){
        canContinue = true;
    }

    // Update is called once per frame
    void Update(){
        HandleScreen();

        //update timer
        HandleTimer();

    }

    //gets called every frame
    private void HandleScreen(){
        //make sure max visible character is never over the total number of characters
        if (textObj.maxVisibleCharacters > textObj.textInfo.characterCount){
            textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
        }
        //if the player presses continue (or is automatically continued by pressing a choice)
        if (canContinue == true && (controls.Keyboard.Continue.triggered || forceContinue)){
            forceContinue=false;

            //if still displaying the previous entry when clicked, set maxVisibleCharacters to max
            if (textObj.maxVisibleCharacters < textObj.textInfo.characterCount) {
                SkipFade();

                //skip to the end of all commands (if possible e.g. not waiting for user input)
                if (!commandController.ReadyToContinue()){
                    SkipCommands();
                }

            } else if (!commandController.ReadyToContinue()){ //if a command's animation is still playing, then skip to the end result
                SkipCommands(); //maybe require a double click?
                
            } else if (commandController.ReadyToContinue()){ //otherwise start displaying the next entry
                //if we haven't reached the end yet, then continue displaying
                if (!book.IsEnd()){
                    DialogueEntry currentEntry = book.GetNextEntryAndIncrement();

                    //add text
                    string text = currentEntry.dialogue;
                    text = PostprocessText(text);
                    AddText(text);

                    //also add the text to the log object
                    AddToLog(currentEntry);


                    //execute functions (must happen after so commands can add text)
                    if (currentEntry.commands != ""){
                        commandController.ExecuteFunction(currentEntry.commands);
                    }
                } else {
                    Debug.Log("Chapter Ended");
                }

            }
        //if the user tries to click when dialogue is paused (such as options are displayed)
        } else if (canContinue == false && controls.Keyboard.Continue.triggered){
            if (!commandController.ReadyToContinue()){
                SkipCommands();
            }
        }
        // Debug.Log(textObj.text);
        // Debug.Log(textObj.textInfo.characterCount);
        // Debug.Log(textObj.maxVisibleCharacters);
        // Debug.Log(currentCharIndex);

        //display the letters
        AddLettersToScreen();
    }

    //__f
    private void AddToLog(DialogueEntry entry){
        logNames += entry.name +"\n\n";
        logText += entry.dialogue.Replace("\n", "") +"\n\n";
        if (logTextTMP!=null && logNamesTMP !=null) {
            int initialNumTextLines = logTextTMP.textInfo.lineCount;
            logTextTMP.text = logText;
            logTextTMP.ForceMeshUpdate();
            int addedNumTextLines = logTextTMP.textInfo.lineCount - initialNumTextLines;

            int initialNumNamesLines = logNamesTMP.textInfo.lineCount;
            logNamesTMP.text = logNames;
            logNamesTMP.ForceMeshUpdate();
            int addedNumNamesLines = logNamesTMP.textInfo.lineCount - initialNumNamesLines;

            //add padding so they line up
            if (addedNumTextLines > addedNumNamesLines){
                for (int i =0; i<addedNumTextLines-addedNumNamesLines; i++){
                    logNames += "\n";
                    logNamesTMP.text += "\n";
                }
            } else if (addedNumTextLines < addedNumNamesLines){
                for (int i =0; i<addedNumNamesLines-addedNumTextLines; i++){
                    logText += "\n";
                    logTextTMP.text += "\n";
                }
            }
        }
    }
/*
<font="VarelaRound1"><link=$opt_1a$></link><link=1a>1.   "Aoko, you speak too much"</link>
1. choice A
<font="VarelaRound2>2. choice B
*/
    //removes the font and link tags from the dialogue options while also deleting the unchosen options' text
    public void PlayerChoseAnOption(string chosenOption){
        //remove the color from the options
        int index = textObj.text.IndexOf("<link=$opt_");
        while (index!=-1){
            bool removeResidualTag = false;
            int l = FindIndexOfTagBefore(index);
            int r = FindIndexOfTagAfter(index)+7;
            string tagToRemove = textObj.text.Substring(l, r-l); //removes the font tag

            //remove the link and option text as well for the unselected options 
            if (tagToRemove.IndexOf("$opt_"+chosenOption+"$")==-1){
                int closeTag = textObj.text.IndexOf("</link>", r)+7;
                tagToRemove = textObj.text.Substring(l, closeTag-l);
            } else { //else just remove the other link tag
                removeResidualTag = true;
                tagToRemove = textObj.text.Substring(l, r+7+chosenOption.Length-l);
            }
            RemoveTag(l, tagToRemove);

            //if removed only the other link tag, also remove the closing link tag
            if (removeResidualTag){
                int residualTag = textObj.text.IndexOf("</link>", l);
                tagToRemove = textObj.text.Substring(residualTag, 7);
                RemoveTag(residualTag, tagToRemove);
            }
            
            index = textObj.text.IndexOf("<link=$opt_");
        }
    }
    //allows the player to continue
    public void ReturnControlAfterChoice(){
        canContinue = true;
        displayingChoices = false;
        forceContinue=true;
    }

    //displays choices 
    public void DisplayChoices(List<string> choices, List<string> result){
        canContinue = false;
        displayingChoices = true; 
        
        string text="\n\n";
        string middle="";
        string left="";
        for (int i = 0; i<choices.Count; i++){
            int num = i+1;
            left = "<font=\""+normalFont+"\"><link=$opt_"+result[i]+"$></link>"+aTag1+"<link="+result[i]+">"+num+".   ";
            middle = choices[i]+"</link>";
            text+= left+middle+"\n";
        } //__s
        text+="\n";

        //add the text
        textObj.text += text;
        textObj.ForceMeshUpdate();
        textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
        currentCharIndex = textObj.text.Length;

        if (textObj.isTextOverflowing){
            //clear current text and set as new text
            // ClearTextThenAdd(text);
            textObj.text = text;
            textObj.ForceMeshUpdate();
            if (textObj.isTextOverflowing){
                textObj.text = "Current line is too big to display on the screen. Consider making it smaller.";
                textObj.ForceMeshUpdate();
            }
            // textObj.maxVisibleCharacters = 0;
            // currentCharIndex = 0;
            textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
            currentCharIndex = textObj.text.Length;
            undimTagIndex=-1;
        }
    }

    public void SkipFade(){
        //set all letters to alpha/opacity to 100% (by removing the alpha tags)
        for (int index = textObj.text.Length-1; index>=0; index--){
            //if there's enough room for an alpha tag
            if (index+aTagLength<=textObj.text.Length-1) {
                //if there is indeed an alpha tag here and it is a fade tag
                if (GetTagId(index)=="$fade$"){
                    textObj.text = textObj.text.Remove(index, aTagLength);
                }
            }
        }
        textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
        currentCharIndex = textObj.text.Length;
    }

    public void SkipCommands(){
        commandController.Skip();
        //what if there are some animations that want to be played while dialogue is happening?
    }

    public void AddText(string text){
        //add the text to the textObj 
        textObj.text += text;
        textObj.ForceMeshUpdate();

        // ===================== DIM TAGS =============================
        // if the text doesn't have the dim tags yet, add them 
        if (textObj.text.Length>=dimTagLength){
            if (textObj.text.Substring(0, dimTagLength)!=dimTag){
                AddTag(0, dimTag);
            }
        } else {
            AddTag(0, dimTag);
        }

        if (text != ""){
            // remove the old undim tag and place the new undim tag =====
            if (undimTagIndex!=-1){
                RemoveTag(undimTagIndex, undimTag);
            }
            undimTagIndex=currentCharIndex;
            AddTag(currentCharIndex, undimTag);
            // =========================================================
            // check if lines have exceeded maxLines     =================
            if (textObj.isTextOverflowing){
                //clear current text and set as new text
                ClearTextThenAdd(text);
            }
        }
        moreLettersToDisplay=true;
    }

    //clears then restates the previous entry
    public void ClearTextThenRestateEntry(){
        textObj.text = book.GetCurrentEntry().dialogue;
        textObj.ForceMeshUpdate();
        if (textObj.isTextOverflowing){
            textObj.text = "Current line is too big to display on the screen. Consider making it smaller.";
            textObj.ForceMeshUpdate();
        }
        textObj.maxVisibleCharacters = 0;
        currentCharIndex = 0;
        undimTagIndex=-1;
    }

    public void ClearTextThenAdd(string text) {
        textObj.text = text;
        textObj.ForceMeshUpdate();
        if (textObj.isTextOverflowing){
            textObj.text = "Current line is too big to display on the screen. Consider making it smaller.";
            textObj.ForceMeshUpdate();
        }
        textObj.maxVisibleCharacters = 0;
        currentCharIndex = 0;
        undimTagIndex=-1;
    }

    //adds newline indentation or automatically adds a space between sentences
    private string PostprocessText(string text){
        if (text.Length>=1){
            //automatically adds a space between sentences when not on a new line
            if (text.Substring(0,1)!="\n" && textObj.textInfo.characterCount!=0){
                text = " "+ text;
            }
            //automatically adds white space when on a new line, or adds white space 
            //when on a new page
            else if (text.Substring(0,1)=="\n"){
                text = text.Replace("\n", $"\n{newLineSpace}");
            } 
            //when on a new page
            else if (textObj.textInfo.characterCount==0){
                text = newLineSpace+text;
            } 
        }
        return text;
    }

    private void AddLettersToScreen(){
        if (textObj.maxVisibleCharacters==textObj.textInfo.characterCount){ 
            moreLettersToDisplay=false;
        }

        int currentTextLength = textObj.text.Length;

        //in a faster timer, update every alpha tag in the list, removing tags that reach 100%
        if (fadeTimer >= fadeSpeed){
            int toRemove = -1;
            //loop through the text to find all the alpha tags
            for (int index = 0; index <= currentTextLength-1; index++){
                //if there's enough letters to possibly house another alpha tag, then continue
                if (index+aTagLength<=currentTextLength-1) {
                    //if there is an alpha tag here and it is a fade tag
                    if (GetTagId(index)=="$fade$"){
                    // if (GetTag(index)=="alpha" && GetTagId(index)=="fade"){
                        // Debug.Log("ALTS: "+textObj.text.Substring(index, aTagLength));
                        string oldTag = textObj.text.Substring(index, aTagLength);
                        //get the new tag e.g. <alpha=#99>
                        string hex = oldTag.Substring(8, 2);
                        string newHex;
                        //add the appropriate alpha increment
                        if (displayingChoices){
                            newHex = AddPercentToHex(hex, alphaIncrement/5);
                        } else {
                            newHex = AddPercentToHex(hex, alphaIncrement);
                        }
                        string newTag = $"<alpha=#{newHex}><link=$fade$></link>";
                        //mark for removal
                        if (newHex=="FF"){
                            toRemove = index;
                           // newHex="00";
                        }
                        //replace the old tag with the new tag
                        textObj.text = textObj.text.Replace(oldTag, newTag);
                    }
                    
                }
            }
            //remove the 100% alpha tag (since it is useless)
            if (toRemove != -1){
                RemoveTag(toRemove, "<alpha=#FF><link=$fade$></link>");
            }

            fadeTimer = 0.0f;
        }

        //display the next letter at the routine time
        if (displayTimer >= displaySpeed){
            //if there are more letters to fade in
            if (textObj.maxVisibleCharacters<textObj.textInfo.characterCount){     
                moreLettersToDisplay=true;

                //add the alpha tag
                AddTag(currentCharIndex, aTag1);

                currentCharIndex+=1; //move the pointer to the next letter so we can place a tag on it
                textObj.maxVisibleCharacters+=1;
            }
            displayTimer = 0.0f;
        }

        // also lower opacity of previous entries
        // check if lines have exceeded maxLines

        /*
        <color=#aaaaaa> 15
        <alpha=#b8> 11
        <alpha=20> 10

        <alpha=#b8><color=#aaaaaa>...<alpha=#ff><color=#ffffff> 26
        */
    }

    private void HandleTimer(){
        currentTime += Time.deltaTime;
        displayTimer += Time.deltaTime;
        fadeTimer += Time.deltaTime;
    }

    private string GetTag(int index){
        //continue only if this is a tag
        if (textObj.text[index]=='<'){
            //finds the index of the first '=' after the '<'
            int equalsIndex = textObj.text.IndexOf('=', index);
            int closeIndex = textObj.text.IndexOf('>', index);
            if (equalsIndex != -1 && closeIndex !=-1 && equalsIndex < closeIndex){
                //find the type of tag
                string tagType = textObj.text.Substring(index + 1, equalsIndex - index - 1);
                return tagType.Trim(); // Trim any leading or trailing spaces
            }
            return "malformed-tag";

        } else {
            return "not-a-tag";
        }
    }

    //__s
    //<link=><color=#ffffff><link=choice_option_color></link>
    //<link=1a><color=#aaaaaa><l></l><alpha>1. hi there</link>
    public void ChangeOptionColor(string id, string fontName){
        // return;
        // Debug.Log(id);
        int index = textObj.text.IndexOf("<link=$opt_"+id);
        int l = FindIndexOfTagBefore(index);
        int r = FindIndexOfTagAfter(index)+7;
        if (l < 0 || r-l<0) return;
        string fontToRemove = textObj.text.Substring(l, r-l);

        string newFont = "<font=\""+fontName+"\"><link=$opt_"+id+"$></link>";
        ReplaceTag(l, fontToRemove, newFont);
    }


    //gets the id for the tag at the index in the following form
    //<tag><link=id></link>
    private string GetTagId(int index){
        //continue only if this is a tag
        if (textObj.text[index]=='<'){
            //continue only if the tag immediately following is a link tag
            for (int i = index; i<textObj.text.Length; i++){
                //if current char is the closing tag
                if (textObj.text[i]=='>'){
                    //continue only if the the next character is a starting tag '<'
                    if (i+1<textObj.text.Length && textObj.text[i+1]=='<'){
                        int r = i+1;
                        //continue only if the next tag is an id (link) tag
                        if (r+6 <= textObj.text.Length && textObj.text.Substring(r, 6)=="<link="){
                            string subString = textObj.text.Substring(r);
                            //regex to find the link id
                            Match match = Regex.Match(subString, @"<link=(.*?)>");
                            if (match.Success && match.Groups.Count > 1){
                                return match.Groups[1].Value;
                            }
                            return "malformed-link-tag";

                        } else {
                            return "no-id-tag";
                        }
                    }
                    //return if there is not an id tag associated with this tag 
                    return "tag-not-followed-by-a-tag";
                } 
            }
            //no closing tag '>'
            return "no-closing-tag";
        }
        return "not-a-tag";
    
    }

    private string AddPercentToHex(string hexColor, int percentageToAdd)
    {
        //convert hex to percentage (00 to FF = 0% to 100%)
        int intValue = int.Parse(hexColor, System.Globalization.NumberStyles.HexNumber);
        double percentageValue = intValue / 255.0 * 100;

        //add the percentage
        percentageValue += percentageToAdd;

        //clamp between 0 and 100
        if (percentageValue<0){
            percentageValue=0;
        } else if (percentageValue>100){
            percentageValue=100;
        }

        //convert percent back to hex
        intValue = (int) Math.Round(percentageValue / 100 * 255);
        string newHexColor = intValue.ToString("X2");

        return newHexColor;
    }

    //add the tag at the given index
    private void AddTag(int index, string tag){
        textObj.ForceMeshUpdate();
        //happens if user puts into edits text in editor
        if (index>=textObj.text.Length){
            Debug.Log("Invalid Index When Adding Tag.");
            // for (int i = textObj.text.Length-1; i>=0; i--){
            //     if (c==0) return;
            //         //if there's enough room for an alpha tag
            //         if (i+aTagLength<=textObj.text.Length-1) { 
            //             //if there is indeed an alpha tag here and it is a fade tag
            //             if (textObj.text.Substring(i, 6)=="<alpha" && GetTagId(i)=="$fade$"){
            //                 textObj.text = textObj.text.Remove(i, aTagLength);
            //             }
            //         }
            //     }
            textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
            currentCharIndex = textObj.text.Length;
            return;
        }
        textObj.text = textObj.text.Insert(index, tag);
        currentCharIndex+=tag.Length;
    }

    //removes the tag at the given index
    private void RemoveTag(int index, string tag){
        textObj.ForceMeshUpdate();

        if (index>textObj.text.Length){
            return;
        }
        if (textObj.text[index]!='<'){
            Debug.Log($"RemoveTag() Error: Attempted to remove tag: {tag} at index {index}, but it is not a tag");
            Debug.Log("Attempted to remove from: "+textObj.text.Substring(index));
            return;
        }
        if (index>=textObj.text.Length){
            Debug.Log("RemoveTag() Error: Invalid Index When Removing Tag");
            return;
        }

        //checks if the string here is actually the tag to be removed
        if (textObj.text.Substring(index, tag.Length)!=tag){
            Debug.Log($"Tag to remove ({tag}) was not found at the given index ({index})");
            return;
        }
        // Debug.Log(index+" | "+tag +" | "+textObj.text.Substring(index, tag.Length));
        textObj.text = textObj.text.Remove(index, tag.Length);
        currentCharIndex-=tag.Length;        
    }

    private void ReplaceTag(int index, string oldTag, string newTag){
        RemoveTag(index, oldTag);
        AddTag(index, newTag);
        // textObj.ForceMeshUpdate(); //?
    }

    private int FindIndexOfTagBefore(int index){
        for (int i = index-1; i>=0; i--){
            if (textObj.text[i]=='<'){
                return i;
            }
        }
        return -1;
    }

    private int FindIndexOfTagAfter(int index){
        //find index of tag after this one
        for (int i = index+1; i<textObj.text.Length; i++){
            if (textObj.text[i]=='<'){
                return i;
            }
        }
        return -1;
    }

    //checks if 6-character sequence is valid hex
    public static bool IsValidHex(string hex){
        string pattern = @"^[0-9A-Fa-f]{6}$";
        return Regex.IsMatch(hex, pattern);
    }

    //returns if there are more letters to display
    public bool LettersStillDisplaying(){
        return moreLettersToDisplay;
    }

    public bool IsMenu(){
        if (displayingChoices==true)
            return true;

        return false;
    }

// ============== INNER CLASSES =========================================

//required for deserializing json
//In the .json file, must look like  { dialogueEntries: [ {entry1}, {entry2}, {...} ] }
    [System.Serializable]
    public class DialogueWrapper{
        public List<DialogueEntry> dialogueEntries;

        public DialogueWrapper(){
            dialogueEntries = new List<DialogueEntry>();
        }
    }
    //required for deserializing json as described above
    //each entry must have these fields
    [System.Serializable]
    public class DialogueEntry{
        public string route;
        public string name;
        public string dialogue;
        public string commands; //might be beyond scope of dialogue system
        public string voice;
        public string label;

        public DialogueEntry(){
            route="";
            name="";
            dialogue="";
            commands="";
            voice="";
            label="";
        }
    }

    [System.Serializable]
    public class Book{
        //the container for all the entries in the loaded chapter. Each route corresponds to a list of entries
        protected Dictionary<string, List<DialogueEntry>> currentChapter;
        protected DialogueWrapper wrapper;
        protected List<DialogueEntry> currentRoute; //the list of entries of the current route
        protected int bookmark=0;//saves the index for the current dialogueEntry in currentRoute
        protected string endName = ".END"; //marker that marks the end of the chapter (must be set as the name in a dialogue entry)
        protected string defaultRoute = defaultRouteName;
        protected DialogueEntry currentEntry = null;
        public bool allowDynamicReading = false;

        public Book(){
            currentChapter = new Dictionary<string, List<DialogueEntry>>();
        }

        //reads in a chapter file and returns a list of name/text/.../command entries
        public virtual void LoadChapter(string file){ 
            string jsonText = File.ReadAllText($"Assets/EZDialogue/JSONs/{file}");;
            wrapper = JsonUtility.FromJson<DialogueWrapper>(jsonText);

            LoadIntoDictionary();
    
        }

        private void LoadIntoDictionary(){
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
            currentRoute = currentChapter[defaultRoute];
        }

        //moves the current route to another route or another entry (part of the route)
        public void JumpTo(string route, int part=0){
            currentRoute = currentChapter[route];
            bookmark=part;
        }

        public virtual List<DialogueEntry> GetCurrentRoute(){
            return currentRoute;
        }

        public virtual DialogueEntry GetNextEntry(){
            return currentRoute[bookmark];
        }

        public virtual DialogueEntry GetCurrentEntry(){
            // if (bookmark == 0){
            //     throw new Exception("No entry has been read yet");
            // }
            return currentRoute[bookmark];
        }

        public virtual DialogueEntry GetNextEntryAndIncrement(){
            if (IsEnd()){
                DialogueEntry entry = new DialogueEntry();
                entry.dialogue = "REACHED END OF ROUTE";
                // Debug.Log("Reached the End of Route");
                return entry;
            }
            currentEntry = currentRoute[bookmark];
            bookmark++;

            return currentEntry;
        }

        public virtual DialogueEntry GetEntry(int i){
            return currentRoute[i];
        }

        public virtual void SetBookmark(int num){
            bookmark = num;
        }
        public virtual int GetBookmark(){
            return bookmark;
        }

        public virtual bool IsEnd(){
            if (bookmark >= currentRoute.Count){
                return true;
            }
            return false;
        }

        public virtual void Export(){
        }

        public virtual string[] GetLines(){
            return null;
        }
        public virtual void SetPointer(int p){
        }

    }

    [System.Serializable]
    public class Textbook: Book{        
        //needed because dictionaries don't preserve order
        private List<string> routeOrder;

        //for dynamically accessing the text file
        private string pointerRoute = null;
        private int pointer=0;
        private string[] lines;
        private string filepath;

        //files
        private string jsonConvertFilePath = dialogueFolderPath + "TEXTtoJSON/";
        private string textConvertFilePath = dialogueFolderPath + "JSONtoText/";
        private string filename; //includes extension type
        private string extension;

        public Textbook(){
            RefreshVariables();
        }

        //reads in a chapter file 
        public override void LoadChapter(string file){
            // ======= refresh variables =======
            RefreshVariables();

            extension = file.Substring(file.IndexOf("."));

            if (extension == ".json") {
                filename = file.Substring(0, file.IndexOf("."));
                filepath = Path.Combine(dialogueFolderPath+"JSONs/",file);
                LoadAsJson(file);

            } else if (extension == ".txt"){
                filename = file.Substring(0, file.IndexOf("."));
                filepath = Path.Combine(dialogueFolderPath+"TEXTs/",file);

                pointerRoute = null;
                pointer = 0;
                // RefreshLines();
                if (allowDynamicReading==false){
                    ParseAllEntries();
                }
            } else {
                throw new Exception("File type is neither .txt or .json");
            }
        }

        //reads in a chapter file and returns a list of name/text/.../command entries
        private void LoadAsJson(string file){ 
            try {
                string jsonText = File.ReadAllText($"{dialogueFolderPath}JSONs/{file}");;
                wrapper = JsonUtility.FromJson<DialogueWrapper>(jsonText);

                LoadIntoDictionary();
            }
            catch {
                throw new Exception("File not found");
            }
        }

        private void LoadIntoDictionary(){
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
            currentRoute = currentChapter[defaultRoute];
        }


        private void RefreshVariables(){
            bookmark = 0;
            pointer = 0;
            pointerRoute = null;
            currentChapter = new Dictionary<string, List<DialogueEntry>>();
            routeOrder = new List<string>();
            wrapper = new DialogueWrapper();
        }

        public void RefreshLines(){
            lines = File.ReadAllLines(filepath);
        }

        //parses all entries and loads into the dictionary
        public void ParseAllEntries(){
            List<DialogueEntry> list = new List<DialogueEntry>();
            DialogueEntry entry = new DialogueEntry();

            int insurance = 100000;
            while (entry != null){
                entry = ParseNextEntry();
                if (entry != null){
                    list.Add(entry);
                }

                insurance--;
                if (insurance==0){
                    Debug.Log("Infinite loop warning in ParseAllEntries()");
                    break;
                }
            }
            wrapper = new DialogueWrapper();
            wrapper.dialogueEntries = list;

            LoadIntoDictionary();
        }

        //gets the next entry block from the text file
        public DialogueEntry ParseNextEntry(){
            //refresh lines so you can dynamically change the file while in game
            RefreshLines();

            DialogueEntry entry = new DialogueEntry();
            entry.route = pointerRoute;
            string line;
            bool blockFound = false;
            int insurance = 1000000;

            string[] nameAndDialogue = null;
            string commands = null;
            string voice = null;
            string label = null;

            //loop through the lines until you find a dialogueEntry block
            while(pointer < lines.Length){
                line = lines[pointer].Trim(); //trim the line
                pointer++; //move pointer
                
                insurance--; //make sure no infinite loop happens
                if (insurance==0){
                    Debug.Log("Infinite loop warning in ParseNextEntry()\nMax number of lines in a file cannot exceed 1,000,000");
                    break;
                }

                //continue if haven't found block yet, return if found end of block
                if (line.Length==0){
                    if (blockFound){
                        //each entry block must be attached to a route
                        if (entry.route==null){
                            throw new Exception("No route attached to this entry block"); 
                        }
                        return entry;
                    } else {
                        continue;
                    }
                }

                if (IsCommentLine(line)) continue;

                //handle the type of line
                string typeOfLine = HasKeyWord(line);
                if (typeOfLine!=null){
                    if (typeOfLine==".route"){
                        pointerRoute = ParseRoute(line, pointerRoute);
                        entry.route = pointerRoute;

                        //make sure there are not duplicate routes
                        if (!routeOrder.Contains(entry.route)){
                            routeOrder.Add(entry.route);
                        } else {
                            throw new Exception(filename+extension+": Duplicate route. Routes must be unique.");
                        }

                    } else if (typeOfLine==".say"){
                        nameAndDialogue = ParseDialogue(line, nameAndDialogue);
                        entry.name = nameAndDialogue[0];
                        entry.dialogue = nameAndDialogue[1];
                        blockFound = true;
                    } else if (typeOfLine==".func"){
                        commands = ParseCommandsBasic(line, commands);
                        entry.commands = commands;
                        blockFound = true;
                    } else if (typeOfLine==".voice"){
                        voice = ParseVoice(line, voice);
                        entry.voice = voice;
                        blockFound = true;
                    } else if (typeOfLine==".label"){
                        label = ParseLabel(line, label);
                        entry.label = label;
                        label = null;
                    } else {
                        Debug.Log("WEIRD THING HAPPENED IN PARSENEXTENTRY() FUNCTION");
                    }
                } else {
                    Debug.Log("Unknown type of statement on line "+pointer);
                }
                // DialogueEntry entry = new DialogueEntry(); 
            }

            //if no more blocks, then return null

            return null;
        }

        private bool IsCommentLine(string line){
            if (line.Length>=1){
                if (line[0]=='#'){
                    return true;
                }
            }
            return false;
        }

        //verifies that if there's a keyword, then it must be formatted correctly (space after it)
        private string HasKeyWord(string line){
            string[] list = new string[5];
            list[0]=".route";
            list[1]=".say";
            list[2]=".func";
            list[3]=".voice";
            list[4]=".label";

            int index = -1;
            //if there is indeed a dot
            foreach (string command in list){
                index = line.IndexOf(command);

                //if it was found
                if (index!=-1){
                    return command;
                }         
            }

            return null;
        }

        //parse as dialogue
        private string[] ParseDialogue(string line, string[] nameAndDialogue){
            string[] list = new string[2];
            string type = ".say";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (nameAndDialogue!=null){
                    // Debug.Log("Multiple .say lines in same block on line "+pointer);
                    throw new Exception(filename+extension+": Multiple .say lines in same block on line "+pointer);
                    //throw exception?
                }

                list[0] = line.Substring(0, loc).Trim();
                list[1] = line.Substring(loc + type.Length).Trim().Replace("\\n", "\n");
                return list;
            }
            return nameAndDialogue;
        }

        private string ParseVoice(string line, string oldVoice){
            string voice = oldVoice;
            string type = ".voice";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (oldVoice!=null){
                    // Debug.Log("Multiple .voice lines in same block on line "+pointer);
                    throw new Exception(filename+extension+": Multiple .voice lines in same block on line "+pointer);
                }
                voice = line.Substring(loc + type.Length).Trim();
            }
            return voice;
        }

        private string ParseCommandsBasic(string line, string oldCommands){
            string commands = oldCommands;
            string type = ".func";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (oldCommands!=null){
                    // Debug.Log("Multiple .func lines in same block on line "+pointer);
                    // throw new Exception(filename+extension+": Multiple .func lines in same block on line "+pointer);
                    commands += " "+line.Trim();
                } else {
                    commands = line.Trim();
                }
                
            }
            return commands;
        }
        private string ParseRoute(string line, string oldRoute){
            string route = oldRoute;
            string type = ".route";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                // if (oldRoute!=null){
                //     Debug.Log(filename+extension+": Multiple .route lines in same block on line "+pointer);
                // }
                route = line.Substring(loc + type.Length).Trim();
            } 
            return route;
        }

        private string ParseLabel(string line, string oldLabel){
            string label = oldLabel;
            string type = ".label";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                label = line.Substring(loc + type.Length).Trim();
            } 
            return label;
        }

        public override void Export(){
            if (extension==".json"){
                ExportToText();
            } else {
                if (allowDynamicReading==false){
                    ExportToJson();
                } else {
                    Debug.Log("CANNOT EXPORT WHEN IN DYNAMIC READING MODE");
                }
            }
        }

        //export to json file
        private void ExportToJson(){
            string newFile = filename+".json";

            try {
                //create directory if doesn't exist
                if (!Directory.Exists(jsonConvertFilePath)){
                    Directory.CreateDirectory(jsonConvertFilePath);
                }
                string jsonData = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(jsonConvertFilePath + newFile, jsonData);
                Debug.Log("Text exported to JSON and saved to file: " + jsonConvertFilePath + newFile);
            } catch (System.Exception ex){
                Debug.LogError("Error exporting JSON: " + ex.Message + "\nPlease have "+jsonConvertFilePath);
            }
        }

        //exports to text
        private void ExportToText(){
            string newFile = filename+".txt";
            
                //create directory if doesn't exist
                if (!Directory.Exists(textConvertFilePath)){
                    Directory.CreateDirectory(textConvertFilePath);
                }

                List<string> list = new List<string>();
               
                string line = "\n";
                foreach (DialogueEntry entry in wrapper.dialogueEntries){
                    if (!list.Contains(entry.route)){
                        line += $".route {entry.route}\n";
                        list.Add(entry.route);
                    }

                    if (entry.label!=""){
                        line += $".label {entry.label}\n";
                    }

                    if (entry.dialogue != ""){
                        string n = entry.name;
                        if (n!=""){
                            n+=" ";
                        }
                        line += $"\t{n}.say {entry.dialogue.Replace("\n", "\\n")}\n"; 
                    }
                    if (entry.commands != ""){
                        line += $"\t.func {entry.commands}\n"; 
                    }
                    if (entry.voice != ""){
                        line += $"\t.voice {entry.voice}\n"; 
                    }
                    line += "\n";
                    }
                
                // Debug.Log(line);
                File.WriteAllText(textConvertFilePath + newFile, line);

                Debug.Log("JSON exported to text and saved to file: " + textConvertFilePath + newFile);
          
        }

        public override DialogueEntry GetCurrentEntry(){
            if (extension == ".txt" && allowDynamicReading==true){
                return currentEntry;
            } else {
                return base.GetCurrentEntry();
            }

        }

        //gets the next entry and increments as needed
        public override DialogueEntry GetNextEntryAndIncrement(){
            if (extension == ".txt" && allowDynamicReading==true){
                if (IsEnd()){
                    DialogueEntry entry = new DialogueEntry();
                    entry.dialogue = "REACHED END OF ROUTE";
                    return entry;
                }
                currentEntry = ParseNextEntry();
                return currentEntry;

            } else {
                return base.GetNextEntryAndIncrement();
            }
        }

        public override bool IsEnd(){
            RefreshLines();
            if (extension == ".txt" && allowDynamicReading==true){
                int tempPointer = pointer;
                string line;
                while (tempPointer < lines.Length){
                    line = lines[tempPointer];
                    tempPointer++;
                    string typeOfLine = HasKeyWord(line);
                    if (typeOfLine!=null){
                        return false;
                    }
                }

                return true;
            } else {
                return base.IsEnd();
            }
        }

        public override string[] GetLines(){
            return lines;
        }

        public override void SetPointer(int i){
            pointer=i;
        }

    }

    /*
    1. get text to appear on screen + lower alpha of previous entries + add blinking marker at end
    2. log system
    3. choice system
    4. hide button 
    */

}