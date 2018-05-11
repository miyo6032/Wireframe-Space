using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

//Handles most of the play zone: spawn ships, and handles winning and losing
public class PlayZoneManager : MonoBehaviour {

    public static PlayZoneManager instance = null;

    //Building blocks to be instantiated
    public ShipModule module;
    public ShipModule playerGun;
    public ShipModule enemyGun;
    public ShipModule zeroBeam;
    public ShipModule movementModule;

    public Ship player;
    public Ship enemyPrefab;

    public GameObject pauseCanvas;
    public GameObject playzoneObjects;
    public GameObject winloseCanvas;

    public Text winloseText;
    public Text winloseInfoText;

    public ParticleSystem smallExplosion;
    public ParticleSystem missileExplosion;

    public GameObject UICamera;

    private Vector2[] hexagonalPositions = {new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1), };

    void Awake () {

        Radar radar = player.GetComponent<Radar>();

        if(instance == null)
        {
            instance = this;
        }

        if(instance != this)
        {
            Destroy(gameObject);
        }

        foreach(ModuleSaveData mod in GameManager.instance.player.modules)//Add the player
        {
            player.AddModule(mod.xPos, mod.yPos, GameManager.instance.database.GetPrefabByIdPlayZone(mod.Id, "PlayerModule"));
        }

        float angle = 0;
        List<ShipSave> ships = GameManager.instance.currentLoadedMap.shipsToSpawn;

        //Add all of the enemy ships
        foreach (ShipSave enemy in ships)
        {
            Ship instance = Instantiate(enemyPrefab);
            //Load the modules
            foreach(ModuleSaveData mod in enemy.modules)
            {
                instance.AddModule(mod.xPos, mod.yPos, GameManager.instance.database.GetPrefabByIdPlayZone(mod.Id, "EnemyModule"));
            }

            instance.transform.position = Random.Range(25, 60) * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

            angle += 360 * Mathf.Deg2Rad / ships.Count;

            radar.AddToRadar(instance.gameObject);
            instance.transform.eulerAngles = new Vector3(0, 0, enemy.direction);
            instance.transform.SetParent(playzoneObjects.transform);
            instance.GetComponent<EnemyMovement>().rotationOffset = enemy.direction;
        }

    }

    public void MissionCompleted()
    {
        player.SetInvincible();
        Invoke("DisplayMissionComplete", 3);
    }

    void DisplayMissionComplete()
    {
        UICamera.SetActive(true);
        GameManager.instance.missionCompleted = true;
        playzoneObjects.SetActive(false);
        winloseCanvas.SetActive(true);
        winloseText.text = "You Win!";
        winloseInfoText.text = "EPIC STATS";
    }

    public void MissionFailed()
    {
        Invoke("DisplayMissionFailed", 3);
    }

    void DisplayMissionFailed()
    {
        UICamera.SetActive(true);
        GameManager.instance.missionCompleted = false;
        playzoneObjects.SetActive(false);
        winloseCanvas.SetActive(true);
        winloseText.text = "You Lost!";
        winloseInfoText.text = "EPIC STATS";
    }

    public void Pause()
    {
        UICamera.SetActive(true);
        playzoneObjects.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void Resume()
    {
        UICamera.SetActive(false);
        playzoneObjects.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene(1);
    }

}
