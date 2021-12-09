using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : Projectile
{
    [SerializeField] float explosionRange;
    protected override void OnReachingTarget()
    {
        //List<UnitBehaviour> _affected = new List<UnitBehaviour>();
        bool isHostile = target.tag == "Ally";

        foreach (var _col in Physics2D.OverlapCircleAll(transform.position, explosionRange))
        {
            if (_col.tag != (isHostile ? "Ally" : "Enemy")) continue;
            _col.GetComponent<UnitBehaviour>().HP -= power;
        }
        
        gameObject.SetActive(false);

    }
}
