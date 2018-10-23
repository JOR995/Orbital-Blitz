using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyer
{
    /// <summary>
    /// Managed the AI behaviour of enemy ships once they are spawned in
    /// Positioning in front of the player and method of attacking depends on the enemy type - this variable is assigned at spawn
    /// </summary>

    private List<GameObject> rayPoints;
    private GameObject thisShip, orbit, thisParent, player, playerShip, playerTarget;
    private Collider thisCollider;
    private LineRenderer targetLine;

    private bool alive, active, moveLeft, moveRight, canAttack, explode;
    private float forwardSpeed, sideSpeed, activateDistance, distanceFromPlayer;
    private int health;
    private string enemyType, defaultDirection;

    public EnemyFlyer(GameObject parent, List<GameObject> points, string type, string direction, float distance)
    {
        thisParent = parent;
        enemyType = type;
        defaultDirection = direction;
        activateDistance = distance;

        player = GameObject.FindGameObjectWithTag("Player");
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        playerTarget = playerShip.transform.GetChild(0).gameObject;
        thisShip = thisParent.transform.GetChild(0).gameObject;
        orbit = thisParent.transform.GetChild(1).gameObject;
        thisCollider = thisShip.GetComponent<Collider>();

        targetLine = thisParent.GetComponent<LineRenderer>();
        targetLine.positionCount = 2;
        targetLine.enabled = false;

        rayPoints = points;

        explode = true;
        alive = false;
        active = false;
        moveRight = false;
        moveLeft = false;
        forwardSpeed = Player.playerInstance.PlayerSpeed + 40;
        sideSpeed = 220;
        health = 3;
    }

    /// <summary>
    /// When an enemy is spawned, it will move at a faster speed than the player in order to get ahead of them
    /// Once ahead, the enemy will slow down to match the players speed and can begin attacking
    /// </summary>
    public void Spawn()
    {
        thisParent.SetActive(true);
        thisParent.transform.position = player.transform.position + new Vector3(0, 0, -6);
        thisParent.transform.eulerAngles = (player.transform.eulerAngles + new Vector3(0, 0, Random.Range(100, 260)));

        alive = true;
        active = false;
        explode = true;
        health = 3;
        forwardSpeed = Player.playerInstance.PlayerSpeed + 40;
    }


    public void MoveForward()
    {
        if (alive)
        {
            distanceFromPlayer = Mathf.Round(Vector3.Distance(playerShip.transform.position, thisShip.transform.position));

            if (!active)
            {
                if (distanceFromPlayer >= activateDistance)
                {
                    active = true;
                    forwardSpeed = Player.playerInstance.PlayerSpeed;
                }
            }

            if (distanceFromPlayer >= 100.0f)
            {
                explode = false;
                TakeDamage(health);
            }

            Debug.DrawRay((thisShip.transform.position + new Vector3(0, 0.6f, -1.0f)), (playerTarget.transform.position - thisShip.transform.position), Color.magenta);

            thisParent.transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
            CastPathRays();
            targetLine.SetPosition(0, thisShip.transform.position);
            targetLine.SetPosition(1, playerTarget.transform.position);
        }
    }

    /// <summary>
    /// Method called to continuously generate raycasts from the enemy ship object
    /// </summary>
    void CastPathRays()
    {
        RaycastHit hit;
        //Layer mask used to determine which objects are to be ignored by raycasts, this one is used to only detect objects in layer 10 (Hazards)
        //Change this if Hazards is moved to a different layer
        int layerMask = 1 << 10;

        //Drawrays for debugging, these rays are visible in editor during run time. Set to have same values as Raycasts.
        #region DebugRays
        //Debug.DrawRay(rayPoints[0].transform.position, rayPoints[0].transform.forward * 30, Color.blue);
        //Debug.DrawRay(rayPoints[1].transform.position, rayPoints[1].transform.forward * 30, Color.blue);
        //Debug.DrawRay(rayPoints[2].transform.position, rayPoints[2].transform.forward * 25, Color.red);
        //Debug.DrawRay(rayPoints[7].transform.position, rayPoints[7].transform.forward * 25, Color.red);
        //Debug.DrawRay(rayPoints[3].transform.position, rayPoints[3].transform.forward * 25, Color.red);
        //Debug.DrawRay(rayPoints[8].transform.position, rayPoints[8].transform.forward * 25, Color.red);
        //Debug.DrawRay(rayPoints[4].transform.position, rayPoints[4].transform.forward * 25, Color.yellow);
        //Debug.DrawRay(rayPoints[5].transform.position, rayPoints[5].transform.forward * 25, Color.yellow);
        //Debug.DrawRay(rayPoints[6].transform.position, rayPoints[6].transform.forward * 10, Color.green);
        #endregion

        if (!active)
        {
            if (Physics.Raycast((rayPoints[1].transform.position), rayPoints[3].transform.forward, out hit, 20.0f, 1 << 9))
            {
                moveRight = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[0].transform.position), rayPoints[2].transform.forward, out hit, 20.0f, 1 << 9))
            {
                moveLeft = true;
                MoveSideways();
            }
        }

        //Raycasts used in three stages starting from the base of the ship collider, moving up towards the center of the tube to the top of the ship collider.
        //Order of priority starts at bottom level raycasts as obstacles found by these would require the largest side movement to avoid, priority then moves to next set of rays if the first is not hitting anything
        //When raycasts hit a hazard they will call the MoveSideways method to move the ship sideways in one direction, direction to move away from the side of the ship that ray was.
        //To try and prevent multiple enemies following the same paths in a line, have added a default direction variable - this is the direction the enemy will move in first when it detects obstacles with rays on both sides.
        if (defaultDirection == "Right")
        {
            #region firstSetRays
            //Raycast used and offset to the right of the ship, uses the ships transform, a set distance and the layerMask
            if (Physics.Raycast((rayPoints[1].transform.position), rayPoints[1].transform.forward, out hit, 30.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[0].transform.position), rayPoints[0].transform.forward, out hit, 30.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            #endregion

            #region secondSetRays
            else if (Physics.Raycast((rayPoints[3].transform.position), rayPoints[3].transform.forward, out hit, 25.0f, layerMask) || Physics.Raycast((rayPoints[8].transform.position), rayPoints[8].transform.forward, out hit, 25.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[2].transform.position), rayPoints[2].transform.forward, out hit, 25.0f, layerMask) || Physics.Raycast((rayPoints[7].transform.position), rayPoints[7].transform.forward, out hit, 25.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            #endregion

            #region thirdSetRays
            else if (Physics.Raycast((rayPoints[5].transform.position), rayPoints[5].transform.forward, out hit, 25.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[4].transform.position), rayPoints[4].transform.forward, out hit, 25.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            #endregion
            else if (Physics.Raycast((rayPoints[6].transform.position), rayPoints[6].transform.forward, out hit, 5.0f, 1 << 9))
            {
                moveRight = true;
                MoveSideways();
            }
        }
        else if (defaultDirection == "Left")
        {
            #region firstSetRays
            //Raycast used and offset to the right of the ship, uses the ships transform, a set distance and the layerMask
            if (Physics.Raycast((rayPoints[0].transform.position), rayPoints[0].transform.forward, out hit, 30.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[1].transform.position), rayPoints[1].transform.forward, out hit, 30.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            #endregion

            #region secondSetRays
            else if (Physics.Raycast((rayPoints[2].transform.position), rayPoints[2].transform.forward, out hit, 25.0f, layerMask) || Physics.Raycast((rayPoints[7].transform.position), rayPoints[7].transform.forward, out hit, 25.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[3].transform.position), rayPoints[3].transform.forward, out hit, 25.0f, layerMask) || Physics.Raycast((rayPoints[8].transform.position), rayPoints[8].transform.forward, out hit, 25.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            #endregion

            #region thirdSetRays
            else if (Physics.Raycast((rayPoints[4].transform.position), rayPoints[4].transform.forward, out hit, 25.0f, layerMask))
            {
                moveLeft = true;
                MoveSideways();
            }
            else if (Physics.Raycast((rayPoints[5].transform.position), rayPoints[5].transform.forward, out hit, 25.0f, layerMask))
            {
                moveRight = true;
                MoveSideways();
            }
            #endregion
            else if (Physics.Raycast((rayPoints[6].transform.position), rayPoints[6].transform.forward, out hit, 5.0f, 1 << 9))
            {
                moveRight = true;
                MoveSideways();
            }
        }

        //If not raycasts hit any obstacles then no sideways movement is made
        else
        {
            moveLeft = false;
            moveRight = false;
        }

    }

    /// <summary>
    /// Called when raycasts hit and obstacle in front of the enemy.
    /// Will move sideways in a chosen direction depending on which direction boolean variable is true.
    /// </summary>
    void MoveSideways()
    {
        if (moveLeft)
        {
            thisParent.transform.RotateAround(orbit.transform.position, Vector3.back, sideSpeed * Time.deltaTime);
        }
        if (moveRight)
        {
            thisParent.transform.RotateAround(orbit.transform.position, Vector3.forward, sideSpeed * Time.deltaTime);
        }
        moveLeft = false;
        moveRight = false;
    }

    /// <summary>
    /// Called after set intervals to see if the enemy can attack at the player.
    /// </summary>
    public IEnumerator Attack(float attackDelay)
    {
        if (enemyType == "Shooter")
        {
            while (alive)
            {
                yield return new WaitForSeconds(attackDelay);

                RaycastHit hit;

                //Casts a ray from the enemy towards the player, if it hits (enemy has clear line of sight and player is in range) the enemy will shoot a bullet towards the player's position
                if (Physics.Raycast((thisShip.transform.position + new Vector3(0, 0.6f, -1.0f)), (playerTarget.transform.position - thisShip.transform.position), out hit, 50.0f))
                {
                    if (hit.transform.tag == "Player")
                    {
                        targetLine.enabled = true;
                        yield return new WaitForSeconds(0.5f);

                        BulletPool.bulletPoolInstance.FireBullet(thisShip.transform, playerTarget.transform, thisCollider, 0);
                        targetLine.enabled = false;
                    }
                }
            }
        }
        else if (enemyType == "Bomber")
        {
            while (alive)
            {
                yield return new WaitForSeconds(attackDelay);

                EnemyController.enemyControllerInstance.SpawnEnemy(thisShip.transform, "Seeker", 1);
            }
        }
    }

    /// <summary>
    /// Called when the enemy takes damage from any source.
    /// Will subtract the amount of damage taken from the current health, if health then equals or is less than zero, the enemy dies.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            alive = false;
            active = false;
            thisParent.SetActive(false);
            if (explode)
            {
                BulletPool.FindExplosion(thisShip);
                Player.playerInstance.GetComponent<AudioSource>().PlayOneShot(Player.playerInstance.Explosion, 1.3F);
            }
        }
    }


    public void Stop()
    {
        alive = false;
    }


    #region publicAccessors
    public string EnemyType
    {
        get
        {
            return enemyType;
        }
    }

    public float ActivateDistance
    {
        get
        {
            return activateDistance;
        }
    }

    public bool Alive
    {
        get
        {
            return alive;
        }
    }

    public bool Active
    {
        get
        {
            return active;
        }
    }

    public GameObject ThisParent
    {
        get
        {
            return thisParent;
        }
    }
    #endregion
}
