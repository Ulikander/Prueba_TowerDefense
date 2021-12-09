using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTowerUnit : UnitBehaviour
{

    /* pathTargetId
     * 
     */
    [SerializeField] UnitDefinition unitToSpawn;
   
    protected override void BehaviourHandle()
    {
        if (HP <= 0 && State != UnitState.Deactivated) State = UnitState.Dead;

        switch (State)
        {
            case UnitState.Waiting:
                WaitingBehaviour();
                break;
            case UnitState.Attacking:
                Attack();
                break;
            case UnitState.Dead:
                //Explode Anim?
                Deactivate();
                break;
            case UnitState.Deactivated:
                return;
        }
        
    }


    void WaitingBehaviour()
    {
        if(attackDelayCont > 0)
        {
            attackDelayCont -= Time.deltaTime;
            return;
        }
        State = UnitState.Attacking;
    }

    protected override void Attack()
    {
        if(GamePlay.instance.AvailableAllyPools[unitToSpawn].AvailableUnits.Count == 0)
        {
            //cannot spawn units yet.
            return;
        }

        var _unit = GamePlay.instance.AvailableAllyPools[unitToSpawn].Dequeue();
        Debug.LogWarning($"Tower spawns unit at: {pathTargetId}");
        _unit.Activate(pathTargetId);
        attackDelayCont = AtkSpeed;

        State = UnitState.Waiting;
    }

}
