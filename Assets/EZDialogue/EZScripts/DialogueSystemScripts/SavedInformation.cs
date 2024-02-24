using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedInformation : MonoBehaviour
{
    //data objects to hold important information/items
    public Dictionary<string, int> routeInfo; //contains route keys (just markers for which routes were taken)
    public Dictionary<string, int> inventory; //inventory items.

    void Start(){
        routeInfo = new Dictionary<string, int>();
        inventory = new Dictionary<string, int>();
    }

    //adds the item to the inventory
    public void AddItem(string item, int amount){
        if (inventory.ContainsKey(item)){
            inventory[item] += amount;
        } else {
            inventory[item] = amount;
        }
    }

    public bool HasItem(string item, int amount){
        if (inventory.ContainsKey(item)){
            if (inventory[item] >= amount){
                return true;
            }
        } 
        return false;
    }

    //adds the route key to the route dict
    public void AddRouteInfo(string routeKey, int amount){
        if (inventory.ContainsKey(routeKey)){
            inventory[routeKey] += amount;
        } else {
            inventory[routeKey] = amount;
        }
    }

    public bool RouteHas(string routeKey){
        if (inventory.ContainsKey(routeKey)){
            return true;
        } 
        return false;
    }

    //loads the save data from the given file
    public void LoadSave(string filename){
        //todo
    }

}
