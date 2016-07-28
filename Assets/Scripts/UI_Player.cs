using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Player : MonoBehaviour
{
    Camera main;
    public Scrollbar bar;
    float life;

    void Start()
    {
        main = Camera.main;
    }

    void Update()
    {
        if (transform.tag == "Player")
            life = GetComponent<Player>().currentHealth / GetComponent<Player>().health;

        if (transform.tag == "Enemy" || transform.tag == "Ally")
            life = GetComponent<AI_player>().currentHealth / GetComponent<AI_player>().health;
        
        if(transform.tag== "EnemyMotherShip" ||  transform.tag=="AllyMotherShip")
            life = GetComponent<MotherShip>().currentHealth / GetComponent<MotherShip>().health;

        bar.size = life;

        if (transform.tag != "Player")
            bar.transform.parent.transform.LookAt(main.transform);
    }
}

