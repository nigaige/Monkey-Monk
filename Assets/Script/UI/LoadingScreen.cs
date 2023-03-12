using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField] private Image panel;
    [SerializeField] private Image roundPanel;

    private void Awake()
    {
        Instance = this;
    }


    public IEnumerator FadeIn(float duration, TransitionType type)
    {
        switch (type)
        {
            case TransitionType.Fade:
                yield return FadeIn(duration);
                break;
            case TransitionType.Round:
                yield return FadeInRound(duration);
                break;
        }
    }

    public IEnumerator FadeOut(float duration, TransitionType type)
    {
        switch(type)
        {
            case TransitionType.Fade:
                yield return FadeOut(duration);
                break;
            case TransitionType.Round:
                yield return FadeOutRound(duration);
                break;
        }
    }


    public IEnumerator FadeIn(float duration)
    {
        panel.gameObject.SetActive(true);
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float newAlpha = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, newAlpha);

            lerpVal += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 1.0f);
    }

    public IEnumerator FadeOut(float duration)
    {
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float newAlpha = 1.0f - Mathf.Lerp(0.0f, 1.0f, lerpVal);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, newAlpha);

            lerpVal += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0.0f);
        panel.gameObject.SetActive(false);

        yield return null;
    }


    public IEnumerator FadeInRound(float duration)
    {
        roundPanel.gameObject.SetActive(true);
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float per = 1.0f - Mathf.Lerp(0.0f, 1.0f, lerpVal);
            roundPanel.materialForRendering.SetFloat("_Percentage", per);

            lerpVal += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        roundPanel.materialForRendering.SetFloat("_Percentage", 0f);
    }

    public IEnumerator FadeOutRound(float duration)
    {
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float per = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            roundPanel.materialForRendering.SetFloat("_Percentage", per);

            lerpVal += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        roundPanel.materialForRendering.SetFloat("_Percentage", 1f);
        roundPanel.gameObject.SetActive(false);

        yield return null;
    }



    public IEnumerator DisplayLoading(params AsyncOperation[] asyncOperations)
    {
        int indexProgress = 0;

        while (indexProgress < asyncOperations.Length)
        {
            //Debug.Log(indexProgress + " : " + asyncOperations[indexProgress].progress);
            if (asyncOperations[indexProgress].isDone) indexProgress++;
            yield return null;
        }
    }

    public enum TransitionType
    {
        Fade,
        Round
    }

}
