using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadPanel : MapPanel
{
    public static string prefabsPath = "LoadPanel";

    private TMP_Text loadingText;
    private string baseText = "Loading";
    private string extraText;
    private float currentTime;
    private int x = 0;
    private Coroutine currentCoroutine;
    public override void ShowMeFirst()
    {
        loadingText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
    }
    public override void ShowMeEveryTime()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        x = 2;
        currentTime = 0;
        extraText = "...";
        loadingText.text = baseText + extraText;
        currentCoroutine = StartCoroutine(DoText());
    }
    IEnumerator DoText()
    {
        yield return new WaitUntil(() =>
        {
            if (loadingText == null)
                return false;
            else
                return true;
        });
        yield return new WaitUntil(() =>
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 0.2f)
            {
                if (x % 3 == 0)
                    extraText = ".";
                else if (x % 3 == 1)
                    extraText = "..";
                else if (x % 3 == 2)
                    extraText = "...";

                currentTime = 0;
                loadingText.text = baseText + extraText;
                x += 1;
            }
            return false;
        });
    }
}
