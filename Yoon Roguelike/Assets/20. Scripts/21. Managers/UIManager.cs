using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void UION(CanvasGroup cg)
    {
        cg.alpha = 1.0f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    public void UIOFF(CanvasGroup cg)
    {
        cg.alpha = 0.0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }
}
