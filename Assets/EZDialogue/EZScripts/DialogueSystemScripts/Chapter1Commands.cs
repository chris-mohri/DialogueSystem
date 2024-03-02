using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===========================================================================================
// ================================= CHAPTER 1 STORY SCRIPTS =================================
// ===========================================================================================

public partial class CommandsController {

    //chapter1_func1
    public IEnumerator c1_func1(){
        //ask for input
        List<string> options = new List<string>(){"\"Aoko, you speak too much\"", "Stay silent"};

        //display options and wait for input
        yield return StartCoroutine(DisplayAndWaitForChoices(options));
        
        //then perform these
        if (chosenOption == "option1"){
            save.AddRouteFlag("1b"); 
            Jump(".label happy");
        } else {
            save.AddRouteFlag("1b");
            Jump(".route 1b");
        }
    }


}