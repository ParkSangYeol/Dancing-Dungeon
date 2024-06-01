using System.Collections;
using System.Collections.Generic;
using System.Linq; // List 작업을 위해 Linq 사용
using UnityEngine;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager Instance { get; private set; }

    public int perfectScore = 100; // Perfect 점수
    public int greatScore = 50; // Great 점수
    public int badScore = 10; // Bad 점수

  

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
    public void StartCoroutineFromManager(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
  
}
