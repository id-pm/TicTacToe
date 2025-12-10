using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrawGamePlane : MonoBehaviour
{
    [SerializeField]
    GameObject gameFieldLine;
    [SerializeField]
    GameObject gameCell;
    [SerializeField]
    AudioSource AudioDrawingLines;
    internal static bool isDrawn = false;
    public delegate void FieldReadiness();
    public static event FieldReadiness IsDrawnEvent;
    void DrawLine(GameObject parent, float x, float z, float scale, bool rotate = false)
    {
        GameObject newLine = Instantiate(gameFieldLine, new Vector3(x, 0, z), Quaternion.identity);
        newLine.transform.DOScaleX(5.65f, .5f);
        newLine.transform.SetParent(parent.transform, false);
        if (rotate)
        {
            newLine.transform.Rotate(new Vector3(0, 90, 0));
        }
    }
    public void DrawStandart(int colLine)
    {
        if (isDrawn) return;
        isDrawn = true;
        MainScript.disableTouch = true;
        int leftIndex = 0;
        for (float i = 0.34f * colLine; float.Parse(i.ToString("0.00")) >= -(0.34f * colLine); i-=0.34f)
        {
            int rightIndex = 0;
            for (float j = -(0.34f * colLine); float.Parse(j.ToString("0.00")) <= 0.34f * colLine; j += 0.34f)
            {
                GameObject cube = Instantiate(gameCell, new Vector3(j, 0, i), Quaternion.identity);
                cube.transform.name = $"cell:{leftIndex}.{rightIndex}";
                cube.transform.SetParent(gameObject.transform, false);
                rightIndex++;
            }
            leftIndex++;
        }
        for(int i = 0; i < colLine; i++)
        {
            StartCoroutine(DrawPlayingField(i, colLine));
        }
        
        MainScript.disableTouch = false;
    }
    IEnumerator DrawPlayingField(int i, int colLine, int sec = 1)
    {
        DrawLine(gameObject, -0.17f - 0.34f * i, -0.51f, colLine, true);
        AudioDrawingLines.Play();
        yield return new WaitForSeconds(sec);
        DrawLine(gameObject, 0.17f + 0.34f * i, -0.51f, colLine, true);
        AudioDrawingLines.Play();
        yield return new WaitForSeconds(sec);
        DrawLine(gameObject, 0.51f, -0.17f - 0.34f * i, colLine);
        AudioDrawingLines.Play();
        yield return new WaitForSeconds(sec);
        DrawLine(gameObject, 0.51f, 0.17f + 0.34f * i, colLine);
        AudioDrawingLines.Play();
        yield return new WaitForSeconds(sec);
        IsDrawnEvent?.Invoke();
    }
}
