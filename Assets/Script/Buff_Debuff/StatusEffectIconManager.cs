using UnityEngine;
using System.Collections;

public class StatusEffectIconManager : MonoBehaviour
{
    private BuffDebuffHolder _holder;

    void Awake()
    {
        _holder = GetComponentInChildren<BuffDebuffHolder>();
    }

    void OnEnable()
    {
        // The BuffDebuffHolder now handles all the detection and UI updates
        // This manager is kept for future expansion if needed
    }

    public void RefreshUI()
    {
        _holder?.RefreshUI();
    }

    public void ClearAllIcons()
    {
        _holder?.ClearAll();
    }
}