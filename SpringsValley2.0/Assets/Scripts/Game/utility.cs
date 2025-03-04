using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class utility : MonoBehaviour
{
    public GameObject player;


    void Start()
    {

    }

    void Update()
    {

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
