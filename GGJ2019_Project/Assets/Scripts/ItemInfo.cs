using UnityEngine;

[CreateAssetMenu(fileName = "New ItemInfo", menuName = "ItemInfo")]
public class ItemInfo : ScriptableObject {

    public Texture2D artwork;
	public ItemType type;
	
}
