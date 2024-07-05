using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoCollisionHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tornado has collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("SkeletonEnemy"))
        {
            Debug.Log("Player has been hit by a Skeleton!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SkeletonEnemy"))
        {
            Debug.Log("Player has been hit by a Skeleton!");
        }
    }
}
