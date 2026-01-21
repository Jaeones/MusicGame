using System;
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

    private AudioSource mainAudioSource;
    [SerializeField] MainManager mainManager;

    [Header("Rhythm Settings")]
    public int bpm = 0;
    public double noteSpeed = 0f; // 인스펙터에서 500으로 설정하셔도 되고, 아래 코드에서 강제 적용도 됩니다.
    public float noteFallDistance = 1779f; // 거리 1779 고정

    public List<NoteData> sheetMusic;

    private double currentTime = 0d;
    private int noteIndex = 0;
    private double secPerBeat;
    private double noteFallTime;
    private double dspStartTime;

    void Start()
    {
        // 1. 리스트 초기화
        sheetMusic = new List<NoteData>();

        // 2. 선택된 곡 로드
        int selectedSongIndex = StartManager.musicNum;

        switch (selectedSongIndex)
        {
            case 0:
                LoadSong0(); // TheFatRat - Unity
                break;
            case 1:
                LoadSong1(); // HONEYZ (예시)
                break;
            default:
                Debug.LogError("채보 데이터가 없는 곡 번호입니다: " + selectedSongIndex);
                break;
        }

        // 3. 공통 변수 안전 장치 (LoadSong에서 설정 안 됐을 경우)
        if (bpm > 0 && secPerBeat == 0) secPerBeat = 60d / bpm;
        if (noteSpeed > 0 && noteFallTime == 0) noteFallTime = noteFallDistance / noteSpeed;

        // 4. 정렬
        sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));

        // 5. 오디오 재생 스케줄링 (싱크 핵심)
        if (mainManager != null)
        {
            mainAudioSource = mainManager.GetComponent<AudioSource>();

            // [싱크] 노트가 떨어지는 시간(3.558초)만큼 노래 시작을 늦춤
            dspStartTime = AudioSettings.dspTime + noteFallTime;
            mainAudioSource.PlayScheduled(dspStartTime);
        }
    }

    // -------------------------------------------------------------
    // 곡별 데이터
    // -------------------------------------------------------------

    void LoadSong0()
    {
        // =========================================================
        // TheFatRat - Unity (Refined Ver.)
        // BPM: 105
        // Speed: 500
        // End Beat: 200 (약 1분 54초) - 엔딩 타이밍 보정 완료
        // =========================================================

        bpm = 105;
        noteSpeed = 500;
        noteFallDistance = 1779;

        // 계산
        secPerBeat = 60d / bpm;
        noteFallTime = noteFallDistance / noteSpeed;

        sheetMusic = new List<NoteData>();

        // ---------------------------------------------------------
        // [Phase 1] Intro (0 ~ 32박) - "Sparkling Start"
        // ---------------------------------------------------------
        // 단순 계단을 없애고, 음악의 통통 튀는 느낌을 살려 
        // 흩뿌리는 듯한(Sparkle) 랜덤 느낌의 패턴을 배치합니다.

        // 0~16: 2박 간격 (양 사이드 핑퐁)
        for (double b = 0; b < 16; b += 2)
        {
            // 0 -> 4 -> 1 -> 3 순서로 왔다갔다
            int[] lines = { 0, 4, 1, 3 };
            int index = (int)((b / 2) % 4);
            sheetMusic.Add(new NoteData { beat = b, line = lines[index] });
        }

        // 16~32: 1박 간격 (중앙 지향 패턴)
        for (double b = 16; b < 32; b += 1)
        {
            // 짝수 박자: 센터(2) 고정 (중심 잡기)
            if (b % 2 == 0)
            {
                sheetMusic.Add(new NoteData { beat = b, line = 2 });
            }
            // 홀수 박자: 외곽(0, 4) 번갈아 치기
            else
            {
                sheetMusic.Add(new NoteData { beat = b, line = (b % 4 == 1 ? 0 : 4) });
            }

            // 4박마다 엇박 추가 (살짝 긴장감)
            if (b % 4 == 3)
                sheetMusic.Add(new NoteData { beat = b + 0.5, line = 2 });
        }

        // ---------------------------------------------------------
        // [Phase 2] Verse (32 ~ 64박) - "Call & Response"
        // ---------------------------------------------------------
        // 보컬 파트. 한쪽에서 패턴이 나오면 반대쪽에서 대답하는 형식.
        // 왼쪽(0,1) 4마디 -> 오른쪽(3,4) 4마디 구조로 다양성 확보.

        for (double b = 32; b < 64; b += 1)
        {
            // 베이스 킥은 항상 정박에 (리듬 가이드)
            if (b % 2 == 0) sheetMusic.Add(new NoteData { beat = b, line = 2 });

            // 32~48: 왼쪽 구역 집중 (Left Side)
            if (b < 48)
            {
                // 엇박 멜로디를 왼손으로 처리
                if (b % 2 == 1) sheetMusic.Add(new NoteData { beat = b, line = 0 });
                if (b % 4 == 2) sheetMusic.Add(new NoteData { beat = b + 0.5, line = 1 });
            }
            // 48~64: 오른쪽 구역 집중 (Right Side) - 대칭
            else
            {
                if (b % 2 == 1) sheetMusic.Add(new NoteData { beat = b, line = 4 });
                if (b % 4 == 2) sheetMusic.Add(new NoteData { beat = b + 0.5, line = 3 });
            }
        }

        // ---------------------------------------------------------
        // [Phase 3] Glitch Hop (64 ~ 96박) - "Trill & Tech"
        // ---------------------------------------------------------
        // 기계음 구간. '트릴(Trill, 0-1-0-1)' 패턴을 짧게 넣어 
        // 글리치 음악 특유의 끊기는 느낌을 구현합니다.

        for (double b = 64; b < 96; b += 1)
        {
            // 기본 비트
            if (b % 2 == 0) sheetMusic.Add(new NoteData { beat = b, line = 2 });

            // 4마디마다 짧은 16비트 트릴 (따다닥!)
            if (b % 8 == 6) // 70, 78, 86, 94 박자 부근
            {
                sheetMusic.Add(new NoteData { beat = b, line = 0 });
                sheetMusic.Add(new NoteData { beat = b + 0.25, line = 1 });
                sheetMusic.Add(new NoteData { beat = b + 0.5, line = 0 });
            }
            else if (b % 2 != 0) // 평소 엇박
            {
                sheetMusic.Add(new NoteData { beat = b, line = (b % 4 == 1 ? 4 : 3) });
            }
        }

        // ---------------------------------------------------------
        // [Phase 4] Pre-Drop (96 ~ 140박) - "Escalation"
        // ---------------------------------------------------------
        // 빌드업. 단타에서 시작해 -> 겹치기(Dual)로 발전

        for (double b = 96; b < 128; b += 1)
        {
            int line = (int)((b / 2) % 5); // 천천히 이동
            sheetMusic.Add(new NoteData { beat = b, line = line });

            // 후반부(112~)부터는 대칭 노트 추가 (양손 사용 유도)
            if (b >= 112)
                sheetMusic.Add(new NoteData { beat = b + 0.5, line = 4 - line });
        }

        // 128~140: 드럼 롤 (Drum Roll) - 안쪽으로 조여오는 패턴
        for (double b = 128; b < 140; b += 0.5)
        {
            // 128~136: 넓게 (0, 4)
            if (b < 136)
                sheetMusic.Add(new NoteData { beat = b, line = (b % 1 == 0 ? 0 : 4) });
            // 136~140: 좁게 (1, 3) -> 긴장감 Max
            else
                sheetMusic.Add(new NoteData { beat = b, line = (b % 1 == 0 ? 1 : 3) });
        }

        // 140박 공백 (Drop 직전 정적)

        // ---------------------------------------------------------
        // [Phase 5] THE DROP (141 ~ 200박) - "Main Highlights"
        // ---------------------------------------------------------
        // 기존보다 길이를 4박자 늘려서(196->200) 엔딩 공백을 메움

        for (double b = 141; b < 200; b += 1)
        {
            // [Rhythm Anchor] 쿵(0)-짝(4) 패턴 유지 (User Feedback Good)
            if (b % 2 == 0) sheetMusic.Add(new NoteData { beat = b, line = 0 });
            else sheetMusic.Add(new NoteData { beat = b, line = 4 });

            // [Melody Center] 중앙(1,2,3) 멜로디
            // 173박 전: 8비트 정박 위주
            if (b < 173)
            {
                int melody = (int)(1 + (b % 3));
                sheetMusic.Add(new NoteData { beat = b, line = melody });

                // 가끔 엇박 추가
                if (b % 4 == 0) sheetMusic.Add(new NoteData { beat = b + 0.5, line = 2 });
            }
            // 173박 후: 클라이맥스 (살짝 더 화려하게)
            else
            {
                // 멜로디 역순
                int melody = (int)(3 - (b % 3));
                sheetMusic.Add(new NoteData { beat = b, line = melody });

                // 196박부터 200박 사이(Outro)는 16비트 계단으로 화려하게 마무리
                if (b >= 196)
                {
                    // 0.5박마다 하나씩 더 추가해서 꽉 채움
                    sheetMusic.Add(new NoteData { beat = b + 0.5, line = (b % 2 == 0 ? 3 : 1) });
                }
                else
                {
                    // 평소엔 엇박 하나
                    sheetMusic.Add(new NoteData { beat = b + 0.5, line = (b % 2 == 0 ? 3 : 1) });
                }
            }
        }

        // ---------------------------------------------------------
        // [Final Finish] 200박 (1분 54.5초)
        // ---------------------------------------------------------
        // 기존 196박에서 200박으로 이동됨. 이제 노래 끝과 딱 맞을 겁니다.
        sheetMusic.Add(new NoteData { beat = 200, line = 0 });
        sheetMusic.Add(new NoteData { beat = 200, line = 2 });
        sheetMusic.Add(new NoteData { beat = 200, line = 4 });

        // 정렬
        sheetMusic.Sort((a, b) => a.beat.CompareTo(b.beat));
    }

    void LoadSong1()
    {
        // ... (다른 곡 로직) ...
        // 만약 다른 곡도 있다면 bpm, noteSpeed를 여기서도 각각 설정해주는 것이 좋습니다.
    }

    // -------------------------------------------------------------

    void Update()
    {
        if (!mainManager.isGame) return;
        if (mainAudioSource == null) return;

        // [싱크 로직]
        // 노래 시작 전(-3.558초 ~ 0초)에도 시간이 흐르게 함
        double currentDspTime = AudioSettings.dspTime;
        currentTime = currentDspTime - dspStartTime;

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

    void SpawnNote(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= noteSpawnPoints.Length) return;
        GameObject noteObj = Instantiate(notePrefabs[lineIndex]);
        noteObj.transform.SetParent(gameUi.transform, false);
        noteObj.transform.position = noteSpawnPoints[lineIndex].position;
    }
}