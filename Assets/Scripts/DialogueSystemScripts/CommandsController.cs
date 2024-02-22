using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsController : MonoBehaviour
{

    string baseImgDirectory = "Assets/Images/";
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(baseImgDirectory);
    }

    // Update is called once per frame
    void Update()
    {
        
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
