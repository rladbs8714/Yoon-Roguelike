using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHorizontalScrollManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public int itemCount;
    public int targetItemIconIndex;
    public float itemIconDragThrust;
    public string itemType;

    public Scrollbar scrollbar;
    public Transform content;

    private bool isDrag = false;
    private float iconDistance;
    private float targetItemIconPos;
    private float currentItemIconPos;

    private List<float> itemIconPos = new List<float>();

    private void Update()
    {
        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetItemIconPos, 0.1f);
        }
    }

    public void Init(int itemCount)
    {
        itemIconPos.Clear();
        this.itemCount = itemCount;

        iconDistance = 1f / (itemCount - 1);
        for (int i = 0; i < itemCount; i++)
        {
            itemIconPos.Add(iconDistance * i);
        }
        targetItemIconPos = GetItemIconPosition();

        EquipmentManager.instance.ChangeEquipment(itemType, targetItemIconIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;

        currentItemIconPos = GetItemIconPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        targetItemIconPos = GetItemIconPosition();

        if(currentItemIconPos == targetItemIconPos)
        {
            if(eventData.delta.x > itemIconDragThrust && currentItemIconPos - iconDistance >= 0)
            {
                targetItemIconIndex--;
                targetItemIconPos = currentItemIconPos - iconDistance;
            }
            else if(eventData.delta.x < -itemIconDragThrust && currentItemIconPos + iconDistance <= 1.01f)
            {
                targetItemIconIndex++;
                targetItemIconPos = currentItemIconPos + iconDistance;
            }
        }

        EquipmentManager.instance.ChangeEquipment(itemType, targetItemIconIndex);

        isDrag = false;
    }

    private float GetItemIconPosition()
    {
        // 절반거리를 기준으로 가까운 위치를 반환
        for (int i = 0; i < itemCount; i++)
        {
            if (scrollbar.value < itemIconPos[i] + iconDistance * 0.5f && scrollbar.value > itemIconPos[i] - iconDistance * 0.5f)
            {
                targetItemIconIndex = i;
                return itemIconPos[i];
            }
        }
        return currentItemIconPos;
    }
}
