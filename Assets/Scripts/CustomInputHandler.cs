using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomInputHandler : MonoBehaviour {
    private ControlMap controlMap;
    public Vector2 moveVector;

    private void Start() {
        controlMap = new ControlMap();
        controlMap.Enable();

        controlMap.Player.Jump.performed += ctx => print("pogo pogo pogo");
    }

    private void Update(){
        moveVector = controlMap.Player.Move.ReadValue<Vector2>();
    }
}
