using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    void Start () {
        PlayerPrefs.DeleteAll();
    }

    void Update () {
        if(Input.anyKeyDown){
            //TODO load game scene
        }
    }
	
}
