using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    Text musicNameText;
    [SerializeField]
    AudioClip[] backMusicClips;
    AudioSource backAudio;

    [SerializeField]
    Slider playerHpSlider;
    public float playerHp = 100f;
    public bool isGame = true;

    void Start()
    {
        backAudio = GetComponent<AudioSource>();
        backAudio.clip = backMusicClips[StartManager.musicNum];
        backAudio.Play();

        musicNameText.text = StartManager.musicName;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGame)
        {
            playerHpSlider.value = playerHp;

            if (playerHp <= 0)
            {
                isGame = false;
                SceneManager.LoadScene("End");
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                SceneManager.LoadScene("End");
            }
        }
    }
}
