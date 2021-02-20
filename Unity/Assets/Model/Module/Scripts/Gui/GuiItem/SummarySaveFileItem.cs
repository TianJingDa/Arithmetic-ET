using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SummarySaveFileItem : SaveFileItem, IPointerDownHandler, IPointerExitHandler
{
    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
        saveFileName = CommonTool.GetComponentContainsName<Text>(gameObject, "SaveFileName");
        saveFileType_Time = CommonTool.GetComponentContainsName<Text>(gameObject, "SaveFileType_Time");
        saveFileType_Number = CommonTool.GetComponentContainsName<Text>(gameObject, "SaveFileType_Number");
        saveFileAchiOrBLE = CommonTool.GetComponentContainsName<Image>(gameObject, "SaveFileAchiOrBLE");
    }

    public new void OnPointerDown(PointerEventData eventData) { }
    public new void OnPointerExit(PointerEventData eventData) { }
}
