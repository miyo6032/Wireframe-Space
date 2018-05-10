using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used in the main menu as the three individual save slots
public class GameSlot : MonoBehaviour {

    public Text title;
    public Text stats;
    public int slot;

    public void LoadExistingGame()
    {
        MainMenu.instance.LoadProfile(slot);
    }

    public void DeleteGame()
    {
        MainMenu.instance.ActivateDeletePanel(slot);
    }

    public void CreateGame()
    {
        MainMenu.instance.NewProfile(slot);
    }

    public void CopyGame()
    {

    }

}
