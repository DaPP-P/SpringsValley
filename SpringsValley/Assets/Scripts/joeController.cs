using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joeController : MonoBehaviour
{

    public Transform target;
    public float speed;
    public GameObject joe;
    public GameObject joeChatBubble;
    public GameObject canTalkToo;

    private bool inTrigger;
    private bool command = false;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
     void Update() {
         float step = speed * Time.deltaTime;
         transform.position = Vector3.MoveTowards(transform.position, target.position, step);
         
         if (joe.transform.position == target.position && !command && inTrigger && Input.GetKeyDown(KeyCode.E)) {
            chat();
            command = true;
         }
     }

     public void chat() {
         canTalkToo.SetActive(false);
        joeChatBubble.SetActive(true);
     }


}
