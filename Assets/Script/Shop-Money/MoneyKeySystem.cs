using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyKeySystem : MonoBehaviour
{
    [System.Serializable]
    public class MoneyKeyData
    {
        public int money;
        public int keys;

        public MoneyKeyData(int initialMoney = 0, int initialKeys = 0)
        {
            money = initialMoney;
            keys = initialKeys;
        }
    }

    [Header("Current Values")]
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int currentKeys = 0;

    [Header("Starting Values")]
    public int startingMoney = 100;
    public int startingKeys = 0;

    // Events for UI updates
    public System.Action<int> OnMoneyChanged;
    public System.Action<int> OnKeysChanged;
    public System.Action<int, int> OnValuesChanged;

    // Singleton instance
    public static MoneyKeySystem Instance { get; private set; }

    public int Money
    {
        get => currentMoney;
        private set
        {
            int oldValue = currentMoney;
            currentMoney = Mathf.Max(0, value);
            if (oldValue != currentMoney)
            {
                OnMoneyChanged?.Invoke(currentMoney);
                OnValuesChanged?.Invoke(currentMoney, currentKeys);
            }
        }
    }

    public int Keys
    {
        get => currentKeys;
        private set
        {
            int oldValue = currentKeys;
            currentKeys = Mathf.Max(0, value);
            if (oldValue != currentKeys)
            {
                OnKeysChanged?.Invoke(currentKeys);
                OnValuesChanged?.Invoke(currentMoney, currentKeys);
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize(startingMoney, startingKeys);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(int money = 0, int keys = 0)
    {
        Money = money;
        Keys = keys;
    }

    // Money methods
    public bool AddMoney(int amount)
    {
        if (amount < 0) return false;
        Money += amount;
        return true;
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0 || Money < amount) return false;
        Money -= amount;
        return true;
    }

    public bool CanAfford(int amount)
    {
        return Money >= amount;
    }

    // Key methods
    public bool AddKeys(int amount)
    {
        if (amount < 0) return false;
        Keys += amount;
        return true;
    }

    public bool SpendKeys(int amount)
    {
        if (amount < 0 || Keys < amount) return false;
        Keys -= amount;
        return true;
    }

    public bool HasKeys(int amount)
    {
        return Keys >= amount;
    }

    // Combined transactions
    public bool CanAfford(int moneyCost, int keyCost)
    {
        return Money >= moneyCost && Keys >= keyCost;
    }

    public bool MakePurchase(int moneyCost, int keyCost)
    {
        if (!CanAfford(moneyCost, keyCost)) return false;
        SpendMoney(moneyCost);
        SpendKeys(keyCost);
        return true;
    }
}