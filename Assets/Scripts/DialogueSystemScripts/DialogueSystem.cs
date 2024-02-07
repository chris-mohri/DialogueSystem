using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public int value;
    // public string chapter1;

    public TextAsset chapter1;

    private bool displayTextFinished=false;
    private bool chapterEnd=false;

    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        readFile();
    }

    private void readFile(){
        string jsonText = chapter1.text;

        DialogueDataWrapper wrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonText);

        // Process the dialogue entries
        foreach (DialogueEntry entry in wrapper.dialogueEntries)
        {
            Debug.Log("Name: " + entry.Name);
            Debug.Log("Dialogue: " + entry.Dialogue);
            Debug.Log("Commands: " + entry.Commands);
        }
    }

    [System.Serializable]
    public class DialogueDataWrapper
    {
        public DialogueEntry[] dialogueEntries;
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string Name;
        public string Dialogue;
        public string Commands;
    }

}
