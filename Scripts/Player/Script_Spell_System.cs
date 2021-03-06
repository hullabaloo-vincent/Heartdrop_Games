﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Spell_System : MonoBehaviour
{
    void Start()
    {
        activeSlots = new bool[] { false, false, false };
        inActiveValue = 0.2f;
        selectedSpell = 666;
        spells = new Dictionary<int, string>();
        spells.Add(1, "null");
        spells.Add(2, "null");
        spells.Add(3, "null");
        SetSpell("Fireball", 1);
        SetSpell("Icewave", 2);
        SetSpell("Waterblast", 3);

        //series of buttons
        Button fbt1 = fireButton1.GetComponent<Button>();
        fbt1.onClick.AddListener(TaskOnClickFire1);
        Button wbt1 = waterButton1.GetComponent<Button>();
        wbt1.onClick.AddListener(TaskOnClickWater1);
        Button ibt1 = iceButton1.GetComponent<Button>();
        ibt1.onClick.AddListener(TaskOnClickIce1);

        Button fbt2 = fireButton2.GetComponent<Button>();
        fbt2.onClick.AddListener(TaskOnClickFire2);
        Button wbt2 = waterButton2.GetComponent<Button>();
        wbt2.onClick.AddListener(TaskOnClickWater2);
        Button ibt2 = iceButton2.GetComponent<Button>();
        ibt2.onClick.AddListener(TaskOnClickIce2);

        Button fbt3 = fireButton3.GetComponent<Button>();
        fbt3.onClick.AddListener(TaskOnClickFire3);
        Button wbt3 = waterButton3.GetComponent<Button>();
        wbt3.onClick.AddListener(TaskOnClickWater3);
        Button ibt3 = iceButton3.GetComponent<Button>();
        ibt3.onClick.AddListener(TaskOnClickIce3);
    }

    public void SetSpell(string spellType, int slot)
    {
        int slotSelection = slot - 1;
        spells.Remove(slot);
        spells.Add(slot, spellType);
        Animator spellAnim = slots[slotSelection].GetComponent<Animator>();
        //reset animation for the slot
        ResetAnimation(spellAnim);
        //set the new animation
        spellAnim.SetBool(spellType, true);
        //activate the slot
        activeSlots[slotSelection] = true;
        //set the slot to semi-transparent
        var newColor = slots[slotSelection].GetComponent<Image>().color;
        newColor.a = inActiveValue;
        slots[slotSelection].GetComponent<Image>().color = newColor;
    }

    public void SelectSpell(int slot)
    {
        int slotSelection = slot - 1;
        selectedSpell = slotSelection;

        var fadeColor = slots[0].GetComponent<Image>().color;
        fadeColor.a = inActiveValue;
        var offColor = fadeColor;
        offColor.a = 0f;

        //make all slots turn off or make semi-transparent
        if (activeSlots[0])
        {
            slots[0].GetComponent<Image>().color = fadeColor;
        }
        else
        {
            slots[0].GetComponent<Image>().color = offColor;
        }

        if (activeSlots[1])
        {
            slots[1].GetComponent<Image>().color = fadeColor;
        }
        else
        {
            slots[1].GetComponent<Image>().color = offColor;
        }

        if (activeSlots[2])
        {
            slots[2].GetComponent<Image>().color = fadeColor;
        }
        else
        {
            slots[2].GetComponent<Image>().color = offColor;
        }

        //turn on selected slot
        fadeColor.a = 1f;
        slots[slotSelection].GetComponent<Image>().color = fadeColor;
    }

    public void DeselectSlot(int slot)
    {
        int slotSelection = slot - 1;
        //666 = no spells are selected
        selectedSpell = 666;
        var fadeColor = slots[0].GetComponent<Image>().color;
        fadeColor.a = inActiveValue;
        slots[slotSelection].GetComponent<Image>().color = fadeColor;
    }

    private void ResetAnimation(Animator anim)
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }

    public int CurrentlySelected()
    {
        return selectedSpell;
    }

    public string ActiveSpell()
    {
        int current = selectedSpell + 1;
        return spells[current];
    }

    //When a certain button is clicked at the shop, set the corresponding spell slot to that spell
    void TaskOnClickFire1()
    {
        SetSpell("Fireball", 1);
    }
    void TaskOnClickFire2()
    {
        SetSpell("Fireball", 2);
    }
    void TaskOnClickFire3()
    {
        SetSpell("Fireball", 3);
    }

    void TaskOnClickIce1()
    {
        SetSpell("Icewave", 1);
    }
    void TaskOnClickIce2()
    {
        SetSpell("Icewave", 2);
    }
    void TaskOnClickIce3()
    {
        SetSpell("Icewave", 3);
    }

    void TaskOnClickWater1()
    {
        SetSpell("Waterblast", 1);
    }
    void TaskOnClickWater2()
    {
        SetSpell("Waterblast", 2);
    }
    void TaskOnClickWater3()
    {
        SetSpell("Waterblast", 3);
    }




    public Button iceButton1;
    public Button fireButton1;
    public Button waterButton1;

    public Button iceButton2;
    public Button fireButton2;
    public Button waterButton2;

    public Button iceButton3;
    public Button fireButton3;
    public Button waterButton3;

    public GameObject[] slots;
    public bool[] activeSlots;
    int selectedSpell;
    string selectedSpellType;

    float inActiveValue;

    Dictionary<int, string> spells;
}
