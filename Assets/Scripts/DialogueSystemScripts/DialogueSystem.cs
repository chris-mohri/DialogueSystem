using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class DialogueSystem : MonoBehaviour
{

    public PlayerControls controls;
    private bool displayTextFinished=false;
    private bool chapterEnd=false;

    //the dialogue object that has the text component
    [SerializeField]
    private GameObject DialogueObject;
    //the text component to the DialogueObject
    private TMP_Text text;

    //number of lines that the text occupies on screen
    //search by text.textInfo.lineCount
    public int maxLines = 20;
    private int currentShownCharacter=0;
    private int currentTotalCharacters=0;

    //the main data object that holds the dialogue information
    private Book book;


    //================== CHAPTERS ===========================================================

    //ADD ALL CHAPTERS HERE (in the editor, place the .jsons here: chapter1.json should be placed here)
    public TextAsset chapter1;

    // =======================================================================================

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
        //gets the text component from the DialogueObject
        text = DialogueObject.GetComponent<TMP_Text>();
    }

    void Start(){
        //creates the book
        book = new Book();
        book.loadChapter(chapter1);

        Debug.Log(book.currentRoute[book.bookmark].name);

    }

    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }

    // Update is called once per frame
    void Update(){
        if (controls.Keyboard.Continue.triggered){
            Debug.Log("asdf");
        }

        text.maxVisibleCharacters=9;
        //Debug.Log(text.textInfo.lineCount);

    }

    [System.Serializable]
    public class DialogueWrapper{
        public List<DialogueEntry> dialogueEntries;
    }

    [System.Serializable]
    public class DialogueEntry{
        public string name;
        public string dialogue;
        public string route;
        public string voiceFile;
        public string commands;
    }

    [System.Serializable]
    public class Book{
        //the container for all the entries in the loaded chapter. Each route corresponds to a list of entries
        public Dictionary<string, List<DialogueEntry>> currentChapter;
        //the list of entries of the current route
        public List<DialogueEntry> currentRoute;
        public int bookmark=0;

        public Book(){
            currentChapter = new Dictionary<string, List<DialogueEntry>>();
        }

        //reads in a chapter file and returns a list of name/text/.../command entries
        public void loadChapter(TextAsset file){
            string jsonText = file.text;
            DialogueWrapper wrapper = JsonUtility.FromJson<DialogueWrapper>(jsonText);

            // Process the dialogue entries
            foreach (DialogueEntry entry in wrapper.dialogueEntries)
            {
                Debug.Log(entry.name+": " + entry.dialogue);
                Debug.Log(entry.voiceFile+": " + entry.commands);
            }
            currentRoute = wrapper.dialogueEntries;
        }

        //moves the current route to another route or another entry (part of the route)
        public void jumpTo(string route, int part=0){
            currentRoute = currentChapter[route];
            bookmark=part;
        }
    }

}