using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CommandsController : MonoBehaviour
{

    private string baseImgDirectory = "Assets/Images/";

    //data objects to hold important information/items
    public Dictionary<string, int> routeInfo;
    public Dictionary<string, int> inventory;

    //the dialogue system script (needed to access methods)
    private DialogueSystem ds;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(baseImgDirectory);
        ds = GetComponent<DialogueSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void alisterInvoke(string line){

    }

    //choices = what is displayed on screen
    //result = items to be added to inventory
    //jump = route to jump to 
    public void choose(List<string> choices, List<string> result, List<string> jump){

    }

    // INSERT CUSTOM SCRIPTS HERE 
    // MAKE THEM PUBLIC!!
    // e.g. public void customScriptName(input){ ... } 
}
