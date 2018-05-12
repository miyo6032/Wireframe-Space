using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Handles all of the map menu gui, except the map itself, which is handled by the map generator.
public class MapMenu : MonoBehaviour {

    public float zoomInScale;

    float zoomOutScale;

    public Text currentShipName;

    public GameObject currentShipVisual;

    public SavedShipList shipList;

    public static MapMenu instance;

    public Tooltip tooltip;

    public MapNode mapToLoad;

    public int currentShipIndex = -1;

    public bool currentShipPreset;

    public MapGenerator mapGenerator;

    public Text shipPoints;

    public RectTransform content;

    public GameObject noShipPanel;

    GameObject collectorObj;

    public GameObject levelUpPanel;

    public Text levelUpText;

    int[] levelCutoffs = { 25, 32, 45, 64, 90, 130, 180, 250, 320, 400};

    bool leveledup = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        zoomOutScale = 1 / zoomInScale;

        gameObject.SetActive(false);
    }

    public void OpenMapMenu()//When the map is opened, sets up all of the features, and aligns content position
    {
        content.transform.localScale = new Vector3(1, 1, 1);
        tooltip.transform.localScale = new Vector3(1, 1, 1);
        mapGenerator.OpenMap();//Loads all of the saved mapGeneratorNodes

        if (GameManager.instance.profile != -1)
        {
            content.anchoredPosition = GameManager.instance.contentPosition;
        }

        if (GameManager.instance.profile != -1 && GameManager.instance.missionCompleted)
        {
            if (!GameManager.instance.currentLoadedMap.arenaComplete)
                MainMenu.instance.shipPoints += GameManager.instance.currentLoadedMap.reward;

            //Level up stuff
            int level = 1;
            while(MainMenu.instance.shipPoints >= levelCutoffs[level - 1])
            {
                level++;
            }

            if (MainMenu.instance.level < level)
            {
                levelUpPanel.SetActive(true);
                levelUpText.text = "You are now level " + level + ". Check the ship editor to see what you've unlocked!";
                MainMenu.instance.level = level;
                leveledup = true;
            }

            mapGenerator.CompleteNode(GameManager.instance.currentLoadedMap);
            MainMenu.instance.SaveProfile();
            mapGenerator.SaveMap();

            GameManager.instance.missionCompleted = false;
        }

        shipPoints.text = "Ship Points: " + MainMenu.instance.shipPoints + "\nLevel: " + MainMenu.instance.level;

        //Set current ship
        if (currentShipIndex != -1)
        {
            SetCurrentShip(currentShipIndex, currentShipPreset);
        }
        else
        {
            SetCurrentShip(0, true);
        }
    }

    public bool GetLeveledUP()
    {
        return leveledup;
    }

    public void SetLeveledUp(bool b)
    {
        leveledup = b;
    }

    public void ZoomOut()
    {
        if (content.transform.localScale.magnitude > 0.9)
        {
            content.transform.localScale = content.transform.localScale * zoomOutScale;
            tooltip.transform.localScale = tooltip.transform.localScale * zoomInScale;
        }
    }

    public void ZoomIn()
    {
        if (content.transform.localScale.magnitude < 2)
        {
            content.transform.localScale = content.transform.localScale * zoomInScale;
            tooltip.transform.localScale = tooltip.transform.localScale * zoomOutScale;
        }
    }

    public void OpenEditor(bool editing)//Opens the editor gui called by buttons that open the editor gui
    {
        mapGenerator.ClearMap();
        gameObject.SetActive(false);
        Editor.instance.OpenEditor(editing);
    }

    public void ExitToMainMenu()
    {
        mapGenerator.ClearMap();
        MainMenu.instance.SaveProfile();
        MainMenu.instance.gameObject.SetActive(true);
        ClearCurrentShip();
        gameObject.SetActive(false);
    }

    public void ClearCurrentShip()
    {
        if (collectorObj != null)
        {
            Destroy(collectorObj.gameObject);
        }
        currentShipIndex = -1;
    }

    public void OpenShipList()//Opens the list to choose the current ship
    {
        mapGenerator.ClearMap();
        shipList.gameObject.SetActive(true);
        shipList.LoadList(false);
    }

    public void SetCurrentShip(int id, bool preset)//When the player chooses the current ship from the ships list, this is called
    {
        ClearCurrentShip();

        collectorObj = new GameObject();
        collectorObj.transform.SetParent(currentShipVisual.transform);
        collectorObj.transform.localScale = new Vector3(1, 1, 1);

        ShipSave ship;
        if (preset) {
            ship = Editor.instance.GetPresetShips()[id];
            currentShipPreset = true;
        }
        else
        {
            ship = Editor.instance.GetSavedShips()[id];
            currentShipPreset = false;
        }
        currentShipName.text = ship.title;

        foreach (ModuleSaveData module in ship.modules)
        {
            EditorShipModule mod = Instantiate(GameManager.instance.database.GetEditorModule(module.Id));
            mod.transform.SetParent(collectorObj.transform);
            mod.transform.localScale = Editor.instance.shipInfoUnitScale * new Vector3(1, 1, 1);
            Editor.instance.SetHexPositon(new Vector2(module.xPos, module.yPos), currentShipVisual.transform.position, mod.gameObject, Editor.instance.unitSize * Editor.instance.shipInfoUnitScale * MainMenu.instance.globalScale.localScale.x);
            mod.editable = false;
        }

        currentShipIndex = id;

    }

    public void LoadTooltip(string title, string body, MapNode t)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.Body.text = body;
        tooltip.Title.text = title;
        mapToLoad = t;
        int yOffset = 0;
        int xOffset = 0;
        if(Input.mousePosition.y < Screen.height * 0.5f)
        {
            yOffset = (int)(tooltip.GetComponent<RectTransform>().rect.height * transform.localScale.x);
        }
        if(Input.mousePosition.x < Screen.width * 0.5f)
        {
            xOffset = (int)(tooltip.GetComponent<RectTransform>().rect.width * transform.localScale.x);
        }
        tooltip.transform.position = new Vector3(t.transform.position.x + xOffset, t.transform.position.y + yOffset, 0);
    }

    public void DeactivateTooltip()//Will check to deactivate
    {
        if(!tooltip.mouseOverTooltip && !mapToLoad.mouseOverNode)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void PlayMap()//Loads the map node and loads the play zone scene. The game manager will carry the required information over
    {
        if(mapToLoad != null)
        {
            GameManager.instance.contentPosition = content.anchoredPosition;//Records the current position of the scroll view
            GameManager.instance.currentLoadedMap = mapToLoad.map;

            if(currentShipIndex == -1)
            {
                noShipPanel.SetActive(true);
                return;
            }

            if (currentShipPreset)
            {
                GameManager.instance.player = Editor.instance.GetPresetShips()[currentShipIndex];
            }
            else
            {
                GameManager.instance.player = Editor.instance.GetSavedShips()[currentShipIndex];
            }

            GameManager.instance.profile = MainMenu.instance.profile;
            MainMenu.instance.SaveProfile();
            SceneManager.LoadScene(2);
        }
    }

}
