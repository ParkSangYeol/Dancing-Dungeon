using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> streetLamps;
    [SerializeField] 
    private Light2D directionalLight;
    
    [SerializeField]
    private float maxLight;
    [SerializeField]
    private float minLight;
    [SerializeField]
    private float lampActiveLight;

    [SerializeField] 
    private float speed;
    
    private bool isLightUp;
    private bool isLampActive;
    private void Awake()
    {
        isLightUp = false;
    }

    private void Start()
    {
        if (streetLamps == null)
        {
            streetLamps = GameObject.FindGameObjectsWithTag("StreetLamps").ToList();
        }

        if (directionalLight == null)
        {
            directionalLight = GameObject.Find("DirectionalLight").GetComponent<Light2D>();
        }

        directionalLight.intensity = maxLight;
        InvokeRepeating("SetLight", 2, 2);
    }
    

    private void SetLight()
    {
        if (isLightUp)
        {
            directionalLight.intensity += speed;

            if (isLampActive && directionalLight.intensity >= lampActiveLight)
            {
                isLampActive = false;
                foreach (var streetLamp in streetLamps)
                {
                    streetLamp.SetActive(false);
                }
            }
            
            if (directionalLight.intensity >= maxLight)
            {
                isLightUp = false;
            }
        }
        else
        {
            directionalLight.intensity -= speed;

            if (!isLampActive && directionalLight.intensity <= lampActiveLight)
            {
                isLampActive = true;
                foreach (var streetLamp in streetLamps)
                {
                    streetLamp.SetActive(true);
                }
            }
            
            if (directionalLight.intensity <= minLight)
            {
                isLightUp = true;
            }
        }
    }
}
