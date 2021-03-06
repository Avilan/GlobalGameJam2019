﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstPersonPlayerController : PlayerController {

	[Header("Components")]
	[SerializeField] CharacterController cc;
	[SerializeField] Camera cam;
	[SerializeField] Transform heldItemParent;
	[SerializeField] Canvas guiCanvas;
	[SerializeField] Image stressLevelImage;

	[Header("Settings")]
	[SerializeField] float slopeLimit;
	[SerializeField] float walkSpeed;
	[SerializeField] float speedWhenHoldingAnItem;
	[SerializeField] float sprintSpeed;
	[SerializeField] float jumpHeight;
	[SerializeField] float interactRange;
	[SerializeField] float heldItemScale;
	[SerializeField] float itemBobStrength;
	[SerializeField] float itemBobSpeed;
	[SerializeField] float minutesToFullStress;
	[SerializeField] string mainMenuSceneName;

	List<ControllerColliderHit> hits;
	Vector3 externalVelocity;
	bool jumped;
	WorldItem heldItem;
	Vector3 originalHeldItemParentPosition;
	float itemBob;

    protected override void Start () {
        base.Start();
        cc.slopeLimit = 90f;
        hits = new List<ControllerColliderHit>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if(mr != null) mr.enabled = false;
        originalHeldItemParentPosition = heldItemParent.localPosition;
        itemBob = 0f;
        guiCanvas.planeDistance = cam.nearClipPlane + 0.01f;
    }

    void OnControllerColliderHit (ControllerColliderHit hit) {
	    hits.Add(hit);
    }

    protected override void ExecuteUpdate (bool hasFocus, bool pauseMenuOpen) {
	    bool readInput = (hasFocus && !PauseMenu.Instance.IsOpen);
	    Vector2 mouseInput = (readInput ? GetMouseInput() : Vector2.zero);
	    Vector3 dirInput = (readInput ? GetDirInput() : Vector3.zero);
	    if(readInput){
			if(Input.GetKeyDown(keybinds.keyInteract)){
				if(heldItem == null){
					if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange)){
						WorldItem hitItem = hit.collider.gameObject.GetComponent<WorldItem>();
						if(hitItem != null){
							hitItem.transform.parent = heldItemParent;
							hitItem.transform.localPosition = Vector3.zero;
							hitItem.transform.localScale *= heldItemScale;
							hitItem.PickUp();
							heldItem = hitItem;
                            IsHoldingItem = true;
                            HeldItemType = heldItem.type;
                        }
					}
				}else{
					heldItem.transform.localScale /= heldItemScale;
					heldItem.transform.parent = null;
					heldItem.DropToGround();
					heldItem = null;
                    IsHoldingItem = false;
                }

			}
			if(Input.GetKeyDown(KeyCode.Mouse0)) Cursor.lockState = CursorLockMode.Locked;
			#if UNITY_EDITOR
				if(Input.GetKeyDown(KeyCode.Mouse1)){
				    hits.Clear();
					externalVelocity += cam.transform.forward * 50f;
				}
		    #endif
	    }
	    if (hasFocus) {
		    Look(mouseInput);
	    }
	    Move(dirInput);
    }

    void Look (Vector2 mouseInput) {
	    mouseInput *= mouseSensitivity;
	    cc.transform.Rotate(new Vector3(0f, mouseInput.x, 0f));
	    cam.transform.Rotate(new Vector3(-mouseInput.y, 0f, 0f));
	    if (Vector3.Scale(cam.transform.localEulerAngles, new Vector3(0f, 1f, 1f)) != Vector3.zero) {
		    if (cam.transform.localEulerAngles.x > 180f) {
			    cam.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
		    }else {
			    cam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		    }
	    }
    }

    void Move (Vector3 dirInput) {
	    bool isGrounded = (GetIsGrounded(hits, out Vector3 groundNormal) && !jumped);
	    jumped = false;
	    Vector3 desiredVelocity = GetSurfaceMoveVector(cc.transform.TransformDirection(dirInput), groundNormal)* dirInput.magnitude;
	    float desiredSpeed;
	    if(heldItem == null){
		    desiredSpeed = (Input.GetKey(keybinds.keySprint) ? sprintSpeed : walkSpeed);
	    }else{
			desiredSpeed = speedWhenHoldingAnItem;
	    }
	    desiredVelocity *= desiredSpeed;
	    if(isGrounded){
		    if(Vector3.Angle(groundNormal, Vector3.up) < slopeLimit){
			    externalVelocity = Vector3.zero;
			    UpdateItemBobbing(desiredVelocity.magnitude * Time.deltaTime * itemBobSpeed / walkSpeed);
			    if(Input.GetKey(keybinds.keyJump)){
				    desiredVelocity.y = 0f;
				    externalVelocity = Vector3.up * Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);
				    jumped = true;
			    }
		    }else{
				desiredVelocity = Vector3.zero;
				externalVelocity = Vector3.ProjectOnPlane(externalVelocity, groundNormal);
				externalVelocity += Vector3.ProjectOnPlane(Physics.gravity, groundNormal) * Time.deltaTime;
		    }
	    }
	    externalVelocity += Physics.gravity * Time.deltaTime;
	    hits.Clear();
	    cc.Move((desiredVelocity + externalVelocity)* Time.deltaTime);
    }

    void UpdateItemBobbing (float delta) {
		itemBob += delta;
		heldItemParent.transform.localPosition = originalHeldItemParentPosition + new Vector3(0f, Mathf.Sin(Mathf.PI * itemBob) * itemBobStrength, 0f);
    }

    protected override void ManageStressLevel() {
	    float secondsToFullStress = minutesToFullStress * 60;
	    float stressPerSecond = 1f / secondsToFullStress;
	    StressLevel += stressPerSecond * Time.deltaTime;
	    stressLevelImage.color = new Color(1f, 1f, 1f, StressLevel);
	    if(StressLevel >= 1f){
			Debug.Log("YOU'RE DEAD");
			SceneManager.LoadScene(mainMenuSceneName);
	    }
    }

    public override void SaveBeforeLevelChange () {
		if(heldItem != null){
			GameState.SetStateForItem(heldItem.type, new GameState.ItemState(GameState.ItemLocation.INDOORS, false, Vector3.zero, Vector2.zero));
		}
    }

    Vector3 GetDirInput () {
	    Vector3 output = Vector3.zero;
	    if (Input.GetKey(keybinds.keyForward)) output += Vector3.forward;
	    if (Input.GetKey(keybinds.keyBack)) output += Vector3.back;
	    if (Input.GetKey(keybinds.keyLeft)) output += Vector3.left;
	    if (Input.GetKey(keybinds.keyRight)) output += Vector3.right;
	    if(output.sqrMagnitude > 1f) output = output.normalized;
	    return output;
    }

    Vector2 GetMouseInput () {
	    Vector2 output = Vector2.zero;
	    output.x = Input.GetAxisRaw("Mouse X");
	    output.y = Input.GetAxisRaw("Mouse Y");
	    return output;
    }

    Vector3 GetSurfaceMoveVector(Vector3 inputVector, Vector3 inputNormal){
	    inputVector = inputVector.normalized;
	    inputNormal = inputNormal.normalized;
	    float ix = inputVector.x;
	    float iz = inputVector.z;
	    float nx = inputNormal.x;
	    float ny = inputNormal.y;
	    float nz = inputNormal.z;
	    float deltaY = -((ix * nx) + (iz * nz)) / ny;
	    return new Vector3(ix, deltaY, iz).normalized;
    }

    bool GetIsGrounded (List<ControllerColliderHit> hits, out Vector3 groundNormal) {
	    bool grounded;
	    groundNormal = Vector3.up;
	    float biggestDot = 0f;
	    foreach(ControllerColliderHit hit in hits){
		    float dot = Vector3.Dot(hit.normal, Vector3.up);
		    if(dot > biggestDot){
			    biggestDot = dot;
			    groundNormal = hit.normal;
		    }
	    }
	    grounded = (biggestDot > 0f);
	    return grounded;
    }
	
}
