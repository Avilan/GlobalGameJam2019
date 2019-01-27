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
        public Vector2 indoorPosition;

        public ItemState (ItemLocation location, bool hasPos, Vector3 pos, Vector2 indoorPos) {
			this.itemLocation = location;
			this.hasCustomPosition = hasPos;
			this.customPosition = pos;
            this.indoorPosition = indoorPos;
        }

		public override string ToString() {
			return $"location: {itemLocation}\n" +
			       $"hascustomposition: {hasCustomPosition}\n" +
			       $"customposition: {customPosition}";
		}
	}

	public static bool isNewGame;

	static Dictionary<ItemType, ItemState> map;

	public static void Reset () {
		map.Clear();
		foreach(ItemType type in System.Enum.GetValues(typeof(ItemType))){
			map.Add(type, new ItemState(ItemLocation.OUTDOORS, false, Vector3.zero, Vector2.zero));
		}
		isNewGame = true;
	}

	static GameState () {
		map = new Dictionary<ItemType, ItemState>();
	}

	public static ItemState GetStateForItem (ItemType itemType) {
		if(map.TryGetValue(itemType, out ItemState state)){
			return state;
		}else{
			throw new UnityException("No state found for item type \"" + itemType.ToString() + "\"");
		}
	}

	public static void SetStateForItem (ItemType itemType, ItemState newItemState) {
		map.Remove(itemType);
		map.Add(itemType, newItemState);
	}
	
}
