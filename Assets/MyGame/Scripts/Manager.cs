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
    public enum GameOverType
    {
        FISHER_WIN,
        FISH_WIN,
        TIE
    }
    public List<GameObject> movables = new List<GameObject>();
    [SerializeField]
    private Button rollDiceButton;
    [SerializeField]
    private GameObject rotationAnchor;
    [SerializeField]
    private float rollDuration = 5;
    private void Start()
    {
        movables = GameObject.FindGameObjectsWithTag("Fish").Concat(GameObject.FindGameObjectsWithTag("Boat")).ToList();
    }
    public void RollDice()
    {
        rollDiceButton.interactable = false;
        Movable movable = GetRandomMovable();
        LeanTween.rotateZ(rotationAnchor, (movable.GetAngle() - 360 * 3), rollDuration)
        .setEase(LeanTweenType.easeInOutQuad)
        .setOnComplete(() => { DiceRollComplete(movable); });
    }
    private MovableType GetRandomType()
    {
        Type type = typeof(MovableType);
        List<MovableType> invalidTypes = movables.FindAll(m =>
        {
            Movable mov = m.GetComponent<Movable>();
            return mov.endReached || mov.captured;
        })
        .Select(m => m.GetComponent<Movable>().type)
        .ToList();

        List<MovableType> validTypes = type.GetEnumValues().Cast<MovableType>().ToList().FindAll(t => !invalidTypes.Contains(t));

        //increase chance of boat
        validTypes.Add(MovableType.BOAT);

        int index = UnityEngine.Random.Range(0, validTypes.Count());
        return validTypes[index];
    }
    private Movable GetRandomMovable()
    {
        MovableType type = GetRandomType();
        return movables.Find(m =>
        {
            Movable mov = m.GetComponent<Movable>();
            return mov.type == type && !mov.endReached;
        })
        .GetComponent<Movable>();
    }
    private void DiceRollComplete(Movable movable)
    {
        movable.Move(MoveFinished);
    }
    private void MoveFinished()
    {
        rollDiceButton.interactable = true;
        bool gameOver = true;
        foreach (GameObject item in movables)
        {
            Movable mov = item.GetComponent<Movable>();
            if (mov.type == MovableType.BOAT)
            {
                GetFishesAtPosition(mov.position).ForEach(m => m.DoCapture(item.transform.position));
                continue;
            };
            if (mov.position >= 12)
            {
                mov.endReached = true;
                item.transform.localScale = Vector3.zero;
            }
            if (!mov.endReached && !mov.captured)
            {
                gameOver = false;
            }
        }
        HandleWinner();
    }
    private void HandleWinner()
    {
        int fishesCaptured = movables.Where(m =>
        {
            Movable mov = m.GetComponent<Movable>();
            return mov.type != MovableType.BOAT && mov.captured;
        }).Count();

        int fishesReachedEnd = movables.Where(m =>
        {
            Movable mov = m.GetComponent<Movable>();
            return mov.type != MovableType.BOAT && mov.endReached;
        }).Count();

        Debug.Log("Fishes Captured: " + fishesCaptured + ", Fishes Reached End: " + fishesReachedEnd);

        if (fishesCaptured > 2) GameOver(GameOverType.FISHER_WIN);
        else if (fishesReachedEnd > 2) GameOver(GameOverType.FISH_WIN);
        else if (fishesCaptured == 2 && fishesReachedEnd == 2) GameOver(GameOverType.TIE);
    }

    private void GameOver(GameOverType type)
    {
        Debug.Log("Game Over, " + type);
        switch (type)
        {
            case GameOverType.FISHER_WIN:
                Debug.Log("Fisher wins");
                break;
            case GameOverType.FISH_WIN:
                Debug.Log("Fish wins");
                break;
            case GameOverType.TIE:
                Debug.Log("Tie");
                break;
        }
    }

    private List<Movable> GetFishesAtPosition(int pos)
    {
        return movables.Where(m =>
        {
            Movable mov = m.GetComponent<Movable>();
            return mov.position == pos && mov.type != MovableType.BOAT && !mov.captured;
        }).Select(m => m.GetComponent<Movable>()).ToList();
    }

}
