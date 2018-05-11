using System.Collections.Generic;

[System.Serializable]
public class ShipSave
{
    public string title;
    public int shipPoints;
    public int firePower;
    public int boost;
    public float direction;
    public List<ModuleSaveData> modules = new List<ModuleSaveData>();

    public ShipSave(List<ModuleSaveData> m, string t, int s, int f, float d, int b)
    {
        modules = m;
        title = t;
        shipPoints = s;
        firePower = f;
        direction = d;
        boost = b;
    }

}
