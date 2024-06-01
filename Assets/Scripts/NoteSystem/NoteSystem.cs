using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class NoteSystem : MonoBehaviour
{
    [SerializeField]
    public GameObject notePrefeb; // 노트를 사용할 프리펩
    public int poolSize = 10; // 풀 몇개를 관리하는 것인지
    
    public Transform leftSpawn;
    public Transform rightSpawn;
    private List<GameObject> notePool; // 노트 풀
    private int nextIndex = 0; //노트들을 관리하기 위한 인덱스 변수
    private int allNote=0;
  
    
    void Start()
    {
        notePool = new List<GameObject>(); // 풀에 이 노트들을 넣어준다.

        for (int i = 0; i < poolSize; i++)// 내가 원하는 사이즈 만큼
        {
            GameObject note = Instantiate(notePrefeb);// 생성한 후 active false 해준다 
            note.SetActive(false);
            notePool.Add(note);
        }
    }
    public GameObject GetPooledNote()
    {
        GameObject note = notePool[nextIndex];
        nextIndex = (nextIndex + 1) % poolSize;
        return note;
    }
    public void SpawnNote()
    {
        GameObject leftNote = GetPooledNote();
        leftNote.tag="LeftNote";
        GameObject rightNote = GetPooledNote();
        rightNote.tag="RightNote";
        allNote++;
        leftNote.transform.SetParent(leftSpawn);
        rightNote.transform.SetParent(rightSpawn);
        leftNote.transform.position = leftSpawn.transform.position; 
        rightNote.transform.position = rightSpawn.transform.position;
        leftNote.SetActive(true);
        rightNote.SetActive(true);
    }
    public int GetAllNote()
    {
        return allNote;
    }
}
