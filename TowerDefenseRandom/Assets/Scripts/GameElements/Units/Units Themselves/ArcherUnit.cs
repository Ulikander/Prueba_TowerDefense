using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : UnitBehaviour
{
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
            foreach(var proj in spawnedProjectiles[0])
            {
                if (proj.gameObject.activeInHierarchy) continue;
                _projectile = proj;
                break;
            }

            if (_projectile == null) return;

            _projectile.LaunchThis(transform.position, Power, attackTarget.transform, true);
            attackDelayCont = AtkSpeed;

            if (attackTarget.State == UnitState.Dead || attackTarget.State == UnitState.Deactivated)
            {
                State = UnitState.Waiting;
                return;
            }

            return;
        }

        attackDelayCont -= Time.deltaTime;
    }
}
