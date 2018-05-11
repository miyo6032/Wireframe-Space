using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

//Handles the vast majority of the ship editor gui, saving, loading, updating and checking. 
public class Editor : MonoBehaviour {

    public static Editor instance;

    public float unitSize = 1.0f;

    public float shipInfoUnitScale = 0.4f;

    public bool editingCurrentShip = false;

    Dictionary<Vector2, SlotVertex> slots = new Dictionary<Vector2, SlotVertex>();//The reason why we have a graph of the slots is to shift the entire ship

    Dictionary<Vector2, ModuleVertex> modules = new Dictionary<Vector2, ModuleVertex>();//Only used when loading and saving

    public Slot slotPrefab;

    public GameObject slotPanelPrefab;

    public Transform editorPanel;

    public GameObject lockedSlot;

    public GameObject slotPanel;

    public ShiftPanel shiftPanel;

    public GameObject saveAsPanel;

    public LevelUpText infoPanel;

    public InputField shipName;

    public SavedShipList savedShipList;

    public ModuleDatabase database;

    public Text shipStats;

    public Text shipStats2;

    public Text tooltipTitle;

    public Text tooltipBody;

    public GameObject shipDirection;

    public GameObject saveOrExitPanel;

    public Transform centerOfMassDot;

    public ScrollModuleBank bank;

    public int loadedShipIndex = -1;

    int shipPoints = 0;

    int firePower = 0;

    int boost = 0;

    int[] editorSizeLevels = { 1, 3, 5, 8, 10};

    BinaryFormatter bf = new BinaryFormatter();

    //List of all adjacent slot positions
    Vector2[] slotPositions = { new Vector2(0, 0), new Vector2(0, 1) , new Vector2(-1, 1) , new Vector2(-1, 0) , new Vector2(0, -1) , new Vector2(1, -1) , new Vector2(1, 0) };

	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        shiftPanel.GenerateShiftButtons();

        infoPanel.gameObject.SetActive(false);
        saveAsPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OpenEditor(bool editing)
    {
        gameObject.SetActive(true);
        bank.LoadBanks();
        GenerateEditorSpace();
        if (editing && MapMenu.instance.currentShipIndex != -1)
        {
            LoadShip(MapMenu.instance.currentShipIndex, MapMenu.instance.currentShipPreset);
            editingCurrentShip = true;
        }
        if (MapMenu.instance.GetLeveledUP())
        {
            infoPanel.ActivateDialogue(MainMenu.instance.level);
            MapMenu.instance.SetLeveledUp(false);
        }
    }

    public void TryToExit()//Will exit unless there are unsaved changes on an already loaded ship
    {
        if (loadedShipIndex != -1 && !ShipsEquivalent(GetCurrentEditorShip(), GetSavedShips()[loadedShipIndex]))
        {
            saveOrExitPanel.SetActive(true);
            return;
        }
        Exit();
    }

    public void Exit()//Exit to map menu
    {
        bank.ClearBanks();
        ClearEditor();
        slots.Clear();
        Destroy(slotPanel.gameObject);
        MapMenu.instance.gameObject.SetActive(true);
        MapMenu.instance.OpenMapMenu();
        gameObject.SetActive(false);
    }

    public void EnableShipList()
    {
        savedShipList.gameObject.SetActive(true);
        savedShipList.LoadList(false);
    }

    void GenerateEditorSpace()
    {

        slotPanel = Instantiate(slotPanelPrefab, editorPanel.transform, false);

        GenerateLockedHexes();
        int radius= 0;
        while(editorSizeLevels[radius] <= MainMenu.instance.level || radius == editorSizeLevels.Length)
        {
            radius++;
        }
        GenerateHexGrid(radius);
    }

