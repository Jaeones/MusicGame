using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioSource musicSoundAudio;
    [SerializeField]
    AudioSource effectAudio;
    [SerializeField]
    Slider musicSoundSlider;
    [SerializeField]
    Slider effectSoundSlider;

    [SerializeField]
    GameObject GameAudioUI;


    private void Update()
    {
        musicSoundAudio.volume = musicSoundSlider.value;
        effectAudio.volume = effectSoundSlider.value;

        //비활성화 되어있다면 활성화(열기)
        if(GameAudioUI.activeSelf == false)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0f;
                GameAudioUI.SetActive(true);
            }
            
        }
        //활성화 되어있다면 비활성화(닫기)
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1f;
                GameAudioUI.SetActive(false);
            }
        }

        

    }
}
