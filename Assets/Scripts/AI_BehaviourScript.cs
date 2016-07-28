using UnityEngine;
using System.Collections.Generic;

public class AI_BehaviourScript : MonoBehaviour
{
    Transform target;                               //Variable que guarda el objetivo a seguir o atacar.
    AI_player aiPlayer;                             //Variable que permite acceder a los métodos del script del Ai-Player.
    ShootScript shoot;                              //Variable que permite acceder a los métodos des script de disparo.
    float step;                                     //Tiempo de espera entre disparos de laser.
    float step2;                                    //Tiempo de espera entre recarga de disparos de laser.
    public bool front, over, right;                 //Variables que determinan la localización del objetivo.
    public bool tgtOnrange;                         //Variable que determina si el objetivo esta en rango de ataque.
    bool tgtLock;                                   //Variable que determina si el objetivo esta frente al AI-Player.

    void Awake()
    {
        aiPlayer = GetComponent<AI_player>();
        shoot = GetComponent<ShootScript>();
        tgtLock = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    public Transform selecNearTarget(List<Transform> targets)
    {
        if (targets.Count > 1)
        {
            float iDist = 0;
            float jDist = 0;

            for (int i = 0; i < targets.Count; i++)
                for (int j = 1; j < targets.Count; j++)
                {
                    iDist = Vector3.Distance(transform.position, targets[i].position);
                    jDist = Vector3.Distance(transform.position, targets[j].position);

                    if (iDist < jDist)
                        target = targets[i];

                    if (iDist > jDist)
                        target = targets[j];
                }
        }

        if (targets.Count == 1)
        {
            target = targets[0];
        }

        return target;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public void Pursuit(Transform target)
    {
        if (aiPlayer.currentSpeed < aiPlayer.speed)
            aiPlayer.currentSpeed++;

        if (aiPlayer.currentSpeed > aiPlayer.speed && aiPlayer.currentSpeed > 0)
            aiPlayer.currentSpeed--;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetToEscort"></param>
    public void Escort(Transform targetToEscort)
    {
        if (targetToEscort.tag == "Player")
        {
            if (aiPlayer.currentSpeed < targetToEscort.GetComponent<Player>().currentSpeed)
                aiPlayer.currentSpeed++;

            if (aiPlayer.currentSpeed > targetToEscort.GetComponent<Player>().currentSpeed)
                aiPlayer.currentSpeed--;
        }
        else
        {
            if (aiPlayer.currentSpeed < targetToEscort.GetComponent<MotherShip>().currentSpeed)
                aiPlayer.currentSpeed++;

            if (aiPlayer.currentSpeed > targetToEscort.GetComponent<MotherShip>().currentSpeed)
                aiPlayer.currentSpeed--;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="Distance"></param>
    public void Attack(Transform target, float Distance)
    {
        if (Distance < aiPlayer.laserRange)
            if (Time.time > step)
            {
                step = aiPlayer.fireRate + Time.time;
                shoot.Shoot(aiPlayer.damage, aiPlayer.laserMuzzles, aiPlayer.laser);
                aiPlayer.currentLaserCount -= aiPlayer.laserMuzzles.Count;
            }

        if (aiPlayer.missileMuzzles.Count != 0)
            if (Distance < aiPlayer.missileRange)
                if (Time.time > step)
                {
                    step = 5 + Time.time;
                    shoot.Shoot(aiPlayer.damage, aiPlayer.missileMuzzles, aiPlayer.missil);
                    aiPlayer.currentMissileCount--;
                }

        if (aiPlayer.currentLaserCount < aiPlayer.laserMaxCount)
            if (Time.time > step2)
            {
                step2 = aiPlayer.laserRecoverRate + Time.time;
                aiPlayer.currentLaserCount++;
            }
    }

    public void Moves(Transform target)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(target.position);

        if (relativePosition.x > 0)
            right = true;
        else
            right = false;

        if (relativePosition.z > 0)
            front = true;
        else
            front = false;

        if (relativePosition.y > 0)
            over = true;
        else
            over = false;
    }

    public void Maneuvers(bool targetOnRange)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (targetOnRange == true)
        {
            if (Physics.Raycast(ray, out hit, 10.0f))
                tgtLock = true;
            else
                tgtLock = false;

            if (tgtLock == false)
            {
                if (right == true)
                    transform.Rotate(Vector3.forward * -40.0f * Time.deltaTime);
                else
                    transform.Rotate(Vector3.forward * 40.0f * Time.deltaTime);

                if (over == true || front == false)
                    transform.Rotate(Vector3.right * -40 * Time.deltaTime);
                else
                    transform.Rotate(Vector3.right * 40 * Time.deltaTime);
            }
        }
    }
}