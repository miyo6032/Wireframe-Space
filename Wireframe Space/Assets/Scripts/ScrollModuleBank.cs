using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the scrolling of the module bank and initializes the bank
public class ScrollModuleBank : MonoBehaviour {

    float unitSize;
    float panelSize;

    public GridLayoutGroup scrollingPanel;

    public Transform mask;

    public Scrollbar scrollbar;

    public List<ModuleBank> bankPrefabs;

    GridLayoutGroup panel;

	public void LoadBanks () {
        panel = Instantiate(scrollingPanel);
        panel.transform.SetParent(mask);
        panel.transform.localPosition = Vector3.zero;
        panel.transform.localScale = new Vector3(1, 1, 1);
        int moduleCount = 0;
        foreach (ModuleBank bank in bankPrefabs)
        {
            if (bank.modulePrefab.requiredLevel <= MainMenu.instance.level)
            {
                GameObject instance = Instantiate(bank).gameObject;
                instance.transform.SetParent(panel.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);
                moduleCount++;
            }
        }

        unitSize = (int)panel.cellSize.x + (int)panel.spacing.x;
        panelSize = (int)Mathf.Clamp((Mathf.Ceil(moduleCount * 0.5f) - 5) * unitSize, 0, float.PositiveInfinity);
        scrollbar.size = Mathf.Clamp(5 / (float)Mathf.Ceil(moduleCount * 0.5f), 0.1f, 1);
        scrollbar.value = 0;
    }

    public void ClearBanks()
    {
        Destroy(panel.gameObject);
        panel = null;
    }

    public void Scroll()
    {
        Vector3 pos = panel.transform.position;
        pos.y = scrollbar.value * panelSize * MainMenu.instance.globalScale.localScale.x + transform.position.y;
        panel.transform.position = pos;
    }

}
