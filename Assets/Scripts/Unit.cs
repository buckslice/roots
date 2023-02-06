using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Unit : NetworkBehaviour {

    [HideInInspector] public NavMeshAgent agent;

    public float health; // network variable?

    public string enemyTag; // not used currently

    public static Collider[] colliders = new Collider[32];
    public static RaycastHit[] hits = new RaycastHit[32];

    protected Animator anim;

    // Start is called before the first frame update
    protected virtual void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start() {
        anim = GetComponentInChildren<Animator>();
        if (!IsServer) { // only server controls this
            Destroy(agent);
            return;
        }
    }

    [ServerRpc]
    public void MoveUnit_ServerRpc(Vector3 destination) {
        //Debug.Log($"Moving Unit here {destination}");
        agent.destination = destination;
        agent.isStopped = false;
        (this as NanoBot).targetBug = null;

    }

    // Update is called once per frame
    protected virtual void Update() {

        if (!IsServer) {
            return;
        }

    }

    public bool dying = false;
    protected IEnumerator DeathRoutine() {
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

    void FindEnemy() {
        var colliders = Physics.OverlapSphere(transform.position, 10, 1 << Layers.Unit);
        Debug.Log(colliders.Length);
    }
}
