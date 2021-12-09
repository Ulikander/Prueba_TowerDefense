using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPool : MonoBehaviour
{
    public UnitDefinition Unit;
    [SerializeField] GameObject prefabUnit;
    [SerializeField] int unitsToSpawn;
    //[SerializeField] float chanceToBeSelected;
    public Queue<UnitBehaviour> AvailableUnits = new Queue<UnitBehaviour>();
    public int TotalUnits { get => unitsToSpawn; }
    public void InitializePool(bool _isHostile)
    {
        for(int i = 0; i < unitsToSpawn; i++)
        {
            var _unit = Instantiate(prefabUnit, transform).GetComponent<UnitBehaviour>();
            _unit.IsHostile = _isHostile;
            _unit.SpawnProjectiles();
            //_unit.ImgHpBar.transform.parent.gameObject.SetActive(false);
            _unit.Deactivate();
            //Debug.LogWarning($"Pool {transform.parent.gameObject.name}/Unit {Unit} #{i} deactivated from gen");

        }

    }

    public UnitBehaviour Dequeue(bool _activate = true)
    {
        var _unit = AvailableUnits.Dequeue();
        if(_activate) _unit.Activate();
        return _unit;
    }

    public void Enqueue(UnitBehaviour _unit) => AvailableUnits.Enqueue(_unit);
}
