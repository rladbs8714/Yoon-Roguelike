using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                spriteRenderer.flipX = true;
                MovementController("left");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                spriteRenderer.flipX = false;
                MovementController("right");
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MovementController("up");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MovementController("down");
            }
        }

        if (!isActioning && turnCount <= 0)
        {
            GameUIManager.instance.PlayerUseUpTurnCount();
            if (SettingManager.instance.autoTurnEnd)
            {
                GameUIManager.instance.PlayerTurnEnd();
                TurnEnd();
            }
        }
    }

    /// <summary>
    /// 이동 관련 처리와 플레이어의 이동에 따른 타일 매개변수 값 변경도 병행 함.
    /// </summary>
    public void MovementController(string dir)
    {
        Vector3 targetPos;
        switch(dir)
        {
            case "up":
                targetPos = GetTargetPos(Vector3.up);
                break;
            case "down":
                targetPos = GetTargetPos(Vector3.down);
                break;
            case "left":
                targetPos = GetTargetPos(Vector3.left);
                break;
            case "right":
                targetPos = GetTargetPos(Vector3.right);
                break;

            default:
                Debug.Log("MovementController: 옳지 않은 값이 들어왔습니다.");
                targetPos = Vector3.zero;
                break;
        }

        StartCoroutine(Movement(targetPos));
    }

    public void Attack(string dir)
    {
        Vector3 targetPos = Vector3.zero;
        switch (dir)
        {
            case "up":
                targetPos = GetTargetPos(Vector3.up);
                break;
            case "down":
                targetPos = GetTargetPos(Vector3.down);
                break;
            case "left":
                targetPos = GetTargetPos(Vector3.left);
                break;
            case "right":
                targetPos = GetTargetPos(Vector3.right);
                break;

            default:
                Debug.Log("MovementController: 옳지 않은 값이 들어왔습니다.");
                return;
        }

        turnCount--;
        
        Attack(targetPos);
        GameUIManager.instance.AttackButtonsActivate();
    }

    private Vector3 GetTargetPos(Vector3 dir)
    {
        Vector2 targetPos = this.transform.position + dir;
        if (targetPos.x < 0 || targetPos.x >= MapManager.instance.tiles.GetLength(0) || targetPos.y < 0 || targetPos.y >= MapManager.instance.tiles.GetLength(1))
        {
            return this.transform.position;
        }
        if (MapManager.instance.tiles[(int)targetPos.x, (int)targetPos.y].tag.Equals("Wall"))
        {
            return this.transform.position;
        }

        return targetPos;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Next Point"))
        {
            if (!isMoving && !isActioning)
            {
                RoundManager.instance.NextRound();
            }
        }
    }
}
