using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FadeInEffect : MonoBehaviour
{
    public bool InitializeOnStart;

    public float StartDelay;

    public float FadeSpeed;

    public List<GameObject> NoiseObjects;

    public Camera Camera;

    public int NoiseCount;

    public float NoiseRemoveSpeed;

    public float CameraShakeIntensity;

    public float CameraShakeFadeSpeed;
    
    /////

    private SpriteRenderer _renderer;

    private bool running;

    private float waitDelay;

    private float currentOpacity;

    private List<GameObject> noiseObjects;

    private Vector3 cameraOriginalPosition;

    private Vector2 cameraBottomLeft;

    private Vector2 cameraTopRight;

    private float noiseRemoveCountdown;

    private float cameraShakeCountdown;

    void Start()
    {
        cameraOriginalPosition = Camera.transform.position;
        noiseObjects = new List<GameObject>();

        _renderer = GetComponent<SpriteRenderer>();
        currentOpacity = _renderer.color.a;

        cameraBottomLeft = (Vector2)Camera.ScreenToWorldPoint(new Vector3(0, 0, Camera.nearClipPlane));
        cameraTopRight = (Vector2)Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth, Camera.pixelHeight, Camera.nearClipPlane));

        if (InitializeOnStart)
        {
            running = true;
            waitDelay = StartDelay;
            noiseRemoveCountdown = 1.0f;
            cameraShakeCountdown = 1.0f;
            InitializeNoise();
        }
    }

    public void Initialize()
    {
        waitDelay = StartDelay;
        noiseRemoveCountdown = 1.0f;
        cameraShakeCountdown = 1.0f;
        InitializeNoise();
        running = true;
    }

    Vector2 GetRandomScreenCoordinates()
    {
        return new Vector2(Random.Range(cameraBottomLeft.x, cameraTopRight.x), Random.Range(cameraBottomLeft.y, cameraTopRight.y));
    }

    void InitializeNoise()
    {
        foreach (var noise in noiseObjects)
        {
            Destroy(noise);
        }
        noiseObjects.Clear();

        var random = new System.Random();
        for (var i = 0; i < NoiseCount; i++)
        {
            var index = random.Next(0, NoiseObjects.Count);
            noiseObjects.Add(Instantiate(NoiseObjects[index]));
        }
    }

    void Update()
    {
        if (running)
        {
            if (waitDelay > 0)
            {
                waitDelay -= Time.deltaTime;
            }
            else
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(currentOpacity, 0.0f, FadeSpeed * Time.deltaTime));
                currentOpacity = _renderer.color.a;

                if (noiseObjects.Any())
                {
                    foreach (var noiseObject in noiseObjects)
                    {
                        noiseObject.transform.position = GetRandomScreenCoordinates();
                        noiseObject.transform.rotation = Random.rotation;
                    }

                    noiseRemoveCountdown -= NoiseRemoveSpeed * Time.deltaTime;
                    if (noiseRemoveCountdown < 0)
                    {
                        noiseRemoveCountdown = 1.0f;
                        var firstObject = noiseObjects.First();
                        Destroy(firstObject);
                        noiseObjects.Remove(firstObject);
                    }
                }

                cameraShakeCountdown -= CameraShakeFadeSpeed * Time.deltaTime;

                if (cameraShakeCountdown < 0)
                {
                    cameraShakeCountdown = 1.0f;
                    CameraShakeIntensity -= 0.1f;
                }

                if (CameraShakeIntensity > 0)
                {
                    Camera.transform.position = cameraOriginalPosition + new Vector3(Random.Range(-CameraShakeIntensity, CameraShakeIntensity), Random.Range(-CameraShakeIntensity, CameraShakeIntensity), 0);
                }
                else
                {
                    Camera.transform.position = cameraOriginalPosition;
                }

                if (Math.Abs(_renderer.color.a) < 0.1f && !noiseObjects.Any() && CameraShakeIntensity < 0)
                {
                    running = false;
                }
            }
        }
    }
}
