using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Components")]
	[SerializeField] CharacterController cc;
	[SerializeField] Camera cam;

	[Header("Keybinds")]
	[SerializeField] KeyCode keyFwd;
	[SerializeField] KeyCode keyBwd;
	[SerializeField] KeyCode keyLeft;
	[SerializeField] KeyCode keyRight;
	[SerializeField] KeyCode keyJump;
	[SerializeField] KeyCode keySprint;

	[Header("Settings")]
	[SerializeField] float walkSpeed;
	[SerializeField] float sprintSpeed;
	[SerializeField] float jumpHeight;
	[SerializeField] float mouseSensitivity;

	List<ControllerColliderHit> hits;
	bool hasFocus;
	bool jumped;
	float verticalVelocity;

    void Start () {
	    Cursor.lockState = CursorLockMode.Locked;
	    hits = new List<ControllerColliderHit>();
	    MeshRenderer mr = GetComponent<MeshRenderer>();
	    if(mr != null) mr.enabled = false;
    }

    void Update () {
	    if(Input.GetKeyDown(KeyCode.Mouse0)) Cursor.lockState = CursorLockMode.Locked;
	    Vector2 mouseInput = GetMouseInput();
	    Vector3 dirInput = (hasFocus ? GetDirInput() : Vector3.zero);
	    if (hasFocus) {
		    Look(mouseInput);
	    }
		Move(dirInput);
    }

    void OnApplicationFocus (bool hasFocus) {
	    this.hasFocus = hasFocus;
    }

    void OnControllerColliderHit (ControllerColliderHit hit) {
		hits.Add(hit);
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
		desiredVelocity *= (Input.GetKey(keySprint) ? sprintSpeed : walkSpeed);
		if(isGrounded){
			verticalVelocity = 0f;
			if(Input.GetKey(keyJump)){
				desiredVelocity.y = 0f;
				verticalVelocity = Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);
				jumped = true;
			}
		}
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		desiredVelocity += Vector3.up * verticalVelocity;
		hits.Clear();
		cc.Move(desiredVelocity * Time.deltaTime);
    }

    Vector3 GetDirInput () {
	    Vector3 output = Vector3.zero;
	    if (Input.GetKey(keyFwd)) output += Vector3.forward;
	    if (Input.GetKey(keyBwd)) output += Vector3.back;
	    if (Input.GetKey(keyLeft)) output += Vector3.left;
	    if (Input.GetKey(keyRight)) output += Vector3.right;
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
