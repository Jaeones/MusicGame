using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    Text perfectText;
    [SerializeField]
    Text goodText;
    [SerializeField]
    Text badText;
    [SerializeField]
    Text missText;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Sprite[] gradeImages;
    [SerializeField]
    Image gradeUI;


    private void Start()
    {
        perfectText.text = ScorePoint.perfectPoint.ToString();
        goodText.text = ScorePoint.goodPoint.ToString();
        badText.text = ScorePoint.badPoint.ToString();
        missText.text = ScorePoint.missPoint.ToString();
        scoreText.text = ScorePoint.score.ToString();

        SetGradeImage();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickHomeButton();
        }
    }

    void SetGradeImage()
    {
        int index = 4; // 기본값 F

        int score = ScorePoint.score;

        if (score > 2000)
        {
            index = 0; // S등급
        }
        else if (score > 1500)
        {
            index = 1; // A등급
        }
        else if (score > 1000)
        {
            index = 2; // B등급
        }
        else if (score > 500)
        {
            index = 3; // C등급
        }
        else
        {
            index = 4; // F등급
        }

        if (gradeUI != null && gradeImages.Length > index)
        {
            gradeUI.sprite = gradeImages[index];
        }
    }
    public void OnClickHomeButton()
    {
        SceneManager.LoadScene("Start");
    }
}
