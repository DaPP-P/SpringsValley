using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class utility : MonoBehaviour
{
    public GameObject player;
    private PlayerHealth playerHealth;

    //todo DELETE AT SOME POINT THIS IS JUST FOR TESTING
    public GameObject testobject;

    void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        testobject = testobject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (playerHealth != null)
            {
                playerHealth.Damage(10, gameObject);
            }
        }
    }

    //        StartCoroutine(Utility.DelayedAction(FLOAT TIME, () =>
    //    {
    //       ACTION PREFORMED AFTER TIME
    //    }));
    public static IEnumerator DelayedAction(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
