using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCam : MonoBehaviour {
    class CameraState {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t) {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation) {
            x += translation.x;
            y += translation.y;
            z += translation.z;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct) {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t) {
            t.eulerAngles = new Vector3(-pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }

    CameraState targCamState = new CameraState();
    CameraState currCamState = new CameraState();

    void OnEnable() {
        targCamState.SetFromTransform(transform);
        currCamState.SetFromTransform(transform);
    }

    public float sensitivity = 5.0f;
    public float moveSpeed = 100.0f;

    float positionLerpTime = 0.25f;
    float rotationLerpTime = 0.05f;


    // Update is called once per frame
    void Update() {
        CameraControl();
    }

    void CameraControl() {
        int lookMouseButton = 2;

        // Hide and lock cursor when right mouse button pressed
        if (Input.GetMouseButtonDown(lookMouseButton)) {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetMouseButtonUp(lookMouseButton)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(lookMouseButton)) {
            targCamState.yaw += Input.GetAxis("Mouse X") * sensitivity;
            targCamState.pitch += Input.GetAxis("Mouse Y") * sensitivity;
        }
        targCamState.pitch = Mathf.Clamp(targCamState.pitch, -89, 89);

        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 move = forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");

        moveSpeed += Input.mouseScrollDelta.y;
        if (moveSpeed < 1) {
            moveSpeed = 1;
        }
        if (moveSpeed > 100) {
            moveSpeed = 100;
        }

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) {
            speed *= 5;
        }
        move = move.normalized * speed;

        float upDir = 0.0f;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Period)) {
            upDir += 1.0f;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Semicolon)) {
            upDir -= 1.0f;
        }
        move += upDir * Vector3.up * speed;

        targCamState.Translate(move * Time.deltaTime);

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        currCamState.LerpTowards(targCamState, positionLerpPct, rotationLerpPct);

        currCamState.UpdateTransform(transform);
    }

}
