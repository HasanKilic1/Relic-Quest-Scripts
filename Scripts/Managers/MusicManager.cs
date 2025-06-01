using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip[] tracks;
    [SerializeField] float fadeBetweenTracks = 3f;
    [Range(0,1)][SerializeField] float trackEndAndStartVolume = 3f;
    private readonly float standardVolume = 1f;
    float trackTransitionTime;
    int nextTrackId = 0;
    void Start()
    {        
        HandleNextPlay();
    }

    private void Update()
    {
        CatchTrackTransition();
    }

    private void HandleNextPlay()
    {
        if(nextTrackId >= tracks.Length)
        {
            nextTrackId = 0;
        }
        AudioClip currentClip = tracks[nextTrackId];
       
        StartCoroutine(FadeInOutVolume(currentClip , trackEndAndStartVolume , fadeBetweenTracks));        
        trackTransitionTime = Time.time + currentClip.length - fadeBetweenTracks;

        nextTrackId++;
    }

    private void CatchTrackTransition()
    {
        if(Time.time > trackTransitionTime)
        {
            HandleNextPlay();
        }
    }
    
    private IEnumerator FadeInOutVolume(AudioClip targetClip,float fadeOutVolume ,float time)
    {
        StartCoroutine(FadeVolume(fadeOutVolume, time));

        yield return new WaitForSeconds(time - 0.2f); //wait to fade out

        musicSource.clip = targetClip; //change clip
        musicSource.Play();
      

        StopAllCoroutines();
        StartCoroutine(FadeVolume(standardVolume, time));
    }

    private IEnumerator FadeVolume(float targetVolume , float timeToFade)
    {
        float currentVolume = musicSource.volume;
        float changePerSec = (targetVolume - currentVolume) / timeToFade;
        float timeElapsed = 0f;
        while (timeElapsed < timeToFade)
        {
            timeElapsed += Time.deltaTime;
            currentVolume += changePerSec * Time.deltaTime;
            musicSource.volume = currentVolume;
            yield return null;
        }
    }

    
}
