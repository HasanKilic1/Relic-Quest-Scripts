using UnityEngine;

public class SimplePlayerFollower : MonoBehaviour
{
    public bool FollowActiveCharacter = true;
    public bool FollowAtStart = true;
    public bool lockYPosition = true;
    private Transform target;
    private float yPos;

    void Start()
    {
        if (FollowActiveCharacter)
        {            
            Transform character = PlayerController.Instance.GetComponent<PlayerStateMachine>().selectedCharacter.transform;
            if (character.GetComponentInChildren<RootRig>() != null)
            {
                target = character.GetComponentInChildren<RootRig>().transform;
            }
            else target = character;
        }
        else target = PlayerController.Instance.transform;

        yPos = transform.position.y;
    }
    void Update()
    {
        transform.position = new Vector3(target.position.x , yPos , target.position.z);
    }
}