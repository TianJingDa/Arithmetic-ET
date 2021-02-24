//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
///// <summary>
///// 挂在GridPanel上
///// </summary>
//[RequireComponent(typeof(GridLayoutGroup))]
//public class InfiniteList : MonoBehaviour 
//{
//    private int itemAmount;                          //子物体的数量
//    private int dataAmount;                          //信息的数量
//    private int minAmount;                           //Mathf.Min(itemAmount, dataAmount)
//    private int realIndex;                           //信息的序号
//    private int extra = 2;                           //额外行（列）数
//    private int maxInView;                           //可显示的最大行（列）数
//    private bool init = false;                       //初始化
//    private GuiItemID id;                         //prefabItem名字
//    private ArrayList dataList;                      //实际信息
//    private Vector3 startPosition;
//    private RectTransform gridRectTransform;
//    private RectTransform parentRectTransform;
//    private GridLayoutGroup gridLayoutGroup;
//    private ScrollRect scrollRect;
//    private List<RectTransform> children;
//    private List<Vector2> childrenAnchoredPostion;


//    private void Init(GuiItemID id, GameObject detailWin = null, GameObject deleteWin = null)
//    {
//        if (init) return;
//        this.id = id;
//        gridRectTransform = GetComponent<RectTransform>();
//        gridLayoutGroup = GetComponent<GridLayoutGroup>();
//        scrollRect = transform.parent.GetComponent<ScrollRect>();
//        scrollRect.onValueChanged.AddListener((data) => { ScrollCallback(data); });
//        parentRectTransform = scrollRect.transform as RectTransform;
//        children = new List<RectTransform>();
//        childrenAnchoredPostion = new List<Vector2>();
//        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
//        {
//            int row = (int)(parentRectTransform.rect.height / (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
//            itemAmount = (2 * row + extra) * gridLayoutGroup.constraintCount;
//            maxInView = row;
//        }
//        else
//        {
//            int column = (int)(parentRectTransform.rect.width / (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x));
//            itemAmount = (2 * column + extra) * gridLayoutGroup.constraintCount;
//            maxInView = column;
//        }
//        for (int i = 0; i < itemAmount; i++)
//        {
//            GameObject item = GameManager.Instance.GetPrefabItem(id);
//            item.transform.SetParent(transform);
//            item.transform.localScale = Vector3.one;
//        }
//        if (detailWin != null)
//        {
//            for (int i = 0; i < transform.childCount; i++)
//            {
//                transform.GetChild(i).SendMessage("InitDetailWin", detailWin);
//            }
//        }
//        if (deleteWin != null)
//        {
//            for (int i = 0; i < transform.childCount; i++)
//            {
//                transform.GetChild(i).SendMessage("InitDeleteWin", deleteWin);
//            }
//        }
//        StartCoroutine(RecordChildrenAnchoredPostion());
//        init = true;
//    }

//    private void InitList(ArrayList dataList, GuiItemID id, GameObject detailWin = null, GameObject deleteWin = null)
//    {
//        Init(id, detailWin, deleteWin);
//        gridRectTransform.offsetMin = Vector2.zero;
//        gridRectTransform.offsetMax = Vector2.zero;
//        this.dataList = dataList;
//        dataAmount = dataList.Count;
//        realIndex = -1;
//        children.Clear();
//        minAmount = Mathf.Min(itemAmount, dataAmount);
//        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
//        {
//            float height = Mathf.CeilToInt(minAmount / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
//            gridRectTransform.offsetMin -= new Vector2(0f, (height - gridRectTransform.rect.height));
//            startPosition = GetMaxToWorldPos();
//        }
//        else
//        {
//            float width = Mathf.CeilToInt(minAmount / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
//            gridRectTransform.offsetMax += new Vector2((width - gridRectTransform.rect.width), 0f);
//            startPosition = GetMinToWorldPos();
//        }

//        for (int index = 0; index < itemAmount; index++)
//        {
//            children.Add(transform.GetChild(index).GetComponent<RectTransform>());
//            children[index].gameObject.SetActive(index < dataAmount);
//            if(index < dataAmount)
//            {
//                realIndex++;
//                children[index].gameObject.name = id + realIndex.ToString();
//                children[index].gameObject.SendMessage("InitPrefabItem", dataList[realIndex]);
//            }
//        }
//        if (childrenAnchoredPostion.Count > 0)
//        {
//            for (int index = 0; index < transform.childCount; index++)
//            {
//                children[index].anchoredPosition = childrenAnchoredPostion[index];
//            }
//        }
//        else
//        {
//            StartCoroutine(RefreshChildrenAnchoredPostion());
//        }
//        StartCoroutine(EnableGrid());
//        //if(itemAmount <= childrenAnchoredPostion.Count)
//        //{
//        //    for(int index = 0; index < itemAmount; index++)
//        //    {
//        //        children[index].anchoredPosition = childrenAnchoredPostion[index];
//        //    }
//        //}
//    }

