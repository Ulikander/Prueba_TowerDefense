using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleUnit : UnitBehaviour
{
    protected override void BehaviourHandle()
    {
        //base.BehaviourHandle();
        if (State != UnitState.Deactivated && HP <= 0) State = UnitState.Dead;

        switch (State)
        {
            case UnitState.Waiting:
                break;
            case UnitState.Dead:
                Die();
                break;
        }
    }
    protected override void Die()
    {
        if (dieCont == deathTime)
        {
            //anim.Play("Die");
            GamePlay.instance.Lose();
        }
        dieCont -= Time.deltaTime;
        //if (dieCont <= 0) Deactivate();
    }

    private void OnMouseDown()
    {
        if (GamePlay.instance.UnitSelected < 0) return;

        var _button = GamePlay.instance.GameplayUI.UnitSelectorButtons[GamePlay.instance.UnitSelected];
        if (_button.UnitInfo.PlaceableAt != UnitPlacement.Spawnable)
        {
            Advice.ShowTextInWorld(transform.position, "Not Placeable Here");
            return;
        }

        if (!_button.Interactable)
        {
            Advice.ShowTextInWorld(transform.position, "Not Available");
            return;
        }

        if (!GamePlay.PlayerHasEnoughCurrency(_button.UnitInfo.Cost))
        {
            Advice.ShowTextInWorld(transform.position, "Not Enough Currency");
            return;
        }

        if (GamePlay.instance.AvailableAllyPools[_button.UnitInfo.Unit].AvailableUnits.Count == 0)
        {
            Advice.ShowTextInWorld(transform.position, "Can't Spawn Yet");
            return;
        }

        var _unit = GamePlay.instance.AvailableAllyPools[_button.UnitInfo.Unit].Dequeue();
        GamePlay.instance.Currency -= _button.UnitInfo.Cost;
        _unit.Activate(PathPoints.Points.Length - 1);
        attackDelayCont = AtkSpeed;

    }
}
