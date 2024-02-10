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

    [SerializeField]
    private GameObject DialogueObject;
    private TMP_Text textObject;

    DialogueEntry[] currentChapterEntries;

    //ADD ALL CHAPTERS HERE
    public TextAsset chapter1;

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
        textObject = DialogueObject.GetComponent<TMP_Text>();
    }

    void Start(){
        currentChapterEntries = readFile(chapter1);
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

        textObject.maxVisibleCharacters=9;
        //Debug.Log(textObject.textInfo.lineCount);

    }

    //reads in a chapter file and returns a list of name/text/command entries
    private DialogueEntry[] readFile(TextAsset file){
        string jsonText = file.text;
        DialogueDataWrapper wrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonText);

        // Process the dialogue entries
        foreach (DialogueEntry entry in wrapper.dialogueEntries)
        {
            Debug.Log(entry.Name+": " + entry.Dialogue);
            Debug.Log(entry.VoiceFile+": " + entry.Commands);
        }
        return wrapper.dialogueEntries;
    }

    [System.Serializable]
    public class DialogueDataWrapper{
        public DialogueEntry[] dialogueEntries;
    }

    [System.Serializable]
    public class DialogueEntry{
        public string Name;
        public string Dialogue;
        public int Route;
        public string VoiceFile;
        public string Commands;
    }

}