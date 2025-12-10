using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject gamePlane;
    void Start()
    {
        slider.onValueChanged.AddListener((z) =>
        {
            gamePlane.transform.position = new Vector3(gamePlane.transform.position.x, gamePlane.transform.position.y, - z);
            
        });
    }
}
