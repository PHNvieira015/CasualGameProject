using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public List<Stat> Stats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Stats = new List<Stat> ();
        for (int i = 0; i < (int)StatType.Dexterity + 1; i++)
        {
            Stat stat = new Stat();
            stat.Type = (StatType)i;
            stat.Value = Random.Range(0, 100);
            Stats.Add(stat);
        }
    }
}
