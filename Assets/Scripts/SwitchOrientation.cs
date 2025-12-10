using UnityEngine;
using DG.Tweening;

public class SwitchOrientation : MonoBehaviour
{
    public void SwitchRotation()
    {
        gameObject.transform.DOLocalRotate(new Vector3((int)gameObject.transform.localRotation.eulerAngles.x == 0 ? -90 : 0, 0, 0), .5f);
    }
}
