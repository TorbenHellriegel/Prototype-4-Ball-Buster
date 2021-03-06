using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public GameObject powerupIndicator;
    public GameObject powerupIndicator2;
    public GameObject bullet;
    public GameObject spawner;
    public SpawnManager spawnManager;
    public Text waveCounterText;
    public Text pushCounterText;

    public bool hasPowerupMulti = false;
    public bool hasPowerupShoot = false;
    private float powerupMultiStrength = 15;
    public float speed = 1;
    public float pushForce = 1;
    public float pushCounter;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        spawnManager = spawner.GetComponent<SpawnManager>();
        pushCounter = 0;
        InvokeRepeating("ShootEnemy", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Moves the powerup along with the player
        powerupIndicator.gameObject.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
        powerupIndicator2.gameObject.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);

        // Move the player 
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        // Gives the player a stong push in the direction the camera is facing
        if(Input.GetKeyDown(KeyCode.Space) && pushCounter > 0)
        {
            playerRb.AddForce(focalPoint.transform.forward * forwardInput * pushForce, ForceMode.Impulse);
            pushCounter--;
        }

        // Update the ui to display wave and push counter
        waveCounterText.text = "Wave " + spawnManager.waveNumber;
        pushCounterText.text = "X " + pushCounter;

        // If the player falls down restart the game
        if(transform.position.y < -10)
        {
            spawnManager.waveNumber = Mathf.Max(1, spawnManager.waveNumber - 2);
            transform.position = new Vector3(0, 0.2f, 0);
            playerRb.velocity = new Vector3(0, 0, 0);
        }
    }

    // Checks for trigges
    private void OnTriggerEnter(Collider other)
    {
        // If the player collides with multiply powerup activate it
        if(other.CompareTag("PowerupMulti"))
        {
            hasPowerupMulti = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            StopCoroutine("PowerupCountdownRoutine");
            StartCoroutine("PowerupCountdownRoutine");
        }
        // If the player collides with shoot powerup activate it
        else if(other.CompareTag("PowerupShoot"))
        {
            hasPowerupShoot = true;
            powerupIndicator2.gameObject.SetActive(true);
            Destroy(other.gameObject);
            StopCoroutine("Powerup2CountdownRoutine");
            StartCoroutine("Powerup2CountdownRoutine");
        }
        // If the player collides with push powerup increase usability counter
        else if(other.CompareTag("PowerupPush"))
        {
            pushCounter += 3;
            Destroy(other.gameObject);
        }
    }

    // Counts down until the powerup dissapears
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerupMulti = false;
        powerupIndicator.gameObject.SetActive(false);
    }
    
    // Counts down until the powerup dissapears
    IEnumerator Powerup2CountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerupShoot = false;
        powerupIndicator2.gameObject.SetActive(false);
    }

    // Checks for collisions
    private void OnCollisionEnter(Collision collision)
    {
        // If player collides with enemy while having powerup repell enemy
        if(collision.gameObject.CompareTag("Enemy") && hasPowerupMulti)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPLayer = collision.gameObject.transform.position - transform.position;
            enemyRigidbody.AddForce(awayFromPLayer * powerupMultiStrength, ForceMode.Impulse);
        }
    }

    // Shoot projectiles at the enemys when the powerup is active
    private void ShootEnemy()
    {
        if(hasPowerupShoot)
        {
            // Find all enemys
            Enemy[] enemys = FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemys)
            {
                // Instantiate a bullet and aim it at an enemy
                GameObject currentBullet = Instantiate(bullet, transform.position + new Vector3(0, 1, 0), transform.rotation);
                BulletAim bulletScript = currentBullet.GetComponent<BulletAim>();
                bulletScript.enemyPosition = enemy.gameObject.transform.position;
            }
        }
    }
}
