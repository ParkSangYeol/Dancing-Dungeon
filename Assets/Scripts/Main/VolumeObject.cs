using System;
using System.Collections.Generic;
using com.kleberswf.lib.core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeObject : Singleton<VolumeObject>
{
    private Volume volume;
    public List<VolumeProfile> volumeProfile;

    private void Start()
    {
        volume = gameObject.GetComponent<Volume>();
    }

    public void SetVolumeProfile(int index)
    {
        volume.profile = volumeProfile[index];
    }
}
