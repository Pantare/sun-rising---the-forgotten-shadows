using UnityEngine;
using System.Collections;

public class Ally : MonoBehaviour
{
    public float speed;
    public float health;

    public int missilCount;
    public int lasserCount;

    float rotationSpeed, x, y;

    Rigidbody allyRB;
    MeshCollider allyColl;

    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<MeshCollider>();
    }

    void Start()
    {
        allyRB = GetComponent<Rigidbody>();
        allyRB.isKinematic = true;
        allyRB.useGravity = false;

        allyColl = GetComponent<MeshCollider>();
        allyColl.convex = true;

        rotationSpeed = speed / 0.5f;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
