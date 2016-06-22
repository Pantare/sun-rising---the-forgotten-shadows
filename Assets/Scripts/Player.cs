using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float speed;
    public float health;

    public int missilCount;
    public int lasserCount;

    float rotationSpeed, x, y;

    Rigidbody playerRB;
    MeshCollider playerColl;

    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<MeshCollider>();
    }
	
	void Start ()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.isKinematic = true;
        playerRB.useGravity = false;

        playerColl = GetComponent<MeshCollider>();
        playerColl.convex = true;

        rotationSpeed = speed / 0.5f;
	}
	
	
	void Update ()
    {
        x = Input.GetAxis("Horizontal") * -rotationSpeed * Time.deltaTime;
        y = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.Rotate(y, 0, x);
	}
}