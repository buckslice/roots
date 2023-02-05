using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        transform.LookAt(transform.parent.position);

    }

    float yaw = 0;
    float pitch = 0;

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButton(1)) {
            float sensitivity = 1.0f;
            float x = Input.GetAxis("Mouse X") * sensitivity;
            float y = Input.GetAxis("Mouse Y") * sensitivity;

            yaw += x;
            pitch += -y;

            transform.parent.parent.Rotate(Vector3.up, x, Space.Self);
            transform.parent.Rotate(Vector3.right, -y, Space.Self);

            //transform.parent.Rotate(Vector3.up, x);
            //transform.parent.Rotate(transform.right, -y);
            //transform.parent.rotation *= Quaternion.Euler(-y, 0, 0);

            // two parents, direct parent does pitch, then its parent does yaw
            //transform.parent.parent.rotation = Quaternion.Euler(0, yaw, 0);
            //transform.parent.rotation = Quaternion.Euler(pitch, 0, 0);

        }

        float scroll = Input.mouseScrollDelta.y;
        transform.position = transform.position + transform.forward * scroll;

        //
        //transform.parent.Rotate(Vector3.up, 10 * Time.deltaTime);


        //Vector3 move = Vector3.zero;

        //if (Input.GetKeyDown(KeyCode.W)) {
        //    move += transform.forward;
        //}


    }
}

