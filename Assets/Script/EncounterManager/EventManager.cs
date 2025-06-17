using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleNodeEvent(NodeType nodeType)
    {
        switch (nodeType)
        {
            case NodeType.Event:
                Debug.Log("Event node clicked! This will trigger special events later.");
                MenuController.Instance.ShowMessage("Special event triggered!");
                break;

            case NodeType.MinorEnemy:
            case NodeType.EliteEnemy:
            case NodeType.Boss:
                EncounterManager.Instance.StartEncounter(GetEncounterType(nodeType));
                break;

            case NodeType.RestSite:
                MenuController.Instance.ChangeScreen(MenuController.Screens.RestSite);
                Debug.Log("Resting at campfire");
                break;

            case NodeType.Store:
                MenuController.Instance.ChangeScreen(MenuController.Screens.Shop);
                Debug.Log("Entered shop");
                break;

            case NodeType.Treasure:
                MenuController.Instance.ChangeScreen(MenuController.Screens.Reward);
                Debug.Log("Found treasure!");
                break;

            default:
                Debug.LogWarning($"Unknown node type: {nodeType}");
                break;
        }
    }

    private EncounterType GetEncounterType(NodeType nodeType)
    {
        return nodeType switch
        {
            NodeType.MinorEnemy => EncounterType.Normal,
            NodeType.EliteEnemy => EncounterType.Elite,
            NodeType.Boss => EncounterType.Boss,
            _ => EncounterType.Normal
        };
    }
}