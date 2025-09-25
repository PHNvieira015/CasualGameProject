using UnityEngine;
using System.Collections;

public class CardDestroyAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float scaleDuration = 0.3f;
    public float fadeDuration = 0.2f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("Effects")]
    public ParticleSystem destroyParticles;
    public AudioClip destroySound;
    
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        originalScale = transform.localScale;
    }
    
    public IEnumerator PlayDestroyAnimation()
    {
        // Trigger sound effect
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, Camera.main.transform.position, 0.5f);
        }
        
        // Trigger particle effect
        if (destroyParticles != null)
        {
            Instantiate(destroyParticles, transform.position, Quaternion.identity);
        }
        
        float elapsed = 0f;
        
        // Scale down and fade out simultaneously
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / scaleDuration;
            
            // Scale animation
            float scaleValue = scaleCurve.Evaluate(progress);
            transform.localScale = originalScale * scaleValue;
            
            // Fade animation
            if (canvasGroup != null)
            {
                float alphaValue = fadeCurve.Evaluate(progress);
                canvasGroup.alpha = alphaValue;
            }
            
            yield return null;
        }
        
        // Ensure final state
        transform.localScale = Vector3.zero;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        // Actually destroy the card
        Destroy(gameObject);
    }
    
    // Quick destroy method for one-line calls
    public static void DestroyCardWithAnimation(GameObject cardObject)
    {
        CardDestroyAnimation anim = cardObject.GetComponent<CardDestroyAnimation>();
        if (anim != null)
        {
            anim.StartCoroutine(anim.PlayDestroyAnimation());
        }
        else
        {
            Destroy(cardObject);
        }
    }
}