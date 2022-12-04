using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TurnOtherController))]
public class Unit : MonoBehaviour
{
    public bool isMoving = false;
    public bool isActioning = false;

    [Space(10)]
    public int turnMaxCount;
    public int turnCount;
    public int maxHP;
    public int HP;
    public int atk;
    public int defence;
    [Multiline]
    public string description;
    public Sprite sprite;

    public int killCount;

    [Space(10)]
    public Tile nowTile;

    public List<PassiveSkill> passiveSkills = new List<PassiveSkill>();

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected TurnOtherController turnOtherController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        turnOtherController = GetComponent<TurnOtherController>();

        passiveSkills = GetComponents<PassiveSkill>().ToList();
    }

    protected void UnitStart()
    {
        nowTile = MapManager.instance.GetTileByVector3(transform.position);
        nowTile.OnUnit(this);

        turnCount = turnMaxCount;
    }

    public void UnitPlacement(Vector2 placePos)
    {
        transform.position = placePos;
        UnitStart();
    }

    public void TurnEnd()
    {
        turnCount = turnMaxCount;
        turnOtherController.TurnEnd();
    }

    /// <summary>
    /// 타격으로 인해 유닛이 죽으면 True 값을 반환합니다.
    /// </summary>
    /// <returns>true is Die</returns>
    public bool TakeHit(int dmg)
    {
        int newDefence = defence + GetAdditionalDefenceWithPassiveSkill();
        if(CompareTag("Player"))
        {
            newDefence += EquipmentManager.instance.GetAdditionalArmor();
        }
        int newDmg = dmg - newDefence <= 0 ? 1 : dmg - newDefence;
        HP = HP - newDmg < 0 ? 0 : HP - newDmg;

        if(HP <= 0)
        {
            animator.SetTrigger("Die");
            Debug.Log(name + " is Die...");
            // Die();

            return true;
        }
        else
        {
            animator.SetTrigger("Hit");

            return false;
        }
    }

    public void RecoveryHealth(int rh)
    {
        HP = HP + rh > maxHP ? maxHP : HP + rh;
    }

    protected IEnumerator Movement(Vector3 targetPos)
    {
        nowTile.ExitUnit();
        nowTile = MapManager.instance.GetTileByVector3(targetPos);
        nowTile.OnUnit(this);

        isMoving = true;
        isActioning = true;
        animator.SetBool("Walk", isMoving);

        // 유닛 방향
        if (targetPos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.01f);
            yield return null;

            if (transform.position.Equals(targetPos))
            {
                break;
            }
        }
        isMoving = false;
        isActioning = false;
        animator.SetBool("Walk", isMoving);

        turnCount--;

        if(CompareTag("Player"))
        {
            GameUIManager.instance.MovementButtonsActivate();
        }
    }

    protected void Attack(Vector3 targetPos)
    {
        Unit target = MapManager.instance.tiles[(int)targetPos.x, (int)targetPos.y].unit;

        int newAtk = atk + GetAdditionalAtkWithPassiveSkill();
        if (CompareTag("Player"))
        {
            newAtk += EquipmentManager.instance.GetAdditionalAtk();
        }

        if (target.TakeHit(newAtk))
        {
            killCount++;
        }
        animator.SetTrigger("Attack");
    }

    protected int GetAdditionalAtkWithPassiveSkill()
    {
        int additionalAtk = 0;
        foreach(PassiveSkill passive in passiveSkills)
        {
            additionalAtk += passive.additionalAtk;
        }
        return additionalAtk;
    }

    protected int GetAdditionalDefenceWithPassiveSkill()
    {
        int additionalDefence = 0;
        foreach(PassiveSkill passive in passiveSkills)
        {
            additionalDefence += passive.additionaldefence;
        }
        return additionalDefence;
    }
}
