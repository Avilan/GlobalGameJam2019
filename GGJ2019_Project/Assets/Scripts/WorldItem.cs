using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour {

	public ItemType type => itemInfo.type;

	[SerializeField] ItemInfo itemInfo;
	[SerializeField] MeshRenderer mr;

	MaterialPropertyBlock mpb;
	bool held;

    void Start () {
        mpb = new MaterialPropertyBlock();
		mpb.SetTexture("_MainTex", itemInfo.artwork);
		DropToGround();
    }

    void Update () {
	    Camera mainCamera = Camera.main;
	    Vector3 toCamera = mainCamera.transform.position - transform.position;
	    transform.rotation = Quaternion.LookRotation(-toCamera, (held ? mainCamera.transform.up : Vector3.up));		//minus because the quad is "the wrong way around"
	    mr.SetPropertyBlock(mpb);
    }

    public void PickUp () {
		held = true;
    }

    //TODO only test for "ground geometry". currently dropping onto another item is possible...

    public void DropToGround () {
		if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit)){
			Debug.Log("snapping to \"" + hit.collider.gameObject.name + "\"");
			float scaleMax = Mathf.Max(transform.localScale.x, Mathf.Max(transform.localScale.y, transform.localScale.z));
			transform.position = hit.point + (Vector3.up * 0.5f * scaleMax);
		}else{
			//do nothing...
		}
		held = false;
    }
	
}
