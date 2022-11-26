using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private float transitionDuration;

    public IEnumerator FadeIn()
    {
        panel.gameObject.SetActive(true);
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float newAlpha = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, newAlpha);

            lerpVal += Time.deltaTime / transitionDuration;
            yield return null;
        }

        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 1.0f);
    }

    public IEnumerator FadeOut()
    {
        float lerpVal = 0.0f;

        while (lerpVal < 1.0f)
        {
            float newAlpha = 1.0f - Mathf.Lerp(0.0f, 1.0f, lerpVal);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, newAlpha);

            lerpVal += Time.deltaTime / transitionDuration;
            yield return null;
        }

        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0.0f);
        panel.gameObject.SetActive(false);

        yield return null;
    }

    public IEnumerator DisplayLoading(params AsyncOperation[] asyncOperations)
    {
        int indexProgress = 0;

        while (indexProgress < asyncOperations.Length)
        {
            Debug.Log(indexProgress + " : " + asyncOperations[indexProgress].progress);
            if (asyncOperations[indexProgress].isDone) indexProgress++;
            yield return null;
        }
    }

}
