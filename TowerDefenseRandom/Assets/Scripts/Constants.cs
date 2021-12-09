using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the Names in string of any Scene in the project.
/// </summary>
public class SceneID
{
    public const string MainMenu = "MainMenu";
    public const string GamePlay = "Gameplay";
}



/// <summary>
/// For Units. For priority detection.
/// </summary>
public enum UnitType
{
    Undefined,
    Building,
    Unit,
    Special
}

/// <summary>
/// For Units. Makes the unit behaviour controller know what to tell a unit to do next.
/// </summary>
public enum UnitState
{
    Deactivated, Waiting, MovingToPoint, ReachedEndPoint, Attacking, Dead
}