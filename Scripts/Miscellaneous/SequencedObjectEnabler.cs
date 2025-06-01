using System.Collections;
using UnityEngine;

public class SequencedObjectEnabler : MonoBehaviour
{
    public GameObject[] Objects;
    public float TotalDuration;
    public bool DisableAllBeforeSequence = true;
    public bool DisableAllAfterSequence = true;
    public bool StartSequenceOnInit;
    private bool isPlayingSequence;

    private void Start()
    {
        if (StartSequenceOnInit) 
        {
            StartSequence();
        }
    }
    public void StartSequence()
    {
        if (!isPlayingSequence)
        {           
            isPlayingSequence = true;
            if (DisableAllBeforeSequence)
            {
                DisableAll();
            }

            StartCoroutine(EnableSequence());
        }     
    }
    private IEnumerator EnableSequence()
    {
        float timeBetween = TotalDuration / Objects.Length;
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i].SetActive(true);
            if (Objects[i].TryGetComponent(out ScaleAnimationer scaleAnimationer))
            {
                scaleAnimationer.Perform();
            }
            yield return new WaitForSeconds(timeBetween);
        }

        isPlayingSequence = false;

        if (DisableAllAfterSequence) 
        {
            DisableAll();
        }
    }

  
    private void DisableAll()
    {
        foreach (var obj in Objects)
        {
            obj.SetActive(false);
        }
    }
}
