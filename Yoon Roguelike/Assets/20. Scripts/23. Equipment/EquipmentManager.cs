using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance { private set; get; } = null;

    public Equipment nowWeapon;
    public Equipment nowArming;

    public List<Equipment> weapons;
    public List<Equipment> armings;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void AddEquipment(Equipment equipment)
    {
        switch(equipment.Tag)
        {
            case "weapon":
                weapons.Add(equipment);
                break;
            case "armor":
                armings.Add(equipment);
                break;

            default:
                Debug.Log("���� ���� �������� �߰��Ϸ� �մϴ�.");
                return;
        }
    }

    public void RemoveEquipment(Equipment equipment)
    {
        switch (equipment.Tag)
        {
            case "weapon":
                weapons.Remove(equipment);
                break;
            case "armor":
                armings.Remove(equipment);
                break;

            default:
                Debug.Log("���� ���� �������� �����Ϸ� �մϴ�.");
                return;
        }
    }

    public void ChangeEquipment(string switchCase, int equipmentIndex)
    {
        switch (switchCase)
        {
            case "weapon":
                nowWeapon = weapons[equipmentIndex];
                print(nowWeapon.Name);
                break;
            case "armor":
                nowArming = armings[equipmentIndex];
                print(nowArming.Name);
                break;

            default:
                Debug.Log("���� ���� ���������� �����Ϸ� �մϴ�.");
                return;
        }
    }

    public int GetAdditionalAtk()
    {
        return nowWeapon != null ? nowWeapon.additionalBuff : 0;
    }

    public int GetAdditionalArmor()
    {
        return nowArming != null ? nowArming.additionalBuff : 0;
    }

    public int GetAdditionalRange()
    {
        return nowWeapon != null ? nowWeapon.additionalRange : 0;
    }
}
