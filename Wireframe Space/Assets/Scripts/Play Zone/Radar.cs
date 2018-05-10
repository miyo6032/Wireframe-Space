using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {

    public GameObject objPrefab;
    List<GameObject> radarObjects = new List<GameObject>();
    List<GameObject> borderObjects = new List<GameObject>();
    float switchDistance;
    public Transform helperTransform;

    void Start()
    {
        switchDistance = GameObject.Find("Radar Camera").GetComponent<Camera>().orthographicSize * 0.8f;
        GameObject.Find("Radar Camera").transform.rotation = Quaternion.Euler(0, 0, PlayZoneManager.instance.player.transform.eulerAngles.z - 90 + GameManager.instance.player.direction);
    }
	
    public void AddToRadar(GameObject obj)//Add a gameobject to radar checking
    {
        GameObject radar = Instantiate(objPrefab);
        GameObject border = Instantiate(objPrefab);
        radar.transform.position = obj.transform.position;
        radar.transform.SetParent(obj.transform);
        radarObjects.Add(radar);
        borderObjects.Add(border);
    }

	void Update () {
        if (radarObjects.Count == 0)//Move this inward later!!!
        {
            PlayZoneManager.instance.MissionCompleted();
        }
        for (int i = 0; i < radarObjects.Count; i++)//Creates a radar by using radar objects in the radar circle, and border objects on the border of the circle
        {
            if(radarObjects[i] == null)
            {
                radarObjects.Remove(radarObjects[i]);
                borderObjects.Remove(borderObjects[i]);
                continue;
            }
            if(Vector2.Distance(radarObjects[i].transform.position, transform.position) > switchDistance)
            {
                helperTransform.LookAt(radarObjects[i].transform);
                borderObjects[i].transform.position = transform.position + switchDistance * helperTransform.forward;
                borderObjects[i].layer = LayerMask.NameToLayer("Radar");
                radarObjects[i].layer = LayerMask.NameToLayer("Invisible");
            }
            else
            {
                radarObjects[i].layer = LayerMask.NameToLayer("Radar");
                borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
            }
        }
	}
}
