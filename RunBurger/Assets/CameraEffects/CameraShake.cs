using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;

    [Header("Debug Mode")]
    [SerializeField] private bool isDebugMode = true;

    private float timer;


    void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        StopShake();
    }

    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }

    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;
        timer = 0f;
    }

    public void Update()
    {
        if (isDebugMode)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ShakeCamera();
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (timer <= 0) StopShake();
            }
        }
    }

}
