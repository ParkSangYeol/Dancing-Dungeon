using UnityEngine;

public class NoteSpawnController : MonoBehaviour
{
    public NoteSystem noteSystem;
    public BGMPlayer musicSource; // 다른 곳에서 재생되는 오디오 소스
    public double bpm;
    private double interval;
    private double nextNoteTime;
    private double songStartTime;
    private bool musicStarted = false;

    void Start()
    {
        interval = 60d / bpm;
    }

    void Update()
    {
        if (!musicStarted && musicSource.IsBGMPlaying())
        {
            // 음악이 재생되기 시작한 시점을 기록
            songStartTime = AudioSettings.dspTime;
            nextNoteTime = songStartTime + interval;
            musicStarted = true;
        }
        if (musicStarted)
        {
            double currentTime = AudioSettings.dspTime;
            if (currentTime >= nextNoteTime)
            {
                noteSystem.SpawnNote();
                nextNoteTime += interval;
            }

            // 음악이 중지되었는지 확인
            if (!musicSource.IsBGMPlaying())
            {
                musicStarted = false;
            }
        }
    }
}
