using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NanoBot : Unit {

    //GameObject selectionLight;
    MeshRenderer mr;

    public Bug targetBug;
    float range = 8.0f;

    public GameObject gunEffect;

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

    [ClientRpc]
    public void Shoot_ClientRpc(Vector3 target) {
        anim.SetTrigger("Shoot");
        // spawn particles
        Vector3 diff = (target - transform.position);
        float dist = diff.magnitude;
        var go = Instantiate(gunEffect, Vector3.zero, Quaternion.identity);
        go.transform.position = transform.position;
        go.transform.forward = diff.normalized;
        go.transform.position = go.transform.position + go.transform.forward * dist / 2.0f + transform.forward + Vector3.up * .5f;
        go.transform.localScale = new Vector3(1, 1, dist);
        Destroy(go, 2.0f);
    }

    protected override void Update() {
        base.Update();
        if (!IsServer || dying) {
            return;
        }

        anim.SetBool("Moving", agent.velocity.magnitude > 0.1f);

        if (health <= 0.0f) {
            StopAllCoroutines();
            //if (idleRoutine != null) {
            //    StopCoroutine(idleRoutine);
            //}
            if (!dying) {
                dying = true;
                StartCoroutine(DeathRoutine());
            }
        }
    }

    IEnumerator ChaseAndDestroy() {
        float timeInRange = 0.0f;
        float shootTimer = 0.0f;
        while (true) {
            if (targetBug != null) {
                Vector3 targetPos = targetBug.transform.position;
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
                    shootTimer -= Time.deltaTime;
                    if (shootTimer <= 0.0f && !targetBug.dying) {
                        targetBug.health -= 5.0f;
                        // notify clients
                        Shoot_ClientRpc(targetBug.transform.position);
                        shootTimer = 2.0f;
                    }
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
