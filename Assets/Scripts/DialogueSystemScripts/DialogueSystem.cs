using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public int value;
    // public string chapter1;

    public PlayerControls controls;

    public TextAsset chapter1;

    private bool displayTextFinished=false;
    private bool chapterEnd=false;

    DialogueEntry[] currentChapterEntries;

    void Awake(){
        //creates the player controls
        controls = new PlayerControls();
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
        public string VoiceFile;
        public string Commands;
    }

}
