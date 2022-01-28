using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Scriptable object which can be placet in UnitSelectorButton to make its info appear.
/// </summary>
[CreateAssetMenu(fileName = "Wave0", menuName = "TowerDefenseRandom/Wave")]
public class WaveInfo : ScriptableObject
{
    public UnitDefinition[] Units;
    /// <summary>
    /// Ignored for now
    /// </summary>
    public Vector2[] UnitTimeBetween;
    public Vector2Int[] UnitSpawnCountRange;
    public Vector2 TimeDelayRange;
    public int RepeatTimes;
    public float CurrencyScalar;
}
