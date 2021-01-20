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
    [Range(0, 90)]
    public float maxGroundAngle = 25;
    float minGroundDotProduct;
    private ControlMap controlMap;
    private Vector2 movement => controlMap.Player.Move.ReadValue<Vector2>();
    private Vector2 playerInput;
    //private Vector3 v = Vector3.zero;
    private Rigidbody body;
    private bool desireToJump;
    public bool onGround;
    private int jumpPhase;
    private Vector3 velocity;
    private Vector3 contactNormal = Vector3.up;
    private Vector3 pos {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
    private void Start() {
        OnValidate();
        body = GetComponent<Rigidbody>();
        controlMap = new ControlMap();
        controlMap.Enable();
        controlMap.Player.Jump.started += ctx => desireToJump = true;
    }

    private void OnValidate(){
        minGroundDotProduct = Mathf.Cos(maxGroundAngle);
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

            if(n.y >= minGroundDotProduct){
                onGround = true;
                contactNormal = n;
            }            
        }
    }

    private void FixedUpdate(){
        // var vt = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;
        // float accel = onGround ? maxAccel : maxAirAccel;
        // var da = accel * Time.deltaTime;

        // velocity = body.velocity;
        // velocity.x = Mathf.MoveTowards(velocity.x, vt.x, da);
        // velocity.z = Mathf.MoveTowards(velocity.z, vt.z, da);

        AdjustVelocity();

        if(desireToJump){
            Jump();
        }

        UpdateState();
    }

    private Vector3 ProjectOnContactPlane(Vector3 v){
        return v - contactNormal * Vector3.Dot(v, contactNormal);
    }

    private void AdjustVelocity(){
        var vt = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        velocity = body.velocity;
        float x = Vector3.Dot(velocity, xAxis);
        float z = Vector3.Dot(velocity, zAxis);

        float accel = onGround ? maxAccel : maxAirAccel;
        var da = accel * Time.deltaTime;

        var newX = Mathf.MoveTowards(x, vt.x, da);
        var newZ = Mathf.MoveTowards(z, vt.z, da);

        velocity += xAxis * (newX - x) + zAxis * (newZ - z);
    }

    private void UpdateState(){
        body.velocity = velocity;

        if(onGround)
            jumpPhase = 0;

        else
            contactNormal = Vector3.up;

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
