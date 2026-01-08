using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform[] noteSpawnPoint; // 노트가 생성될 위치
    [SerializeField] GameObject[] notePrefabs;    // 노트 프리팹

    [SerializeField] GameObject gameUi;

    void Start()
    {
        InvokeRepeating("CreatePoint", 2f, UnityEngine.Random.Range(1, 6));
        //CreatePoint();
    }

    private void CreatePoint()
    {
        for (int i = 0; i < noteSpawnPoint.Length; i++)
        {
            GameObject noteSpawnpoints = Instantiate(notePrefabs[i]);
            noteSpawnpoints.transform.SetParent(gameUi. transform);
            noteSpawnpoints.transform.localPosition = noteSpawnPoint[i].localPosition;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
