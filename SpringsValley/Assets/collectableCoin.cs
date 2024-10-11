using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectableCoin : MonoBehaviour
{

    public AudioSource source;
    public AudioClip pickupSound;

    // Start is called before the first frame update
    void Start()
    {
    
    GameObject spriteObject = GameObject.Find("Main_Character/Sprite");
    source = spriteObject.GetComponent<AudioSource>();
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            source.PlayOneShot(pickupSound);
            game.coinCount +=1;
            Destroy(gameObject);
        }
    }
}
