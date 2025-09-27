using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("UI References - Multiple Screens")]
    [SerializeField] private List<TextMeshProUGUI> moneyTexts = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> keysTexts = new List<TextMeshProUGUI>();
    [SerializeField] private string moneyFormat = "{0}";
    [SerializeField] private string keysFormat = "{0}";

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
                UpdateAllMoneyDisplays();
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
                UpdateAllKeysDisplays();
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

    void Start()
    {
        // Initial UI update for all displays
        UpdateAllDisplays();
    }

    public void Initialize(int money = 0, int keys = 0)
    {
        Money = money;
        Keys = keys;
        UpdateAllDisplays();
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

    // UI Display Methods for Multiple References
    private void UpdateAllMoneyDisplays()
    {
        foreach (var moneyText in moneyTexts)
        {
            if (moneyText != null)
            {
                moneyText.text = string.Format(moneyFormat, currentMoney);
            }
        }
    }

    private void UpdateAllKeysDisplays()
    {
        foreach (var keysText in keysTexts)
        {
            if (keysText != null)
            {
                keysText.text = string.Format(keysFormat, currentKeys);
            }
        }
    }

    private void UpdateAllDisplays()
    {
        UpdateAllMoneyDisplays();
        UpdateAllKeysDisplays();
    }

    // Public methods to manage UI references
    public void AddMoneyTextReference(TextMeshProUGUI newMoneyText)
    {
        if (newMoneyText != null && !moneyTexts.Contains(newMoneyText))
        {
            moneyTexts.Add(newMoneyText);
            newMoneyText.text = string.Format(moneyFormat, currentMoney);
        }
    }

    public void AddKeysTextReference(TextMeshProUGUI newKeysText)
    {
        if (newKeysText != null && !keysTexts.Contains(newKeysText))
        {
            keysTexts.Add(newKeysText);
            newKeysText.text = string.Format(keysFormat, currentKeys);
        }
    }

    public void RemoveMoneyTextReference(TextMeshProUGUI moneyTextToRemove)
    {
        if (moneyTexts.Contains(moneyTextToRemove))
        {
            moneyTexts.Remove(moneyTextToRemove);
        }
    }

    public void RemoveKeysTextReference(TextMeshProUGUI keysTextToRemove)
    {
        if (keysTexts.Contains(keysTextToRemove))
        {
            keysTexts.Remove(keysTextToRemove);
        }
    }

    public void ClearAllMoneyTextReferences()
    {
        moneyTexts.Clear();
    }

    public void ClearAllKeysTextReferences()
    {
        keysTexts.Clear();
    }

    // Method to force UI refresh
    public void RefreshAllUI()
    {
        UpdateAllDisplays();
    }

    // Reset methods
    public void ResetToDefault()
    {
        Money = startingMoney;
        Keys = startingKeys;
    }

    public void ResetToValues(int money, int keys)
    {
        Money = money;
        Keys = keys;
    }

    // Debug method to see how many references we have
    public void PrintReferenceCount()
    {
        Debug.Log($"Money Texts: {moneyTexts.Count}, Keys Texts: {keysTexts.Count}");
    }
}