using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float movSpeed;

    public void LaunchThis(Vector3 _spawnPoint, int _power, Transform _target, bool _lookAt)
    {
        transform.position = _spawnPoint;
        target = _target;
        lookAtTarget = _lookAt;
        power = _power;

        gameObject.SetActive(true);
    }

    protected Transform target;
    bool lookAtTarget;
    protected int power;
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, movSpeed * Time.deltaTime);
        //transform.LookAt(target);

        if(Vector2.Distance(transform.position, target.position) < 0.01f)
        {
            OnReachingTarget();
        }
    }

    protected virtual void OnReachingTarget()
    {
        target.GetComponent<UnitBehaviour>().HP -= power;
        gameObject.SetActive(false);
    }
}
