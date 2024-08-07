using System;
using System.Collections.Generic;
using com.kleberswf.lib.core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeObject : Singleton<VolumeObject>
{
    [SerializeField]
    private Volume volume;
    public List<VolumeProfile> volumeProfile;

    protected override void Awake()
    {
        base.Awake();
        if (volume == null)
        {
            volume = gameObject.GetComponent<Volume>();
        }
    }

    public void SetVolumeProfile(int index)
    {
        volume.profile = volumeProfile[index];
    }
}
