using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeConnection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private Color lineColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private float curveHeight = 30f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.useWorldSpace = true;
    }

    public void DrawConnection(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null) return;

        Vector3 midPoint = (start + end) / 2;
        midPoint.z -= curveHeight * Mathf.Sign(start.x - end.x);

        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, midPoint);
        lineRenderer.SetPosition(2, end);
    }
}