//    void ScrollCallback(Vector2 data)
//    {
//        UpdateChildren();
//    }

//    void UpdateChildren()
//    {
//        gridLayoutGroup.enabled = false;

//        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
//        {
//            Vector3 currentPos = GetMaxToWorldPos();

//            float offsetY = currentPos.y - startPosition.y;

//            if (offsetY > 0.001 && gridRectTransform.offsetMax.y > 0)
//            {
//                //向上拉，向下扩展;
//                {
//                    if (realIndex >= dataAmount - 1)
//                    {
//                        startPosition = currentPos;
//                        return;
//                    }

//                    float scrollRectUp = scrollRect.transform.TransformPoint(parentRectTransform.rect.max + new Vector2(0, maxInView * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y))).y;
//                    float childBottom = children[0].transform.TransformPoint(children[0].rect.min).y;

//                    if (childBottom >= scrollRectUp)
//                    {
//                        //GridLayoutGroup 底部加长;
//                        gridRectTransform.offsetMin -= new Vector2(0, gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

//                        //移动到底部;
//                        for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
//                        {
//                            children[index].SetAsLastSibling();
//                            children[index].anchoredPosition = new Vector2(children[index].anchoredPosition.x, children[children.Count - 1].anchoredPosition.y - gridLayoutGroup.cellSize.y - gridLayoutGroup.spacing.y);
//                            realIndex++;
//                            if (realIndex > dataAmount - 1)
//                            {
//                                children[index].gameObject.SetActive(false);
//                            }
//                            else
//                            {
//                                children[index].gameObject.name = id + realIndex.ToString();
//                                children[index].gameObject.SendMessage("InitPrefabItem", dataList[realIndex]);
//                            }
//                        }

//                        //更新child;
//                        for (int index = 0; index < children.Count; index++)
//                        {
//                            children[index] = transform.GetChild(index).GetComponent<RectTransform>();
//                        }
//                    }
//                }
//            }
//            else //if (offsetY <= 0 )//|| gridRectTransform.offsetMax.y <= 0)
//            {
//                //向下拉，下面收缩;
//                if (realIndex  <= children.Count - 1)
//                {
//                    startPosition = currentPos;
//                    return;
//                }
//                float scrollRectBottom = scrollRect.transform.TransformPoint(parentRectTransform.rect.min).y;
//                float childUp = children[children.Count - 1].transform.TransformPoint(children[children.Count - 1].rect.max).y;

//                if (childUp < scrollRectBottom)
//                {
//                    //GridLayoutGroup 底部缩短;
//                    gridRectTransform.offsetMin += new Vector2(0, gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

//                    //把底部的一行 移动到顶部
//                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
//                    {
//                        children[children.Count - 1 - index].SetAsFirstSibling();
//                        children[children.Count - 1 - index].anchoredPosition = new Vector2(children[children.Count - 1 - index].anchoredPosition.x, children[0].anchoredPosition.y + gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
//                        children[children.Count - 1 - index].gameObject.SetActive(true);
//                        children[children.Count - 1 - index].gameObject.name = id + (realIndex - minAmount).ToString();
//                        children[children.Count - 1 - index].gameObject.SendMessage("InitPrefabItem", dataList[realIndex - minAmount]);
//                        realIndex--;
//                    }


//                    //更新child;
//                    for (int index = 0; index < children.Count; index++)
//                    {
//                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
//                    }
//                }
//            }
//            startPosition = currentPos;
//        }
//        else
//        {
//            Vector3 currentPos = GetMinToWorldPos();

//            float offsetX = currentPos.x - startPosition.x;

//            if (offsetX < -0.001 && gridRectTransform.offsetMin.x < 0)
//            {
//                //向左拉，向右扩展;
//                {
//                    if (realIndex >= dataAmount - 1)
//                    {
//                        startPosition = currentPos;
//                        return;
//                    }

//                    float scrollRectLeft = scrollRect.transform.TransformPoint(parentRectTransform.rect.min - new Vector2(maxInView * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x), 0)).x;
//                    float childRight = children[0].transform.TransformPoint(children[0].rect.max).x;

