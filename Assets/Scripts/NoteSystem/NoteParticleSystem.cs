using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

public class NoteParticleSystem : MonoBehaviour
{
    
    private Dictionary<string, ParticleSystem> particleSystems;
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

            
            particleSystems[timing].Play();
        }
        else
        {
            Debug.Log($"Invalid condition: {timing}");
        }
    }
    
}
