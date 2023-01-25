using UnityEngine;

public class Movable : MonoBehaviour
{

    public Manager.MovableType type;
    public int position;
    [SerializeField]
    private float swimDuration = 2;
    [SerializeField]
    private float wiggleDuration = 0.5f;
    [SerializeField]
    private float wiggleAmount = 0.2f;
    public bool endReached = false;
    public bool captured = false;
    private void Start()
    {
        position = type == Manager.MovableType.BOAT ? 0 : 6;
    }
    public void Move(System.Action completeCallback)
    {
        position++;
        Debug.Log("Move " + type + " to " + GetMoveDestination(position));

        LeanTween.moveLocalY(gameObject, transform.position.y + wiggleAmount, wiggleDuration)
        .setLoopPingPong(Mathf.FloorToInt((swimDuration / wiggleDuration) / 2))
        .setEase(LeanTweenType.easeInOutQuad).setOnComplete(completeCallback);

        LeanTween.moveLocalX(gameObject, GetMoveDestination(position), swimDuration)
        .setEase(LeanTweenType.easeInOutQuad);

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
        return positionIndex * 1.5f - 8.5f;
    }
    public void DoCapture(Vector3 boatPosition)
    {
        captured = true;
        LeanTween.move(gameObject, boatPosition, 0.5f)
        .setEase(LeanTweenType.easeInOutQuad);

        LeanTween.scale(gameObject, Vector3.zero, 0.5f)
        .setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            // gameObject.SetActive(false);
        });
    }
}