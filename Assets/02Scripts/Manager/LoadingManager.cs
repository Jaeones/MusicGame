using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    Slider loadingBar;
    [SerializeField]
    float loadingSp;
    [SerializeField]
    Text musicNameText;

    private void Start()
    {
        musicNameText.text = StartManager.musicName;
        LoadingStart("Main");
    }

    public void LoadingStart(string name)
    {
        StartCoroutine(Loading(name));
    }

    IEnumerator Loading(string name)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(name);
        loadingOperation.allowSceneActivation = false; //로딩이 완료되는 대로 씬을 활성화하지않겠다고 지정

        while (!loadingOperation.isDone)
        {
            //loadingBar.value = loadingOperation.progress;         원래라면 로딩씬은 이렇게 작성
            loadingBar.value += loadingSp * Time.deltaTime;         //의도적으로 로딩바 연출을 위해 씬 활성화에 대기시간을 검

            if (loadingBar.value >= 1f)
            {
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
