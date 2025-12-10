using UnityEngine;

public class ToggleCamera : MonoBehaviour
{
    [SerializeField]
     Camera ARCamera;
    [SerializeField]
     Camera MainCamera;
    public void ChangeCamera()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        //ARcamera.enabled = !ARcamera.enabled;
        MainCamera.gameObject.SetActive(!MainCamera.gameObject.activeSelf);
    }
}
