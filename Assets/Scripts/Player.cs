using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Range(5f,50f)]
    public float speed;                             //Velocidad maxima de la nave.
    [Range(10f, 5000f)]
    public float health;                            //Vida maxima de la nave.
    public float damage;                            //Daño de cada disparo, si la nave posee mas cañones el daño se divide entre estos.
    public float detectionRange;                    //Rango de deteccion.
    public float fireRate;                          //Tiempo entre disparos.
    public float laserRange;                        //Distancia en que el laser surte mejor efecto.
    public float missileRange;                      //Distancia en que el misil adquiere el objetivo.
    public float laserRecoverRate;                  //Tiempo de recarga del laser.

    public float currentSpeed;                      //Velocidad de la nave, esta puede aumentar o disminuir segun lo desee el jugador.
    public float currentHealth;                     //Vida de la nave esta varia dependiendo del daño recibido.    

    public int missilMaxCount;                      //Cantidad maxima de misiles que porta la nave.
    public int laserMaxCount;                       //Cantidad maxima de lasers que porta la nave.

    public GameObject laser;                        //Prefab del laser.
    public GameObject missil;                       //Prefab del misil.
    public GameObject explosion;                    //Prefab del explosión.
    public List<Transform> laserMuzzles;            //Posiciones donde apareceran los prefabs de laser instanciados.
    public List<Transform> missileMuzzles;          //Posiciones donde apareceran los prefabs de misil instanciados.
    
    float rotationSpeed;                            //Velocidad de rotación de la nave incrementa o disminuye dependiendo la variable currentSpeed.
    float x;                                        //Variable de ayuda para rotar en el eje horizontal.
    float y;                                        //Variable de ayuda para rotar en el eje vertical.
    float step;                                     //Tiempo de espera entre disparos de laser.
    float step2;                                    //Tiempo de espera entre recarga de disparos de laser.
    float stepEngines;

    public int currentLaserCount;                   //Cantidad de laser, se actualiza por cada disparo o recarga.
    public int currentMissileCount;                 //Cantidad de misiles, se actualiza por cada disparo o recarga.

    Rigidbody playerRB;                             //Variable correspondiente al rigidbody de la nave.

    List<Transform> targets;                        //Lista de enemigos detectados en el nivel.

    ShootScript shoot;                              //Variable encargada de llamar al método de disparo.
    [HideInInspector]
    public bool hit;                                //

    void Awake()
    {
        gameObject.AddComponent<ShootScript>();
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<BoxCollider>();

        shoot = GetComponent<ShootScript>();

        playerRB = GetComponent<Rigidbody>();
        playerRB.mass = health;
        playerRB.isKinematic = false;
        playerRB.useGravity = false;

        missileRange = detectionRange - 20;
        laserRange = detectionRange / 4;
        currentLaserCount = laserMaxCount;
        currentMissileCount = missilMaxCount;
        currentHealth = health;
    }
	
	void Start ()
    {
        StartCoroutine(TargetDetection());
        StartCoroutine(Controls());
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// Corrutina correspondiente a los controles del jugador.
    /// </summary>
    /// <returns></returns>
    IEnumerator Controls()
    {
        while (true)
        { 
            x = Input.GetAxis("Horizontal") * -rotationSpeed * Time.deltaTime;
            y = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

            if (currentLaserCount < laserMaxCount)
                if (Time.time > step2)
                {
                    step2 = laserRecoverRate + Time.time;
                    currentLaserCount++;
                }

            rotationSpeed = currentSpeed / 0.5f;

            //Este if se encarga de permitir los disparos de laser siempre y cuando este tenga carga, el contador no este en cero.
            if (currentLaserCount > 0)
                if (Input.GetKey(KeyCode.Space))
                    if (Time.time > step)
                    {
                        step = fireRate + Time.time;
                        shoot.Shoot(damage, laserMuzzles, laser);
                        currentLaserCount -= laserMuzzles.Count;
                    }
            
            //Este if se encarga de permitir los disparos de misiles siempre y cuando este tenga carga, el contador no este en cero.
            if (currentMissileCount > 0)
                if (Input.GetKeyUp(KeyCode.M))
                {
                    if (targets.Count != 0)
                    {
                        float tgtDistance = Vector3.Distance(transform.position, targets[0].position);

                        if (tgtDistance < missileRange)
                            shoot.Shoot(damage, missileMuzzles, missil, targets[0]);
                        else
                            shoot.Shoot(damage, missileMuzzles, missil, null);
                    }
                    else
                        shoot.Shoot(damage, missileMuzzles, missil, null);

                    currentMissileCount -= missileMuzzles.Count;
                }

            if (Input.GetKey(KeyCode.PageUp))
                if (currentSpeed < speed)
                    currentSpeed++;

            if (Input.GetKey(KeyCode.PageDown))
                if (currentSpeed > 0)
                    currentSpeed--;

            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            transform.Rotate(y, 0, x);

            yield return new WaitForSeconds(0.01f);
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

    /// <summary>
    /// Esta corutina se encarga de adquirir todos los objetivos dentro del nivel, se complementa con el método
    /// GetTargets, ejecutandose cada 3 segs.
    /// </summary>
    /// <returns></returns>
    IEnumerator TargetDetection()
    {
        while (true)
        {
            GetTargets();
            yield return new WaitForSeconds(3f);
        }
    }

    /// <summary>
    /// Este método es el encargado de detectar todos los objetivos existentes en el nivel.
    /// Dentro de este se conjuntan en una sola lista los objetos previamente localizados.
    /// </summary>
    void GetTargets()
    {
        GameObject[] tgtsDetected = GameObject.FindGameObjectsWithTag("Enemy");

        targets = new List<Transform>();

        foreach (GameObject tgt in tgtsDetected)
            targets.Add(tgt.transform);

        for (int i = 0; i < targets.Count; i++)
            if (targets[i] == null)
                targets.RemoveAt(i);
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
            col.transform.SendMessage("DamageRecieved", currentHealth);
        }
        else
            hit = false;
    }
}