using System;
using System.Collections;
using UnityEngine;

public class utility : MonoBehaviour
{
    public GameObject player;

    public static IEnumerator DelayedAction(float time, Action action)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, time));
        action?.Invoke();
    }
}
