using UnityEngine;

//Holds the main information and transfers information across the two scenes
public class GameManager : MonoBehaviour {

    public static GameManager instance;

    //When the playzone is loaded, this map contains the info on how to generate the play zone
    public Map currentLoadedMap;

    //What the player chose as their ship
    public ShipSave player;

    public ModuleDatabase database;

    //Holds the position that the map view was at
    public Vector2 contentPosition;

    //-1 when the main menu just loaded, will be set when the playzone is entered, so that when the playzone is exited, the correct map menu will be loaded.
    public int profile = -1;

    public bool missionCompleted = false;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        database = GetComponent<ModuleDatabase>();

    }

    public void ResetGameManager()
    {
        currentLoadedMap = null;
        player = null;
        missionCompleted = false;
    }

}
