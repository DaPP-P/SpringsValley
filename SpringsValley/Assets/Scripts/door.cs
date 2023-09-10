using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class door : MonoBehaviour
{
    Scene currentScene;
    private bool inTrigger;
    public GameObject interactionObject;

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {

            inTrigger = true;
            interactionObject.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player") {
            inTrigger = false;
            interactionObject.SetActive(false);

        }
    }

    void Update() {

        if (inTrigger && Input.GetKeyDown(KeyCode.E)) {
            
            Scene currentScene = SceneManager.GetActiveScene(); 

            if (currentScene.name == "StarterHouse") {
                SceneManager.LoadScene("OutsideStarterHouse");
            }

            if (currentScene.name == "OutsideStarterHouse") {
                SceneManager.LoadScene("StarterHouse");
            }
        }
    }

}
