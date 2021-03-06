﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the gui of all the saved ships, as well as loading them into the editor/map menu current ship
public class SavedShipList : MonoBehaviour {

    List<ShipInfo> loadedShips = new List<ShipInfo>();

    public ShipInfo shipInfoPrefab;

    public ShipInfo shipPresetPrefab;

    public GameObject scrollingPanel;

    public GameObject deletePanel;

    public Scrollbar scrollbar;

    public GameObject noSavedShips;

    int unitSize;
    int panelSize;

    ShipInfo stagedToDelete = null;
	
    public void LoadList(bool loadPresets)//Load the ships by literally constructing them out of editor ship modules (that are not editable)
    {
        ShipInfo prefab;
        List<ShipSave> shipList;
        if (loadPresets)
        {
            shipList = Editor.instance.GetPresetShips();
            prefab = shipPresetPrefab;
        }
        else
        {
            shipList = Editor.instance.GetSavedShips();
            prefab = shipInfoPrefab;

            if (shipList == null || shipList.Count == 0)
            {
                noSavedShips.SetActive(true);
                return;
            }
        }

        LoadShipInfos(prefab, shipList, loadPresets);

        SetupScrolling();
    }

    void LoadShipInfos(ShipInfo prefab, List<ShipSave> shipList, bool isPreset)
    {
        int index = 0;
        foreach (ShipSave ship in shipList)//Generates all of the ShipInfo displays
        {
            ShipInfo instance = Instantiate(prefab);

            string infoText = "ShipPoints: " + ship.shipPoints +
                "\nFire Power: " + ship.firePower;

            instance.Init(infoText, ship.title, new ShipIndex(index, isPreset), ship);

            instance.transform.SetParent(scrollingPanel.transform);
            instance.transform.localScale = new Vector3(1, 1, 1);
            loadedShips.Add(instance);
            index++;
        }
    }

    void SetupScrolling()
    {
        unitSize = (int)scrollingPanel.GetComponent<GridLayoutGroup>().cellSize.x + (int)scrollingPanel.GetComponent<GridLayoutGroup>().spacing.x;//Calculates scrolling stuff
        panelSize = (int)Mathf.Clamp((loadedShips.Count - 4) * unitSize, 0, float.PositiveInfinity);
        scrollbar.size = Mathf.Clamp(4 / (float)loadedShips.Count, 0.1f, 1);
        scrollbar.value = 0;
    }

    public void OpenList()
    {
        ClearList();
        LoadList(false);
    }

    public void OpenPresets()
    {
        ClearList();
        LoadList(true);
    }

    //Clear the list when exiting, or switching between presets and personal saved ships
    public void ClearList()
    {
        noSavedShips.SetActive(false);
        foreach(ShipInfo instance in loadedShips)
        {
            Destroy(instance.gameObject);
        }
        loadedShips.Clear();
    }

    public void ActivateDeletePanel(ShipInfo item)//Stages the time for deletion
    {
        stagedToDelete = item;
        deletePanel.SetActive(true);
    }

    public void Scroll()
    {
        Vector3 pos = scrollingPanel.transform.position;
        pos.x = -scrollbar.value * panelSize * MainMenu.instance.globalScale.localScale.x + transform.position.x;
        scrollingPanel.transform.position = pos;
    }

}
