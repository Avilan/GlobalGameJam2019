using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class StressVisuals : MonoBehaviour
{
    public List<GameObject> NoiseObjects;

    public int NoiseCount;

    public float StressLevel;

    private SpriteRenderer VignetteRenderer;

    private List<GameObject> noiseObjectsInstances;

    void Awake()
    {
        noiseObjectsInstances = new List<GameObject>();
        VignetteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (NoiseObjects.Any())
        {
            var random = new System.Random();
            for (var i = 0; i < NoiseCount; i++)
            {
                var index = random.Next(0, NoiseObjects.Count);
                noiseObjectsInstances.Add(Instantiate(NoiseObjects[index]));
            }
        }
    }

    Vector2 GetRandomCoordinates()
    {
        return new Vector2(
            Random.Range(VignetteRenderer.bounds.min.x, VignetteRenderer.bounds.max.x), 
            Random.Range(VignetteRenderer.bounds.min.y, VignetteRenderer.bounds.max.y)
        );
    }

    void Update()
    {
        VignetteRenderer.color = new Color(1, 1, 1, StressLevel);

        if (!(VignetteRenderer.color.a > 0))
        {
            foreach (var noiseObject in noiseObjectsInstances.ToList())
            {
                noiseObjectsInstances.Remove(noiseObject);
                Destroy(noiseObject);
            }
        }

        var noiseAlpha = VignetteRenderer.color.a / 20;

        if (noiseObjectsInstances.Any())
        {
            foreach (var noiseObject in noiseObjectsInstances)
            {
                var rotation = new Quaternion(0, 0, Random.rotation.z, 0);

                noiseObject.transform.position = GetRandomCoordinates();
                noiseObject.transform.rotation = rotation;
                noiseObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1, noiseAlpha);
            }
        }
    }
}
