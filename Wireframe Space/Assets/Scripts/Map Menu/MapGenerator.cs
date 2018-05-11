using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Generates the map in the map menu, as well as specific game levels randomly
public class MapGenerator : MonoBehaviour {

    Dictionary<Vector2, MapNodeVertex> mapNodes = new Dictionary<Vector2, MapNodeVertex>();

    public MapNode nodePrefab;

    public GameObject link0d;

    public GameObject link60d;

    public GameObject link120d;

    public Transform transformPrefab;

    Transform helperTransform;

    MapNodeVertex origin;

    Vector2[] hexPositions = { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0) };

    public void Start()
    {
        /*FileStream loadFile;
        BinaryFormatter bf = new BinaryFormatter();
        loadFile = File.Create(Application.persistentDataPath + "/map" + MainMenu.instance.profile + ".dat");//Creates a brand new file for the maps
        List<Map> newList = new List<Map>();

        newList.Add(nodePrefab.map);

        bf.Serialize(loadFile, newList);
        loadFile.Close();*/
    }
    
    public void OpenMap()//loads map
    {
        helperTransform = Instantiate(transformPrefab);
        helperTransform.SetParent(transform);
        helperTransform.transform.localScale = new Vector3(1, 1, 1);
        helperTransform.transform.position = transform.position;

        FileStream loadFile;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/map" + MainMenu.instance.profile + ".dat"))
        {
            loadFile = File.Open(Application.persistentDataPath + "/map" + MainMenu.instance.profile + ".dat", FileMode.Open);
        }
        else
        {
            loadFile = File.Create(Application.persistentDataPath + "/map" + MainMenu.instance.profile + ".dat");//Creates a brand new file for the maps
            List<Map> newList = new List<Map>();

            newList.Add(NewMap(nodePrefab.map, Vector2.zero, Random.Range(7, 10), 4, 10));

            bf.Serialize(loadFile, newList);
            loadFile.Close();
            OpenMap();
            return;
        }

        List<Map> nodes = (List<Map>)bf.Deserialize(loadFile);
        loadFile.Close();

        foreach(Map map in nodes)//Populate the dictionary
        {
            MapNodeVertex vertex = NewNode(map);

            if(vertex.mapNode.xyPos == Vector2.zero)
            {
                origin = vertex;
            }
        }

        foreach(KeyValuePair<Vector2, MapNodeVertex> kv in mapNodes)//Assign all previous links to all completed nodes
        {
            if (kv.Value.mapNode.map.arenaComplete)
            {
                GenerateLinks(kv.Value.mapNode.xyPos);
            }
        }

    }

    public void CompleteNode(Map map)
    {
        MapNodeVertex vertex;
        mapNodes.TryGetValue(new Vector2(map.xPos, map.yPos), out vertex);
        vertex.mapNode.SectorCleared();
    }

    MapNodeVertex NewNode(Map map)
    {
        MapNode instance = Instantiate(nodePrefab);
        instance.transform.SetParent(helperTransform);
        instance.transform.localScale = new Vector3(1, 1, 1);
        instance.map = map;
        instance.xyPos = new Vector2(map.xPos, map.yPos);
        Editor.instance.SetHexPositon(
            instance.xyPos,
            helperTransform.transform.position, 
            instance.gameObject, 
            Editor.instance.unitSize * MainMenu.instance.globalScale.localScale.x);
        MapNodeVertex vertex = new MapNodeVertex(instance, false);
        mapNodes.Add(vertex.mapNode.xyPos, vertex);
        return vertex;
    }

    public void GenerateNewNodes(MapNode node)//Called when the player defeats the previous node
    {
        if (node.map.arenaComplete) return;

        MapNodeVertex vertex;
        mapNodes.TryGetValue(node.xyPos, out vertex);
        if (vertex == origin)//For the first node, add three in order
        {
            Vector2[] nodePos = { new Vector2(3, 0), new Vector2(-3, 3), new Vector2(0, -3) };

            foreach (Vector2 v in nodePos)
            {
                Vector2 newPos = new Vector2(v.x + node.map.xPos, v.y + node.map.yPos);
                Map map = NewMap(node.map, newPos, Random.Range(10, 15), 8, 10);
                NewNode(map);
            }

        }
        else
        {
            foreach (Vector2 pos in hexPositions)//Add up to three nodes going outward
            {
                Vector2 newPos = vertex.mapNode.xyPos + pos * 3;
                if (Vector2.SqrMagnitude(XYtoHex(vertex.mapNode.xyPos)) + 0.5f < Vector2.SqrMagnitude(XYtoHex(newPos)))//If the position is going outward
                {
                    if (!mapNodes.ContainsKey(newPos))//Generate new nodes
                    {
                        Map map = GenerateMap(node.map, newPos);
                        NewNode(map);
                    }
                }
            }
        }

        GenerateLinks(node.xyPos);
    }

    //A map generating algorithm that set random difficulty, size, and stuff like that
    Map GenerateMap(Map map, Vector2 newPos)
    {
        float scaleDifficulty = Random.Range(1.5f, 2f);

        int shipSize = (int)(map.shipSize * scaleDifficulty);
        int numShips = (int)(Random.Range(10, 30) / scaleDifficulty);
        int reward = (int)(Mathf.Sqrt(map.shipSize * scaleDifficulty));

        return NewMap(map, newPos, numShips, shipSize, reward);
    }

    //Generates the map and the ships, which are generated by three arena sizes - small, med, and large
    Map NewMap(Map map, Vector2 newPos, int numShips, int shipSize, int reward)
    {
        List<ShipSave> enemyShips = new List<ShipSave>();

        List<ShipSave> possibleShips = GetPossibleShips(shipSize);
        
        for(int i = 0; i < numShips; i++)
        {
             ShipSave random = possibleShips[Random.Range(0, possibleShips.Count)];
             enemyShips.Add(random);
        }

        return new Map(enemyShips, shipSize, (int)newPos.x, (int)newPos.y, reward, false);
    }

    List<ShipSave> GetPossibleShips(float shipPoints)//Gets possible ships with the specified shipPoints and firepower
    {
        int shipPointCat = Mathf.Clamp(Mathf.NextPowerOfTwo((int)shipPoints + 1), 0, 1024);
        int firepower = Mathf.Clamp((int)((float)shipPointCat / 35), 0, 32);
        List<ShipSave> shipList = Editor.instance.GetPresetShips();
        List<ShipSave> toReturn = new List<ShipSave>();
        foreach (ShipSave ship in shipList)
        {
            if (shipPointCat == Mathf.NextPowerOfTwo(ship.shipPoints) && firepower <= Mathf.NextPowerOfTwo(ship.firePower))
            {
                toReturn.Add(ship);
            }
        }
        return toReturn;
    }

    void GenerateLinks(Vector2 node)//Generates links between map nodes, for visuals yo
    {
        foreach(Vector2 vec in hexPositions)
        {
            if(mapNodes.ContainsKey(node + vec*3))
            {
                GameObject instance;
                GameObject instance2;
                if(vec == new Vector2(1, 0) || vec == new Vector2(-1, 0))
                {
                    instance = Instantiate(link0d);
                    instance2 = Instantiate(link0d);
                }
                else if (vec == new Vector2(0, 1) || vec == new Vector2(0, -1))
                {
                    instance = Instantiate(link60d);
                    instance2 = Instantiate(link60d);
                }
                else
                {
                    instance = Instantiate(link120d);
                    instance2 = Instantiate(link120d);
                }
                instance.transform.SetParent(helperTransform);
                instance2.transform.SetParent(helperTransform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                instance2.transform.localScale = new Vector3(1, 1, 1);
                Editor.instance.SetHexPositon(node + vec, helperTransform.position, instance.gameObject, Editor.instance.unitSize * MainMenu.instance.globalScale.localScale.x);
                Editor.instance.SetHexPositon(node + vec * 2, helperTransform.position, instance2.gameObject, Editor.instance.unitSize * MainMenu.instance.globalScale.localScale.x);
            }
        }
    }

    public Vector2 XYtoHex(Vector2 xyPos)
    {
        float posY = xyPos.y - (Mathf.Tan(Mathf.PI / 6) * 0.25f * xyPos.y);//Math to offset y to fit hexagonal grid

        float posX = xyPos.x + (0.5f * xyPos.y);//Offsets the x position to create a hexagonal grid instead of xy grid

        return new Vector2(posX, posY);
    }

    public void ClearMap()
    {
        SaveMap();
        Destroy(helperTransform.gameObject);
        mapNodes.Clear();
    }

    public void SaveMap()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/map" + MainMenu.instance.profile + ".dat");
        List<Map> newList = new List<Map>();

        foreach (KeyValuePair<Vector2, MapNodeVertex> kv in mapNodes)
        {
            newList.Add(kv.Value.mapNode.map);
        }

        bf.Serialize(file, newList);
        file.Close();
    }

    class MapNodeVertex
    {
        public bool visited;
        public MapNode mapNode;

        public MapNodeVertex(MapNode m, bool v)
        {
            mapNode = m;
            visited = v;
        }

    }

}


