using UnityEngine;

public class PlayerLoot : MonoBehaviour
{

    public static int coinAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void coinIncrease (int amount) {
        coinAmount += amount;
    }

    public static void coinDecrease (int amount) {
        if ((coinAmount - amount) < 0) {
            coinAmount = 0;
        } else {
            coinAmount -= amount;
        }
    }

    public static int getCoinAmount() {
        return coinAmount;
    }
}
