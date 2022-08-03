using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2.0f;

    private Rigidbody playerRb;

    private GameObject focalPoint;

    public bool hasPowerup = false;

    private float powerupStrength = 15.0f;

    public GameObject powerupIndicator;

    public PowerupType currentPowerup = PowerupType.None;

    public GameObject rocketPrefab;

    private GameObject tmpRocket;

    private Coroutine powerupCountdown;

    public float hangTime;

    public float smashSpeed;

    public float explosionForce;

    public float explosionRadius;

    bool smashing = false;

    float floorY;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerup == PowerupType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if (currentPowerup == PowerupType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerup = other.gameObject.GetComponent<Powerup>().powerupType;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountDownRoutine());
        }
    }

    IEnumerator PowerupCountDownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerup = PowerupType.None;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && currentPowerup == PowerupType.Pushback)
        {
            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Debug.Log("Collided with " + other.gameObject.name + " with powerup set to " + currentPowerup.ToString());
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehavior>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        floorY = transform.position.y;

        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        for (var i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        smashing = false;
    }
}
