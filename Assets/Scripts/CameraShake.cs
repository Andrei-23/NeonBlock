using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineCamera CinemachineVirtualCamera;
    private float shakeIntensity = 1.0f;
    private float shakeTime = 0.2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;
    private Vector3 originalPos;
    private Quaternion originalRotation;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            CinemachineVirtualCamera = GetComponent<CinemachineCamera>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        originalPos = Camera.main.transform.localPosition;
        originalRotation = Camera.main.transform.localRotation;
        StopShake();
        //maybe?
        //DontDestroyOnLoad(gameObject);
    }

    public void ShakeCamera(float duration, float intensity)
    {
        _cbmcp = CinemachineVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.AmplitudeGain = intensity;
        timer = duration;
    }
    public void ShakeCamera()
    {
        ShakeCamera(shakeIntensity, shakeTime);
    }

    public void StopShake()
    {
        Camera.main.transform.localPosition = originalPos;
        Camera.main.transform.localRotation = originalRotation;

        _cbmcp = CinemachineVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.AmplitudeGain = 0f;
        timer = 0;
    }

    public void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                StopShake();
            }
        }
    }
}
