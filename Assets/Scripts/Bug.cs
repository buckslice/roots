using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Bug : Unit {

    public enum State {
        IDLE,
        WANDER,
        CHASE,
    }

    State state;

    Coroutine currentRoutine = null; // tracks the currently active routine, incase needs to be interrupted

    public float wanderSpeed = 2.5f;
    public float wanderAnimPlayrate = 1.0f;
    public float chaseSpeed = 3.5f;
    public float chaseAnimPlayrate = 1.5f;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        StartCoroutine(RaycastToGround());

        if (!IsServer) { // only server after this
            return;
        }

        state = State.IDLE;
        currentRoutine = StartCoroutine(WanderRoutine());

    }

    Vector3 targetUp = Vector3.up;

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (dying) {
            return;
        }

        RotateToGround();

        if (!IsServer) { // only server after this
            return;
        }

        agent.speed = state == State.CHASE ? chaseSpeed : wanderSpeed;
        // set animations
        anim.speed = state == State.CHASE ? chaseAnimPlayrate : wanderAnimPlayrate;
        anim.SetBool("Walk Forward", agent.velocity.magnitude > 0.1f);

        // check if recently attacked, put in chase mode if so

        if (health <= 0.0f) {
            if (currentRoutine != null) {
                StopCoroutine(currentRoutine);
            }
            if (!dying) {
                dying = true;
                StartCoroutine(DeathRoutine());
            }
        }

    }

    public bool dying = false;
    IEnumerator DeathRoutine() {
        anim.speed = 1.0f;
        anim.SetTrigger("Die");
        Destroy(agent);
        float t = 0.0f;
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(3.0f);
        while (t < 1.0f) {
            t += Time.deltaTime * 0.05f;
            transform.position = startPos - Vector3.up * 2.0f * t;
            yield return null;
        }
        NetworkObject.Despawn();
    }

    void RotateToGround() {
        targetUp = Vector3.Lerp(targetUp, groundNormal, Time.deltaTime);
        Vector3 right = Vector3.Cross(transform.forward, targetUp);
        Vector3 trueForward = Vector3.Cross(targetUp, right).normalized;
        //Debug.DrawLine(transform.position, transform.position + trueForward * 5.0f, Color.white);
        //Debug.DrawLine(transform.position, transform.position + groundNormal * 5.0f, Color.red);
        anim.transform.localRotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(trueForward, targetUp);
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
            //Debug.Log("Idle");
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));

            // find random place to walk to
            Vector3 pos = transform.position;
            Vector2 rand = Random.insideUnitCircle.normalized * 15.0f;
            Vector3 start = new Vector3(pos.x + rand.x, pos.y + 10.0f, pos.z + rand.y);
            int count = Physics.RaycastNonAlloc(start, Vector3.down, hits, 100.0f, 1 << Layers.Scalp);
            if (count > 0) {
                agent.destination = hits[0].point;
            }
            //Debug.Log("Wandering");

            float lookForHairTimer = 0.0f;
            // wait until we get there
            while (true) {
                if ((transform.position - agent.destination).magnitude <= agent.stoppingDistance) {
                    break;
                }

                lookForHairTimer -= Time.deltaTime;
                if (lookForHairTimer < 0.0f) {
                    var hair = LookForHair();
                    if (hair != null) {
                        currentRoutine = StartCoroutine(EatHairRoutine(hair));
                        yield break; // exit this coroutine
                    }
                    lookForHairTimer = 1.0f;
                }

                yield return null;
            }
            //Debug.Log("Arrived");
        }
    }

    Collider LookForHair() {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 5.0f, colliders, 1 << Layers.Hair);
        if (count > 0) {
            float closestDist = float.MaxValue;
            int closestIndex = 0;
            // find closest hair
            for (int i = 0; i < count; i++) {
                var pos = colliders[i].transform.position;
                float dist = (transform.position - pos).sqrMagnitude;
                if (dist < closestDist) {
                    closestDist = dist;
                    closestIndex = i;
                }
            }
            return colliders[closestIndex];
        }
        return null;
    }

    IEnumerator EatHairRoutine(Collider hair) {
        agent.destination = hair.transform.position;
        while (hair.enabled) { // move to hair
            if ((transform.position - agent.destination).magnitude <= agent.stoppingDistance) {
                break;
            }
            yield return null;
        }

        // look at hair
        Vector3 target = hair.transform.position;
        target.y = transform.position.y;
        transform.LookAt(target);

        //while (true) {
        //    target.y = transform.position.y;
        //    transform.LookAt(Vector3.Lerp(transform.position + transform.forward, target, Time.deltaTime));
        //    if (Vector3.Dot(transform.forward, target - transform.position) > 0.9f) {
        //        break;
        //    }
        //    yield return null;
        //}

        if (hair.enabled) {

            anim.SetTrigger("Smash Attack");
            yield return new WaitForSeconds(1.0f);
            anim.SetTrigger("Stab Attack");
            yield return new WaitForSeconds(1.0f);

            // destroy hair
            if (hair.enabled) {
                HairManager.instance.KillHair(hair.gameObject);
                // could spawn some baby bugs here
                // or become bigger / stronger
            }
        }

        currentRoutine = StartCoroutine(WanderRoutine());
    }

    // chase routine ideas
    // anim change is happening above right?
    // chase target updating destination
    // if close enough attack on cooldown
    // if target dies or something revert to idle / wander

}
