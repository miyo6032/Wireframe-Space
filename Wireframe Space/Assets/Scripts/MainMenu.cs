using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Handles most of the main menu gui, and loading and saving game saves
public class MainMenu : MonoBehaviour {

    public static MainMenu instance;

    public int profile;

    public int shipPoints;

    public int level;

    public Transform savedProfilesPanel;

    public GameObject deletePanel;

    public GameSlot savedProfilePrefab;

    public GameSlot newProfilePrefab;

    public GameObject loadPanel;

    public Transform globalScale;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if(GameManager.instance != null && GameManager.instance.profile != -1)//Indicates when the main menu should load directly into a map menu
        {
            LoadProfile(GameManager.instance.profile);
        }

        /*FileStream loadFile;
        BinaryFormatter bf = new BinaryFormatter();
        loadFile = File.Create(Application.persistentDataPath + "/profiles.dat");//Creates a brand new file for the profiles
        ProfileSave[] newList = new ProfileSave[3];
        bf.Serialize(loadFile, newList);
        loadFile.Close();*/
    }

    public void ActivateLoadScreen()//CAlled when new game button is pressed
    {
        FileStream loadFile;
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/profiles.dat"))
        {
            loadFile = File.Open(Application.persistentDataPath + "/profiles.dat", FileMode.Open);
        }
        else {
            loadFile = File.Create(Application.persistentDataPath + "/profiles.dat");//Creates a brand new file for the profiles
            ProfileSave[] newList = new ProfileSave[3];
            bf.Serialize(loadFile, newList);
            loadFile.Close();
            ActivateLoadScreen();
            return;
        }

        ProfileSave[] profiles = (ProfileSave[])bf.Deserialize(loadFile);
        loadFile.Close();

        for(int i = 0; i < 3; i++)
        {

            if (profiles[i] == null)
            {
                GameSlot instance = Instantiate(newProfilePrefab);
                instance.slot = i;
                instance.title.text = "Empty Save Slot";
                instance.stats.text = "";
                instance.transform.SetParent(savedProfilesPanel);
                instance.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                GameSlot instance = Instantiate(savedProfilePrefab);
                instance.slot = i;
                instance.title.text = "Save " + (i + 1);
                instance.stats.text = "Ship Points: " + profiles[i].shipPoints + "\nLevel: " + profiles[i].level;
                instance.transform.SetParent(savedProfilesPanel);
                instance.transform.localScale = new Vector3(1, 1, 1);
            }
        }

    }

    public void ClearLoadScreen()//Clears the objects in the load screen before exiting
    {
        for(int i = 0; i < savedProfilesPanel.childCount; i++)
        {
            Destroy(savedProfilesPanel.GetChild(i).gameObject);
        }
    }

    public void ActivateDeletePanel(int index)
    {
        profile = index;
        deletePanel.SetActive(true);
    }
    
    public void DeleteProfile()//Deletes profile from list
    {
        if (File.Exists(Application.persistentDataPath + "/ships" + profile + ".dat"))
        {
            File.Delete(Application.persistentDataPath + "/ships" + profile + ".dat");
        }
        if(File.Exists(Application.persistentDataPath + "/map" + profile + ".dat"))
        {
            File.Delete(Application.persistentDataPath + "/map" + profile + ".dat");
        }

        FileStream loadFile = File.Open(Application.persistentDataPath + "/profiles.dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        ProfileSave[] profiles = (ProfileSave[])bf.Deserialize(loadFile);
        loadFile.Close();

        profiles[profile] = null;

        FileStream file = File.Create(Application.persistentDataPath + "/profiles.dat");

        bf.Serialize(file, profiles);
        file.Close();

        ClearLoadScreen();
        ActivateLoadScreen();
    }

    public void SaveProfile()//Called when there is progress in the profile, or when the player quits to the main menu
    {
        FileStream loadFile = File.Open(Application.persistentDataPath + "/profiles.dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        ProfileSave[] profiles = (ProfileSave[])bf.Deserialize(loadFile);
        loadFile.Close();

        ProfileSave newProfile = new ProfileSave();

        newProfile.shipPoints = shipPoints;
        newProfile.level = level;
        newProfile.currentShip = MapMenu.instance.currentShipIndex;
        newProfile.presetShip = MapMenu.instance.currentShipPreset;

        profiles[profile] = newProfile;

        FileStream file = File.Create(Application.persistentDataPath + "/profiles.dat");

        bf.Serialize(file, profiles);
        file.Close();
    }

    public void LoadProfile(int index)//Loads a profile, and opens the map menu
    {
        FileStream loadFile = File.Open(Application.persistentDataPath + "/profiles.dat", FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        ProfileSave[] profiles = (ProfileSave[])bf.Deserialize(loadFile);
        loadFile.Close();

        profile = index;
        shipPoints = profiles[index].shipPoints;
        level = profiles[index].level;
        MapMenu.instance.currentShipIndex = profiles[index].currentShip;
        MapMenu.instance.currentShipPreset = profiles[index].presetShip;

        ClearLoadScreen();
        loadPanel.SetActive(false);

        MapMenu.instance.gameObject.SetActive(true);
        MapMenu.instance.OpenMapMenu();
        gameObject.SetActive(false);
    }

    public void NewProfile(int index)//Creates a new profile and opens the map menu
    {
        ProfileSave newProfile = new ProfileSave();
        profile = index;
        shipPoints = 1000;
        level = 1;

        SaveProfile();
        LoadProfile(index);
    }

    public void Quit()
    {
        Application.Quit();
    }

}

[System.Serializable]
public class ProfileSave
{
    public int shipPoints;
    public int level;
    public int currentShip;
    public bool presetShip;

    public ProfileSave()
    {
        shipPoints = 0;
        level = 0;
        currentShip = -1;
    }

}

