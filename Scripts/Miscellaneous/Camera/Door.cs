using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    // LISTEN LEVEL MANAGER EVENTS

    public static event Action OnPlayerEnteredDoor;
    [SerializeField] BoxCollider doorCollider;
    [SerializeField] float timeBeforeDoorOpen = 1f;
    [SerializeField] MMF_Player doorOpenFeedBack;
    bool alreadyEntered = false;
  

    private void Awake()
    {
        doorCollider = GetComponent<BoxCollider>();
        doorCollider.isTrigger = false;
    }
 
    public void OpenDoor(int i)
    {
        alreadyEntered = false;
        
        Invoke(nameof(ActivateTrigger), timeBeforeDoorOpen);
    }
    private void ActivateTrigger()
    {
        doorCollider.isTrigger = true;
        doorOpenFeedBack.PlayFeedbacks();
    }

    private void CloseDoor(int i)
    {
        doorCollider.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerStateMachine>() != null)
        {
            if (alreadyEntered) return;
            alreadyEntered = true;
            OnPlayerEnteredDoor?.Invoke();
        }
    }
   
    
}
