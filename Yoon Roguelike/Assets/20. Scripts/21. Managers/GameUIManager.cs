using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : UIManager
{
    public static GameUIManager instance;

    [Space(10)]
    public CanvasGroup activityButtons;
    public CanvasGroup moveButtons;
    public CanvasGroup attackButtons;
    public CanvasGroup equipmentButtons;
    public CanvasGroup turnEnd;

    [Space(10)]
    public CanvasGroup moveActivityButton;
    public CanvasGroup attackActivityButton;
    public CanvasGroup equipmentActivityButton;

    [Space(10)]
    public List<CanvasGroup> moveDirButtons;    // 0: up, 1: right, 2: down, 3: left
    public List<CanvasGroup> attackDirButtons;  // 0: up, 1: right, 2: down, 3: left

    [Space(10)]
    public Transform weaponItemIconOptionsParent;
    public Transform armorItemIconOptionsParent;
    public GameObject itemIconButtonSample;
    public ItemHorizontalScrollManager weaponListScrollManager;
    public ItemHorizontalScrollManager armorListScrollManager;

    private PlayerController playerController;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        UION(activityButtons);
        UION(moveActivityButton);
        UION(attackActivityButton);
        UION(equipmentActivityButton);

        UIOFF(moveButtons);
        UIOFF(attackButtons);
        UIOFF(equipmentButtons);
        UIOFF(turnEnd);
    }

    public void MovementButtonsActivate()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Tile nowTIle = MapManager.instance.tiles[(int)playerPos.x, (int)playerPos.y];
        
        foreach(CanvasGroup cg in moveDirButtons)
        {
            cg.interactable = false;
        }

        foreach(Tile tile in nowTIle.adjacent)
        {
            Vector3 dir = tile.position - playerPos;
            
            if(dir == Vector3.up && !tile.unit)
            {
                UION(moveDirButtons[0]);
            }
            else if(dir == Vector3.right && !tile.unit)
            {
                UION(moveDirButtons[1]);
            }
            else if(dir == Vector3.down && !tile.unit)
            {
                UION(moveDirButtons[2]);
            }
            else if(dir == Vector3.left && !tile.unit)
            {
                UION(moveDirButtons[3]);
            }
        }
    }

    public void AttackButtonsActivate()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Tile nowTIle = MapManager.instance.tiles[(int)playerPos.x, (int)playerPos.y];

        foreach (CanvasGroup cg in attackDirButtons)
        {
            cg.interactable = false;
        }

        foreach(Tile tile in nowTIle.adjacent)
        {
            Vector3 dir = tile.position - playerPos;

            if (dir == Vector3.up && tile.unit)
            {
                UION(attackDirButtons[0]);
            }
            else if (dir == Vector3.right && tile.unit)
            {
                UION(attackDirButtons[1]);
            }
            else if (dir == Vector3.down && tile.unit)
            {
                UION(attackDirButtons[2]);
            }
            else if (dir == Vector3.left && tile.unit)
            {
                UION(attackDirButtons[3]);
            }
        }
    }

    public void PlayerUseUpTurnCount()
    {
        Init();

        moveActivityButton.interactable = false;
        moveActivityButton.blocksRaycasts = false;
        attackActivityButton.interactable = false;
        attackActivityButton.blocksRaycasts = false;
        equipmentActivityButton.interactable = false;
        equipmentActivityButton.blocksRaycasts = false;
    }

    public void PlayerTurnEnd()
    {
        UION(turnEnd);
        UIOFF(activityButtons);

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.TurnEnd();
    }

    public void PlayerMovement(string dir)
    {
        if (!playerController.isMoving && !playerController.isActioning)
        {
            playerController.MovementController(dir);
        }
    }

    public void PlayerAttack(string dir)
    {
        playerController.Attack(dir);
    }

    public void EquipmentButtonsActivate()
    {
        for(int i = 0; i < weaponItemIconOptionsParent.childCount; i++)
        {
            Destroy(weaponItemIconOptionsParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < armorItemIconOptionsParent.childCount; i++)
        {
            Destroy(armorItemIconOptionsParent.GetChild(i).gameObject);
        }

        foreach (Equipment equipment in EquipmentManager.instance.weapons)
        {
            GameObject go = Instantiate(itemIconButtonSample, weaponItemIconOptionsParent);
            go.transform.GetChild(0).GetComponent<Image>().sprite = equipment.sprite; // sprite
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = equipment.Name; // name
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = equipment.additionalBuff.ToString(); // buff by equipment
            go.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = equipment.additionalRange.ToString(); // range by equipment
        }

        foreach (Equipment equipment in EquipmentManager.instance.armings)
        {
            GameObject go = Instantiate(itemIconButtonSample, armorItemIconOptionsParent);
            go.transform.GetChild(0).GetComponent<Image>().sprite = equipment.sprite; // sprite
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = equipment.Name; // name
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = equipment.additionalBuff.ToString(); // buff by equipment
            go.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = equipment.additionalRange.ToString(); // range by equipment
        }

        weaponListScrollManager.Init(EquipmentManager.instance.weapons.Count);
        armorListScrollManager.Init(EquipmentManager.instance.armings.Count);
    }
}
