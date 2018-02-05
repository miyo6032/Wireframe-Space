using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the scrolling of the module bank
public class ScrollModuleBank : MonoBehaviour {

    float unitSize;
    float panelSize;

    public GridLayoutGroup scrollingPanel;

    public Scrollbar scrollbar;

    public List<GameObject> bankPrefabs;

	void Start () {
        foreach(GameObject bank in bankPrefabs)
        {
            GameObject instance = Instantiate(bank);
            instance.transform.SetParent(scrollingPanel.transform);
            instance.transform.localScale = new Vector3(1, 1, 1);
        }

        unitSize = (int)scrollingPanel.cellSize.x + (int)scrollingPanel.spacing.x;
        panelSize = (int)Mathf.Clamp((Mathf.Ceil(bankPrefabs.Count * 0.5f) - 5) * unitSize, 0, float.PositiveInfinity);
        scrollbar.size = Mathf.Clamp(5 / (float)Mathf.Ceil(bankPrefabs.Count * 0.5f), 0.1f, 1);
        scrollbar.value = 0;
    }

    public void Scroll()
    {
        Vector3 pos = scrollingPanel.transform.position;
        pos.y = scrollbar.value * panelSize * MainMenu.instance.globalScale.localScale.x + transform.position.y;
        scrollingPanel.transform.position = pos;
    }

}
