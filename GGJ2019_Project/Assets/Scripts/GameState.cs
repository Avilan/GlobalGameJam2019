using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class GameState {

	public enum ItemLocation {
		INDOORS,
		OUTDOORS
	}

	public struct ItemState {

		public ItemLocation itemLocation;
		public bool hasCustomPosition;
		public Vector3 customPosition;

		public ItemState (ItemLocation location, bool hasPos, Vector3 pos) {
			this.itemLocation = location;
			this.hasCustomPosition = hasPos;
			this.customPosition = pos;
		}

	}

	static Dictionary<ItemType, ItemState> map;

	static GameState () {
		map = new Dictionary<ItemType, ItemState>();
		foreach(ItemType type in System.Enum.GetValues(typeof(ItemType))){
			map.Add(type, new ItemState(ItemLocation.OUTDOORS, false, Vector3.zero));
		}
	}

	public static ItemState GetStateForItem (ItemType itemType) {
		map.TryGetValue(itemType, out ItemState state);
		return state;
	}

	public static void SetStateForItem (ItemType itemType, ItemState newItemState) {
		map.Remove(itemType);
		map.Add(itemType, newItemState);
	}
	
}
