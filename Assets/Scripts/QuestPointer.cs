using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class QuestPointer : MonoBehaviour
{
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private Texture arrowSprite;
    [SerializeField] private Texture CrossSprite;
    private GameObject targetObject;

    private RawImage rawImage;
    private RectTransform minimapRect;
    private Vector3 targetPosition;
    private float border = 0.05f;
    private Vector2 minimapSize;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        minimapRect = GetComponent<RectTransform>();
        minimapSize = minimapRect.sizeDelta;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPos = miniMapCamera.WorldToViewportPoint(targetPosition);
        //Debug.Log("viewportPos: " + viewportPos);

        bool isOffScreen =
            viewportPos.x < 0 || viewportPos.x > 1 ||
            viewportPos.y < 0 || viewportPos.y > 1 ||
            viewportPos.z < 0;

        if (isOffScreen)
        {
            RotatePointerTowardsTargetPosition();

            rawImage.texture = arrowSprite;
            Vector3 cappedViewportPos = viewportPos;
            if (cappedViewportPos.x < border) cappedViewportPos.x = border;
            if (cappedViewportPos.x > 1 - border) cappedViewportPos.x = 1 - border;
            if (cappedViewportPos.y < border) cappedViewportPos.y = border;
            if (cappedViewportPos.y > 1 - border) cappedViewportPos.y = 1 - border;
            //Debug.Log("cappedViewportPos: " + cappedViewportPos);

            Vector2 anchoredPos = new Vector2(
                (cappedViewportPos.x - 0.5f) * minimapSize.x,
                (cappedViewportPos.y - 0.5f) * minimapSize.y
            );
            //Debug.Log("anchoredPos: " + anchoredPos);

            minimapRect.anchoredPosition = anchoredPos;
        }
        else
        {
            rawImage.texture = CrossSprite;
            Vector2 anchoredPos = new Vector2(
                (viewportPos.x - 0.5f) * minimapSize.x,
                (viewportPos.y - 0.5f) * minimapSize.y
            );
            minimapRect.anchoredPosition = anchoredPos;
            minimapRect.localEulerAngles = Vector3.zero;

        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = miniMapCamera.transform.position;
        fromPosition.y = 0f; // Keep the y position constant
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = UtilsClass.GetAngleFromVectorFloat(dir) + miniMapCamera.transform.eulerAngles.y;
        minimapRect.localEulerAngles = new Vector3(0, 0, angle);
    }

    private void Hide()
    {
        rawImage.enabled = false;
    }

    public void SetTarget(GameObject target)
    {
        targetObject = target;
        targetPosition = target.transform.position;
    }
}
