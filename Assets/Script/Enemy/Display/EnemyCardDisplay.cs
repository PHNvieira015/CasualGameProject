using UnityEngine;
using System.Collections;

public class EnemyCardDisplay : MonoBehaviour
{
    [SerializeField] private Transform cardDisplayArea;
    [SerializeField] private GameObject cardPreviewPrefab;

    public IEnumerator ShowCardPreview(Card card, float duration)
    {
        GameObject preview = Instantiate(cardPreviewPrefab, cardDisplayArea);
        // Configure preview with card data

        yield return new WaitForSeconds(duration);

        Destroy(preview);
    }
}