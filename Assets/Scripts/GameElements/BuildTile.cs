using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildTile : MonoBehaviour
{
    [SerializeField] Transform transPathSpawn;
    UnitBehaviour currentUnit;
    [SerializeField] int pathId;

    public bool Occupied { get => currentUnit != null && currentUnit.State != UnitState.Deactivated; }

    private void Start()
    {
       pathId = Array.IndexOf(PathPoints.Points, transPathSpawn);
    }

    public void Button_InstantiateSelection()
    {
        //Added here because Start might not work. (Script Execution Order?)
        if(pathId < 0) ValidatePathId();

        if (GamePlay.instance.UnitSelected < 0) return;

        var button = GamePlay.instance.GameplayUI.UnitSelectorButtons[GamePlay.instance.UnitSelected];

        if (button.UnitInfo.PlaceableAt == UnitPlacement.BuildPlace || button.UnitInfo.PlaceableAt == UnitPlacement.BuildPlaceAndPath)
        {
            if(Occupied)
            {
                Advice.ShowTextInWorld(transform.position, "Occupied");
                return;
            }

            if (!button.Interactable)
            {
                Advice.ShowTextInWorld(transform.position, "Not Available");
                return;
            }

            if (!GamePlay.PlayerHasEnoughCurrency(button.UnitInfo.Cost))
            {
                Advice.ShowTextInWorld(transform.position, "Not Enough Currency");
                return;
            }

            if (GamePlay.instance.AvailableAllyPools[button.UnitInfo.Unit].AvailableUnits.Count == 0)
            {
                Advice.ShowTextInWorld(transform.position, "Can't Spawn Yet");
                return;
            }

            GamePlay.instance.Currency -= button.UnitInfo.Cost;

            currentUnit = GamePlay.instance.AvailableAllyPools[button.UnitInfo.Unit].Dequeue();
            currentUnit.transform.position = transform.position;
            currentUnit.pathTargetId = pathId;
            currentUnit.Activate(-1);
        }
        else //Not Placeable
        {
            Advice.ShowTextInWorld(transform.position, "Not Placeable Here");
            return;
        }
        
    }


    void ValidatePathId()
    {
        pathId = Array.IndexOf(PathPoints.Points, transPathSpawn);
    }

    private void OnMouseDown()
    {
        Button_InstantiateSelection();
    }
}
