using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnightActiveSkill : MonoBehaviour , IActiveSkill
{
    public UnityEvent OnStart;
    public UnityEvent OnDamage;
    GameObject character;
    [SerializeField] ParticleSystem tornado;
    [SerializeField] Vector3 tornadoOffset;
    [SerializeField] float duration;
    [SerializeField] float movementSpeed;
    [SerializeField] GameObject cleavePrefab;

    [Header("Check")]
    float currentDamage;
    [SerializeField] float baseDamage;     
    [SerializeField] int damagePerLevel;
    [SerializeField] float detectionRadius;
    [SerializeField] float capsuleHeight;
    [SerializeField] float damageCheckInterval;
    [SerializeField] LayerMask enemyLayer;
    float timer;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player useFeedbacks;
    [SerializeField] MMF_Player finishFeedbacks;
    [SerializeField] GameObject firework;
    PlayerStateMachine playerStateMachine;
    private bool isUsing;
    private void Awake()
    {
        tornado.gameObject.SetActive(false);
        tornado.transform.SetParent(null);
        timer = damageCheckInterval;
    }

    private void Update()
    {
        if (isUsing)
        {
            timer += Time.deltaTime;
            if(timer > damageCheckInterval)
            {
                timer = 0f;
                CheckDamage();
            }

            Vector3 movementVector = new Vector3(InputReader.Instance.GetMovementVector().x, 0, InputReader.Instance.GetMovementVector().y);
            playerStateMachine.characterController.Move(Time.deltaTime * movementSpeed * movementVector.normalized);
            tornado.transform.position = playerStateMachine.transform.position + tornadoOffset;
        }
    }

    public void SetSkillData(int level , int abilityDamage)
    {
        currentDamage = baseDamage + damagePerLevel * level + abilityDamage;
    }

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {       
        playerStateMachine = stateMachine;
        useFeedbacks?.PlayFeedbacks();
        OnStart?.Invoke();
        StartCoroutine(TornadoRoutine());
      //  StartCoroutine(FinisherCoroutine());
       // Invoke(nameof(SendCleaves), 0.3f);
    }
    private IEnumerator TornadoRoutine()
    {
        character = playerStateMachine.selectedCharacter.GetComponent<Character>().model;
        character.SetActive(false);
        tornado.gameObject.SetActive(true);
        tornado.Play();
        isUsing = true;
        PlayerHealth.Instance.CloseHealthBar();
        Instantiate(firework , playerStateMachine.transform.position + Vector3.up * 3f , Quaternion.identity);
        yield return new WaitForSecondsRealtime(duration);

        isUsing = false;
        character.SetActive(true);
        tornado.gameObject.SetActive(false);
        tornado.Stop();
        PlayerHealth.Instance.OpenHealthBar();
        finishFeedbacks?.PlayFeedbacks();
    }

    private void CheckDamage()
    {
        Vector3 capsuleStart = playerStateMachine.transform.position + Vector3.up * (capsuleHeight / 2);
        Vector3 capsuleEnd = playerStateMachine.transform.position - Vector3.up * (capsuleHeight / 2);

        RaycastHit[] hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, detectionRadius, transform.forward, 0f, enemyLayer);
        foreach (var hit in hits)
        {
            if(hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage((int)currentDamage, Vector3.zero, true);
                OnDamage?.Invoke();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 capsuleStart = transform.position + Vector3.up * (capsuleHeight / 2);
        Vector3 capsuleEnd = transform.position - Vector3.up * (capsuleHeight / 2);

        Gizmos.DrawWireSphere(capsuleStart, detectionRadius);
        Gizmos.DrawWireSphere(capsuleEnd, detectionRadius);
        Gizmos.DrawLine(capsuleStart + Vector3.forward * detectionRadius, capsuleEnd + Vector3.forward * detectionRadius);
        Gizmos.DrawLine(capsuleStart - Vector3.forward * detectionRadius, capsuleEnd - Vector3.forward * detectionRadius);
        Gizmos.DrawLine(capsuleStart + Vector3.right * detectionRadius, capsuleEnd + Vector3.right * detectionRadius);
        Gizmos.DrawLine(capsuleStart - Vector3.right * detectionRadius, capsuleEnd - Vector3.right * detectionRadius);
    }
    /*    
        private IEnumerator FinisherCoroutine()
        {
            yield return new WaitForSeconds(duration);
            CheckNearbyEnemies();
            finishFeedBacks?.PlayFeedbacks();
        }

        private void SendCleaves()
        {
            int count = 8;
            float angle = 360f / count;
            Vector3 startDir = playerStateMachine.selectedCharacter.transform.forward;
            for (int i = 0; i < count; i++)
            {
                Vector3 cleaveDirection = Quaternion.AngleAxis(angle * i, Vector3.up) * startDir;
                GameObject cleave = Instantiate(cleavePrefab);
                cleave.transform.position = playerStateMachine.transform.position + Vector3.up * 2f;
                cleave.transform.forward = cleaveDirection.normalized;
                //SendRayOnDirection(cleave.transform.position, cleaveDirection);
            }
        }

        private void SendRayOnDirection(Vector3 start,Vector3 direction)
        {
            Ray ray = new Ray(start, direction);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit , damageRange))
            {
                if(hit.collider.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    enemyHealth.TakeDamage((int)baseDamage , direction , isUnstoppableAttack:true);
                }
            }
        }

        private void CheckNearbyEnemies()
        {
            Collider[] enemies = Physics.OverlapSphere(playerStateMachine.transform.position, damageRange, enemyLayer);

            foreach (var enemy in enemies)
            {
                if(enemy.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    enemyHealth.TakeDamage((int)currentDamage, Vector3.zero, isUnstoppableAttack:true);
                }
            }
        }*/
}
