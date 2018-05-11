using UnityEngine;
using UnityEngine.EventSystems;

//An individual part of the space map: represents a playable level in the map
public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Map map;
    public Vector2 xyPos;

    public bool mouseOverNode = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MapMenu.instance.LoadTooltip("Sector", "Ship Difficulty: " + map.shipSize + "\nShip Count:" + map.shipsToSpawn.Count + "\nReward:" + map.reward, this);
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

