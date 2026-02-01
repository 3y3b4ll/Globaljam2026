using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class Slide
{
    public Sprite sprite;
    public float duration = 2f;
}

public class SlideshowManager : MonoBehaviour
{
    [Header("UI")]
    public Image image;

    [Header("Slides")]
    public List<Slide> slides = new List<Slide>();
    public bool loop = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public bool stopAudioOnFinish = false;

    [Header("Trigger Settings")]
    public List<GameObject> validTriggerObjects = new List<GameObject>();
    public bool triggerOnlyOnce = true;

    [Header("Finish Behavior")]
    public GameObject enableOnFinish;
    public UnityEvent onSlideshowFinished;

    private int currentIndex = 0;
    private Coroutine slideshowCoroutine;
    private bool hasTriggered = false;

    void Start()
    {
        if (image != null)
            image.gameObject.SetActive(false);

        if (enableOnFinish != null)
            enableOnFinish.SetActive(false);
    }

    public void StartSlideshow()
    {
        if (slides.Count == 0 || image == null)
            return;

        if (slideshowCoroutine != null)
            StopCoroutine(slideshowCoroutine);

        image.gameObject.SetActive(true);

        // Start audio
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }

        slideshowCoroutine = StartCoroutine(RunSlideshow());
    }

    private IEnumerator RunSlideshow()
    {
        currentIndex = 0;

        while (true)
        {
            Slide slide = slides[currentIndex];

            image.sprite = slide.sprite;
            image.color = Color.white;

            yield return new WaitForSeconds(slide.duration);

            currentIndex++;

            if (currentIndex >= slides.Count)
            {
                if (loop)
                    currentIndex = 0;
                else
                    break;
            }
        }

        SlideshowFinished();
    }

    private void SlideshowFinished()
    {
        if (stopAudioOnFinish && audioSource != null)
            audioSource.Stop();

        if (enableOnFinish != null)
            enableOnFinish.SetActive(true);

        onSlideshowFinished?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (validTriggerObjects.Contains(other.gameObject))
        {
            hasTriggered = true;
            StartSlideshow();
        }
    }
}

