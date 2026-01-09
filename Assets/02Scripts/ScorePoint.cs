using UnityEngine;
using UnityEngine.UI;

public class ScorePoint : MonoBehaviour
{
    public static int missPoint;
    public static int badPoint;
    public static int goodPoint;
    public static int perfectPoint;

    public static int score = 0;
    [SerializeField]
    Text scoreText;

    private void Start()
    {
        score = 0;
        missPoint = 0;
        badPoint = 0;
        goodPoint = 0;
        perfectPoint = 0;
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }
}
