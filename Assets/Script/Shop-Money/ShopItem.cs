using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int moneyCost;
    public int keyCost;
    public Sprite itemIcon;
    public bool isPurchased;

    // You can add more properties depending on what you're selling
    public Card itemCard; // if selling cards
    public GameObject itemPrefab; // if selling prefabs
    public int healthRestore; // if selling potions
    public int maxHealthIncrease; // if selling upgrades

    public ShopItem(string name, int money, int keys)
    {
        itemName = name;
        moneyCost = money;
        keyCost = keys;
        isPurchased = false;
    }
}