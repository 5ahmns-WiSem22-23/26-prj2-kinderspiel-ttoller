using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        TIE,
        NONE
    }
    public List<GameObject> movables = new List<GameObject>();
    [SerializeField]
    private Button rollDiceButton;
    [SerializeField]
    private GameObject rotationAnchor;
    [SerializeField]
    private float rollDuration = 5;

    [SerializeField]
    private GameObject menuCanvas;
    [SerializeField]
    private GameObject boatImages;
    [SerializeField]
    private GameObject fishImages;
    [SerializeField]
    private GameObject gameCanvas;
    [SerializeField]
    private Text titleText;
    private GameOverType gameOverType = GameOverType.NONE;
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
            return mov.endReached || mov.captured || mov.type == MovableType.BOAT;
        })
        .Select(m => m.GetComponent<Movable>().type)
        .ToList();

        List<MovableType> validTypes = type.GetEnumValues().Cast<MovableType>().ToList().FindAll(t => !invalidTypes.Contains(t));
        int longestDistance = 0;
        foreach (GameObject item in movables)
        {
            Movable mov = item.GetComponent<Movable>();
            if (!validTypes.Contains(mov.type)) continue;
            if (mov.position - GetBoatPos() > longestDistance) longestDistance = mov.position;
        }
        for (int i = 0; i < Mathf.Max(longestDistance - 5, 2); i++)
        {
            validTypes.Add(MovableType.BOAT);
        }
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
        gameOverType = type;
        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        switch (type)
        {
            case GameOverType.FISHER_WIN:
                titleText.text = "Die Fischer gewinnen!";
                boatImages.SetActive(true);
                break;
            case GameOverType.FISH_WIN:
                titleText.text = "Die Fische gewinnen!";
                fishImages.SetActive(true);
                break;
            case GameOverType.TIE:
                titleText.text = "Unendschieden!";
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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private int GetBoatPos()
    {
        return movables.Find(m => m.GetComponent<Movable>().type == MovableType.BOAT).GetComponent<Movable>().position;
    }
}
