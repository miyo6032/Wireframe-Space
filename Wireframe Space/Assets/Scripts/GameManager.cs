using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Map currentLoadedMap;//When the playzone is loaded, this map contains the info on how to generate the play zone

    public ShipSave player;//What the player chose as their ship

    public ModuleDatabase database;

    public Vector2 contentPosition;//Holds the position that the map view was at

    public int profile = -1;//-1 when the main menu just loaded, will be set when the playzone is entered, so that when the playzone is exited, the correct map menu will be loaded.

    public bool missionCompleted = false;

    public bool staggerGuns = true;

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
