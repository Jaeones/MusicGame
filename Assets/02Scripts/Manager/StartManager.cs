using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public AudioSource playerAudio;

    [SerializeField]
    AudioClip[] selectAudioClips;

    [SerializeField]
    int selectNum;
    [SerializeField]
    Slider volumeSlider;
    [SerializeField]
    GameObject[] musicSelects;


    public static string musicName;
    public static int musicNum;


    private void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        playerAudio.PlayOneShot(selectAudioClips[selectNum]);
    }

    //다음, 이전 버튼 함수
    //해당되는 이미지, 노래가 재생
    public void MusicBtn(string btnName)
    {
        if(btnName == "Next")
        {
            //다음 버튼
            if(selectNum < musicSelects.Length - 1)
            {
                playerAudio.Stop();
                musicSelects[selectNum].SetActive(false);
                selectNum++;
                musicSelects[selectNum].SetActive(true);
                playerAudio.PlayOneShot(selectAudioClips[selectNum]);
            }
        }
        else
        {
            //이전 버튼
            if (selectNum > 0)
            {
                playerAudio.Stop();
                musicSelects[selectNum].SetActive(false);
                selectNum--;
                musicSelects[selectNum].SetActive(true);
                playerAudio.PlayOneShot(selectAudioClips[selectNum]);
            }
        }
    }


    // 1. 뮤직선택
    // 2. 로딩씬으로 전환
    // 3. 선택한 음악의 정보를 전달
    // 4. 스태틱(번호,이름) -> 로딩(곡정보를 노출)

    public void LoadingBtn()
    {
        musicNum = selectNum;
        musicName = musicSelects[selectNum].transform.GetChild(1).GetComponent<Text>().text;
        Debug.Log(musicName);
        SceneManager.LoadScene("Loading");
    }
}
