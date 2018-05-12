using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class LevelUpText : MonoBehaviour {

    List<Info> database = new List<Info>();
    JsonData dialogueData;

    public Image sprite;
    public Text text;
    public Text missingText;

    private Info currentDialogue;

    public void Awake()
    {
        dialogueData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Dialogue.json"));
        ConstructDatabase();
    }

    public Info GetDialogueByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (id == database[i].Id)
                return database[i];
        }
        return null;
    }

    void ConstructDatabase()
    {
        for (int i = 0; i < dialogueData.Count; i++)
        {
            database.Add(new Info(
            dialogueData[i]["sprite"].ToString(),
            dialogueData[i]["text"].ToString(),
            dialogueData[i]["missingtext"].ToString(),
            (int)dialogueData[i]["id"],
            (int)dialogueData[i]["next"]
        ));
        }
    }

    public void ActivateDialogue(int id)
    {
        currentDialogue = GetDialogueByID(id);
        ActivateDialogue(currentDialogue.Text, currentDialogue.MissingText, currentDialogue.Sprite, currentDialogue.Next);
    }

    public void ActivateDialogue(string dialogue, string missing, string sprite, int next)
    {
        gameObject.SetActive(true);
        text.text = dialogue;
        missingText.text = missing;
        if (sprite != "")
        {
            this.sprite.sprite = Resources.Load<Sprite>(sprite);
        }
        else
        {
            this.sprite.gameObject.SetActive(false);
        }
        currentDialogue = new Info(sprite, dialogue, missing, -1, next);
    }

    public void LoadNextDialogue()
    {
        if (currentDialogue.Next != -1)
        {
            ActivateDialogue(currentDialogue.Next);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}


public class Info
{
    public string Sprite;
    public string Text;
    public string MissingText;
    public int Id;
    public int Next;

    public Info(string sprite, string text, string missingText, int id, int next)
    {

        this.Sprite = sprite;
        this.Text = text;
        this.MissingText = missingText;
        this.Id = id;
        this.Next = next;

    }

}
