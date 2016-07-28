using UnityEngine;

public class GameControl : MonoBehaviour
{
    Transform player;

    public float offset;

	void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void FixedUpdate ()
    {
        transform.position = player.position + new Vector3(0, offset, 0);
        transform.rotation = new Quaternion(0.7f, 0, 0, 0.7f);
	}
}
