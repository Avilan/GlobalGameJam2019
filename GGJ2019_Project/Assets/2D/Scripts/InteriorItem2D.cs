using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Vector3 = UnityEngine.Vector3;

public class InteriorItem2D : MonoBehaviour
{
    public float PlacementOffsetX;

    public float PlacementOffsetY;

    public bool Glowing;

    public GameObject GlowOverlay;

    public AudioClip Music;

    private Renderer render;

    private Collider2D collisionBox;

    private AudioSource audioSource;

    private Renderer glowOverlayRenderer;

    void Awake()
    {
        render = GetComponent<Renderer>();
        collisionBox = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (GlowOverlay != null)
        {
            glowOverlayRenderer = GlowOverlay.GetComponent<Renderer>();
            glowOverlayRenderer.enabled = false;
        }

        audioSource.clip = Music;
        audioSource.volume = 0;
        StartMusic();
        Hide();
    }

    public void Hide()
    {
        render.enabled = false;
        collisionBox.enabled = false;
    }

    public void ShowAt(Vector3 position)
    {
        render.enabled = true;
        collisionBox.enabled = true;
        transform.position = position;
    }

    public void StartMusic()
    {
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void EnableGlow()
    {
        Glowing = true;
        if (glowOverlayRenderer != null)
        {
            glowOverlayRenderer.enabled = true;
        }
    }

    public void DisableGlow()
    {
        Glowing = false;
        if (glowOverlayRenderer != null)
        {
            glowOverlayRenderer.enabled = false;
        }
    }
}
