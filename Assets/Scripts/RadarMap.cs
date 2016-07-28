using UnityEngine;
using System.Collections;

public class RadarMap : MonoBehaviour
{
    public Transform player;

	void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update ()
    {
        Vector3 followTgt;
        followTgt = new Vector3(player.position.x, transform.position.y, player.position.z);

        transform.position = followTgt;
	}
}
