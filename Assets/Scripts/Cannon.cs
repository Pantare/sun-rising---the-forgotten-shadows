using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cannon : MonoBehaviour
{
    public float attackRange;
    public float rotationSpeed;
    public float damage;
    public float fireRate;

    public int laserMaxCount;

    public GameObject laserBeam;
    public List<Transform> Muzzles;

    float step;
    public Vector3 originalPosition;

    ShootScript shoot;                              //Variable encargada de llamar al método de disparo.
    Transform motherShip;

    List<Transform> targets;

    List<string> targetTag;

    void Awake()
    {
        gameObject.AddComponent<ShootScript>();

        shoot = GetComponent<ShootScript>();
    }

    void Start()
    {
        targetTag = new List<string>();
        motherShip = transform.parent;

        if (motherShip.tag == "AllyMotherShip")
        {
            targetTag.Add("EnemyMotherShip");
            targetTag.Add("Enemy");
        }

        if (motherShip.tag == "EnemyMotherShip")
        {
            targetTag.Add("Player");
            targetTag.Add("Ally");
            targetTag.Add("AllyMotherShip");
        }

        originalPosition = transform.rotation.eulerAngles;

        StartCoroutine(TargetDetection());
        StartCoroutine(AttackTarget());
    }

    /// <summary>
    /// Este método es el encargado de detectar todos los objetivos existentes en el nivel, dependiendo del tag del 
    /// AI-Player, si el AI-Player tiene tag enemy, adquirirá como objetivos todos los demas Ai-PLayer con tags de
    /// Ally o Player. Dentro de este se conjuntan en una sola lista todos los objetos previamente localizados.
    /// </summary>
    void GetTargets()
    {
        targets = new List<Transform>();

        for (int i = 0; i < targetTag.Count; i++)
            foreach (GameObject toAttack in GameObject.FindGameObjectsWithTag(targetTag[i]))
                if (toAttack.tag == targetTag[i])
                    targets.Add(toAttack.transform);
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

    
    IEnumerator AttackTarget()
    {
        while (true)
        {
            for (int i = 0; i < targets.Count; i++)
                if (targets[i] == null)
                    targets.RemoveAt(i);

            foreach (Transform tgt in targets)
            {
                float tgtDistance = Vector3.Distance(tgt.transform.position, transform.position);
                float step = rotationSpeed * Time.deltaTime;

                if (tgtDistance < attackRange)
                {
                    Vector3 targetDir = tgt.position - transform.position;
                    
                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                    newDir = new Vector3(newDir.x, Mathf.Clamp(newDir.y, 0, 180), Mathf.Clamp(newDir.z, 0, 0));
                    transform.rotation = Quaternion.LookRotation(newDir);

                    Ray ray = new Ray(transform.position, transform.forward * attackRange);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, attackRange))
                    {
                        if (hit.transform != motherShip)
                            for (int i = 0; i < targetTag.Count; i++)
                                foreach (Transform target in targets)
                                    if (target.tag == targetTag[i])
                                        if (Time.time > step)
                                        {
                                            step = Time.time + fireRate;
                                            shoot.Shoot(damage, Muzzles, laserBeam);
                                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
