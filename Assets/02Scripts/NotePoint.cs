using UnityEngine;
using UnityEngine.UI;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (keyName)
        {
            case "Key_1":
                PointCheck(0, collision);              
                break;
            case "Key_2":
                PointCheck(1, collision);
                break;
            case "Key_3":
                PointCheck(2, collision);
                break;
            case "Key_4":
                PointCheck(3, collision);
                break;
            case "Key_5":
                PointCheck(4, collision);
                break;
        }
    }
    void WhatPoint(string infoTxt, GameObject gameobject)
    {
        infoText.text = infoTxt;
        Destroy(gameobject);
    }

    void PointCheck(int i, Collider2D collision)
    {
        if (m_keyManager != null)
        {
            float dis = Vector2.Distance(transform.position, collision.transform.position);
            if (m_keyManager.isKeyPut[i])
            {
                Debug.Log("i+1");
                //ÆÇÁ¤ (Bad, Good, Perfect)
                if (dis <= 120 && dis > 80)
                {
                    //ScorePoint.badPoint++;
                    //Destroy(collision.gameObject);
                    //infoText.text = "Bad!";
                    ScorePoint.score += 10;
                    mainManager.playerHp -= 2;
                    ScorePoint.badPoint++;
                    WhatPoint("Bad", collision.gameObject);
                }
                else if (dis <= 80 && dis > 40)
                {
                    //ScorePoint.goodPoint++;
                    //Destroy(collision.gameObject);
                    //infoText.text = "Good!";
                    ScorePoint.score += 20;
                    ScorePoint.goodPoint++;
                    WhatPoint("Good", collision.gameObject);
                }
                else if (dis <= 40)
                {
                    //ScorePoint.perfectPoint++;
                    //Destroy(collision.gameObject);
                    //infoText.text = "Perfect";
                    ScorePoint.score += 50;
                    ScorePoint.perfectPoint++;
                    WhatPoint("Perfect", collision.gameObject);
                }
            }
        }
    }
}
