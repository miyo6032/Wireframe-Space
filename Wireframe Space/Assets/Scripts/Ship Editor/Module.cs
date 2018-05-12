using UnityEngine;

//Holds the info for a type of module
public class Module{

    public Sprite mainSprite;
    public Sprite brokenImage;
    public Sprite brokenImage2;
    public Sprite gunSprite;
    public int id;
    public int mass;
    public int maxHealth = 2;
    public Vector2[] connectionPositions;
    public string title;
    public int cost;
    public string description;
    public int requiredLevel = 1;

    public Module(
        string mainSprite, 
        string brokenImage, 
        string brokenImage2, 
        int id, 
        int mass, 
        int maxHealth, 
        int[][] connectionPositions, 
        string title,
        int cost,
        string description,
        int requiredLevel
        )
    {
        this.mainSprite = Resources.Load<Sprite>("/modules/" + mainSprite);
        this.brokenImage = Resources.Load<Sprite>("/modules/" + brokenImage);
        this.brokenImage2 = Resources.Load<Sprite>("/modules/" + brokenImage2);
        this.id = id;
        this.mass = mass;
        this.maxHealth = maxHealth;
        this.title = title;
        this.cost = cost;
        this.description = description;
        this.requiredLevel = requiredLevel;
        //Populate the connection positions by converting from int[][] to vector2[]
        this.connectionPositions = new Vector2[connectionPositions.Length];
        for(int i = 0; i < connectionPositions.Length; i++)
        {
            this.connectionPositions[i] = new Vector2(connectionPositions[i][0], connectionPositions[i][1]);
        }
    }

}
