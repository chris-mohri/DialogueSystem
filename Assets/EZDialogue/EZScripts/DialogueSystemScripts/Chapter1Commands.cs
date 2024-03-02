using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===========================================================================================
// ================================= CHAPTER 1 STORY SCRIPTS =================================
// ===========================================================================================

public partial class CommandsController {

    public IEnumerator chapt1_func1(){
        //ask for input
        List<string> options = new List<string>(){"\"Aoko, you speak too much\"", "Stay silent"};
        List<string> results = new List<string>(){"1a", "1b"};

        //display options and wait for input
        yield return StartCoroutine(DisplayAndWaitForChoices(options, results));

        //then perform these
        if (chosenOption == "1b"){
            save.AddRouteFlag("1b"); 
            Jump(".route 1b");
        } else {
            save.AddRouteFlag("1b");
            Jump(".label happy");
        }
    }


}