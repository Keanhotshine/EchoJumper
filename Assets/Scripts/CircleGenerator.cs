using UnityEngine;

[ExecuteInEditMode] // 允许在编辑器中实时预览
[RequireComponent(typeof(LineRenderer))]
public class CircleGenerator : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("圆的半径")]
    public float radius = 5f;

    [Tooltip("圆的分段数（越高越平滑）")]
    [Range(3, 100)]
    public int segments = 50;

    [Header("圆心位置")]
    [Tooltip("圆心相对于物体原点的偏移")]
    public Vector3 centerOffset = Vector3.zero;

    [Header("圆的方向")]
    [Tooltip("用欧拉角控制圆面朝向")]
    public Vector3 rotationEuler = Vector3.zero;

    private LineRenderer _lineRenderer;
    private float _lastRadius;
    private Vector3 _lastRotationEuler;
    private Vector3 _lastCenterOffset;

    void Start()
    {
        InitializeLineRenderer();
        GenerateCircle();
    }

    void Update()
    {
        // 检测参数变化，动态更新圆
        if (radius != _lastRadius ||
            rotationEuler != _lastRotationEuler ||
            centerOffset != _lastCenterOffset)
        {
            GenerateCircle();
            _lastRadius = radius;
            _lastRotationEuler = rotationEuler;
            _lastCenterOffset = centerOffset;
        }
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = false; // 使用本地坐标
    }

    public void GenerateCircle()
    {
        if (_lineRenderer == null) InitializeLineRenderer();

        _lineRenderer.positionCount = segments + 1;
        Quaternion rotation = Quaternion.Euler(rotationEuler);
        float deltaTheta = 2f * Mathf.PI / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            // 计算基础坐标（XY 平面）
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            Vector3 localPos = new Vector3(x, y, 0);

            // 应用旋转和圆心偏移
            localPos = rotation * localPos + centerOffset;
            _lineRenderer.SetPosition(i, localPos);
            theta += deltaTheta;
        }
    }

    // 编辑器实时预览
    void OnValidate()
    {
        if (_lineRenderer != null) GenerateCircle();
    }
}