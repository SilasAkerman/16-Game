using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public event System.Action OnCameraFinishScroll;
    public event System.Action<bool> OnCameraScroll;

    public float scrollAmount;
    public float scrollSpeed;

    public Vector3 GetFinalPosition { get { return _finalPosition; } }
    Vector3 _finalPosition;

    void Awake()
    {
        _finalPosition = transform.position;
    }

    public void ScrollUp()
    {
        Vector3 position = transform.position + Vector3.forward * scrollAmount;
        _finalPosition = position;
        StartCoroutine(MoveToPosition(position));
        OnCameraScroll?.Invoke(true);
    }

    public void ScrollDown()
    {
        Vector3 position = transform.position - Vector3.forward * scrollAmount;
        _finalPosition = position;
        StartCoroutine(MoveToPosition(position));
        OnCameraScroll?.Invoke(false);
    }

    IEnumerator MoveToPosition(Vector3 position)
    {
        Vector3 startPosition = transform.position;
        float percentage = 0;

        while (percentage < 1)
        {
            percentage += Time.deltaTime * scrollSpeed;
            transform.position = Vector3.Lerp(startPosition, position, percentage);
            yield return null;
        }

        OnCameraFinishScroll?.Invoke();
    }
}
