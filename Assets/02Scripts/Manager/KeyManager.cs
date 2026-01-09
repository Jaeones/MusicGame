using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    AudioSource keyAudio;
    [SerializeField]
    AudioClip keyAudioClip;
    [SerializeField]
    Color[] keyColors;
    [SerializeField]
    Image[] keyImages;

    //키를 눌렀는지 확인
    public bool[] isKeyPut;

    private void Start()
    {
        keyAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isKeyPut[0] = true;
            keyAudio.PlayOneShot(keyAudioClip);
            keyImages[0].color = keyColors[0];
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            isKeyPut[0] = false;
            keyImages[0].color = new Color(1,1,1,0.2f);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isKeyPut[1] = true;
            keyAudio.PlayOneShot(keyAudioClip);
            keyImages[1].color = keyColors[1];
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isKeyPut[1] = false;
            keyImages[1].color = new Color(1, 1, 1, 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isKeyPut[2] = true;
            keyAudio.PlayOneShot(keyAudioClip);
            keyImages[2].color = keyColors[2];
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isKeyPut[2] = false;
            keyImages[2].color = new Color(1, 1, 1, 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            isKeyPut[3] = true;
            keyAudio.PlayOneShot(keyAudioClip);
            keyImages[3].color = keyColors[3];
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            isKeyPut[3] = false;
            keyImages[3].color = new Color(1, 1, 1, 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            isKeyPut[4] = true;
            keyAudio.PlayOneShot(keyAudioClip);
            keyImages[4].color = keyColors[4];
        }
        else if (Input.GetKeyUp(KeyCode.L))
        {
            isKeyPut[4] = false;
            keyImages[4].color = new Color(1, 1, 1, 0.2f);
        }
    }
}
