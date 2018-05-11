using UnityEngine;

[System.Serializable]
public class ModuleSaveData
{
    public int Id;
    public int xPos;
    public int yPos;

    public ModuleSaveData(int id, Vector2 xy)
    {
        Id = id;
        xPos = (int)xy.x;
        yPos = (int)xy.y;
    }

}
