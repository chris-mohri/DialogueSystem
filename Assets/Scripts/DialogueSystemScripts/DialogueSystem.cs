using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using TMPro;
using System.Text.RegularExpressions;

public class DialogueSystem : MonoBehaviour
{

    //controls for the dialogue system
    public PlayerControls controls;

    // ------------------- display variables -------------------
    //true if the letters of the current entry are still being displayed 1 by 1, false if finished
    private bool stillDisplaying=false;

    //the dialogue object that has the text component
    [SerializeField]
    private GameObject DialogueObject;
    //the text component to the DialogueObject
    private TMP_Text textObj;

    // ------------------- text display/animation variables -------------------
    //indentation spaces for new lines. adjust as needed
    private string newLineSpace = "  ";
    //max number of lines that the text can occupy on screen
    //search by text.textInfo.lineCount
    [SerializeField] [Tooltip("Maximum number of lines the text can occupy before moving to next page")] [Range(1, 1000)]
    private int maxLines = 13;
    //tracks the index of the newest letter shown on screen from the raw text
    //  (raw text also counts any of TMPro's alpha or color tags that don't show up
    //  when using textObj.textInfo.characterCount)
    private int currentCharIndex=0;
    //the increment amount for the alpha (opacity) of letters
    [SerializeField] [Tooltip("Increment amount (percent) for the fade-in effect of letters (default: 5)")] [Range(1, 100)]
    private int alphaIncrement = 5;
    //starting alpha value of the newest letter that is displayed (keep at 10 since
    //  since the length must stay as 11)
    private string aTag1 = "<alpha=#10><link=fade></link>"; //11 length
    //dim tag (previously displayed sentences are dimmed)
    private string dimTag = "<color=#aaaaaa><link=dim></link><alpha=#b8><link=dim></link>";
    private string undimTag = "<color=#ffffff><link=undim></link><alpha=#ff><link=undim></link>";
    private int dimTagLength;
    //set as 11
    private int aTagLength;
    //keeps track of the un-dim tags. 
    private int undimTagIndex=-1;

    // ------------------- keeps track of time -------------------
    private double currentTime = 0.0f;
    private double displayTimer = 0.0f;
    [SerializeField] [Tooltip("Time (in seconds) to display the next letter (default: 0.03)")] [Range(0.001f, 10f)]
    private double displaySpeed = 0.03f; //adjust as needed 
    private double fadeTimer = 0.0f;
    [SerializeField] [Tooltip("Time (in seconds) to increment the opacity of letters (default: 0.004)")] [Range(0.001f, 10f)]
    private double fadeSpeed = 0.004f; //adjust as needed
    [SerializeField] [Tooltip("(default: Assets/Dialogue/) File Path for Dialogue files. Make sure to also have subfolders here called 'JSONs' and 'TSVs'")]
    private string dialogueFolderPath = "Assets/Dialogue/";
    [SerializeField] [Tooltip("(default: 1) Default Route name in the story")]
    private static string defaultRouteName = "1";

    //the main data object that holds the dialogue information
    private Book book;
    // private string endName = ".END";

    //================== CHAPTERS ===========================================================
    //ADD ALL CHAPTERS HERE (in the editor, place the .jsons here: chapter1.json should be placed here)
    public TextAsset chapter1;

