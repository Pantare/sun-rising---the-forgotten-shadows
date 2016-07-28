using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	void Update ()
    {
        Destroy(gameObject, 5.0f);
	}
}
