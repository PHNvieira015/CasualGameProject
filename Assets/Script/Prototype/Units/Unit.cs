using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler
{
    public List<Stat> Stats;
    public virtual IEnumerator Recover()
    {
        yield return null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [ContextMenu("Generate Stats")]
    void GenerateStats()
    {
        Stats = new List<Stat>();
        for (int i = 0; i < (int)StatType.Dexterity + 1; i++)
        {
            Stat stat = new Stat();
            stat.Type = (StatType)i;
            stat.Value = Random.Range(0, 100);
            Stats.Add(stat);
        }
    }
    public void OnPointerClick(PointerEventData EventData)
    {
        Debug.Log("aaaa"+EventData);
    }
}
