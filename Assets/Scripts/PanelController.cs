using UnityEngine;

public class PanelController : MonoBehaviour
{
    public float moveSpeed = 1000;

    private Vector2 startPos;
    private Vector2 target;
    private bool blockTouch = false;
    private bool panelIsOpen = false;

    void Start()
    {
        var tr = transform as RectTransform;
        target = tr.anchoredPosition;
    }
    public void ClickButtonMovePanel()
    {
        var tr = transform as RectTransform;
        tr.anchoredPosition = Vector2.MoveTowards(tr.anchoredPosition, target, moveSpeed * Time.deltaTime);
        MovePanel(tr, panelIsOpen);
    }
    public void MovePanel(RectTransform tr, bool Open)
    {
        float x = Open ? tr.sizeDelta.x : -tr.sizeDelta.x;
        target = new Vector2(x / 2, tr.anchoredPosition.y);//show menu
        panelIsOpen = !Open;
        blockTouch = true;
        MainScript.disableTouch = true;
    }
    void Update()
    {
        var tr = transform as RectTransform;
        tr.anchoredPosition = Vector2.MoveTowards(tr.anchoredPosition, target, moveSpeed * Time.deltaTime);

        foreach (Touch touch in InputHelper.GetTouches())
        {
            if (touch.phase == TouchPhase.Began)
                startPos = touch.position;
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.position.x - startPos.x > 200)
                {
                    MovePanel(tr, true);
                }
                if(touch.position.x - startPos.x < -200)
                {
                    MovePanel(tr, false);
                }
                break;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (blockTouch)
                {
                    MainScript.disableTouch = false;
                    blockTouch = false;
                }
            }
        }
    }
}
