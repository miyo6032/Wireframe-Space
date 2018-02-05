using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//An individual part of the space map: represents a playable level in the map
public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Map map;
    public Vector2 xyPos;

    public bool mouseOverNode = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MapMenu.instance.LoadTooltip("Sector", "Difficulty: " + map.difficulty + "\nArena Size:" + map.arenaSize + "\nReward:" + map.reward, this);
        mouseOverNode = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverNode = false;
        Invoke("Deactivate", 0.1f);
    }

    void Deactivate()
    {
        MapMenu.instance.DeactivateTooltip();
    }

    public void SectorCleared()
    {
        MapMenu.instance.mapGenerator.GenerateNewNodes(this);
        map.arenaComplete = true;
        map.reward = 0;
    }
	
}

