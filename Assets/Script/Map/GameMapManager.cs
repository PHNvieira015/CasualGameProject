using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapVisualizer mapVisualizer;

    private IReadOnlyList<MapNode> currentMap;
    private MapNode currentNode;

    private void Start()
    {
        if (mapGenerator == null || mapVisualizer == null)
        {
            Debug.LogError("MapGenerator or MapVisualizer not assigned!");
            return;
        }
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        if (mapGenerator == null || mapVisualizer == null) return;

        currentMap = mapGenerator.GenerateMap();
        if (currentMap == null || currentMap.Count == 0)
        {
            Debug.LogError("Failed to generate map!");
            return;
        }

        mapVisualizer.VisualizeMap(currentMap.ToList());
        currentNode = currentMap.FirstOrDefault(n => n.position == Vector2Int.zero);
        UpdateNodeStates();
    }

    public void OnNodeClicked(MapNode node)
    {
        if (node == null || !IsValidNodeSelection(node)) return;
        currentNode = node;
        UpdateNodeStates();

        if (node.nodeBlueprint != null)
        {
            EventManager.Instance?.HandleNodeEvent(node.nodeBlueprint.nodeType);

            // Change state based on node type using your existing states
            switch (node.nodeBlueprint.nodeType)
            {
                case NodeType.MinorEnemy:
                case NodeType.EliteEnemy:
                case NodeType.Boss:
                    // Clear all card holders before starting new battle
                    ClearAllCardHolders();

                    // Hide map screen and show combat screen
                    if (MenuController.Instance != null)
                    {
                        MenuController.Instance.SetScreenActive(MenuController.Screens.MapMenu, false);
                        MenuController.Instance.SetScreenActive(MenuController.Screens.CombatMenu, true);
                    }
                    StateMachine.Instance.ChangeState<LoadState>();
                    break;

                case NodeType.RestSite:
                    // Hide map screen and handle rest site
                    if (MenuController.Instance != null)
                    {
                        MenuController.Instance.SetScreenActive(MenuController.Screens.MapMenu, false);
                        MenuController.Instance.SetScreenActive(MenuController.Screens.RestSite, true);
                    }
                    Debug.Log("Rest site clicked - implement rest state logic");
                    break;

                case NodeType.Treasure:
                    // Hide map screen and handle treasure
                    if (MenuController.Instance != null)
                    {
                        MenuController.Instance.SetScreenActive(MenuController.Screens.MapMenu, false);
                        // You might want to show a treasure screen or go directly to reward
                        StateMachine.Instance.ShowRewardScreen();
                    }
                    Debug.Log("Treasure clicked - implement treasure state logic");
                    break;

                default:
                    Debug.LogWarning($"Unhandled node type: {node.nodeBlueprint.nodeType}");
                    break;
            }
        }
    }

    private void ClearAllCardHolders()
    {
        // Use the static method to clear all card holders
        CardHolder.ClearAllHolders();
    }

    private bool IsValidNodeSelection(MapNode node)
    {
        return currentNode != null &&
               node != null &&
               currentNode.ConnectedNodes != null &&
               currentNode.ConnectedNodes.Contains(node);
    }

    private void UpdateNodeStates()
    {
        if (currentMap == null || mapVisualizer == null) return;

        foreach (var node in currentMap)
        {
            if (node != null)
            {
                mapVisualizer.SetNodeSelectable(node, IsValidNodeSelection(node));
            }
        }

        if (currentNode != null)
        {
            mapVisualizer.SetNodeSelected(currentNode, true);
        }
    }
}