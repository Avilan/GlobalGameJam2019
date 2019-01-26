using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeTrigger : MonoBehaviour {

	[SerializeField] string sceneToLoad;

    void OnTriggerEnter (Collider otherCollider) {
		if(otherCollider.CompareTag("Player")){
			otherCollider.GetComponent<PlayerController>().SaveBeforeLevelChange();
			SceneManager.LoadScene(sceneToLoad);
		}
    }
	
}
