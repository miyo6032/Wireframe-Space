using System.Collections.Generic;

[System.Serializable]
public class Map
{
    public bool arenaComplete;
    public int reward;
    public int xPos;
    public int yPos;
    public int shipSize;
    public List<ShipSave> shipsToSpawn = new List<ShipSave>();

    public Map(List<ShipSave> ships, int size, int x, int y, int rew, bool complete)
    {
        arenaComplete = complete;
        reward = rew;
        xPos = x;
        yPos = y;
        shipsToSpawn = ships;
        shipSize = size;
    }

}
