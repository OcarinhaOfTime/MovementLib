using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTester : MonoBehaviour {
    private void FixedUpdate() {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        if (gamepad.rightTrigger.wasPressedThisFrame) {
            print("Right trigger");
        }
    }
}
