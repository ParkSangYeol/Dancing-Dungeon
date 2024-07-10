using System;
using System.Collections.Generic;
using UnityEngine;

public class NoteParticleSystem : MonoBehaviour
{
    private Dictionary<string, ParticleSystem> particleSystems;
    private List<AudioClip> battleBgms;
    Dictionary<string, AudioClip> particleSounds;
    private AudioSource battleBgmsource;
    private AudioSource particlSource;
    [SerializeField] CombatSceneUIManager combatSceneUIManager;

    private void Awake()
    {
        var particleSystemArray = GetComponentsInChildren<ParticleSystem>(true);

        particleSystems = new Dictionary<string, ParticleSystem>();

        foreach (var ps in particleSystemArray)
        {
            Debug.Log($"Found particle system: {ps.name}");
            switch (ps.name)
            {
                case "PerfectParticle":
                    particleSystems["Perfect"] = ps;
                    break;
                case "GreatParticle":
                    particleSystems["Great"] = ps;
                    break;
                case "BadParticle":
                    particleSystems["Bad"] = ps;
                    break;
                case "MissParticle":
                    particleSystems["Miss"] = ps;
                    break;
                default:
                    Debug.Log($"Unknown particle system name: {ps.name}");
                    break;
            }
        }

        foreach (var ps in particleSystems.Values)
        {
            ps.Stop();
        }
        particleSounds = new Dictionary<string, AudioClip>
        {
            { "Perfect", Resources.Load<AudioClip>("PerfectTiming") },
            { "Great", Resources.Load<AudioClip>("GreatTiming") },
            { "Bad", Resources.Load<AudioClip>("BadTiming") },
            { "Miss", Resources.Load<AudioClip>("MissTiming") }
        };

        battleBgms = new List<AudioClip>
        {
            Resources.Load<AudioClip>("BattleBgm"),
            Resources.Load<AudioClip>("BattleBgm2"),
        };

        battleBgmsource = gameObject.AddComponent<AudioSource>();
        particlSource = gameObject.AddComponent<AudioSource>();

        // 디버깅 로그 추가
        float allVolume = PlayerPrefs.GetFloat("AllVolume");
        

        battleBgmsource.volume = allVolume;
        particlSource.volume = allVolume;

      
    }

    void Start()
    {
        // 추가 초기화가 필요한 경우 여기에 작성
    }

    void Update()
    {
        // 업데이트 로직이 필요한 경우 여기에 작성
    }

    public void PlayParticle(string timing)
    {
        if (particleSystems.ContainsKey(timing))
        {
            foreach (var ps in particleSystems.Values)
            {
                ps.Stop();
            }

            Debug.Log($"Playing particle: {timing}");
            particleSystems[timing].Play();
            particlSource.PlayOneShot(particleSounds[timing]);
            
        }
        else
        {
            Debug.LogWarning($"No particle system found for timing: {timing}");
        }
    }
}
