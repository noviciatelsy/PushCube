using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectControl : MonoBehaviour
{
    public static CollectControl Instance;
    void Awake()
    {
        Instance = this;
    }

    public GameObject ImageGroup;
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;

    private int collertNum = -1;
    void Start()
    {
        collertNum = -1;

        // 初始化全部为白色
        image1.color = Color.white;
        image2.color = Color.white;
        image3.color = Color.white;
        image4.color = Color.white;

        ImageGroup.SetActive(false);
    }

    public void Collect()
    {
        collertNum++;

        ChangeColor();
    }

    private void ChangeColor()
    {
        switch (collertNum)
        {
            case 0:
                StartCoroutine(ShowImageGroup());
                break;
            case 1:
                image1.color = color1;
                break;

            case 2:
                image2.color = color2;
                break;

            case 3:
                image3.color = color3;
                break;

            case 4:
                image4.color = color4;
                break;
            case 5:
                StartCoroutine(ZoomImageGroup());
                break;
        }
    }

    IEnumerator ShowImageGroup()
    {
        ImageGroup.SetActive(true);

        Image[] images = ImageGroup.GetComponentsInChildren<Image>();

        // 初始化 alpha = 0
        foreach (var img in images)
        {
            Color c = img.color;
            c.a = 0;
            img.color = c;
        }

        float time = 0f;

        while (time < 2f)
        {
            time += Time.deltaTime;

            float a = Mathf.Lerp(0f, 1f, time / 2f);

            foreach (var img in images)
            {
                Color c = img.color;
                c.a = a;
                img.color = c;
            }

            yield return null;
        }

        // 保证最终为1
        foreach (var img in images)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }
    }

    // =========================
    // 放大 UI
    // =========================
    IEnumerator ZoomImageGroup()
    {
        Transform t = ImageGroup.transform;

        Vector3 startScale = t.localScale;
        Vector3 targetScale = Vector3.one * 5f;
        Vector3 startPos = t.localPosition;
        Vector3 targetPos = Vector3.zero;

        Image[] images = ImageGroup.GetComponentsInChildren<Image>();

        float time = 0f;

        // ======================
        // 第1秒：scale缓动
        // ======================
        while (time < 1f)
        {
            time += Time.deltaTime;

            float t01 = Mathf.Clamp01(time / 1f);

            // EaseOut (先快后慢)
            float ease = 1f - Mathf.Pow(1f - t01, 3f);

            t.localPosition = Vector3.Lerp(startPos, targetPos, ease);
            t.localScale = Vector3.Lerp(startScale, targetScale, ease);

            yield return null;
        }

        t.localScale = targetScale;

        // ======================
        // 第2秒：alpha 1 → 0
        // ======================
        time = 0f;
        MovementSystem.Instance.CameraZoomOut();
        while (time < 1f)
        {
            time += Time.deltaTime;

            float a = Mathf.Lerp(1f, 0f, time / 1f);

            foreach (var img in images)
            {
                Color c = img.color;
                c.a = a;
                img.color = c;
            }

            yield return null;
        }

        // 保证最终为0
        foreach (var img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }
}
