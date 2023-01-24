using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public enum MovableType
    {
        BOAT,
        ORANGE,
        BLUE,
        YELLOW,
        PINK,
    }
    public List<GameObject> movables = new List<GameObject>();
    private void Start()
    {
        movables = GameObject.FindGameObjectsWithTag("Fish").Concat(GameObject.FindGameObjectsWithTag("Boat")).ToList();
    }
    public void RollDice()
    {
        GetRandomMovable();
    }
    private MovableType GetRandomType()
    {
        Type type = typeof(MovableType);
        Array values = type.GetEnumValues();
        int index = UnityEngine.Random.Range(0, values.Length);
        return (MovableType)values.GetValue(index);
    }
    private Movable GetRandomMovable()
    {
        MovableType type = GetRandomType();
        return movables.Where(m => m.GetComponent<Movable>().type == type).FirstOrDefault().GetComponent<Movable>();
    }
}
