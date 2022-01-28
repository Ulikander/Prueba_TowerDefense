using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnitSelectorButton : MonoBehaviour
{
    [Header("Info")]
    public UnitSelectorButtonInfo UnitInfo;
    [HideInInspector] public int Index;
    [Header("UI")]
    [SerializeField] Button buttonMain;
    public bool Interactable { get => buttonMain.interactable && !objUnavailable.activeInHierarchy; }
    [SerializeField] Image imgPreview, imgSelected;
    [SerializeField] GameObject objUnavailable;
    [SerializeField] TextMeshProUGUI textCost, textName, textUnitsLeft;

    private void Start()
    {
        imgPreview.sprite = UnitInfo.SpritePreview;
        imgSelected.enabled = false;
        textCost.text = UnitInfo.Cost.ToString();
        textUnitsLeft.text = "-";
        textName.text = UnitInfo.DisplayName;
    }

    /// <summary>
    /// Should be called from the GameplayUI. Updates this button's state and graphics.
    /// </summary>
    /// <param name="_selected">Does the current unit selection belong to this button.</param>
    /// <param name="_interactable">Should this button be selectable</param>
    /// <param name="_unitsLeft">How many units can the player spawn of this particular unit</param>
    public void UpdateValues(bool _selected, bool _interactable, int _unitsLeft)
    {
        imgSelected.enabled = _selected;
        buttonMain.interactable = _interactable;
        objUnavailable.SetActive(!_interactable);
        textUnitsLeft.text = UnitInfo.MaxUnits == 0 ? "-" : $"x{_unitsLeft}";
    }

    public void Button_Select()
    {
        GamePlay.instance.UnitSelected = Index;
    }
}
