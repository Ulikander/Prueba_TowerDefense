using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTowerUnit : UnitBehaviour
{
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
        foreach (var _col in Physics2D.OverlapCircleAll(transform.position, range))
        {
            //print("_col: " + _col.name);
            if (_col.tag != (IsHostile ? "Ally" : "Enemy")) continue;
            attackTarget = _col.GetComponent<UnitBehaviour>();
            State = UnitState.Attacking;
            return;
        }


    }

    protected override void Attack()
    {
        if (!IsTargetCollidersNear)
        {
            State = UnitState.Waiting;
            return;
        }

        if (attackDelayCont <= 0)
        {
            Projectile _projectile = null;
            foreach (var proj in spawnedProjectiles[0])
            {
                if (proj.gameObject.activeInHierarchy) continue;
                _projectile = proj.GetComponent<Projectile>();
                break;
            }

            if (_projectile == null) return;

            _projectile.LaunchThis(transform.position, Power, attackTarget.transform, true);
            attackDelayCont = AtkSpeed;

            State = UnitState.Waiting;

            return;
        }

        attackDelayCont -= Time.deltaTime;

    }
}
