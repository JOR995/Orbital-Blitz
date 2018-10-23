using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// Class manages the assignment of all enemy gameobjects into different lists
    /// These lists are used to check through the active states of enemies to ensure the correct ones are pooled when needed
    /// This class is also used for when enemies are to be spawned
    /// </summary>

    public static EnemyController enemyControllerInstance;

    [SerializeField]
    Transform flyersShooterParent, flyersBomberParent, seekersParent;

    private Transform flyerRayPointParent;
    private List<GameObject> enemyFlyerShooters, enemyFlyerBombers, enemySeekers, flyerRayPoints;
    private List<EnemyFlyer> flyerShooterList, flyerBomberList;
    private EnemyFlyer flyerShooter1, flyerShooter2, flyerShooter3, flyerShooter4, flyerBomber1, flyerBomber2;


    void Awake()
    {
        enemyControllerInstance = this;
    }


    void Start()
    {
        enemyFlyerShooters = new List<GameObject>();
        foreach (Transform enemy in flyersShooterParent)
        {
            enemyFlyerShooters.Add(enemy.gameObject);
        }

        enemyFlyerBombers = new List<GameObject>();
        foreach (Transform enemy in flyersBomberParent)
        {
            enemyFlyerBombers.Add(enemy.gameObject);
        }

        enemySeekers = new List<GameObject>();
        foreach (Transform enemy in seekersParent)
        {
            enemySeekers.Add(enemy.gameObject);
        }

        flyerRayPointParent = enemyFlyerShooters[0].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerShooter1 = new EnemyFlyer(enemyFlyerShooters[0], flyerRayPoints, "Shooter", "Right", 20.0f);

        flyerRayPointParent = enemyFlyerShooters[1].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerShooter2 = new EnemyFlyer(enemyFlyerShooters[1], flyerRayPoints, "Shooter", "Left", 25.0f);

        flyerRayPointParent = enemyFlyerShooters[2].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerShooter3 = new EnemyFlyer(enemyFlyerShooters[2], flyerRayPoints, "Shooter", "Left", 30.0f);

        flyerRayPointParent = enemyFlyerShooters[3].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerShooter4 = new EnemyFlyer(enemyFlyerShooters[3], flyerRayPoints, "Shooter", "Right", 15.0f);

        flyerShooterList = new List<EnemyFlyer>();
        flyerShooterList.Add(flyerShooter1);
        flyerShooterList.Add(flyerShooter2);
        flyerShooterList.Add(flyerShooter3);
        flyerShooterList.Add(flyerShooter4);

        flyerRayPointParent = enemyFlyerBombers[0].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerBomber1 = new EnemyFlyer(enemyFlyerBombers[0], flyerRayPoints, "Bomber", "Right", 50.0f);

        flyerRayPointParent = enemyFlyerBombers[1].transform.GetChild(2);
        flyerRayPoints = new List<GameObject>();
        foreach (Transform point in flyerRayPointParent)
        {
            flyerRayPoints.Add(point.gameObject);
        }
        flyerBomber2 = new EnemyFlyer(enemyFlyerBombers[1], flyerRayPoints, "Bomber", "Right", 60.0f);

        flyerBomberList = new List<EnemyFlyer>();
        flyerBomberList.Add(flyerBomber1);
        flyerBomberList.Add(flyerBomber2);
    }


    void FixedUpdate()
    {
        foreach (EnemyFlyer flyer in flyerShooterList)
        {
            flyer.MoveForward();
        }
        foreach (EnemyFlyer flyer in flyerBomberList)
        {
            flyer.MoveForward();
        }
    }


    public void SpawnEnemy(Transform location, string type, int count)
    {
        for (int x = 0; x < count; x++)
        {
            switch (type)
            {
                case "FlyerShooter":
                    foreach (EnemyFlyer flyer in flyerShooterList)
                    {
                        if (!flyer.Alive)
                        {
                            flyer.Spawn();
                            StartCoroutine(flyer.Attack(Random.Range(1.5f, 3.0f)));
                            break;
                        }
                    }
                    break;
                case "FlyerBomber":
                    foreach (EnemyFlyer flyer in flyerBomberList)
                    {
                        if (!flyer.Alive)
                        {
                            flyer.Spawn();
                            StartCoroutine(flyer.Attack(Random.Range(3.0f, 4.5f)));
                            break;
                        }
                    }
                    break;
                case "Seeker":
                    foreach (GameObject enemy in enemySeekers)
                    {
                        if (!enemy.activeSelf)
                        {
                            enemy.transform.position = location.position + new Vector3(0, 0, -3.0f);
                            enemy.SetActive(true);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }


    public void DamageEnemy(GameObject enemyObject, int damage)
    {
        bool found = false;

        foreach (EnemyFlyer enemy in flyerShooterList)
        {
            if (enemy.ThisParent == enemyObject)
            {
                enemy.TakeDamage(damage);
                found = true;
                break;
            }
        }

        if (!found)
        {
            foreach (EnemyFlyer enemy in flyerBomberList)
            {
                if (enemy.ThisParent == enemyObject)
                {
                    enemy.TakeDamage(damage);
                    break;
                }
            }
        }
    }


    public void StopEnemy(GameObject enemyObject)
    {
        bool found = false;

        foreach (EnemyFlyer enemy in flyerShooterList)
        {
            if (enemy.ThisParent == enemyObject)
            {
                enemy.Stop();
                found = true;
                break;
            }
        }

        if (!found)
        {
            foreach (EnemyFlyer enemy in flyerBomberList)
            {
                if (enemy.ThisParent == enemyObject)
                {
                    enemy.Stop();
                    break;
                }
            }
        }
    }
}