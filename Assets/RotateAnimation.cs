using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public RectTransform rt;

    private void Start()
    {
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * -10);
    }
}