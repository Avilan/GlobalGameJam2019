using UnityEngine;

public class InteriorItem2D : MonoBehaviour
{
    public float PlacementOffsetX;

    public float PlacementOffsetY;

    public bool Glowing;

    public bool BungeeGlow;

    public float BungeeGlowRate = 0.1f;

    public GameObject GlowOverlay;

    public AudioClip Music;

    private Renderer render;

    private Collider2D collisionBox;

    private AudioSource audioSource;

    private SpriteRenderer glowOverlayRenderer;

    private float bungeeGlowMultipler = 0;

    private float bungeeGlowValue = 0;

    private int bungeeGlowDirection = 1;

    private float bungeeGlowMax;

    void Awake()
    {
        render = GetComponent<Renderer>();
        collisionBox = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (GlowOverlay != null)
        {
            glowOverlayRenderer = GlowOverlay.GetComponent<SpriteRenderer>();
            glowOverlayRenderer.enabled = false;
            bungeeGlowMax = glowOverlayRenderer.color.a;
        }

        audioSource.clip = Music;
        audioSource.volume = 0;
        StartMusic();
        Hide();
    }

    void Update()
    {
        if (Glowing && BungeeGlow)
        {
            bungeeGlowMultipler += bungeeGlowDirection * BungeeGlowRate * Time.deltaTime;
            if (bungeeGlowMultipler > 1.0f)
            {
                bungeeGlowDirection *= -1;
                bungeeGlowValue = bungeeGlowMax;
            }
            else if (bungeeGlowMultipler < 0)
            {
                bungeeGlowDirection *= -1;
                bungeeGlowValue = 0.0f;
            }
            else
            {
                bungeeGlowValue = bungeeGlowMax * bungeeGlowMultipler;
            }

            glowOverlayRenderer.color = new Color(glowOverlayRenderer.color.r, glowOverlayRenderer.color.g, glowOverlayRenderer.color.b, bungeeGlowValue);
        }
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
            if (BungeeGlow)
            {
                glowOverlayRenderer.color = new Color(glowOverlayRenderer.color.r, glowOverlayRenderer.color.g, glowOverlayRenderer.color.b, 0);
            }

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