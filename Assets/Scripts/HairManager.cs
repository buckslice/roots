using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HairData {
    public GameObject go;
    public Collider col;
    public float growth;
    public float growthRate;
    public int maxGrowth;

    public HairData(GameObject go, float growth, float growthRate, int maxGrowth) {
        this.go = go;
        col = go.GetComponent<Collider>();
        col.enabled = false;
        this.growth = growth;
        this.growthRate = growthRate;
        this.maxGrowth = maxGrowth;
    }
}

public class HairManager : NetworkBehaviour {

    public GameObject hairPrefab;

    NetworkList<Vector3> hair = new NetworkList<Vector3>();
    NetworkList<int> hairGrowth = new NetworkList<int>(); // quantized growth sent over network

    List<HairData> hairData = new List<HairData>();

    public static HairManager instance = null;

    void Awake() {
        instance = this;
        hair.OnListChanged += Hair_OnListChanged;
        hairGrowth.OnListChanged += HairGrowth_OnListChanged;
    }

    private void HairGrowth_OnListChanged(NetworkListEvent<int> change) {
        //if (hairGrowth.Count != hairData.Count) {
        //    return;
        //}
        //if (change.Type == NetworkListEvent<int>.EventType.Value) {
        //    int i = change.Index;
        //    float h = hairGrowth[i] / 2.0f;
        //    float w = 0.25f + hairGrowth[i] * 0.05f;
        //    hairData[i].go.transform.localScale = new Vector3(w, h, w);
        //}

    }

    private void Hair_OnListChanged(NetworkListEvent<Vector3> change) {
        switch (change.Type) {
            case NetworkListEvent<Vector3>.EventType.Add:
                var go = Instantiate(hairPrefab, change.Value, Quaternion.identity, transform);
                go.transform.localScale = new Vector3(1, 0, 1);
                hairGrowth.Add(0);
                hairData.Add(new HairData(go, 0, Random.Range(.2f, 1f), 30));
                break;
            case NetworkListEvent<Vector3>.EventType.RemoveAt:
                Destroy(hairData[change.Index].go);
                hairGrowth.RemoveAt(change.Index);
                hairData.RemoveAt(change.Index);
                break;
            default:
                Debug.Log("case not handled...");
                break;
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (!IsServer) {
            return;
        }

        int tries = 0;
        while (hair.Count < 400 && tries < 2000) {
            tries++;

            Vector3 pos = transform.position;
            Vector2 rand = Random.insideUnitCircle * 65.0f;
            pos.x += rand.x;
            pos.z += rand.y;
            pos.y += 50.0f;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 200.0f, 1 << Layers.Scalp)) {
                bool tooClose = false;
                for (int i = 0; i < hair.Count; i++) {
                    if (Vector3.Distance(hit.point, hair[i]) < 2.0f) {
                        tooClose = true;
                        break;

                    }
                }
                if (!tooClose) {
                    hair.Add(hit.point - Vector3.up*.5f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (hairGrowth.Count == hairData.Count) {
            for (int i = 0; i < hairGrowth.Count; i++) {
                float h = hairGrowth[i] / 2.0f;
                float w = 0.25f + hairGrowth[i] * 0.05f;
                hairData[i].go.transform.localScale = Vector3.Lerp(hairData[i].go.transform.localScale, new Vector3(w, h, w), Time.deltaTime);
            }
        }

        if (!IsServer) {
            return;
        }

        // grow the hairs
        for (int i = 0; i < hairGrowth.Count; i++) {
            var hd = hairData[i];
            hd.growth += Time.deltaTime * hd.growthRate;
            if (hd.growth > hd.maxGrowth) {
                hd.growth = hd.maxGrowth;
            }
            int newGrowth = (int)hd.growth;
            if (newGrowth < 0) { // let actual growth be negative but clamp it
                newGrowth = 0;
            }
            // not sure if updating without changing would dirty this network list.. but just incase
            if (newGrowth != hairGrowth[i]) {
                hairGrowth[i] = newGrowth;
            }
            hd.col.enabled = newGrowth >= 5; // turn collider on after early stage (so bugs will eat it)

        }
    }

    public void KillHair(GameObject go) {
        foreach (var hd in hairData) {
            if (go == hd.go) {
                hd.growth = -30;
                hd.growthRate *= 0.9f; // permanently nerf growth rate on this hair
                break;
            }
        }
    }


}
