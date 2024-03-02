using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Chapter1Commands
{
    public static IEnumerator chap1_func2(){
        //ask for input
        // List<string> options = new List<string>(){"\"Aoko, you speak too much!\"", "Stay silent!"};
        // List<string> results = new List<string>(){"1a", "1b"};

        // yield return StartCoroutine(DisplayAndWaitForChoices(options, results));

        // //then perform these
        // if (chosenOption == "1b"){
        //     save.AddRouteFlag("1b"); 
        //     Jump(".route 1b");
        // } else {
        //     save.AddRouteFlag("1b");
        //     Jump(".label happy");
        // }
        Debug.Log("a;klasd");
        yield return new WaitForSeconds(0.2f);
    }


}