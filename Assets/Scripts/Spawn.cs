using UnityEngine;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    public List<GameObject> spawnObjs;

    public float spawnRadius;
    public int spawnCount;

    float step;

    Vector3 rdmPos;

    void Start()
    {
        if (transform.tag == "SpawnPoint")
            transform.LookAt(Vector3.zero);

        int i = 0;

        do
        {
            for (int j = 0; j < spawnObjs.Count; j++)
            {
                if (i < spawnCount)
                {
                    GameObject gameObj = (GameObject)Instantiate(spawnObjs[j], transform.position + (rdmPos = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius))), transform.rotation);
                    gameObj.name = spawnObjs[j].name;
                }

                i++;
            }
        }
        while (i < spawnCount);
    }
}
