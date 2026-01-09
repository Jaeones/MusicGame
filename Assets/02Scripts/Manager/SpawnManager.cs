using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public double beat;
    public int line;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] Transform[] noteSpawnPoints;
    [SerializeField] GameObject[] notePrefabs;
    [SerializeField] GameObject gameUi;
    // Start에서 찾을 것이므로 Inspector 연결 없어도 됨
    private AudioSource mainAudioSource;
    [SerializeField] MainManager mainManager;

    [Header("Rhythm Settings")]
    public int bpm = 0;
    public double noteSpeed = 0f;
    public float noteFallDistance = 0f;

    public List<NoteData> sheetMusic; // 인스펙터에서 안 보여도 되면 HideInInspector

    private double currentTime = 0d;
    private int noteIndex = 0;
    private double secPerBeat;
    private double noteFallTime;

    void Start()
    {
        // 1. 리스트 초기화
        sheetMusic = new List<NoteData>();

        // 2. 선택된 곡 번호에 따라 BPM과 채보 데이터 로드
        int selectedSongIndex = StartManager.musicNum; // 선택된 번호 가져오기

        switch (selectedSongIndex)
        {
            case 0:
                LoadSong0(); // 0번 곡: 가까운 듯 먼 그대여
                break;
            case 1:
                LoadSong1(); // 1번 곡: (예시) 다른 노래
                break;
                case 2:
                    LoadSong2();
                break;
                case 3:
                    LoadSong3();
                break;
            // 곡이 추가되면 case 2, case 3... 계속 추가
            default:
                Debug.LogError("채보 데이터가 없는 곡 번호입니다: " + selectedSongIndex);
                break;
        }

        // 3. 계산 (BPM이 위 함수에서 설정된 후에 계산해야 함!)
        if (bpm > 0) secPerBeat = 60d / bpm;
        if (noteSpeed > 0) noteFallTime = noteFallDistance / noteSpeed;

        // 4. 정렬 (필수)
        sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));

        // 5. 오디오 소스 가져오기
        if (mainManager != null)
        {
            mainAudioSource = mainManager.GetComponent<AudioSource>();
        }
    }

    // -------------------------------------------------------------
    // 곡별 데이터 관리 구역
    // -------------------------------------------------------------

    // 0번 곡: 카더가든 - 가까운 듯 먼 그대여
    void LoadSong0()
    {
            // 1. 기본 설정 (BPM 93)
            bpm = 93;
            if (bpm > 0) secPerBeat = 60d / bpm;
            if (noteSpeed > 0) noteFallTime = noteFallDistance / noteSpeed;

            // 2. 리스트 초기화
            sheetMusic = new List<NoteData>();

            // =========================================================
            // [Part 1] Intro (0 ~ 16박자) - 잔잔한 시작
            // =========================================================
            sheetMusic.Add(new NoteData { beat = 1.0, line = 0 });
            sheetMusic.Add(new NoteData { beat = 3.0, line = 4 });
            sheetMusic.Add(new NoteData { beat = 5.0, line = 1 });
            sheetMusic.Add(new NoteData { beat = 7.0, line = 3 });
            sheetMusic.Add(new NoteData { beat = 9.0, line = 2 });
            sheetMusic.Add(new NoteData { beat = 11.0, line = 0 });
            sheetMusic.Add(new NoteData { beat = 13.0, line = 4 });
            sheetMusic.Add(new NoteData { beat = 15.0, line = 2 });

            // =========================================================
            // [Part 2] Verse 1 (17 ~ 64박자) - 보컬 시작
            // 잔잔하게 한 개씩 떨어지는 패턴
            // =========================================================
            for (double b = 17; b < 64; b += 2) // 2박자마다 생성
            {
                // 0 -> 1 -> 2 -> 3 -> 4 순서대로 반복
                int line = (int)((b - 17) / 2) % 5;
                sheetMusic.Add(new NoteData { beat = b, line = line });

                // 4마디마다 엇박자 추가 (재미 요소)
                if (b % 8 == 0)
                {
                    sheetMusic.Add(new NoteData { beat = b + 0.5, line = (line + 2) % 5 });
                }
            }

            // =========================================================
            // [Part 3] Chorus 1 (65 ~ 112박자) - 하이라이트
            // 양손 동시타(화음) 및 계단 패턴
            // =========================================================
            for (double b = 65; b < 112; b += 1) // 1박자마다 생성 (빨라짐)
            {
                if (b % 4 == 1) // 마디의 첫 박자는 '쾅!' (양끝 동시타)
                {
                    sheetMusic.Add(new NoteData { beat = b, line = 0 });
                    sheetMusic.Add(new NoteData { beat = b, line = 4 });
                }
                else // 나머지는 계단 타기
                {
                    // 랜덤한 느낌을 주는 수식
                    int line = (int)(b % 3) + 1; // 1, 2, 3 라인 위주
                    sheetMusic.Add(new NoteData { beat = b, line = line });
                }
            }

            // =========================================================
            // [Part 4] Verse 2 (113 ~ 160박자) - 2절
            // 1절보다 조금 더 복잡하게 (지그재그)
            // =========================================================
            for (double b = 113; b < 160; b += 2)
            {
                sheetMusic.Add(new NoteData { beat = b, line = 2 }); // 가운데 중심
                sheetMusic.Add(new NoteData { beat = b + 1.0, line = (b % 4 == 0) ? 0 : 4 }); // 좌우 왔다갔다
            }

            // =========================================================
            // [Part 5] Bridge & Climax (161 ~ 240박자) - 절정
            // 엇박자와 동시타가 섞인 어려운 패턴
            // =========================================================
            for (double b = 161; b < 240; b += 1)
            {
                // 정박 노트
                sheetMusic.Add(new NoteData { beat = b, line = (int)(b % 5) });

                // 2박자마다 엇박 노트 추가
                if (b % 2 == 0)
                {
                    sheetMusic.Add(new NoteData { beat = b + 0.5, line = (int)((b + 2) % 5) });
                }

                // 8박자마다 3개 동시타 (임팩트)
                if (b % 8 == 0)
                {
                    sheetMusic.Add(new NoteData { beat = b, line = 0 });
                    sheetMusic.Add(new NoteData { beat = b, line = 2 });
                    sheetMusic.Add(new NoteData { beat = b, line = 4 });
                }
            }

            // =========================================================
            // [Part 6] Outro (241 ~ 280박자) - 마무리
            // 다시 느려지며 여운을 줌
            // =========================================================
            sheetMusic.Add(new NoteData { beat = 241, line = 0 });
            sheetMusic.Add(new NoteData { beat = 245, line = 4 });
            sheetMusic.Add(new NoteData { beat = 249, line = 1 });
            sheetMusic.Add(new NoteData { beat = 253, line = 3 });
            sheetMusic.Add(new NoteData { beat = 257, line = 2 });

            // 마지막 롱노트 느낌의 마무리
            sheetMusic.Add(new NoteData { beat = 265, line = 0 });
            sheetMusic.Add(new NoteData { beat = 265, line = 4 });


            // 3. 필수: 시간 순서대로 정렬 (반복문으로 넣어서 뒤죽박죽일 수 있음)
            sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));
        
    }

    // 1번 곡: (예시) 빠른 댄스곡
    void LoadSong1()
    {
            bpm = 73; // 곡의 BPM 설정

            // =========================================================
            // [Part 1] Intro (0 ~ 8박자) - 피아노 도입부
            // =========================================================
            sheetMusic.Add(new NoteData { beat = 1.0, line = 0 });
            sheetMusic.Add(new NoteData { beat = 3.0, line = 4 });
            sheetMusic.Add(new NoteData { beat = 5.0, line = 1 });
            sheetMusic.Add(new NoteData { beat = 7.0, line = 3 });

            // =========================================================
            // [Part 2] Verse 1 (9 ~ 72박자) 
            // "무딘 목소리와 어설픈 자국들..."
            // 잔잔하게 보컬에 맞춰 하나씩 떨어짐
            // =========================================================
            for (double b = 9; b < 72; b += 2)
            {
                // 2 -> 1 -> 3 -> 0 -> 4 패턴으로 이동
                int[] lines = { 2, 1, 3, 0, 4 };
                int index = (int)((b - 9) / 2) % 5;

                sheetMusic.Add(new NoteData { beat = b, line = lines[index] });

                // 가사 끝부분마다 엇박자 포인트
                if (b % 16 == 15)
                {
                    sheetMusic.Add(new NoteData { beat = b + 0.5, line = 2 });
                }
            }

            // =========================================================
            // [Part 3] Chorus 1 (73 ~ 136박자) - 감정 고조
            // "그대 춤을 추는 나무 같아요~"
            // 정박자(쿵)와 엇박자(짝)가 섞여서 리듬감 형성
            // =========================================================
            for (double b = 73; b < 136; b += 4)
            {
                // 첫 소절: 쿵-짝-쿵 (기본 비트)
                sheetMusic.Add(new NoteData { beat = b, line = 0 });       // 쿵
                sheetMusic.Add(new NoteData { beat = b + 1.5, line = 4 }); // 짝 (엇박)
                sheetMusic.Add(new NoteData { beat = b + 3.0, line = 2 }); // 쿵

                // 두번째 소절: 계단 타기 (따라라락)
                if (b + 4 < 136)
                {
                    sheetMusic.Add(new NoteData { beat = b + 4.0, line = 1 });
                    sheetMusic.Add(new NoteData { beat = b + 5.0, line = 2 });
                    sheetMusic.Add(new NoteData { beat = b + 6.0, line = 3 });
                    sheetMusic.Add(new NoteData { beat = b + 7.0, line = 1 }); // 변칙
                }
            }

            // =========================================================
            // [Part 4] Verse 2 & Bridge (137 ~ 200박자)
            // 악기 소리가 커지며 동시타 등장
            // =========================================================
            for (double b = 137; b < 200; b += 2)
            {
                // 기본 노트
                sheetMusic.Add(new NoteData { beat = b, line = (int)(b % 5) });

                // 4박자마다 동시타 (양손)
                if (b % 4 == 0)
                {
                    // 현재 라인의 반대편 라인에 추가 노트
                    int oppositeLine = 4 - (int)(b % 5);
                    sheetMusic.Add(new NoteData { beat = b, line = oppositeLine });
                }
            }

            // =========================================================
            // [Part 5] Climax (201 ~ 264박자) - 절정
            // "밤새 모아둔~"
            // 촘촘한 노트와 롱노트 느낌의 연타
            // =========================================================
            for (double b = 201; b < 264; b += 1)
            {
                // 1, 3, 5, 7 박자에 노트 생성
                if (b % 2 != 0)
                {
                    sheetMusic.Add(new NoteData { beat = b, line = 2 }); // 센터 중심
                    sheetMusic.Add(new NoteData { beat = b, line = (b % 4 == 1) ? 0 : 4 }); // 좌우 서브
                }
                else
                {
                    // 사이사이 채워주는 노트
                    sheetMusic.Add(new NoteData { beat = b, line = (int)((b / 2) % 2 == 0 ? 1 : 3) });
                }
            }

            // =========================================================
            // [Part 6] Outro (265 ~ 300박자) - 여운
            // 피아노 소리에 맞춰 하나씩 천천히
            // =========================================================
            double[] outroBeats = { 265, 269, 273, 277, 281, 285, 290 };
            int[] outroLines = { 0, 4, 1, 3, 2, 0, 2 };

            for (int i = 0; i < outroBeats.Length; i++)
            {
                sheetMusic.Add(new NoteData { beat = outroBeats[i], line = outroLines[i] });

                // 마지막 노트는 3개 동시타로 마무리
                if (i == outroBeats.Length - 1)
                {
                    sheetMusic.Add(new NoteData { beat = outroBeats[i], line = 0 });
                    sheetMusic.Add(new NoteData { beat = outroBeats[i], line = 4 });
                }
            }
    }

    void LoadSong2()
    {
        // 1. 기본 설정 (실제 BPM 131 - Tunebat, ChordWiki 등 확인)
        bpm = 131;
        if (bpm > 0) secPerBeat = 60d / bpm;
        if (noteSpeed > 0) noteFallTime = noteFallDistance / noteSpeed;

        // 2. 리스트 초기화 (TV Size 약 1:30, 총 ~200박자 정도로 맞춤)
        sheetMusic = new List<NoteData>();

        // =========================================================
        // [Part 1] Intro (0 ~ 16박자) - 기타 리프 (D Bm C D F E 패턴 기반, 밝고 귀여운 시작)
        // =========================================================
        // 실제 인트로 리프: 오름차순/내림 패턴 (기타 TAB 참고: 2-4-2-3 느낌)
        for (double b = 0; b < 16; b += 2)
        {
            int line = (int)((b / 2) % 5); // 0->1->2->3->4 반복
            sheetMusic.Add(new NoteData { beat = b, line = line });
            sheetMusic.Add(new NoteData { beat = b + 1.0, line = (line + 2) % 5 }); // 지그재그 추가
        }

        // =========================================================
        // [Part 2] Verse 1 (17 ~ 64박자) - "あの子が昨日..." 보컬 시작 (잔잔, 순차 패턴)
        // Ufret 코드: D Bm C B♭7 G Gm7 C 따라 낮~중 노트
        // =========================================================
        for (double b = 17; b < 64; b += 2)
        {
            int line = (int)((b - 17) / 2 % 5); // 0-1-2-3-4 순환
            sheetMusic.Add(new NoteData { beat = b, line = line });
            // 4마디마다 엇박자 (여운)
            if ((int)b % 8 == 0)
            {
                sheetMusic.Add(new NoteData { beat = b + 0.5, line = (line + 1) % 5 });
                sheetMusic.Add(new NoteData { beat = b + 1.5, line = (line + 3) % 5 });
            }
        }

        // =========================================================
        // [Part 3] Pre-Chorus & Chorus 1 (65 ~ 112박자) -  하이라이트
        //  직진 고백! 동시타 + 계단 (밝고 에너지 ↑)
        // =========================================================
        for (double b = 65; b < 112; b += 1)
        {
            if (b % 4 == 1) // 마디 시작: 쾅! 양끝 화음 (D-F#7 느낌)
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b, line = 4 });
                sheetMusic.Add(new NoteData { beat = b, line = 2 }); // 중간 강조
            }
            else if (b % 2 == 0) // 짝수: 높은 멜로디 (Chorus 후렴 "ダメみたい")
            {
                int line = 3 + (int)(b % 2); // 3-4 위주
                sheetMusic.Add(new NoteData { beat = b, line = line });
                sheetMusic.Add(new NoteData { beat = b + 0.5, line = (line + 1) % 5 }); // 8분 엇박자
            }
            else // 홀수: 계단 타기
            {
                int line = (int)((b % 5) + 1) % 5; // 1-2-3-4-0 순
                sheetMusic.Add(new NoteData { beat = b, line = line });
            }
        }

        // =========================================================
        // [Part 4] Verse 2 (113 ~ 160박자) - 2절 (1절 변형, 더 복잡 지그재그 + 코드 Bm-Am7 등)
        // =========================================================
        for (double b = 113; b < 160; b += 1.5) // 1.5박 간격으로 촘촘히
        {
            sheetMusic.Add(new NoteData { beat = b, line = 1 }); // 왼쪽 중심
            sheetMusic.Add(new NoteData { beat = b + 0.5, line = 3 }); // 오른쪽 교차
            if (b % 4 == 0)
            {
                sheetMusic.Add(new NoteData { beat = b + 1.0, line = 0 });
                sheetMusic.Add(new NoteData { beat = b + 1.0, line = 4 }); // 동시
            }
        }

        // =========================================================
        // [Part 5] Chorus 2 & Climax (161 ~ 192박자) - 2회차 후렴 + 절정 (가루파 EXPERT 밀도 참고: 620노트 전체)
        // 엇박자 + 트리플 동시 + 빠른 계단 (BanG Dream! EXPERT 패턴 영감)
        // =========================================================
        for (double b = 161; b < 192; b += 0.5) // 0.5박 (8분) 단위로 고밀도!
        {
            int line = (int)(b % 5);
            if (b % 4 == 0) // 4박마다 임팩트: 3개 동시
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b, line = 2 });
                sheetMusic.Add(new NoteData { beat = b, line = 4 });
            }
            else if (b % 2 == 0)
            {
                sheetMusic.Add(new NoteData { beat = b, line = line }); // 정박
                sheetMusic.Add(new NoteData { beat = b + 0.25, line = (line + 1) % 5 }); // 16분 엇박자 추가 (어려움 ↑)
            }
            else
            {
                sheetMusic.Add(new NoteData { beat = b, line = (line + 2) % 5 }); // 오프비트
            }
        }

        // =========================================================
        // [Part 6] Outro (193 ~ 200박자) - 페이드아웃 여운 (인트로 미러 + 마지막 동시)
        // =========================================================
        sheetMusic.Add(new NoteData { beat = 193, line = 2 });
        sheetMusic.Add(new NoteData { beat = 195, line = 0 });
        sheetMusic.Add(new NoteData { beat = 197, line = 4 });
        // 마지막 에코: 동시 + 롱 느낌
        sheetMusic.Add(new NoteData { beat = 199, line = 0 });
        sheetMusic.Add(new NoteData { beat = 199, line = 4 });
        sheetMusic.Add(new NoteData { beat = 200, line = 2 });

        // 3. 필수: 시간 순서대로 정렬
        sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));
    }

    void LoadSong3()
    {
        // 1. 기본 설정 (BPM 150 확인: Tunebat/SongBPM)
        bpm = 150;
        if (bpm > 0) secPerBeat = 60d / bpm;
        if (noteSpeed > 0) noteFallTime = noteFallDistance / noteSpeed;

        // 2. 리스트 초기화 (TV Size ≈1:30, 총 ~225박자, 쉬운 버전: 노트 ≈350)
        sheetMusic = new List<NoteData>();

        // =========================================================
        // [Part 1] Intro (0 ~ 24박자) - funky 리프 단순화 (2박 간격 순차)
        // =========================================================
        for (double b = 0; b < 24; b += 2)
        {
            int line = (int)((b / 2) % 5); // 0-1-2-3-4 부드러운 순환
            sheetMusic.Add(new NoteData { beat = b, line = line });
        }

        // =========================================================
        // [Part 2] Verse 1 (25 ~ 72박자) - 보컬 따라가는 직선 패턴 (2박 간격)
        // =========================================================
        for (double b = 25; b < 72; b += 2)
        {
            int line = (int)((b - 25) / 2 % 5); // 낮은~중 라인 순환
            sheetMusic.Add(new NoteData { beat = b, line = line });
            // 가끔 듀얼 (간단 groove)
            if (b % 8 == 1)
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b, line = 4 });
            }
        }

        // =========================================================
        // [Part 3] Pre-Chorus & Chorus 1 (73 ~ 120박자) - 후렴 에너지 (1박 계단 + 듀얼)
        // =========================================================
        for (double b = 73; b < 120; b += 1)
        {
            if (b % 4 == 1) // "Mixed Nuts!" 듀얼 쾅!
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b, line = 4 });
            }
            else // 부드러운 계단 (중앙 위주)
            {
                int line = (int)((b % 4) + 1); // 1-2-3-0 순환
                sheetMusic.Add(new NoteData { beat = b, line = line });
            }
        }

        // =========================================================
        // [Part 4] Verse 2 (121 ~ 168박자) - 2절 (Verse1 변형, 2박 지그재그)
        // =========================================================
        for (double b = 121; b < 168; b += 2)
        {
            sheetMusic.Add(new NoteData { beat = b, line = 2 }); // 센터 중심
            sheetMusic.Add(new NoteData { beat = b + 1, line = (int)((b / 2) % 2) * 4 }); // 0 or 4 왔다갔다
        }

        // =========================================================
        // [Part 5] Chorus 2 & Climax (169 ~ 210박자) - 2회차 후렴 (1박 싱글 위주, 듀얼 최소)
        // 고밀도 → 저밀도로 다운그레이드 (BanG Dream BASIC 스타일)
        // =========================================================
        for (double b = 169; b < 210; b += 1)
        {
            int line = (int)(b % 5);
            if (b % 4 == 0) // 4박 듀얼 임팩트
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b, line = 4 });
            }
            else
            {
                sheetMusic.Add(new NoteData { beat = b, line = line }); // 단순 순환
            }
        }

        // =========================================================
        // [Part 6] Outro (211 ~ 225박자) - 여운 마무리
        // =========================================================
        for (double b = 211; b < 225; b += 3)
        {
            sheetMusic.Add(new NoteData { beat = b, line = 2 });
            sheetMusic.Add(new NoteData { beat = b + 1.5, line = 0 });
        }
        // 마지막 빅 듀얼
        sheetMusic.Add(new NoteData { beat = 225, line = 0 });
        sheetMusic.Add(new NoteData { beat = 225, line = 4 });

        // 3. 필수: 시간 순서대로 정렬
        sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));
    }

    // -------------------------------------------------------------

    void Update()
    {
        if (!mainManager.isGame) return;
        if (mainAudioSource == null) return;

        if (mainAudioSource.isPlaying)
        {
            currentTime = mainAudioSource.time;

            while (noteIndex < sheetMusic.Count)
            {
                double hitTime = sheetMusic[noteIndex].beat * secPerBeat;
                double spawnTime = hitTime - noteFallTime;

                if (currentTime >= spawnTime)
                {
                    SpawnNote(sheetMusic[noteIndex].line);
                    noteIndex++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    void SpawnNote(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= noteSpawnPoints.Length) return;
        GameObject noteObj = Instantiate(notePrefabs[lineIndex]);
        noteObj.transform.SetParent(gameUi.transform, false);
        noteObj.transform.position = noteSpawnPoints[lineIndex].position;
    }
}