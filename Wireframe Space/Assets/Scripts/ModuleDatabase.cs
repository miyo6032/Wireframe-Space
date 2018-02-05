using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleDatabase : MonoBehaviour//This class holds prefabs for editorshipmodules
{
    public EditorShipModule Cockpit;

    public List<ShipModule> playZonePrefabs;

    public List<ShipModule> typeSpecificPrefabs;

    public List<EditorShipModule> prefabList;

    public EditorShipModule GetPrefabByID(int id)//Used in the editor
    {

        foreach (EditorShipModule prefab in prefabList)
        {
            if(prefab.representativeModule.id == id)
            {
                return prefab;
            }
        }

        return null;
    }

    public ShipModule GetPrefabByIdPlayZone(int id, string tag)//used in the play zone
    {
        foreach(ShipModule prefab in typeSpecificPrefabs)
        {
            if (prefab.tag == tag && prefab.id == id)
            {
                return prefab;
            }
        }

        foreach (ShipModule prefab in playZonePrefabs)
        {
            if (prefab.id == id)
            {
                return prefab;
            }
        }

        return null;
    }

}
