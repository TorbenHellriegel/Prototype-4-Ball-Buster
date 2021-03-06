using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    private Rigidbody enemyRb;

    public float speed = 1;
    public int enemyDifficulty;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        enemyRb = GetComponent<Rigidbody>();
        // If the enemy is a pusher repeatedly try to push the player
        if(gameObject.name == "EnemyPusher(Clone)")
        {
            InvokeRepeating("PushPlayer", 2, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the enemy move towards the player
        Vector3 lookDirection = MoveDirection();
        enemyRb.AddForce(lookDirection * speed);
        
        // Destroy enemys that fall of the platform
        if(transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    // Forcefully pushes toward the player
    private void PushPlayer()
    {
        Vector3 lookDirection = MoveDirection();
        enemyRb.AddForce(lookDirection * speed * 2, ForceMode.VelocityChange);
    }

    // Returns the direction the enemy is supposed to move in
    private Vector3 MoveDirection()
    {
        float lookDirectionX = player.transform.position.x - transform.position.x;
        float lookDirectionZ = player.transform.position.z - transform.position.z;

        return new Vector3(lookDirectionX, 0, lookDirectionZ).normalized;
    }
}
