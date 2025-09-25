// Simple usage without ShopItem class
using UnityEngine;

public class Shop : MonoBehaviour
{
    public void BuyHealthPotion()
    {
        if (MoneyKeySystem.Instance.MakePurchase(50, 0)) // 50 money, 0 keys
        {
            Debug.Log("Health potion purchased!");
            // Add health potion to inventory
        }
    }

    public void BuyMagicKey()
    {
        if (MoneyKeySystem.Instance.MakePurchase(100, 0)) // 100 money, 0 keys
        {
            Debug.Log("Magic key purchased!");
            MoneyKeySystem.Instance.AddKeys(1); // Actually add a key
        }
    }
}