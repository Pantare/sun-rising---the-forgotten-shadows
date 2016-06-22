using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;

    public int missilCount;
    public int lasserCount;

    float rotationSpeed, x, y;

    Rigidbody enemyRB;
    MeshCollider enemyColl;

    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<MeshCollider>();
    }

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        enemyRB.isKinematic = true;
        enemyRB.useGravity = false;

        enemyColl = GetComponent<MeshCollider>();
        enemyColl.convex = true;

        rotationSpeed = speed / 0.5f;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
