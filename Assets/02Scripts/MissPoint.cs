using UnityEngine;
using UnityEngine.UI;

public class MissPoint : MonoBehaviour
{
    AudioSource missAudio;
    public AudioClip missClip;
    public float missHp = 20f;

    [SerializeField]
    MainManager mainManager;

    [SerializeField]
    public Text infoText; 

    void Start()
    {
        missAudio = GetComponent<AudioSource>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            if (mainManager.isGame)
            {
                ScorePoint.missPoint++;
                mainManager.playerHp -= missHp;
                Destroy(collision.gameObject);
                missAudio.PlayOneShot(missClip);

                infoText.text = "Miss!";
                Invoke("TextInit", 1.5f);
            }
        }
    }

    void TextInit()
    {
        infoText.text = "";
    }
}
