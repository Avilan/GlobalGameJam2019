using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorsSceneManager : MonoBehaviour {

	WorldItem[] worldItems;

	void Awake () {
		worldItems = GameObject.FindObjectsOfType<WorldItem>();
		List<ItemType> existingTypes = new List<ItemType>();
		for(int i=0; i<worldItems.Length; i++){
			WorldItem item = worldItems[i];
			if(existingTypes.Contains(item.type)){
				throw new UnityException("dude, only one item per type allowed!!!");
			}else{
				existingTypes.Add(item.type);
				GameState.ItemState state = GameState.GetStateForItem(item.type);
				if(state.itemLocation.Equals(GameState.ItemLocation.OUTDOORS)){
					item.gameObject.SetActive(true);
					if(state.hasCustomPosition){
						item.transform.position = state.customPosition;
					}
				}else{
					item.gameObject.SetActive(false);
				}
			}
		}
	}
	
}
