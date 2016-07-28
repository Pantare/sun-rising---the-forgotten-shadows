using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MotherShip : MonoBehaviour
{
    [Range(0f, 50f)]
    public float speed;                             //Velocidad maxima de la nave.
    [Range(0f, 5000f)]
    public float currentSpeed;                      //Velocidad de la nave, esta puede aumentar o disminuir segun lo desee el jugador.
    public float health;                            //Vida maxima de la nave.
    public float currentHealth;                     //Vida de la nave esta varia dependiendo del daño recibido.
    public float detectionRange;                    //Rango de deteccion.

    public List<Transform> weapons;                 //Lista que guarda las posiciones en donde se colocaran las armas de la nave.
    public GameObject weapon;                       //Prefab del tipo de arma que usara la nave.
    public GameObject explosion;                    //Prefab del explosión.

    float rotationSpeed;                            //Velocidad de rotación de la nave incrementa o disminuye dependiendo la variable currentSpeed.

    Rigidbody MS_RB;                                //Variable correspondiente al rigidbody de la nave.;

    List<string> targetTag;                         //Lista correspondiente a los tags de las naves (Enemy/Ally/Player).
    public List<Transform> allytargets;                    //Lista que guarda todos los AI-Players hostiles.
    public List<Transform> targets;                       //Lista que guarda todos los AI-Players aliados.
    AI_BehaviourScript aiBehavior;
    [HideInInspector]
    public bool hit;

    void Awake()
    {
        aiBehavior = GetComponent<AI_BehaviourScript>();
        gameObject.AddComponent<Rigidbody>();

        MS_RB = GetComponent<Rigidbody>();
        MS_RB.mass = health;
        MS_RB.isKinematic = false;
        MS_RB.useGravity = false;        
    }

    void Start()
    {
        currentSpeed = speed;
        currentHealth = health;

        targetTag = new List<string>();
        weapons = new List<Transform>();

        SetWeapon();

        if (transform.tag == "EnemyMotherShip")
        {
            targetTag.Add("Player");
            targetTag.Add("Ally");
            targetTag.Add("AllyMotherShip");
        }

        if (transform.tag == "AllyMotherShip")
        {
            targetTag.Add("EnemyMotherShip");
            targetTag.Add("Enemy");
        }

        float tgtfound = 0;

        for (int i = 0; i < targetTag.Count; i++)
            tgtfound = GameObject.FindGameObjectsWithTag(targetTag[i]).LongLength;

        StartCoroutine(GetTargets());
        StartCoroutine(SendTargetPosition());
    }

    void Update ()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            foreach (Transform point in weapons)
                Instantiate(explosion, point.position, point.rotation);
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);	
	}

    /// <summary>
    /// Este método es el encargado de detectar todos los objetivos existentes en el nivel, dependiendo del tag del 
    /// AI-Player, si el AI-Player tiene tag enemy, adquirirá como objetivos todos los demas Ai-PLayer con tags de
    /// Ally o Player. Dentro de este se conjuntan en una sola lista todos los objetos previamente localizados.
    /// </summary>
    IEnumerator GetTargets()
    {
        while (true)
        {
            foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Ally"))
                allytargets.Add(ally.transform);

            allytargets = allytargets.Distinct().ToList();

            targets = new List<Transform>();

            for (int i = 0; i < targetTag.Count; i++)
                foreach (GameObject toAttack in GameObject.FindGameObjectsWithTag(targetTag[i]))
                    if (toAttack.tag == targetTag[i])
                        targets.Add(toAttack.transform);

            yield return new WaitForSeconds(1.5f);
        }
    }

    /// <summary>
    /// Este método se encarga de dar la orden de atacar a las naves de escolta si es que la nave madre
    /// carece de armamento.
    /// </summary>
    IEnumerator SendTargetPosition()
    {
        while (true)
        {
            if (targets.Count != 0)
            {
                Transform tgt = aiBehavior.selecNearTarget(targets);

                float nearestHostile = Vector3.Distance(transform.position, tgt.position);

                if (nearestHostile < detectionRange)
                    foreach (Transform ally in allytargets)
                    {
                        if (weapons.Count == 0)
                            ally.SendMessage("Pursuit", true);
                    }
            }

            yield return new WaitForSeconds(2f);
        }
    }

    /// <summary>
    /// Este método es el encargado de colocar en su debido lugar las armas de cada nave instanciando
    /// el prefab de cannon.
    /// </summary>
    void SetWeapon()
    {
        foreach (GameObject cannonSlot in GameObject.FindGameObjectsWithTag("cannonSlot"))
        {
            weapons.Add(cannonSlot.transform);
        }

        foreach(Transform cannon in weapons)
        {
            GameObject newCann = (GameObject)Instantiate(weapon, cannon.position, cannon.rotation);
            newCann.transform.SetParent(transform);
            newCann.name = weapon.name;
        }
    }

    /// <summary>
    /// Método encargado de determinar el daño causado por el impacto de los proyectiles enemigos.
    /// </summary>
    /// <param name="dmg">Variable que determina el daño recibido.</param>
    /// <returns>Regresa la cantidad de vida resultante despues de cada impacto recibido.</returns>
    public float DamageRecieved(float dmg)
    {
        currentHealth -= dmg;
        hit = true;
        return currentHealth;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Enemy" || col.transform.tag == "Ally" || col.transform.tag == transform.tag)
        {
            hit = true;
            col.rigidbody.SendMessage("DamageRecieved", currentHealth);
        }
        else
            hit = false;
    }
}
