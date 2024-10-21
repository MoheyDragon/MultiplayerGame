using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager:MonoBehaviour
{
    [SerializeField] CanvasGroup tutorialCG;
    [SerializeField] float fadeInSpeed = 1;
    [SerializeField] Sprite[] tutorialSlides;
    [SerializeField] float timeForTutorialSlide;

    const float speedFactor = 0.001f;
    Image tutorialImage;
    WaitForSeconds tutorialWaitingTime;
    int slideIndex;
    private void Awake()
    {
        tutorialWaitingTime = new WaitForSeconds(timeForTutorialSlide);
        tutorialImage = tutorialCG.GetComponent<Image>();
    }
    private void Start()
    {
        StartCoroutine(CO_FadeInTutorial());
    }
    IEnumerator CO_FadeInTutorial()
    {
        tutorialCG.alpha = 0;
        while (tutorialCG.alpha < 1)
        {
            tutorialCG.alpha += fadeInSpeed * speedFactor;
            yield return null;
        }
        StartCoroutine(CO_NextTutorial());
    }
    IEnumerator CO_NextTutorial()
    {
        while (true)
        {
            yield return tutorialWaitingTime;
            slideIndex = (slideIndex + 1) % tutorialSlides.Length;
            tutorialImage.sprite = tutorialSlides[slideIndex];
        }
    }
}
