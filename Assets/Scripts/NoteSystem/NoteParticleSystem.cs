using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

public class NoteParticleSystem : MonoBehaviour
{
    
    private Dictionary<string, ParticleSystem> particleSystems;
    private List<AudioClip> battleBgms;
    Dictionary<string, AudioClip> particleSounds;
    private AudioSource battleBgmsource;
    private AudioSource particlSource;
    private void Awake() {
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
        battleBgmsource.volume = PlayerPrefs.GetFloat("AllVolume");
        particlSource.volume = PlayerPrefs.GetFloat("AllVolume");
    }
        
        
    
   
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayParticle(string timing)
    {
       
       if (particleSystems.ContainsKey(timing))
        {
            
            foreach (var ps in particleSystems.Values)
            {
                ps.Stop();
            }

            Debug.Log(timing);
            particleSystems[timing].Play();
            Debug.Log(particleSystems[timing].name);
            particlSource.PlayOneShot(particleSounds[timing]);
            
        }
        
    }
    
    
}