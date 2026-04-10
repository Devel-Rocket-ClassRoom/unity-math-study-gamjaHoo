using UnityEngine;
using TMPro;

public class DragAndDropManager : MonoBehaviour
{
    [Header("=== 레이캐스트 설정 ===")]
    [Range(10f, 500f)]
    [SerializeField] private float maxRayDistance = 100f;

    [Header("=== 드래그 ===")]
    [SerializeField] private float placeOffsetY = 0.5f;

    [Header("=== 복귀 ===")]
    [SerializeField] private float returnSpeed = 4f;

    [Header("=== 색상 ===")]
    [SerializeField] private Color colorDragging = Color.yellow;
    [SerializeField] private Color colorDefault = Color.skyBlue;

    [Header("=== UI ===")]
    [SerializeField] private TMP_Text uiInfoText;

    private Camera cam;

    private GameObject dragObject;
    private Renderer dragRenderer;
    private Color originalColor;
    private Vector3 originPos;
    private Quaternion originRot;

    private bool isDragging = false;
    private bool isReturning = false;
    private float returnT = 0f;
    private Vector3 returnStartPos;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!isDragging && !isReturning && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance)
                && hit.collider.CompareTag("Selectable"))
            {
                dragObject = hit.collider.gameObject;
                dragRenderer = dragObject.GetComponent<Renderer>();
                originPos = dragObject.transform.position;
                originalColor = dragRenderer != null ? dragRenderer.material.color : Color.white;

                if (dragRenderer != null) dragRenderer.material.color = colorDragging;
                isDragging = true;
            }
        }

        // 드래그할 때
        if (isDragging)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, maxRayDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == dragObject) continue;

                if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("DropZone"))
                {
                    dragObject.transform.position = hit.point + Vector3.up * placeOffsetY;
                    break;
                }
            }
        }
        // 놓을 때
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            bool dropped = false;

            RaycastHit dropHit = new RaycastHit();

            RaycastHit[] hit = Physics.RaycastAll(ray, maxRayDistance);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.CompareTag("DropZone"))
                {
                    dropHit = hit[i];
                    dropped = true;
                }
            }

            if (dropped)
            {
                // 확정
                dragObject.transform.position = dropHit.point + Vector3.up * placeOffsetY;
                if (dragRenderer != null) dragRenderer.material.color = colorDefault;
                dragObject = null;
            }
            else
            {
                // 복귀 시작
                returnStartPos = dragObject.transform.position;
                returnT = 0f;
                isReturning = true;
            }
        }

        // Lerp
        if (isReturning)
        {
            returnT += Time.deltaTime * returnSpeed;
            dragObject.transform.position = Vector3.Lerp(returnStartPos, originPos, returnT);

            if (returnT >= 1f)
            {
                dragObject.transform.position = originPos;
                if (dragRenderer != null) dragRenderer.material.color = originalColor;
                dragObject = null;
                isReturning = false;
            }
        }
    }
}