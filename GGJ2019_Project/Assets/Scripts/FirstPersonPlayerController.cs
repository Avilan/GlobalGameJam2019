using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FirstPersonPlayerController : PlayerController {

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
	[SerializeField] float slopeLimit;
	[SerializeField] float walkSpeed;
	[SerializeField] float sprintSpeed;
	[SerializeField] float jumpHeight;

	List<ControllerColliderHit> hits;
	Vector3 externalVelocity;
	bool jumped;

    protected override void Start () {
        base.Start();
        cc.slopeLimit = 90f;
        hits = new List<ControllerColliderHit>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if(mr != null) mr.enabled = false;
    }

    void OnControllerColliderHit (ControllerColliderHit hit) {
	    hits.Add(hit);
    }

    protected override void ExecuteUpdate (bool hasFocus, bool pauseMenuOpen) {
	    bool readInput = (hasFocus && !PauseMenu.Instance.IsOpen);
	    Vector2 mouseInput = (readInput ? GetMouseInput() : Vector2.zero);
	    Vector3 dirInput = (readInput ? GetDirInput() : Vector3.zero);
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
	    desiredVelocity *= (Input.GetKey(keySprint) ? sprintSpeed : walkSpeed);
	    if(isGrounded){
		    if(Vector3.Angle(groundNormal, Vector3.up) < slopeLimit){
			    externalVelocity = Vector3.zero;
			    if(Input.GetKey(keyJump)){
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

    Vector3 GetDirInput () {
	    Vector3 output = Vector3.zero;
	    if (Input.GetKey(keyFwd)) output += Vector3.forward;
	    if (Input.GetKey(keyBwd)) output += Vector3.back;
	    if (Input.GetKey(keyLeft)) output += Vector3.left;
	    if (Input.GetKey(keyRight)) output += Vector3.right;
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
