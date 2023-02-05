using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Bug : Unit {

    Animator anim;

    public enum State {
        IDLE,
        WANDER,
        CHASE,
    }

    public State state;

    Coroutine currentRoutine = null;

    public float wanderSpeed = 2.5f;
    public float wanderAnimPlayrate = 1.0f;
    public float chaseSpeed = 3.5f;
    public float chaseAnimPlayrate = 1.5f;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        anim = GetComponentInChildren<Animator>();

        StartCoroutine(RaycastToGround());

        if (!IsServer) { // only server after this
            return;
        }

        state = State.IDLE;
        currentRoutine = StartCoroutine(WanderRoutine());

    }


    float idleTimer = 0.0f;
    Vector3 targetUp = Vector3.up;

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        targetUp = Vector3.Lerp(targetUp, groundNormal, Time.deltaTime);
        Vector3 right = Vector3.Cross(transform.forward, targetUp);
        Vector3 trueForward = Vector3.Cross(targetUp, right).normalized;
        //Debug.DrawLine(transform.position, transform.position + trueForward * 5.0f, Color.white);
        //Debug.DrawLine(transform.position, transform.position + groundNormal * 5.0f, Color.red);
        anim.transform.localRotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(trueForward, targetUp);

        if (!IsServer) { // only server after this
            return;
        }

        agent.speed = state == State.CHASE ? chaseSpeed : wanderSpeed;
        // set animations
        anim.speed = state == State.CHASE ? chaseAnimPlayrate : wanderAnimPlayrate;
        anim.SetBool("Walk Forward", agent.velocity.magnitude > 0.1f);

        // check if recently attacked, put in chase mode if so

        //if (state == State.IDLE) {
        //    idleTimer -= Time.deltaTime;
        //    if (idleTimer < 0.0f) {
        //        state = State.WANDER;
        //        //Vector3 random = 
        //    }
        //} else if (state == State.WANDER) {

        //} else if (state == State.CHASE) {

        //}
    }

    Vector3 groundNormal = Vector3.up;

    IEnumerator RaycastToGround() {
        while (true) {
            int count = Physics.RaycastNonAlloc(transform.position + Vector3.up, Vector3.down, hits, 3.0f, 1 << Layers.Scalp);
            if (count > 0) {
                groundNormal = hits[0].normal;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WanderRoutine() {
        while (true) {
            // wait idly
            Debug.Log("Idle");
            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));

            // find random place to walk to
            Vector3 pos = transform.position;
            Vector2 rand = Random.insideUnitCircle.normalized * 15.0f;
            Vector3 start = new Vector3(pos.x + rand.x, pos.y + 10.0f, pos.z + rand.y);
            int count = Physics.RaycastNonAlloc(start, Vector3.down, hits, 100.0f, 1 << Layers.Scalp);
            if (count > 0) {
                agent.destination = hits[0].point;
            }
            Debug.Log("Traveling");

            // wait until we get there
            while (true) {
                if ((transform.position - agent.destination).magnitude < 1.0f) {
                    break;
                }
                yield return null;
            }
            Debug.Log("Arrived");
        }
    }

}
