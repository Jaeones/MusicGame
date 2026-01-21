using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotePoint : MonoBehaviour
{
    [SerializeField]
    string keyName;
    [SerializeField]
    KeyManager m_keyManager;
    [SerializeField]
    Text infoText;
    [SerializeField]
    MainManager mainManager;

    private List<Collider2D> activeNotes = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activeNotes.Contains(collision))
        {
            activeNotes.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 나갈 때 리스트에 있다면 제거
        if (activeNotes.Contains(collision))
        {
            activeNotes.Remove(collision);
        }
    }

    private void Update()
    {
        int keyIndex = -1;
        switch (keyName)
        {
            case "Key_1": keyIndex = 0; break;
            case "Key_2": keyIndex = 1; break;
            case "Key_3": keyIndex = 2; break;
            case "Key_4": keyIndex = 3; break;
            case "Key_5": keyIndex = 4; break;
        }

        if (keyIndex != -1 && m_keyManager != null)
        {
            // 키가 눌렸을 때 (GetKeyDown 상태라고 가정)
            if (m_keyManager.isKeyPut[keyIndex])
            {
                if (activeNotes.Count > 0)
                {
                    ProcessClosestNote(keyIndex);
                }

                // [중요] 입력을 처리했으므로, 같은 프레임에 중복 처리되지 않도록 키 입력을 소비(false) 시킴
                // (KeyManager 구조에 따라 이 줄은 빼야 할 수도 있습니다. 
                // 만약 KeyManager가 Update마다 입력을 초기화한다면 놔두셔도 됩니다.)
                m_keyManager.isKeyPut[keyIndex] = false;
            }
        }
    }

    void ProcessClosestNote(int keyIndex)
    {
        Collider2D closestNote = null;
        float minDistance = float.MaxValue;

        // 1. 가장 가까운 노트 찾기
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] == null)
            {
                activeNotes.RemoveAt(i);
                continue;
            }

            float dist = Vector2.Distance(transform.position, activeNotes[i].transform.position);

            // 거리가 더 가까운 것을 찾음
            if (dist < minDistance)
            {
                minDistance = dist;
                closestNote = activeNotes[i];
            }
        }

        // 2. 판정 시도
        if (closestNote != null)
        {
            // [중요] PointCheck가 성공(true)했을 때만 리스트에서 제거
            bool isHit = PointCheck(keyIndex, closestNote, minDistance);

            if (isHit)
            {
                activeNotes.Remove(closestNote);
            }
            // 실패(거리 120 밖)했다면 리스트에 남겨둬서 다음 프레임에 다시 체크할 수 있게 함
        }
    }

    void WhatPoint(string infoTxt, GameObject gameobject)
    {
        infoText.text = infoTxt;
        Destroy(gameobject);
    }

    // void -> bool 변경: 판정 성공 여부를 반환
    bool PointCheck(int i, Collider2D collision, float dis)
    {
        // 판정 범위 (Bad: 120 이하)
        if (dis <= 120)
        {
            Debug.Log("Hit Success: " + dis);

            if (dis > 80)
            {
                ScorePoint.score += 10;
                mainManager.playerHp -= 2;
                ScorePoint.badPoint++;
                WhatPoint("Bad", collision.gameObject);
            }
            else if (dis > 40)
            {
                ScorePoint.score += 20;
                ScorePoint.goodPoint++;
                WhatPoint("Good", collision.gameObject);
            }
            else
            {
                ScorePoint.score += 50;
                ScorePoint.perfectPoint++;
                WhatPoint("Perfect", collision.gameObject);
            }
            return true; // 판정 성공 (노트 파괴됨)
        }

        // 거리가 120보다 멀면 아직 칠 수 없는 노트임 (너무 빨리 누름)
        return false; // 판정 실패 (노트 유지)
    }
}