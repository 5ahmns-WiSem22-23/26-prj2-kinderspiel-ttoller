using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private Button rollDiceButton;
    [SerializeField]
    private GameObject rotationAnchor;
    [SerializeField]
    private float rollDuration = 5;
    private bool isRolling;
    private void Start()
    {
        movables = GameObject.FindGameObjectsWithTag("Fish").Concat(GameObject.FindGameObjectsWithTag("Boat")).ToList();
    }
    public void RollDice()
    {
        if (isRolling)
            return;
        isRolling = true;
        Movable movable = GetRandomMovable();
        LeanTween.rotateZ(rotationAnchor, (movable.GetAngle() - 360 * 3), rollDuration)
        .setEase(LeanTweenType.easeInOutQuad)
        .setOnComplete(() => { DiceRollComplete(movable); });
    }
    private MovableType GetRandomType()
    {
        Type type = typeof(MovableType);
        Array values = type.GetEnumValues();
        int[] probabilities = new int[values.Length + 1];
        probabilities[0] = 0;
        for (int i = 1; i < probabilities.Length; i++)
        {
            probabilities[i] = i - 1;
        }
        int index = UnityEngine.Random.Range(0, probabilities.Length);
        return (MovableType)values.GetValue(probabilities[index]);
    }
    private Movable GetRandomMovable()
    {
        MovableType type = GetRandomType();
        return movables.Where(m => m.GetComponent<Movable>().type == type).FirstOrDefault().GetComponent<Movable>();
    }
    private void DiceRollComplete(Movable movable)
    {
        movable.Move();
        isRolling = false;
    }

}
