using UnityEngine;

public class Note : MonoBehaviour
{
    // SpawnManager의 NoteSpeed와 같은 값이어야 함
    public float noteSpeed = 400f;

    private void Start()
    {
        // 시작하자마자 리지드바디에 속도를 줍니다. (중력이 0이므로 이 속도가 끝까지 유지됨)
        // SetVelocity는 월드 좌표계 기준이므로 Vector2.down 사용
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * noteSpeed;
    }

}