    void GenerateLockedHexes()
    {
        int radius = 7;
        for (int x = -radius; x < 0; x++)
        {
            for (int y = -radius - x; y <= radius; y++)
            {
                GameObject instance = Instantiate(lockedSlot);
                instance.transform.SetParent(slotPanel.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                SetHexPositon(new Vector2(x, y), slotPanel.transform.position, instance.gameObject, unitSize * transform.localScale.x);
            }
        }
        for (int x = 0; x <= radius; x++)
        {
            for (int y = radius - x; y >= -radius; y--)
            {
                GameObject instance = Instantiate(lockedSlot);
                instance.transform.SetParent(slotPanel.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                SetHexPositon(new Vector2(x, y), slotPanel.transform.position, instance.gameObject, unitSize * transform.localScale.x);
            }
        }
    }

    void GenerateHexGrid(int radius)//Generates a hexagonal grid of slots to build your custom ship in
    {
        while(MainMenu.instance.level >= editorSizeLevels[radius - 1])
        {
            radius++;
        }
        for(int x = -radius; x < 0; x++)
        {
            for (int y = -radius - x; y <= radius; y++) {
                Slot instance = Instantiate(slotPrefab).GetComponent<Slot>();
                instance.xyPos = new Vector2(x, y);
                List<SlotVertex> adj = new List<SlotVertex>();
                SlotVertex slotVertex = new SlotVertex(instance, adj, false);
                slots.Add(instance.xyPos, slotVertex);
                instance.transform.SetParent(slotPanel.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                SetHexPositon(instance.xyPos, slotPanel.transform.position, instance.gameObject, unitSize * transform.localScale.x);
             }
        }
        for(int x = 0; x <= radius; x++)
        {
            for(int y = radius - x; y >= -radius; y--)
            {
                Slot instance = Instantiate(slotPrefab).GetComponent<Slot>();
                instance.xyPos = new Vector2(x, y);
                List<SlotVertex> adj = new List<SlotVertex>();
                SlotVertex slotVertex = new SlotVertex(instance, adj, false);
                slots.Add(instance.xyPos, slotVertex);
                instance.transform.SetParent(slotPanel.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                SetHexPositon(instance.xyPos, slotPanel.transform.position, instance.gameObject, unitSize * transform.localScale.x);
            }
        }
    }

    public void ShiftModules(Vector2 vec)//Shifts all modules over a certain slot direction.
    {

        foreach (KeyValuePair<Vector2, SlotVertex> kV in slots)
        {
            kV.Value.visited = false;
        }

        foreach (KeyValuePair<Vector2, SlotVertex> kV in slots)
        {
            SlotVertex slot = kV.Value;

            if (slot.visited == true) continue;

            slot.visited = true;
            if (kV.Value.slot.HasItem())
            {
                SlotVertex nextSlot;
                slots.TryGetValue(slot.slot.xyPos + vec, out nextSlot);

                if(nextSlot == null)//Handles the case when some parts are moved out of the editor space
                {
                    Destroy(slot.slot.childModule.gameObject);
                    slot.slot.childModule = null;
                }else
                {

                    nextSlot.visited = true;

                    EditorShipModule displacedChild = slot.slot.childModule;
                    slot.slot.childModule = null;
                    EditorShipModule temp;
                    while (nextSlot != null && nextSlot.slot.HasItem())//Will keep moving over modules until it meets an edge or an empty slot
                    {
                        temp = displacedChild;
                        displacedChild = nextSlot.slot.childModule;
                        nextSlot.slot.childModule = temp;
                        nextSlot.slot.childModule.currentSlot = nextSlot.slot;
                        nextSlot.slot.childModule.transform.position = nextSlot.slot.transform.position;
                        nextSlot.slot.childModule.transform.SetParent(nextSlot.slot.transform);

                        slots.TryGetValue(nextSlot.slot.xyPos + vec, out nextSlot);
                        if(nextSlot != null)nextSlot.visited = true;
                    }

                    //Nothing in the next slot - simply move the displaced child into that slot
                    if (nextSlot != null)
                    {
                        nextSlot.slot.childModule = displacedChild;
                        nextSlot.slot.childModule.currentSlot = nextSlot.slot;
                        nextSlot.slot.childModule.transform.position = nextSlot.slot.transform.position;
                        nextSlot.slot.childModule.transform.SetParent(nextSlot.slot.transform);
                        slot.slot.childModule = null;
                    }
                    else//Or on edge, destroy the displaced child
                    {
                        Destroy(displacedChild.gameObject);
                    }

                }
            }
        }
        DisplayStats();
    }

    void CalculateCenterOfMass()//Puts a center of mass dot to show the center of mass of objects
    {
        Vector2 centerOfMass = Vector2.zero;
        int weight = 0;
        foreach (KeyValuePair<Vector2, SlotVertex> v in slots)
        {
            if (v.Value.slot.HasItem())
            {
                centerOfMass += new Vector2(v.Value.slot.transform.localPosition.x * v.Value.slot.childModule.representativeModule.mass, v.Value.slot.transform.localPosition.y * v.Value.slot.childModule.representativeModule.mass);
                weight += v.Value.slot.childModule.representativeModule.mass;
            }
        }
        if (weight == 0)
        {
            centerOfMassDot.transform.localPosition = new Vector3(-1000, -1000, 0);
            return;
        }
        centerOfMass /= weight;
        centerOfMassDot.transform.localPosition = centerOfMass + (Vector2)slotPanel.transform.localPosition;
    }

    public void SetHexPositon(Vector2 xyPos, Vector2 offset, GameObject obj, float scale)
    {
        float posY = scale * xyPos.y - (scale * Mathf.Tan(Mathf.PI / 6) * 0.25f * xyPos.y);//Math to offset y to fit hexagonal grid

        float posX = scale * xyPos.x + (scale * 0.5f * xyPos.y);//Offsets the x position to create a hexagonal grid instead of xy grid

        Vector2 pos = new Vector2(posX, posY);

        obj.transform.position = offset + pos;
    }

    public Slot GetSlot(Vector2 xyPos)
    {
        SlotVertex toReturn;
        slots.TryGetValue(xyPos, out toReturn);
        if (toReturn != null)
            return toReturn.slot;
        else
            return null;
    }

    public void ClearEditor()//Clears the editor of the ship slots - called when exiting the editor, or refreshing the editor after a delete
    {
        foreach(KeyValuePair<Vector2, SlotVertex> kV in slots)
        {
            if (kV.Value.slot.HasItem())
            {
                Destroy(kV.Value.slot.childModule.gameObject);
                kV.Value.slot.childModule = null;
            }
        }
        modules.Clear();
        shipDirection.transform.eulerAngles = Vector3.zero;
        if (!editingCurrentShip)
        loadedShipIndex = -1;
        DisplayStats();
    }

    //Called by the save or saveasnew button in the editor
    public void SaveShip(bool saveAs)//Attempts to save the ship for both save new and overwrite
    {
        modules.Clear();
        if (IsShipValid(modules))
        {

            if(!File.Exists(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat"))//The first save requires some file setup
            {
                FileStream file = File.Create(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat");
                List<ShipSave> saveData = new List<ShipSave>(new List<ShipSave>());
                bf.Serialize(file, saveData);
                file.Close();
                saveAs = true;
            }

            if (loadedShipIndex == -1)//Can't save as if the editor didn't start from a previously loaded ship
            {
                saveAs = true;
            }

            if (saveAs)//the player will have to save the ship as a new object
            {
                saveAsPanel.SetActive(true);
            }
            else//Else overwrite the loaded ship
            {
                List<ShipSave> savedShips = GetSavedShips();
                FileStream file = File.Create(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat");

                ShipSave shipToSave = GetCurrentEditorShip();
                savedShips[loadedShipIndex] = shipToSave;
                bf.Serialize(file, savedShips);
                file.Close();
            }

        }
    }

    public ShipSave GetCurrentEditorShip()//Because the edtor doesn't actually keep track of ship changes in real time, the ship is constructed here when saved
    {
        ShipSave shipToSave = new ShipSave(new List<ModuleSaveData>(), shipName.text, shipPoints, firePower, shipDirection.transform.eulerAngles.z, boost);

        foreach (KeyValuePair<Vector2, SlotVertex> kV in slots)
        {
            if (kV.Value.slot.HasItem())
            {
                ModuleSaveData save = new ModuleSaveData(kV.Value.slot.childModule.representativeModule.id, kV.Key);
                shipToSave.modules.Add(save);
            }
        }
        return shipToSave;
    }

    public void TryToSaveNew()//Called via the save button in the savenew gui - Indirectly is called by SaveShip()
    {
        if(shipName.text != "")//Add the ship onto the list and save the list
        {

            ShipSave shipToSave = GetCurrentEditorShip();
            List<ShipSave> savedShips = GetSavedShips();

            FileStream file = File.Create(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat");
            savedShips.Add(shipToSave);

            loadedShipIndex = savedShips.Count - 1;
            bf.Serialize(file, savedShips);
            file.Close();

            saveAsPanel.SetActive(false);
        }
        else//Activate info panel
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.ActivateDialogue("You must enter a name for your ship! Press okay to continue.", "", "", -1);
        }
    }

    bool ShipsEquivalent(ShipSave s1, ShipSave s2)//Determines if two ship save classes have the equivalent attributes - used to bring up a dialogue that says "you forgot to save!"
    {
        if(s1.title == s2.title)
            if(s1.shipPoints == s2.shipPoints)
                if(s1.firePower == s2.firePower)
                    if(s1.direction == s2.direction)
                        if(s1.modules.Count == s2.modules.Count)
                        {
                            for(int i = 0; i < s1.modules.Count; i++)
                            {
                                ModuleSaveData m1 = s1.modules[i];
                                ModuleSaveData m2 = s2.modules[i];
                                if (m1.Id != m2.Id || m1.xPos != m2.xPos || m1.yPos != m2.yPos)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
        return false;
    }

    public void SetAsCurrentShip()
    {
        MapMenu.instance.SetCurrentShip(loadedShipIndex, false);
    }

    public void ChangeRotation()
    {
        shipDirection.transform.eulerAngles = new Vector3(0, 0, shipDirection.transform.eulerAngles.z + 90);
    }

    public void LoadShip(int index, bool preset)//Loads a ship into the editor and quits the ship list
    {
        ShipSave shipToLoad;
        ClearEditor();
        if (preset)
        {
            shipToLoad = GetPresetShips()[index];
        }
        else
        {
            shipToLoad = GetSavedShips()[index];
            loadedShipIndex = index;
        }
       
        shipDirection.transform.eulerAngles = new Vector3(0, 0, shipToLoad.direction);
        foreach (ModuleSaveData module in shipToLoad.modules)
        {
            EditorShipModule instance = Instantiate(database.GetPrefabByID(module.Id));
            SlotVertex parentSlot;
            slots.TryGetValue(new Vector2(module.xPos, module.yPos), out parentSlot);
            instance.transform.position = parentSlot.slot.transform.position;
            instance.transform.SetParent(parentSlot.slot.transform);
            instance.transform.localScale = new Vector3(1, 1, 1);
            instance.currentSlot = parentSlot.slot;
            parentSlot.slot.childModule = instance;
        }
        shipName.text = shipToLoad.title;
        savedShipList.gameObject.SetActive(false);
        savedShipList.ClearList();
        Editor.instance.DisplayStats();
    }

    public void DeleteShip(int index)//Delete a ship from the ship list
    {
        List<ShipSave> savedShips = GetSavedShips();
        savedShips.Remove(savedShips[index]);
        if(index == loadedShipIndex)
        {
            ClearEditor();
        }
        if(index == MapMenu.instance.currentShipIndex)//Since the map menu current ship is loaded, it is possible that the loaded ships would be destroyed in the ship list
        {
            MapMenu.instance.ClearCurrentShip();
        }
        else if(index < MapMenu.instance.currentShipIndex)
        {
            MapMenu.instance.currentShipIndex--;
        }
        FileStream file = File.Create(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat");
        bf.Serialize(file, savedShips);
        file.Close();
    }

    public List<ShipSave> GetSavedShips()//Gets all saved ships
    {
        FileStream file;
        if (!File.Exists(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat"))//The first save requires some file setup
        {
            file = File.Create(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat");
            List<ShipSave> saveData = new List<ShipSave>(new List<ShipSave>());
            bf.Serialize(file, saveData);
            file.Close();
            Debug.Log("No Saved Ships!");
            return new List<ShipSave>();
        }
        else
        {
            file = File.Open(Application.persistentDataPath + "/ships" + MainMenu.instance.profile + ".dat", FileMode.Open);
        }

        List<ShipSave> savedShips = ((List<ShipSave>)bf.Deserialize(file));
        file.Close();

        return savedShips;
    }

    public List<ShipSave> GetPresetShips()//Gets all preset ships
    {
        FileStream file = File.Open(Application.streamingAssetsPath + "/ships.dat", FileMode.Open);

        List<ShipSave> savedShips = ((List<ShipSave>)bf.Deserialize(file));
        file.Close();

        return savedShips;
    }

    bool IsShipValid(Dictionary<Vector2, ModuleVertex> modules)//Checks to see that there is 1 cockpit, and also that there are no island nodes from the cockpit.
    {

        if(shipPoints > MainMenu.instance.shipPoints)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.ActivateDialogue("You have exceeded the maximum ship point count by " + (shipPoints - MainMenu.instance.shipPoints) + ". Remove some extra parts!", "", "", -1);
            return false;
        }

        ModuleVertex cockpit = null;

        foreach(KeyValuePair<Vector2, SlotVertex> kV in slots)//Add all modules to the dictionary and also register the cockpit(root) module
        {
            if (kV.Value.slot.HasItem())
            {
                ModuleVertex vertex = new ModuleVertex(kV.Value.slot.childModule, new List<ModuleVertex>(), false);
                modules.Add(kV.Key, vertex);
                SearchAndConnect(kV.Key, modules);
                if(vertex.module.representativeModule.id == database.Cockpit.representativeModule.id)
                {
                    if(cockpit != null)
                    {
                        infoPanel.gameObject.SetActive(true);
                        infoPanel.ActivateDialogue("Your ship has multiple cockpits! There can only be one per ship.", "Cockpit Module", "Hexagon", -1);
                        return false;
                    }
                    cockpit = vertex;
                }
            }
        }

        if(cockpit == null)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.ActivateDialogue("Your ship must have a cockpit! Make sure it is well protected in your ship.", "Cockpit Module", "Hexagon", -1);
            return false;
        }

        Queue<ModuleVertex> q = new Queue<ModuleVertex>();//Mark all connected nodes visited.
        cockpit.visited = true;
        q.Enqueue(cockpit);

        while (q.Count != 0)
        {

            ModuleVertex current = q.Dequeue();

            for (int i = 0; i < current.adj.Count; i++)
            {
                ModuleVertex adj = current.adj[i];
                if (!adj.visited)
                {
                    current.adj[i].visited = true;
                    q.Enqueue(current.adj[i]);
                }
            }
        }
        bool missingValue = false;
        foreach (KeyValuePair<Vector2, ModuleVertex> v in modules)//If any are disconnected
        {
            if (!v.Value.visited)
            {
                missingValue = true;
                v.Value.module.currentSlot.GetComponent<Image>().color = Color.red;
            }
            else
            {
                v.Value.module.currentSlot.GetComponent<Image>().color = Color.white;
            }
        }

        if (missingValue)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.ActivateDialogue("Some parts are not attached to your ship! Unattached modules will be marked in red.", "", "", -1);
            return false;
        }

        return true;
    }

    int AddConnection(Vector2 vec1, Vector2 vec2, Dictionary<Vector2, ModuleVertex> modules)//Add a connection between two modules
    {
        if (modules.ContainsKey(vec1) && modules.ContainsKey(vec2))
        {

            Vector2 v2v1 = vec1 - vec2;
            ModuleVertex v1;
            modules.TryGetValue(vec1, out v1);

            ModuleVertex v2;
            modules.TryGetValue(vec2, out v2);
            if (v2 == null) return 0;

            for (int i = 0; i < v2.module.representativeModule.connectionPositions.Length; i++)
            {
                if (v2.module.representativeModule.connectionPositions[i] == v2v1)
                {
                    break;
                }
                else if (i == v2.module.representativeModule.connectionPositions.Length - 1)
                {
                    return 0;
                }
            }

            v1.adj.Add(v2);
            v2.adj.Add(v1);

            return 1;

        }
        return 0;
    }

    void SearchAndConnect(Vector2 xyPos, Dictionary<Vector2, ModuleVertex> modules)//Connects a new module to all appropriate adjacent modules
    {
        ModuleVertex v;
        modules.TryGetValue(xyPos, out v);
        EditorShipModule module = v.module;

        if (module != null)
        {
            for (int i = 0; i < module.representativeModule.connectionPositions.Length; i++)
            {
                Vector2 offset = module.representativeModule.connectionPositions[i] + xyPos;
                AddConnection(xyPos, offset, modules);
            }
        }
    }

    public void DisplayStats()
    {
        int totalHealth = 0;
        int thrust = 0;
        int mass = 0;
        shipPoints = 0;
        firePower = 0;
        boost = 0;
        foreach(KeyValuePair<Vector2, SlotVertex> kv in slots)
        {
            if (kv.Value.slot.HasItem())
            {
                EditorShipModule module = kv.Value.slot.childModule;
                if (module.tag == "Gun")
                {
                    firePower += module.representativeModule.GetComponentInChildren<Gun>().relativeFirepower;
                }
                else if (module.representativeModule is MovementModule)
                {
                    thrust += 1;
                }
                else if(module.representativeModule.tag == "BoostModule")
                {
                    boost += 200;
                }
                totalHealth += module.representativeModule.health;
                shipPoints += module.cost;
                mass += module.representativeModule.mass;
            }
        }

        shipStats.text = "Max Ship Cost: " + MainMenu.instance.shipPoints +
            "\nShip Cost: " + shipPoints;

        if(MainMenu.instance.shipPoints >= shipPoints)//Maximum ship point limit - will turn red if exceeded
        {
            shipStats.color = Color.black;
        }
        else
        {
            shipStats.color = Color.red;
        }

        shipStats2.text =
            "Ship Health: " + totalHealth +
            "\nShip Thrust: " + thrust +
            "\nShip Mass: " + mass +
            "\nFire Power: " + firePower;
            
        if(boost > 0)
        {
            shipStats2.text += "\nBoost: " + boost;
        }

        CalculateCenterOfMass();

    }

    class SlotVertex
    {
        public Slot slot;
        public List<SlotVertex> adj = new List<SlotVertex>();
        public bool visited;

        public SlotVertex(Slot s, List<SlotVertex> a, bool v)
        {
            slot = s;
            adj = a;
            visited = v;
        }

    }

    class ModuleVertex
    {
        public bool visited;
        public EditorShipModule module;
        public List<ModuleVertex> adj = new List<ModuleVertex>();

        public ModuleVertex(EditorShipModule m, List<ModuleVertex> a, bool v)
        {
            module = m;
            adj = a;
            visited = v;
        }

    }

}
