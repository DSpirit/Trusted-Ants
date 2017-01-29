using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowTooltip : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject TooltipPrefab;
    public GameObject Tooltip;
    // Use this for initialization
    void Start()
    {
        TooltipPrefab = (GameObject) Resources.Load("Prefabs/Tooltip");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Tooltip = (GameObject) Instantiate(TooltipPrefab, eventData.selectedObject.transform);
        eventData.selectedObject.transform.FindChild("Handle Slide Area").transform.FindChild("Handle").localScale = new Vector3(3f,3f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        var handle = eventData.selectedObject.transform.FindChild("Handle Slide Area").transform.FindChild("Handle").gameObject;
        float value = eventData.selectedObject.GetComponent<Slider>().value;
        string format = eventData.selectedObject.GetComponent<Slider>().maxValue <= 1 ? "P0" : "N0";
        handle.GetComponent<Image>().color = Color.cyan;
        if (handle.transform.childCount < 1)
        {
            var go = new GameObject("Value");
            go.transform.parent = handle.transform;
            var text = go.AddComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 10);
            text.fontSize = 10;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;
            go.transform.localPosition = new Vector3(0, 0, 0);
            text.transform.localPosition = new Vector3(0, 0, 0);
            text.text = value.ToString(format);
        }
        else
        {
            var text = handle.GetComponentInChildren<Text>();
            text.text = value.ToString(format);
        }
        
        //var pos = eventData.position;
        //pos.y = pos.y + 15f;
        //Tooltip.transform.position = pos; 
        //Tooltip.GetComponentInChildren<Text>().text = eventData.selectedObject.GetComponent<Slider>().value.ToString();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        eventData.selectedObject.transform.FindChild("Handle Slide Area").transform.FindChild("Handle").localScale = new Vector3(1f, 1f);
        //Destroy(Tooltip);
    }
}
