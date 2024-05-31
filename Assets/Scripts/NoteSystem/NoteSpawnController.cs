using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NoteSpawnController : MonoBehaviour
{
    public NoteSystem noteSystem;
    public double bpm;
    private double currentTime=0d;
    private double interval;

    void Start()
    {
        interval = 60d/bpm;
    }
    void Update() {
        currentTime+=Time.deltaTime;
        SpawnNotes();
        
    }

    public void  SpawnNotes()
    {
        if(currentTime>interval)
        {
            currentTime-=60d/bpm;
            noteSystem.SpawnNote();
        }
    }
}
