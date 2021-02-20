#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 脚本用于显示屏幕中可点击的UI，
/// 但是条件并不充足，例如Image的RaycastTarget = true后，依然不会被射线检测到，
/// 此时需在Image上添加Button组件，才可以被射线检测到
/// </summary>
public class DebugUILine : MonoBehaviour
{
    public bool draw;
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        if (!draw) return;
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

            }
        }
    }
}
#endif
