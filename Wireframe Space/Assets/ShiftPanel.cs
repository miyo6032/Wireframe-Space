using UnityEngine;

public class ShiftPanel : MonoBehaviour {

    Vector2[] slotPositions = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0) };

    public ShiftButton shiftbutton;

    public void GenerateShiftButtons()
    {
        for (int i = 1; i < slotPositions.Length; i++)
        {
            ShiftButton instance = Instantiate(shiftbutton).GetComponent<ShiftButton>();
            instance.xyPos = slotPositions[i];
            instance.transform.SetParent(transform);
            instance.transform.localScale = new Vector3(1, 1, 1);
            Editor.instance.SetHexPositon(instance.xyPos, instance.transform.parent.transform.position, instance.gameObject, Editor.instance.unitSize * transform.localScale.x);
        }
    }
}
