using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour
{
    public float speed;                                                 //Variable de velocidad a la que se desplaza el proyectil
    public float damage;                                                //Variable del daño que causara al impactar.

    public Transform explosion;                                         //Variable que guarda el Prefab de la explosión.

    Transform target;                                                   //Variable del objetivo a atacar.

    void FixedUpdate ()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 2.0f);
    }

    /// <summary>
    /// Método encargado de adquirir el daño de la nave que lo dispara.
    /// </summary>
    /// <param name="dmg">Daño inicial que adquiere de la nave que lo dispara.</param>
    /// <returns>Regresa el daño.</returns>
    public float Damage(float dmg)
    {
        return damage = dmg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag != transform.tag)
            col.transform.SendMessage("DamageRecieved", damage);

        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
    }
}
