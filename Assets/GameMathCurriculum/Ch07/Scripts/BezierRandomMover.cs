using UnityEngine;
using System.Collections.Generic;

public class BezierRandomMover : MonoBehaviour
{
    public int numberOfSpheres = 30;

    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;

    [SerializeField] private float minDuration = 1f;
    [SerializeField] private float maxDuration = 4f;

    private Vector3 startPoint => startTransform.position;
    private Vector3 endPoint => endTransform.position;

    private List<BezierSphere> activeSpheres = new List<BezierSphere>();

    private class BezierSphere
    {
        public GameObject obj;
        public Vector3 p1, p2;
        public float t;
        public float speed;   // 1 / duration
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnSpheres();

        for (int i = activeSpheres.Count - 1; i >= 0; i--)
        {
            BezierSphere bs = activeSpheres[i];

            bs.t += Time.deltaTime * bs.speed;

            if (bs.t >= 1f)
            {
                bs.obj.transform.position = endPoint;
                Destroy(bs.obj);
                activeSpheres.RemoveAt(i);
                continue;
            }

            bs.obj.transform.position = CubicBezier(startPoint, bs.p1, bs.p2, endPoint, bs.t);
        }
    }

    private void SpawnSpheres()
    {
        for (int i = 0; i < numberOfSpheres; i++)
        {
            Vector3 p1 = startPoint + Random.insideUnitSphere * 2f;
            Vector3 p2 = endPoint + Random.insideUnitSphere * 2f;

            float duration = Random.Range(minDuration, maxDuration);

            BezierSphere bs = new BezierSphere
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere),
                p1 = p1,
                p2 = p2,
                t = 0f,
                speed = 1f / duration    // duration초 동안 0→1
            };

            bs.obj.transform.localScale = Vector3.one * 0.04f;

            Color sphereColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f); // 색 선명도 밝기
            
            Renderer renderer = bs.obj.GetComponent<Renderer>();
            renderer.material.color = sphereColor;

            TrailRenderer trail = bs.obj.AddComponent<TrailRenderer>();

            trail.time = 0.5f;              // 꼬리가 유지되는 시간 (길수록 긴 꼬리)
            trail.startWidth = 0.04f;       // 공 쪽 굵기
            trail.endWidth = 0f;          // 꼬리 끝 굵기 (0 = 뾰족하게)
            trail.minVertexDistance = 0.05f; // 꼬리 부드러움

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(sphereColor, 0f),  // 앞쪽: 공 색
                new GradientColorKey(sphereColor, 1f)   // 끝쪽: 동일 색
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),   // 앞쪽: 불투명
                new GradientAlphaKey(0f, 1f)    // 끝쪽: 투명
                }
            );
            trail.colorGradient = gradient;

            // Trail 전용 머티리얼 (없으면 핑크로 깨짐)
            trail.material = new Material(Shader.Find("Sprites/Default"));

            bs.obj.transform.position = startPoint;
            activeSpheres.Add(bs);
        }
    }

    Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(d, e, t);
    }
}