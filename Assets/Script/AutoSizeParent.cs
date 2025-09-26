// You can also control this via script:
using UnityEngine;
using UnityEngine.UI;

public class AutoSizeParent : MonoBehaviour
{
    private RectTransform parentRect;
    private RectTransform childRect;
    private ContentSizeFitter sizeFitter;

    void Start()
    {
        parentRect = GetComponent<RectTransform>();
        childRect = transform.GetChild(0).GetComponent<RectTransform>();
        sizeFitter = GetComponent<ContentSizeFitter>();

        if (sizeFitter == null)
        {
            sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}