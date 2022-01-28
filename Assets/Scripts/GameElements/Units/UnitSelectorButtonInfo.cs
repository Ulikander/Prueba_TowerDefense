using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitPlacement
{
    Spawnable, BuildPlace, Path, BuildPlaceAndPath
}

public enum UnitDefinition
{
    DebugUnit_Single, DebugUnit_Tower, Archer_Tower, Warrior_Tower, Bomb_Tower, Wall, MightyKnight_Single, Warrior_Single, Archer_Single, Juggernaut_Single, Gunner_Single, Magician_Single, Dragon_Single
}


/// <summary>
/// Scriptable object which can be placet in UnitSelectorButton to make its info appear.
/// </summary>
[CreateAssetMenu(fileName = "UnitSelection0", menuName = "TowerDefenseRandom/Unit Selection Button Info")]
public class UnitSelectorButtonInfo : ScriptableObject
{
    public string DisplayName;
    [TextArea()]public string Info;
    public Sprite SpritePreview;
    public int Cost;
    public int MaxUnits;
    public UnitPlacement PlaceableAt;
    public UnitDefinition Unit;
}
