using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class ModuleDatabase : MonoBehaviour//This class holds prefabs for editorshipmodules
{
    public EditorShipModule Cockpit;

    public List<ShipModule> playZonePrefabs;

    public List<ShipModule> enemyGuns;

    public List<ShipModule> playerGuns;

    public List<EditorShipModule> prefabList;

    Dictionary<int, Module> moduleDataList = new Dictionary<int, Module>();

    public void Awake()
    {
        JsonData data = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Modules.json"));
        ConstructDatabase(data);
    }

    //Extracts the information from the json data file and turns it into a Module object
    void ConstructDatabase(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            int[][] itemlist = new int[data[i]["connectionPositions"].Count][];
            for (int j = 0; j < data[i]["connectionPositions"].Count; j++)
            {
                int[] vec = { (int)data[i]["connectionPositions"][j][0], (int)data[i]["connectionPositions"][j][1] };
                itemlist[j] = vec;
            }
            moduleDataList.Add((int)data[i]["id"], new Module(
            data[i]["mainSprite"].ToString(),
            data[i]["brokenImage"].ToString(),
            data[i]["brokenImage2"].ToString(),
            (int)data[i]["id"],
            (int)data[i]["mass"],
            (int)data[i]["maxHealth"],
            itemlist,
            data[i]["title"].ToString(),
            (int)data[i]["cost"],
            data[i]["description"].ToString(),
            (int)data[i]["requiredLevel"]
        ));
        }
    }


    public Module GetModuleStats(int id)
    {
        Module module;
        moduleDataList.TryGetValue(id, out module);
        return module;
    }

    public EditorShipModule GetEditorModule(int id)//Used in the editor
    {
        foreach (EditorShipModule prefab in prefabList)
        {
            if(prefab.id == id)
            {
                return prefab;
            }
        }
        return null;
    }

    public ShipModule GetShipModule(int id, bool player)//used in the play zone
    {
        if (!player)
        {
            foreach (ShipModule prefab in enemyGuns)
            {
                if (prefab.id == id)
                {
                    return prefab;
                }
            }
        }

        foreach (ShipModule prefab in playerGuns)
        {
            if (prefab.id == id)
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
