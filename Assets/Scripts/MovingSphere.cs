using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour {
    public Rect allowedArea = new Rect(0, 0, 10, 10);
    [Range(0, 100)]
    public float maxSpeed = 10;
    [Range(0, 100)]
    public float maxAccel = 10;

    [Range(0f, 1f)]
    public float bounciness = 0.5f;
    private ControlMap controlMap;
    private Vector2 movement => controlMap.Player.Move.ReadValue<Vector2>();
    private Vector3 v = Vector3.zero;
    private Vector3 pos {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
    private void Start() {
        controlMap = new ControlMap();
        controlMap.Enable();
    }

    private void Update() {
        var input = movement;   
        var vt = new Vector3(input.x, 0, input.y) * maxSpeed;
        var da = maxAccel * Time.deltaTime;

        v.x = Mathf.MoveTowards(v.x, vt.x, da);
        v.z = Mathf.MoveTowards(v.z, vt.z, da);

        var d = v * Time.deltaTime;
        var p = pos + d;
        
        // if(!allowedArea.Contains(new Vector2(p.x, p.z))){
        //     p.x = Mathf.Clamp(p.x, allowedArea.xMin, allowedArea.xMax);
        //     p.z = Mathf.Clamp(p.z, allowedArea.yMin, allowedArea.yMax);
        // }

        if(p.x < allowedArea.xMin){
            p.x = allowedArea.xMin;
            v.x = -v.x * bounciness;
        }

        else if(p.x > allowedArea.xMax){
            p.x = allowedArea.xMax;
            v.x = -v.x * bounciness;
        }

        if(p.z < allowedArea.yMin){
            p.z = allowedArea.yMin;
            v.z = -v.z * bounciness;
        }

        else if(p.z > allowedArea.yMax){
            p.z = allowedArea.yMax;
            v.z = -v.z * bounciness;
        }

        pos = p;
    }
}
