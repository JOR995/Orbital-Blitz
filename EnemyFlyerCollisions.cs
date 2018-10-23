using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyerCollisions : MonoBehaviour
{
    /// <summary>
    /// Manages collision detection and handling for flyer type enemy ships
    /// </summary>

    private GameObject parent;


    void Awake()
    {
        parent = transform.parent.gameObject;
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerShip"))
        {
            EnemyController.enemyControllerInstance.DamageEnemy(parent, 3);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SectionGate") && other.transform.parent.name == "BossSpawn")
        {
            EnemyController.enemyControllerInstance.StopEnemy(parent);
        }
       else if (other.gameObject.CompareTag("Enemy"))
        {
            //EnemyController.enemyControllerInstance.DamageEnemy(parent, 3);
        }
    }

}
