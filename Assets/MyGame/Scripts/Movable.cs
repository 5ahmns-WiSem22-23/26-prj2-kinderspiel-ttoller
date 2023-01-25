using UnityEngine;

public class Movable : MonoBehaviour
{

    public Manager.MovableType type;
    private int position;
    private void Start()
    {
        position = type == Manager.MovableType.BOAT ? 0 : 4;
    }
    public void Move()
    {
        position++;
        Debug.Log("Move " + type + " to " + GetMoveDestination(position));
        LeanTween.moveLocalX(gameObject, GetMoveDestination(position), 1);
    }
    public float GetAngle()
    {
        switch (type)
        {
            case Manager.MovableType.ORANGE:
                return 0;
            case Manager.MovableType.YELLOW:
                return 120;
            case Manager.MovableType.BLUE:
                return 180;
            case Manager.MovableType.PINK:
                return 300;
            case Manager.MovableType.BOAT:
                return Random.value > 0.5 ? 60 : 240;
            default:
                return 0;
        }
    }
    public float GetMoveDestination(int positionIndex)
    {
        return positionIndex * 1.5f - 7;
    }
}