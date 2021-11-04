using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CinemachineImpulseSource camImpulseSource;
    // Start is called before the first frame update
    void Start()
    {
        camImpulseSource = Camera.main.GetComponent<CinemachineImpulseSource>();
        DebugComponent.HandleErrorIfNullGetComponent<CinemachineImpulseSource, CameraShake>(camImpulseSource, this, gameObject);
    }

    public static void shake(float shakeAmplitude = 2, float shakeFrequency = 2, float shakeSustain = .2f)
    {
        camImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = shakeAmplitude;
        camImpulseSource.m_ImpulseDefinition.m_FrequencyGain = shakeFrequency;
        camImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = shakeSustain;
        camImpulseSource.GenerateImpulse();
    }

}
