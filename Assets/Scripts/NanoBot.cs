using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NanoBot : Unit {

    //GameObject selectionLight;
    MeshRenderer mr;

    public Bug target;
    float range = 8.0f;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        mr = GetComponentInChildren<MeshRenderer>();
        //selectionLight = transform.Find("SelectionLight").gameObject;
        //selectionLight.SetActive(false);

    }

    protected override void Start() {
        base.Start();
        if (!IsServer) {
            return;
        }
        StartCoroutine(ChaseAndDestroy());
    }

    [ClientRpc]
    public void SetColor_ClientRpc(Color color) {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", color);
        mr.SetPropertyBlock(mpb);
    }

    protected override void Update() {

        if (!IsServer) {
            return;
        }

        anim.SetBool("Moving", agent.velocity.magnitude > 0.1f);
    }

    IEnumerator ChaseAndDestroy() {
        float timeInRange = 0.0f;
        while (true) {
            if (target != null) {
                Vector3 targetPos = target.transform.position;
                float dist = Vector3.Distance(targetPos, transform.position);
                bool inMaxRange = dist < range;
                bool inMinRange = dist < range * 0.9f;
                if (inMaxRange) {
                    timeInRange += Time.deltaTime;
                    if (timeInRange > 0.5f && inMinRange) {
                        agent.isStopped = true;
                    }
                    Vector3 v = targetPos;
                    targetPos.y = transform.position.y;
                    transform.LookAt(Vector3.Lerp(transform.position + transform.forward, v, Time.deltaTime * 2.0f));
                    // shoot
                } else {
                    timeInRange = 0.0f;
                    agent.isStopped = false;
                    agent.destination = targetPos;
                }
            }
            yield return null;
        }
    }

}
