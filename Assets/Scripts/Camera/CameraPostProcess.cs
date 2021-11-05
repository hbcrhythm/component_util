using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class CameraPostProcess : MonoBehaviour
{
    private GameObject postProcessGO;
    private GameObject postProcessVolumeGO;
    // Start is called before the first frame update
    
    Volume volume;

    void Start()
    {
        postProcessGO = new GameObject("PostProcess");
        postProcessVolumeGO = new GameObject("PostProcess-Volume");
        postProcessVolumeGO.transform.parent = postProcessGO.transform;

        volume = postProcessVolumeGO.AddComponent<Volume>();
        volume.profile = new VolumeProfile();

        volume.profile.Add<Bloom>().active = false;
        volume.profile.Add<Tonemapping>().active = false;
        volume.profile.Add<ChromaticAberration>().active = false;
        volume.profile.Add<ColorAdjustments>().active = false;
        volume.profile.Add<Vignette>().active = false;
        volume.profile.Add<FilmGrain>().active = false;
        volume.profile.Add<LensDistortion>().active = false;
        volume.profile.Add<DepthOfField>().active = false;
    }

    void SetBloom(bool on)
    {
        if (volume.profile.TryGet<Bloom>(out Bloom bloom))
        {
            bloom.active = on;
        }
        if (volume.profile.TryGet<Tonemapping>(out Tonemapping tonemapping))
        {
            tonemapping.active = on;
        }
    }

    void SetWeight(float x)
    {
        volume.weight = x;
    }

    void SetProcess(bool on)
    {
        DOVirtual.Float(on ? 0 : 1, on ? 1 : 0, .3f, SetWeight).SetUpdate(true);
    }

}
