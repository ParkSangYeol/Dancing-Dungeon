using System.Collections.Generic;
using System.Linq; // List 작업을 위해 Linq 사용
using UnityEngine;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager Instance { get; private set; }

    public int perfectScore = 100; // Perfect 점수
    public int greatScore = 50; // Great 점수
    public int badScore = 10; // Bad 점수

    private Queue<(NoteMove, string)> leftNoteQueue = new Queue<(NoteMove, string)>(); // 왼쪽 노트를 위한 큐
    private Queue<(NoteMove, string)> rightNoteQueue = new Queue<(NoteMove, string)>(); // 오른쪽 노트를 위한 큐

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 매 프레임마다 타이밍 평가를 호출
        EvaluateTiming();
    }

    public void OnBeat()
    {
        // 비트에 맞춰 타이밍 평가 호출
        EvaluateTiming();
    }

    // 노트가 트리거 영역에 들어오면 큐에 추가
    public void RegisterNoteTiming(NoteMove note, string timing)
    {
        if (note.transform.tag == "LeftNote")
        {
            leftNoteQueue.Enqueue((note, timing));
        }
        else if (note.transform.tag == "RightNote")
        {
            rightNoteQueue.Enqueue((note, timing));
        }
    }

    // 노트가 트리거 영역을 벗어나면 큐에서 제거
    public void DeregisterNoteTiming(NoteMove note)
    {
        if (note.transform.tag == "LeftNote")
        {
            var tempQueue = leftNoteQueue.ToList();
            tempQueue.RemoveAll(n => n.Item1 == note);
            leftNoteQueue = new Queue<(NoteMove, string)>(tempQueue);
        }
        else if (note.transform.tag == "RightNote")
        {
            var tempQueue = rightNoteQueue.ToList();
            tempQueue.RemoveAll(n => n.Item1 == note);
            rightNoteQueue = new Queue<(NoteMove, string)>(tempQueue);
        }
    }

    // 스페이스바 입력 시 노트 타이밍 평가
    private void EvaluateTiming()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 왼쪽 노트와 오른쪽 노트가 모두 있는지 확인
            if (leftNoteQueue.Count > 0 && rightNoteQueue.Count > 0)
            {
                // 왼쪽 노트 평가
                (NoteMove leftnote, string timing) = leftNoteQueue.Dequeue();
                EvaluateNoteTiming(leftnote, timing);
                leftnote.gameObject.SetActive(false);

                // 오른쪽 노트 평가
                (NoteMove rightnote, string timing_right) = rightNoteQueue.Dequeue();
                EvaluateNoteTiming(rightnote, timing_right);
                rightnote.gameObject.SetActive(false);
            }
        }
    }

    // 노트의 타이밍을 평가하여 점수를 계산
    private void EvaluateNoteTiming(NoteMove note, string timing)
    {
        if (timing.Contains("Perfect"))
        {
            Debug.Log("Perfect! Score: " + perfectScore);
        }
        else if (timing.Contains("Great"))
        {
            Debug.Log("Great! Score: " + greatScore);
        }
        else if (timing.Contains("Bad"))
        {
            Debug.Log("Bad! Score: " + badScore);
        }
        else
        {
            Debug.Log("Miss!");
        }
    }
}
