using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoints : MonoBehaviour
{
    public static PathPoints instance;
    [SerializeField] Transform[] pathPoints;
    public static Transform[] Points { get => instance.pathPoints; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pathPoints = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++) pathPoints[i] = transform.GetChild(i);
    }
}
