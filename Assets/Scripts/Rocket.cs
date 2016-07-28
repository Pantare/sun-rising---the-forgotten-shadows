using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed;                                                 //Variable de velocidad a la que se desplaza el proyectil
    public float damage;                                                //Variable del daño que causara al impactar.

    public Transform target;                                               //Variable del objetivo a atacar.

    public Transform explosion;                                         //Variable que guarda el Prefab de la explosión.

    float time;                                                         //Variable de tiempo dedicada la detección del objetivo hasta su impacto con este.

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        time += 0.1f;

        if (time >= 1.0f)
        {
            time = 0;
            FollowTarget(SetTarget(target));
        }

        Destroy(gameObject, 10.0f);
    }

    /// <summary>
    /// Método encargado de seguir el objetivo despues de ser adquirido.
    /// </summary>
    /// <param name="target"></param>
    public void FollowTarget(Transform target)
    {
        float step = speed / 4 * Time.deltaTime;

        try
        {
            Vector3 targetDirection = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        catch
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, Vector3.zero, step, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    /// <summary>
    /// Método encargado de adquirir el objetivo desde la nave que lo dispara.
    /// </summary>
    /// <param name="tgt">Variable donde se guarda la información del objetivo a atacar.</param>
    /// <returns></returns>
    Transform SetTarget(Transform tgt)
    {
        target = tgt;
        return target;
    }

    /// <summary>
    /// Método encargado de adquirir el daño de la nave que lo dispara.
    /// </summary>
    /// <param name="dmg">Daño inicial que adquiere de la nave que lo dispara.</param>
    /// <returns>Regresa el daño aumentado por 10.</returns>
    public float Damage(float dmg)
    {
        return damage = dmg * 10;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        col.transform.SendMessage("DamageRecieved", damage);
        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
        
        //Esta sección se encarga de sumar al score un valor a cada impacto hecho sobre un objetivo.
        if (transform.tag == "Player" && (col.transform.tag == "Enemy" || col.transform.tag == "EnemyMotherShip"))
            SendMessageUpwards("Score", 5);
    }
}
