using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LeaveZone : MonoBehaviour
{
    private bool inTrigger;

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            inTrigger = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player") {
            inTrigger = false;
        }
    }

    void Update() {
        if (inTrigger) {
            SceneManager.LoadScene("OutsideStarterHouse");
        }
    }
}