//                    if (childRight <= scrollRectLeft)
//                    {
//                        //GridLayoutGroup 右侧加长;
//                        gridRectTransform.offsetMax += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);

//                        //移动到右边;
//                        for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
//                        {
//                            children[index].SetAsLastSibling();
//                            children[index].anchoredPosition = new Vector2(children[children.Count - 1].anchoredPosition.x + gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, children[index].anchoredPosition.y);
//                            realIndex++;

//                            if (realIndex > dataAmount - 1)
//                            {
//                                children[index].gameObject.SetActive(false);
//                            }
//                            else
//                            {
//                                children[index].gameObject.name = id + realIndex.ToString();
//                                children[index].gameObject.SendMessage("InitPrefabItem", dataList[realIndex]);
//                            }
//                        }


//                        //更新child;
//                        for (int index = 0; index < children.Count; index++)
//                        {
//                            children[index] = transform.GetChild(index).GetComponent<RectTransform>();
//                        }
//                    }
//                }
//            }
//            else //if (offsetX > 0.01)
//            {
//                //向右拉，右边收缩;
//                if (realIndex  <= children.Count - 1)
//                {
//                    startPosition = currentPos;
//                    return;
//                }
//                float scrollRectRight = scrollRect.transform.TransformPoint(parentRectTransform.rect.max).x;
//                float childLeft = children[children.Count - 1].transform.TransformPoint(children[children.Count - 1].rect.min).x;

//                if (childLeft >= scrollRectRight)
//                {
//                    //GridLayoutGroup 右侧缩短;
//                    gridRectTransform.offsetMax -= new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);

//                    //把右边的一行 移动到左边;
//                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
//                    {
//                        children[children.Count - 1 - index].SetAsFirstSibling();
//                        children[children.Count - 1 - index].anchoredPosition = new Vector2(children[0].anchoredPosition.x - gridLayoutGroup.cellSize.x - gridLayoutGroup.spacing.x, children[children.Count - 1 - index].anchoredPosition.y);
//                        children[children.Count - 1 - index].gameObject.SetActive(true);
//                        children[children.Count - 1 - index].gameObject.name = id + (realIndex - minAmount).ToString();
//                        children[children.Count - 1 - index].gameObject.SendMessage("InitPrefabItem", dataList[realIndex - minAmount]);
//                        realIndex--;
//                    }


//                    //更新child;
//                    for (int index = 0; index < children.Count; index++)
//                    {
//                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
//                    }
//                }
//            }
//            startPosition = currentPos;
//        }
//    }
//    /// <summary>
//    /// 获取Grid右上角的世界坐标
//    /// </summary>
//    /// <returns></returns>
//    private Vector3 GetMaxToWorldPos()
//    {
//        Vector3 max = transform.TransformPoint(gridRectTransform.rect.max);
//        return max;
//    }
//    /// <summary>
//    /// 获取Grid左下角的世界坐标
//    /// </summary>
//    /// <returns></returns>
//    private Vector3 GetMinToWorldPos()
//    {
//        Vector3 min = transform.TransformPoint(gridRectTransform.rect.min);
//        return min;
//    }
//    /// <summary>
//    /// 延迟两帧记录所有子物体的锚点位置
//    /// 因为GridLayoutGroup是下一帧才刷新子物体的布局，如果仅延迟一帧进行记录，有可能记录的是刷新子物体布局前的位置
//    /// 这就会产生一个随机的BUG，如果是先刷新子物体的布局，就正常显示，如果是先记录的布局，就会导致子物体位置异常
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator RecordChildrenAnchoredPostion()
//    {
//        yield return null;
//        yield return null;
//        for (int index = 0; index < transform.childCount; index++)
//        {
//            childrenAnchoredPostion.Add(transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition);
//        }
//    }
//    /// <summary>
//    /// 延迟三帧刷新子物体的锚点，此方法仅在第一次刷新列表时候使用，因为要保证在RecordChildrenAnchoredPostion方法之后执行
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator RefreshChildrenAnchoredPostion()
//    {
//        yield return null;
//        yield return null;
//        yield return null;
//        for (int index = 0; index < transform.childCount; index++)
//        {
//            children[index].anchoredPosition = childrenAnchoredPostion[index];
//        }
//    }
//    /// <summary>
//    /// 添加PrefabItem时会改变grid的位置，从而触发了ScrollCallback，而此时grid还未对PrefabItem进行布局
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator EnableGrid()
//    {
//        yield return new WaitForEndOfFrame();
//        gridLayoutGroup.enabled = true;
//    }
//}
