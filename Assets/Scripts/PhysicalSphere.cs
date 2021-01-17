using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalSphere : MonoBehaviour {
    [Range(0, 100)]
    public float maxSpeed = 10;
    [Range(0, 500)]
    public float maxAccel = 10;

    [Range(0, 500)]
    public float maxAirAccel = 1;

    [Range(0, 10)]
    public float jumpHeight = 2;
    [SerializeField, Range(0, 5)]
	int maxAirJumps = 2;
    private ControlMap controlMap;
    private Vector2 movement => controlMap.Player.Move.ReadValue<Vector2>();
    private Vector2 playerInput;
    //private Vector3 v = Vector3.zero;
    private Rigidbody body;
    private bool desireToJump;
    public bool onGround;
    private int jumpPhase;
    private Vector3 velocity;
    private Vector3 pos {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
    private void Start() {
        body = GetComponent<Rigidbody>();
        controlMap = new ControlMap();
        controlMap.Enable();
        controlMap.Player.Jump.started += ctx => desireToJump = true;
    }

    private void Update() {
        playerInput = movement;        
    }

    void OnCollisionEnter(Collision col) {
        EvaluateCollision(col);
    }

    void OnCollisionExit(Collision col) {
        EvaluateCollision(col);
    }

    private void EvaluateCollision(Collision col) {
        onGround = false;
        for (int i = 0; i < col.contactCount; i++) {
            var n = col.GetContact(i).normal;

            onGround |= n.y >= .9f;
        }
    }

    private void FixedUpdate(){
        var vt = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;
        float accel = onGround ? maxAccel : maxAirAccel;
        var da = accel * Time.deltaTime;

        velocity = body.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, vt.x, da);
        velocity.z = Mathf.MoveTowards(velocity.z, vt.z, da);

        if(desireToJump){
            Jump();
        }

        UpdateState();
    }

    private void UpdateState(){
        body.velocity = velocity;

        if(onGround)
            jumpPhase = 0;

        desireToJump = false;
    }

    private void Jump(){
        if(onGround || jumpPhase < maxAirJumps){
            jumpPhase++;
            desireToJump = false;
            var jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if(velocity.y > 0)
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0);

            velocity.y += jumpSpeed;
        }
        
    }
}
