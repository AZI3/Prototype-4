using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Transform target;

    private float speed = 15.0f;

    private bool homing;

    private float rocketStrength = 15.0f;

    private float aliveTimer = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (homing && target != null)
        {
            Vector3 moveDirection = (target.transform.position - transform.position);
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (target != null)
        {
            if (other.gameObject.CompareTag(target.tag))
            {
                Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -other.GetContact(0).normal;
                rigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
        }
    }

    public void Fire(Transform homingTarget)
    {
        target = homingTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }
}