    //=======================================================================================

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
        //gets the text component from the DialogueObject
        textObj = DialogueObject.GetComponent<TMP_Text>();
        textObj.maxVisibleCharacters=0;
        aTagLength = aTag1.Length;
        dimTagLength = dimTag.Length;
    }

    void Start(){
        //creates the book
        book = new Book();
        book.LoadChapter(chapter1);

        //make sure the counters are counted correctly at the start
        textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
        currentCharIndex = textObj.text.Length;

        //DEV TOOLS
        //batchConvertTSVtoJSON();
        Textbook tbook = new Textbook();
        tbook.LoadChapter("Assets/Text(deprecated)/chapter1.txt");
        tbook.ParseAllEntries();
        
    }

    private void batchConvertTSVtoJSON(){
        convertTSVtoJSON("chapter1.tsv", "chapter1.json");
    }

    private void batchConvertJSONtoTSV(){
        ConvertJSONToTSV("chapter1.json", "chapter1.tsv");
    }

    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
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
        //if the player presses continue
        if (controls.Keyboard.Continue.triggered){
            //if still displaying the previous entry when clicked, set maxVisibleCharacters to max
            if (textObj.maxVisibleCharacters < textObj.textInfo.characterCount) {
                //set all letters to alpha/opacity to 100% (by removing the alpha tags)
                for (int index = textObj.text.Length-1; index>=0; index--){
                    //if there's enough room for an alpha tag
                    if (index+aTagLength<=textObj.text.Length-1) {
                        //if there is indeed an alpha tag here and it is a fade tag
                        if (textObj.text.Substring(index, 6)=="<alpha" && GetTagId(index)=="fade"){
                        // if (GetTag(index)=="alpha" && GetTagId(index)=="fade"){
                            textObj.text = textObj.text.Remove(index, aTagLength);
                        }
                    }
                }
                textObj.maxVisibleCharacters = textObj.textInfo.characterCount;
                currentCharIndex = textObj.text.Length;

            } else if (false){ //if a command's animation is still playing, then skip to the end result
                //TODO
            } 
            else { //otherwise start displaying the next entry
                //if we haven't reached the end yet, then continue displaying
                if (!book.IsEnd()){
                    DialogueEntry currentEntry = book.GetCurrentEntryAndIncrement();
                    string text = currentEntry.dialogue;
                    text = postprocessText(text);

                    //add the text to the textObj 
                    textObj.text += text;
                    textObj.ForceMeshUpdate();
                    // Debug.Log(textObj.textInfo.lineCount);

                    // ===================== DIM TAGS =============================
                    // if the text doesn't have the dim tags yet, add them 
                    if (textObj.text.Length>=dimTagLength){
                        if (textObj.text.Substring(0, dimTagLength)!=dimTag){
                            AddTag(0, dimTag);
                        }
                    } else {
                        AddTag(0, dimTag);
                    }

                    // remove the old undim tag and place the new undim tag =====
                    if (undimTagIndex!=-1){
                        RemoveTag(undimTagIndex, undimTag);
                    }
                    undimTagIndex=currentCharIndex;
                    AddTag(currentCharIndex, undimTag);
                    // ===========================================================

                    // check if lines have exceeded maxLines     =================
                    if (textObj.textInfo.lineCount>=maxLines){
                        //clear current text and set as new text
                        textObj.text = text;
                        textObj.ForceMeshUpdate();
                        if (textObj.textInfo.lineCount>=maxLines){
                            textObj.text = "Current line is too big to display on the screen. Consider making it smaller.";
                            textObj.ForceMeshUpdate();
                        }
                        textObj.maxVisibleCharacters = 0;
                        currentCharIndex = 0;
                        undimTagIndex=-1;
                    }

                    // =========================================================== 
                    stillDisplaying=true;

                } else {
                    Debug.Log("Chapter Ended");
                    Debug.Log("Is still displaying? "+stillDisplaying);
                }

            }

        }
        // Debug.Log(textObj.text);
        // Debug.Log(textObj.textInfo.characterCount);
        // Debug.Log(textObj.maxVisibleCharacters);
        // Debug.Log(currentCharIndex);

        //display the letters
        AddLettersToScreen();
    }

    //adds newline indentation or automatically adds a space between sentences
    private string postprocessText(string text){
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
        stillDisplaying=false;
        int currentTextLength = textObj.text.Length;

        //in a faster timer, update every alpha tag in the list, removing tags that reach 100%
        if (fadeTimer >= fadeSpeed){
            int toRemove = -1;
            //loop through the text to find all the alpha tags
            for (int index = 0; index <= currentTextLength-1; index++){
                //if there's enough letters to possibly house another alpha tag, then continue
                if (index+aTagLength<=currentTextLength-1) {
                    //if there is an alpha tag here and it is a fade tag
                    if (textObj.text.Substring(index, 6)=="<alpha" && GetTagId(index)=="fade"){
                    // if (GetTag(index)=="alpha" && GetTagId(index)=="fade"){
                        string oldTag = textObj.text.Substring(index, aTagLength);
                        //get the new tag e.g. <alpha=#99>
                        string hex = oldTag.Substring(8, 2);
                        string newHex = AddPercentToHex(hex, alphaIncrement);
                        string newTag = $"<alpha=#{newHex}><link=fade></link>";
                        //mark for removal
                        if (newHex=="FF"){
                            toRemove = index;
                            newHex="00";
                        }
                        //replace the old tag with the new tag
                        textObj.text = textObj.text.Replace(oldTag, newTag);
                    }
                    
                }
            }
            //remove the 100% alpha tag (since it is useless)
            if (toRemove != -1){
                RemoveTag(toRemove, "<alpha=#00><link=fade></link>");
            }

            fadeTimer = 0.0f;
        }

        //display the next letter at the routine time
        if (displayTimer >= displaySpeed){
            //if there are more letters to fade in
            if (textObj.maxVisibleCharacters<textObj.textInfo.characterCount){     
                stillDisplaying=true;

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

    //gets the id for the tag at the index
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
            //             if (textObj.text.Substring(i, 6)=="<alpha" && GetTagId(i)=="fade"){
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
        if (index>=textObj.text.Length){
            Debug.Log("Invalid Index When Removing Tag");
            return;
        }
        textObj.text = textObj.text.Remove(index, tag.Length);
        currentCharIndex-=tag.Length;        
    }

    public void convertTSVtoJSON(string tsvName, string jsonName){
        Debug.Log("Attempting to convert "+tsvName);

        string tsvFilePath = dialogueFolderPath+"TSVs/"+tsvName;
        string jsonFilePath = dialogueFolderPath+"JSONs/"+jsonName;
        string[] lines = File.ReadAllLines(tsvFilePath);

        DialogueWrapper dialogueData = new DialogueWrapper();
        
        dialogueData.dialogueEntries = new List<DialogueEntry>();

        int attributes=1;
        foreach (string line in lines)
        {
            //ignore the 1st line
            if (attributes==1){
                attributes--;
                continue;
            }

            string[] parts = line.Split('\t'); // Assuming tab-separated values

            if (parts.Length >= 5)
            {
                var dialogueEntry = new DialogueEntry();
                dialogueEntry.route = parts[0];
                dialogueEntry.name = parts[1];

                // Preserve newline characters and escape quotes in the dialogue
                dialogueEntry.dialogue = parts[2].Replace("\\n", "\n");

                dialogueEntry.commands = parts[3];
                dialogueEntry.voice = parts[4];

                dialogueData.dialogueEntries.Add(dialogueEntry);
            }
        }

        string json = JsonUtility.ToJson(dialogueData, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log(tsvName+ " successfully converted to " + jsonName);
    }

    public void ConvertJSONToTSV(string jsonName, string tsvName)
    {
        Debug.Log("Attempting to convert "+tsvName);
        string tsvFilePath = dialogueFolderPath+"TSVs/"+tsvName;
        string jsonFilePath = dialogueFolderPath+"JSONs/"+jsonName;

        string json = File.ReadAllText(jsonFilePath);
        DialogueWrapper dialogueData = JsonUtility.FromJson<DialogueWrapper>(json);

        using (StreamWriter writer = new StreamWriter(tsvFilePath))
        {
            writer.WriteLine("route\tname\tdialogue\tcommands\tvoice");

            foreach (DialogueEntry entry in dialogueData.dialogueEntries)
            {
                string dialogue = entry.dialogue.Replace("\n", "\\n");
                //tab-separate it
                writer.WriteLine($"{entry.route}\t{entry.name}\t{dialogue}\t{entry.commands}\t{entry.voice}");
            }
        }

        Debug.Log(jsonName + " successfully converted to "+tsvName);
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

        //example 
        /*
        .swap Aoko Aoko_happy
            swaps the sprite of the gameObject with the name "Aoko" to "Aoko_happy"
        .move Aoko left 100 2
            moves gameObject with the name "Aoko" to the left 100 pixels within 2 seconds
        */
        public DialogueEntry(){
            route="";
            name="";
            dialogue="";
            commands="";
            voice="";
        }
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
        private string defaultRoute = defaultRouteName;

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
            //     Debug.Log(entry.voice+": " + entry.commands);
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
            currentRoute = currentChapter[defaultRoute];
        }

        //moves the current route to another route or another entry (part of the route)
        public void JumpTo(string route, int part=0){
            currentRoute = currentChapter[route];
            bookmark=part;
        }

        public List<DialogueEntry> GetCurrentRoute(){
            return currentRoute;
        }

        public DialogueEntry GetCurrentEntry(){
            return currentRoute[bookmark];
        }

        public DialogueEntry GetCurrentEntryAndIncrement(){
            DialogueEntry entry;
            if (IsEnd()){
                entry = new DialogueEntry();
                entry.dialogue = "REACHED END OF ROUTE";
                Debug.Log("Reached the End");
                return entry;
            }

            entry = currentRoute[bookmark];
            bookmark++;

            return entry;
        }

        public DialogueEntry GetEntry(int i){
            return currentRoute[i];
        }

        public void SetBookmark(int num){
            bookmark = num;
        }
        public int GetBookmark(){
            return bookmark;
        }

        public bool IsEnd(){
            if (bookmark >= currentRoute.Count){
                Debug.Log("reached the end of the route");
                return true;
            }
            return false;
            // return GetCurrentEntry().name==endName;
        }

    }

    [System.Serializable]
    public class Textbook: Book{
        //the container for all the entries in the loaded chapter. Each route corresponds to a list of entries
        private Dictionary<string, List<DialogueEntry>> currentChapter;
        //the list of entries of the current route
        private List<DialogueEntry> currentRoute;
        //saves the index for the current dialogueEntry in currentRoute
        private int bookmark=0;
        //marker that marks the end of the chapter (must be set as the name in a dialogue entry)
        private string endName = ".END";
        private string defaultRoute = defaultRouteName;

        //for dynamically accessing the text file
        private string pointerRoute = null;
        private int pointer=0;
        string[] lines;
        string filepath;

        public Textbook(){
            currentChapter = new Dictionary<string, List<DialogueEntry>>();
        }

        //reads in a chapter file and returns a list of name/text/.../command entries
        public void LoadChapter(string path){
            filepath = path;
            RefreshLines();
            pointer = 0;
        }

        public void RefreshLines(){
            lines = File.ReadAllLines(filepath);
        }

        //parses all entries and loads into the dictionary
        public void ParseAllEntries(){
            List<DialogueEntry> list = new List<DialogueEntry>();
            string currentRoute = "1";

            DialogueEntry entry = new DialogueEntry();

            int insurance = 100000;
            while (entry != null){
                entry = ParseNextEntry();
                if (entry != null){
                    list.Add(entry);
                }

                insurance--;
                if (insurance==0){
                    Debug.Log("hit insurance");
                    break;
                }
            }
            DialogueWrapper dw = new DialogueWrapper();
            dw.dialogueEntries = list;

            foreach(DialogueEntry e in dw.dialogueEntries){
                Debug.Log("Route: "+e.route);
                Debug.Log("Name: "+e.name);
                Debug.Log("Dialog: "+e.dialogue);
                Debug.Log("Commands: "+e.commands);
                Debug.Log("Voice: "+e.voice);
                Debug.Log("");
            }

            LoadIntoDictionary(dw);
        }

        //gets the next entry block from the text file
        public DialogueEntry ParseNextEntry(){
            //refresh lines so you can dynamically change the file while in game
            RefreshLines();

            DialogueEntry entry = new DialogueEntry();
            string line;
            bool blockFound = false;
            int insurance = 1000000;

            string[] nameAndDialogue = null;
            string commands = null;
            string voice = null;

            //loop through the lines until you find a dialogueEntry block
            while(pointer < lines.Length){
                line = lines[pointer].Trim(); //trim the line
                pointer++; //move pointer
                insurance--; //make sure no infinite loop happens
                if (insurance==0){
                    Debug.Log("hit insurance 2");
                    break;
                }

                //continue if haven't found block yet, return if found end of block
                if (line.Length==0){
                    if (blockFound){
                        return entry;
                    } else {
                        continue;
                    }
                }
                if (IsCommentLine(line)) continue;

                string typeOfLine = HasKeyWord(line);
                if (HasKeyWord(line)!=null){
                    if (typeOfLine==".route"){
                        pointerRoute = ParseRoute(line, pointerRoute);
                        entry.route = pointerRoute;
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
                    } else {
                        Debug.Log("WEIRD THING HAPPENED IN PARSENEXTENTRY() FUNCTION");
                    }
                }
                // DialogueEntry entry = new DialogueEntry();
            }

            return null;
        }

        public bool IsCommentLine(string line){
            if (line.Length>=1){
                if (line[0]=='#'){
                    return true;
                }
            }
            return false;
        }

        //verifies that if there's a keyword, then it must be formatted correctly (space after it)
        public string HasKeyWord(string line){
            string[] list = new string[4];
            list[0]=".route";
            list[1]=".say";
            list[2]=".func";
            list[3]=".voice";

            int index = -1;
            //if there is indeed a dot
            foreach (string command in list){
                index = line.IndexOf(command);

                //if it was found 
                if (index!=-1){
                    return command;
                }
                
                //verify that there are not multiple of them
                // int indexOther = line.Substring(index+command.Length).IndexOf(command);
                // if (indexOther!=-1){
                //     Debug.Log()
                // }             
            }

            return null;
        }

        //parse as dialogue
        public string[] ParseDialogue(string line, string[] nameAndDialogue){
            string[] list = new string[2];
            string type = ".say";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (nameAndDialogue!=null){
                    Debug.Log("Multiple .say lines in same block on line "+pointer);
                    //throw exception?
                }

                list[0] = line.Substring(0, loc).Trim();
                list[1] = line.Substring(loc + type.Length).Trim();
                return list;
            }
            return nameAndDialogue;
        }

        public string ParseVoice(string line, string oldVoice){
            string voice = oldVoice;
            string type = ".voice";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (oldVoice!=null){
                    Debug.Log("Multiple .voice lines in same block on line "+pointer);
                }
                voice = line.Substring(loc + type.Length).Trim();
            }
            return voice;
        }
        public string ParseCommandsBasic(string line, string oldCommands){
            string commands = oldCommands;
            string type = ".func";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (oldCommands!=null){
                    Debug.Log("Multiple .func lines in same block on line "+pointer);
                }

                commands = line.Substring(loc + type.Length).Trim();
            }
            return commands;
        }
        public string ParseRoute(string line, string oldRoute){
            string route = null;
            string type = ".route";
            int loc = line.IndexOf(type);
            if (loc!=-1){
                if (oldRoute!=null){
                    Debug.Log("Multiple .route lines in same block on line "+pointer);
                }
                route = line.Substring(loc + type.Length).Trim();
            } 
            return route;
        }
    }

    /*
    1. get text to appear on screen + lower alpha of previous entries + add blinking marker at end
    2. log system
    3. choice system
    4. hide button 
    */

}