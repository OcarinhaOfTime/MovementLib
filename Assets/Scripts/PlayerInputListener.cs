using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputListener : MonoBehaviour {
    public void Fire(InputAction.CallbackContext context) {
        print("Fire");
    }
}
