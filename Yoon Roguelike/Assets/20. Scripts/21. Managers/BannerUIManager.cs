using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BannerUIManager : UIManager
{
    public CanvasGroup characterSelectCG;
    public CanvasGroup gameOptionCG;

    public GameObject autoTurnEndCheckMark;
    public GameObject quickUnitMoveCheckMark;

    [Header("Characters Select")]
    public int characterIndex;
    public List<Unit> characters;
    public Image characterImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI turnCountText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI skillDescription;

    private CanvasGroup currentCG;

    public void StartButtonClick()
    {
        CharacterSelect(0);
        UION(characterSelectCG);

        currentCG = characterSelectCG;
    }

    public void GameOptionButtonClick()
    {
        UION(gameOptionCG);

        currentCG = gameOptionCG;
    }

    public void CGClose()
    {
        UIOFF(currentCG);
        currentCG = null;
    }

    public void AutoTurnEndCheck()
    {
        SettingManager.instance.autoTurnEnd = !SettingManager.instance.autoTurnEnd;
        autoTurnEndCheckMark.SetActive(SettingManager.instance.autoTurnEnd);
    }   
    
    public void QuickUnitMoveCheck()
    {
        SettingManager.instance.quickUnitMoveCheckMark = !SettingManager.instance.quickUnitMoveCheckMark;
        quickUnitMoveCheckMark.SetActive(SettingManager.instance.quickUnitMoveCheckMark);
    }

    public void CharacterSelect(int index)
    {
        characterIndex = index;
        characterImage.sprite = characters[index].sprite;
        nameText.text = characters[index].name;
        turnCountText.text = characters[index].turnCount.ToString();
        atkText.text = characters[index].atk.ToString();
        defenceText.text = characters[index].defence.ToString();
        descriptionText.text = characters[index].description;
        skillDescription.text = "<스킬설명>" + "\n" + characters[index].GetComponent<PassiveSkill>().description;
    }

    public void TurnStart()
    {
        GameManager.instance.playerIndex = characterIndex;
        SceneManager.LoadScene(1);
    }
}
