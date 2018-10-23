using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeeker : MonoBehaviour
{
    /// <summary>
    /// Controls behaviour for seeker enemy types
    /// This enemy acts as a mine which remains stationary until the player approaches
    /// At which point the enemy will start to move towards the player in an attempt to detonate on them
    /// </summary>

    private GameObject player, playerShip;
    private Vector3 startPos;

    private bool alive, active;
    private float speed;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }


    void Start()
    {
        alive = false;
        active = false;
        speed = 20.0f;
    }


    void OnEnable()
    {
        alive = true;
    }

    
    void OnDisable()
    {
        alive = false;
        active = false;
    }


    void FixedUpdate()
    {
        if (active)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerShip.transform.position, (speed * Time.deltaTime));
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
        set
        {
            active = value;
        }
    }
}

