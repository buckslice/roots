using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class UnitControl : MonoBehaviour {

    List<NanoBot> selected = new List<NanoBot>();

    RaycastHit[] hitInfo = new RaycastHit[32];

    // Update is called once per frame
    void Update() {

        var id = NetworkManager.Singleton.LocalClientId;

        // left click select nanobot
        if (Input.GetMouseButtonDown(0)) {
            bool shift = Input.GetKey(KeyCode.LeftShift);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float radius = shift ? 2.0f : 0.1f;
            int count = Physics.SphereCastNonAlloc(ray.origin, radius, ray.direction, hitInfo, 1000, 1 << Layers.Unit);
            bool selectedAny = false;
            for (int i = 0; i < count; i++) {
                var hit = hitInfo[i];
                if (hit.collider.CompareTag(Tags.NanoBot)) {
                    var bot = hit.collider.GetComponentInParent<NanoBot>();
                    if (bot.OwnerClientId == id) { // player owns this bot?
                        if (!shift) {
                            selected.Clear();
                        }
                        selectedAny = true;
                        selected.Add(bot);
                        if (!shift) {
                            break;
                        }
                    }
                }
            }
            if (!selectedAny) { // clear selection if you click off
                selected.Clear();
            }

        }

        // right click tell them to move
        if (Input.GetMouseButtonDown(1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // check if should attack bug
            bool didTargetABug = false;
            if (Physics.SphereCast(ray, 1.0f, out var hitInfo, 1000, 1 << Layers.Unit)) {
                if (hitInfo.collider.CompareTag(Tags.Bug)) { // did we hit a bug?
                    // tell all selected bots to kill
                    didTargetABug = true;
                    var bug = hitInfo.collider.GetComponent<Bug>();
                    foreach (var bot in selected) {
                        bot.targetBug = bug;
                    }

                }
            }

            // check just to move somewhere
            if (!didTargetABug && Physics.Raycast(ray, out hitInfo, 1000, 1 << Layers.Scalp)) {
                foreach (var bot in selected) {
                    if (bot.OwnerClientId == id) {
                        var pos = hitInfo.point;
                        //Debug.Log($"Request Unit moves here: {pos}");
                        bot.MoveUnit_ServerRpc(pos);
                    }
                }
            }
        }

    }

    // Draws a line from "startVertex" var to the curent mouse position.
    public Material lineMat;

    float blinkTimer = 0.0f;

    void OnPostRender() {
        blinkTimer += Time.deltaTime;
        for (int i = 0; i < selected.Count; i++) {
            if (selected[i] == null) {
                selected.RemoveAt(i);
                i--;
                continue;
            }

            Transform t = selected[i].transform;
            DrawColoredCircle(t, 1.0f, Color.white);

            if (selected[i].targetBug != null) {
                var bug = selected[i].targetBug;
                if (!bug.dying) {
                    DrawColoredCircle(bug.transform, bug.agent.radius * 1.5f * (0.8f + Mathf.Abs(Mathf.Sin(blinkTimer * 4.0f)) * 0.4f), Color.red);
                }
            }
        }

    }

    void DrawColoredCircle(Transform t, float radius, Color color) {
        lineMat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);

        int segments = 16;
        Vector3 up = Vector3.up * 0.1f;
        float u = radius * .95f;
        for (int j = 0; j < segments; j++) {
            float s = 2.0f * Mathf.PI / segments;
            GL.Vertex(t.position + t.right * Mathf.Sin(j * s) * u + t.forward * Mathf.Cos(j * s) * u + up);
            GL.Vertex(t.position + t.right * Mathf.Sin((j + 1) * s) * u + t.forward * Mathf.Cos((j + 1) * s) * u + up);
        }
        float v = radius * 1.0f;
        for (int j = 0; j < segments; j++) {
            float s = 2.0f * Mathf.PI / segments;
            GL.Vertex(t.position + t.right * Mathf.Sin(j * s) * v + t.forward * Mathf.Cos(j * s) * v + up);
            GL.Vertex(t.position + t.right * Mathf.Sin((j + 1) * s) * v + t.forward * Mathf.Cos((j + 1) * s) * v + up);
        }

        GL.End();
    }
}
