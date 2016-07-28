using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AI_player : MonoBehaviour
{
    [Range(0f, 50f)]
    public float speed;                             //Velocidad maxima de la nave.
    [Range(0f, 5000f)]
    public float health;                            //Vida maxima de la nave.
    public float damage;                            //Daño de cada disparo, si la nave posee mas cañones el daño se divide entre estos.
    public float detectionRange;                    //Rango de deteccion.
    public float fireRate;                          //Tiempo entre disparos.
    public float laserRange;                        //Distancia en que el laser surte mejor efecto.
    public float missileRange;                      //Distancia en que el misil adquiere el objetivo.
    public float laserRecoverRate;                  //Tiempo de recarga del laser.

    [HideInInspector]
    public float currentSpeed;                      //Velocidad de la nave, esta puede aumentar o disminuir segun lo desee el jugador.
    [HideInInspector]
    public float currentHealth;                     //Vida de la nave esta varia dependiendo del daño recibido.    

    [HideInInspector]
    public int missilMaxCount;                      //Cantidad maxima de misiles que porta la nave.
    [HideInInspector]
    public int laserMaxCount;                       //Cantidad maxima de lasers que porta la nave.

    public GameObject laser;                        //Prefab del laser.
    public GameObject missil;                       //Prefab del misil.
    public GameObject explosion;                    //Prefab del explosión.
    public List<Transform> laserMuzzles;            //Posiciones donde apareceran los prefabs de laser instanciados.
    public List<Transform> missileMuzzles;          //Posiciones donde apareceran los prefabs de misil instanciados.

    bool evade;                                     //Variable que regresa true cuando el AI-Player esta bajo ataque o cerca de una colisión.

    [HideInInspector]
    public int currentLaserCount;                   //Cantidad de laser, se actualiza por cada disparo o recarga.
    [HideInInspector]
    public int currentMissileCount;                 //Cantidad de misiles, se actualiza por cada disparo o recarga.

    Rigidbody AiRB;                                 //Variable correspondiente al rigidbody de la nave.;

    List<string> targetTag;                         //Lista correspondiente a los tags de las naves (Enemy/Ally/Player).
    [HideInInspector]
    public List<Transform> targets;                 //Lista de enemigos detectados en el nivel, se alimenta de los arrays de player y targets.
    [HideInInspector]
    public List<Transform> targetsToProtect;        //Array donde se almacenan todos los otros AI-Player alliados a proteger.
    
    AI_BehaviourScript AI_behavior;                 //Variable que permite acesar al componente de Comportamientos.
    [HideInInspector]
    public bool hit;
    SphereCollider triggerDistance;

    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<SphereCollider>();

        AI_behavior = GetComponent<AI_BehaviourScript>();
        triggerDistance = GetComponent<SphereCollider>();

        AiRB = GetComponent<Rigidbody>();
        AiRB.mass = health;
        AiRB.isKinematic = false;
        AiRB.useGravity = false;

        AI_behavior.tgtOnrange = false;

        triggerDistance.center = Vector3.zero;
        triggerDistance.radius = 5;
        triggerDistance.isTrigger = true;

        targetTag = new List<string>();

        //Esta seccion de código determina si el AI-Player es hostil o amigo de los jugadores.
        if (transform.tag == "Enemy")
        {
            targetTag.Add("Player");
            targetTag.Add("Ally");
            targetTag.Add("AllyMotherShip");
        }
        else if (transform.tag == "Ally")
        {
            targetTag.Add("Enemy");
            targetTag.Add("EnemyMotherShip");
        }
    }

    void Start()
    {
        missileRange = detectionRange - 20;
        laserRange = detectionRange / 4;
        currentLaserCount = laserMaxCount;
        currentMissileCount = missilMaxCount;
        currentHealth = health;

        StartCoroutine(GetTargets());
        StartCoroutine(AI_Behavior());

        evade = false;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Instantiate(explosion, transform.position, transform.rotation);
        }

        Move();
    }

    /// <summary>
    /// Método encargado de determinar el daño causado por el impacto de los proyectiles enemigos.
    /// </summary>
    /// <param name="dmg">Variable que determina el daño recibido.</param>
    /// <returns>Regresa la cantidad de vida resultante despues de cada impacto recibido.</returns>
    public float DamageRecieved(float dmg)
    {
        ScoreManager.scoreValue += (int)(dmg / 10);
        currentHealth -= dmg;
        hit = true;
        return currentHealth;
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
            foreach (GameObject ally in GameObject.FindGameObjectsWithTag("AllyMotherShip"))
                if (transform.tag == "Ally")
                    targetsToProtect.Add(ally.transform);

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                if (player.tag == "Player" && transform.tag == "Ally")
                    targetsToProtect.Add(player.transform);

            targetsToProtect = targetsToProtect.Distinct().ToList();

            targets = new List<Transform>();

            for (int i = 0; i < targetTag.Count; i++)
                foreach (GameObject toAttack in GameObject.FindGameObjectsWithTag(targetTag[i]))
                    if (toAttack.tag == targetTag[i])
                        targets.Add(toAttack.transform);

            yield return new WaitForSeconds(1.5f);
        }
    }

    void Move()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        if (evade == true)
        {
            transform.Rotate(Vector3.right * -40.0f * Time.deltaTime);
            transform.Rotate(Vector3.forward * -40.0f * Time.deltaTime);
        }
        else
            AI_behavior.Maneuvers(AI_behavior.tgtOnrange);
    }

    /// <summary>
    /// Esta corrutina se encarga de manejar el comportamiento del AI_player.
    /// </summary>
    /// <returns></returns>
    IEnumerator AI_Behavior()
    {
        while (true)
        {
            Transform hostile = AI_behavior.selecNearTarget(targets);
            Transform tgtToScort = AI_behavior.selecNearTarget(targetsToProtect);

            /*Esta seccion determina si hay objetivos a proteger o no, en caso de no existir el AI-Player pasa 
             al método de persecución de manera automática.*/
            if (targetsToProtect.Count == 0 && targets.Count != 0)
            {
                float tgtDist = Vector3.Distance(transform.position, hostile.position);

                AI_behavior.Pursuit(hostile);
                AI_behavior.Moves(hostile);
                AI_behavior.tgtOnrange = true;
                AI_behavior.Attack(hostile, tgtDist);
            }

            /*Esta sección determina si hay objetivos a proteger, en caso e existir, el AI-Player comienza el
             método de escolta hasta que encuentre objetivos hostiles cerca del objetivo a escoltar.*/
            if (targetsToProtect.Count != 0)
            {
                float tgtEscort = Vector3.Distance(transform.position, tgtToScort.position);

                if(tgtEscort > detectionRange)
                    AI_behavior.Pursuit(tgtToScort);

                if (targets.Count != 0)
                {
                    float hostileDistance = Vector3.Distance(tgtToScort.position, hostile.position);

                    if (hostileDistance <= detectionRange && hostile != null)
                    {
                        AI_behavior.Pursuit(hostile);
                        AI_behavior.Moves(hostile);
                        AI_behavior.tgtOnrange = true;
                        AI_behavior.Attack(hostile, hostileDistance);
                    }
                    else if (tgtEscort < (missileRange * 0.75) && tgtToScort.tag == "AllyMotherShip")
                    {
                        AI_behavior.Escort(tgtToScort);
                        AI_behavior.tgtOnrange = false;
                    }
                    else
                    {
                        AI_behavior.Escort(tgtToScort);
                        AI_behavior.tgtOnrange = false;
                    }
                }
                else
                {
                    AI_behavior.Escort(tgtToScort);
                    AI_behavior.tgtOnrange = false;
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// En este método se calcula por el momento el daño causado contra otro objeto, (nave, edificio, etc).
    /// </summary>
    /// <param name="col"></param>
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

    /// <summary>
    /// Dentro de este método se genera la orden para evadir la posible colisión contra otra nave
    /// o el suelo en caso de estar en un nivel que sea dentro de un "planeta".
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter(Collider collider)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, (triggerDistance.radius * 2)))
            if (collider.tag == hit.transform.tag)
                evade = true;
            else
                evade = false;
    }
}
