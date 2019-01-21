using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTracker : MonoBehaviour {

    // this is where we will store the units in combat
    public List<GameObject> combatants = null;
    TileMap map;


    void Start ()
    {
        map = GetComponent<TileMap>();
        StartBattle();
	}
	
	// Update is called once per frame
	void Update () {
        if(combatants != null)
            map.selectedUnit = combatants[0];
    }

    public void AddUnit(GameObject unit)
    {
        combatants.Add(unit);
    }

    public void CycleCombatants()
    {
        combatants.Add(combatants[0]);
        combatants.Remove(combatants[combatants.Count - 1]);

        map.selectedUnit = combatants[0];
        map.SetupUnit();
    }

    public void StartBattle()
    {
        combatants.AddRange(GameObject.FindGameObjectsWithTag("Combatant"));

        combatants.Sort();
        map.selectedUnit = combatants[0];
    }
}
