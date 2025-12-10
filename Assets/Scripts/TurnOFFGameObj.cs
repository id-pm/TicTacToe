using UnityEngine;

public class TurnOFFGameObj : MonoBehaviour
{
    public void SwitchActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
