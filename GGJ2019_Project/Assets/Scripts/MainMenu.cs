using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] string gameStartScene;

    void Start () {
        Cursor.lockState = CursorLockMode.None;
        GameState.Reset();
    }

    void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }else if(Input.anyKeyDown){
            SceneManager.LoadScene(gameStartScene);
        }
    }
	
}
