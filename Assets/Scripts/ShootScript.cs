using UnityEngine;
using System.Collections.Generic;

public class ShootScript : MonoBehaviour
{
    /// <summary>
    /// Método que crea una copia del modelo de proyectil, en este caso un proyectil no guiado.
    /// </summary>
    /// <param name="damage">Variable correspondiente al daño asignado al proyectil.</param>
    /// <param name="muzzles">Lista correspondiente a los lugares donde apareceran los modelos de proyectil.</param>
    /// <param name="proyectile">Variable correspondiente al modelo de proyectil (misil/laser)</param>
    public void Shoot(float damage, List<Transform> muzzles, GameObject proyectile)
    {
        for (int i = 0; i < muzzles.Count; i++)
        {
            GameObject proy = (GameObject)Instantiate(proyectile, muzzles[i].position, muzzles[i].rotation);
            proy.name = proyectile.name;
            proy.SendMessage("Damage", damage / muzzles.Count);
        }
    }

    /// <summary>
    /// Método que crea una copia del modelo de proyectil, en este caso un proyectil guiado.
    /// </summary>
    /// <param name="damage">Variable correspondiente al daño asignado al proyectil.</param>
    /// <param name="muzzles">Lista correspondiente a los lugares donde apareceran los modelos de proyectil.</param>
    /// <param name="proyectile">Variable correspondiente al modelo de proyectil (misil/laser)</param>
    /// <param name="tgt">Variable correspondiente al objetivo a atacar</param>
    public void Shoot(float damage, List<Transform> muzzles, GameObject proyectile, Transform tgt)
    {
        for (int i = 0; i < muzzles.Count; i++)
        {
            GameObject proy = (GameObject)Instantiate(proyectile, muzzles[i].position, muzzles[i].rotation);
            proy.name = proyectile.name;
            proy.SendMessage("Damage", damage / muzzles.Count);

            if (tgt != null)
                proy.SendMessage("SetTarget", tgt);
        }
    }
}
