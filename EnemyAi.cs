using UnityEngine;
using UnityEngine.AI;

public enum SlimeAnimationState { Idle, Walk, Jump, Attack, Damage }

public class EnemyAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public Transform player;
    public float aggroRange = 6f;
    public float returnRange = 12f;

    private int m_CurrentWaypointIndex;
    private Material faceMaterial;
    private Vector3 originPos;
    private WalkType walkType;
    private EnemyCombat enemyCombat;

    public enum WalkType { Patroll, ToOrigin, Chase }

    void Start()
    {
        originPos = transform.position;

        if (SmileBody != null)
        {
            var rend = SmileBody.GetComponentInChildren<Renderer>();
            if (rend != null)
                faceMaterial = rend.materials[1];
            else
                Debug.LogWarning("Renderer not found on SmileBody.");
        }
        else
        {
            Debug.LogWarning("SmileBody is not assigned.");
        }

        if (agent == null)
            Debug.LogError("NavMeshAgent not assigned to EnemyAi.");

        enemyCombat = GetComponent<EnemyCombat>();
        if (enemyCombat == null)
            Debug.LogError("EnemyCombat not found on EnemyAi object.");

        walkType = WalkType.Patroll;
    }

    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance <= aggroRange)
        {
            walkType = WalkType.Chase;
            currentState = SlimeAnimationState.Walk;
        }
        else if (walkType == WalkType.Chase && playerDistance > returnRange)
        {
            walkType = WalkType.ToOrigin;
            currentState = SlimeAnimationState.Walk;
        }

        switch (currentState)
        {
            case SlimeAnimationState.Idle:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;

            case SlimeAnimationState.Walk:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;

                agent.isStopped = false;
                agent.updateRotation = true;

                if (walkType == WalkType.Chase)
                {
                    agent.SetDestination(player.position);
                    SetFace(faces.attackFace);

                    if (playerDistance <= agent.stoppingDistance + 0.5f)
                    {
                        StopAgent();
                        enemyCombat?.TryAttackPlayer(player);
                        currentState = SlimeAnimationState.Idle;
                    }
                }
                else if (walkType == WalkType.ToOrigin)
                {
                    agent.SetDestination(originPos);
                    SetFace(faces.WalkFace);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        walkType = WalkType.Patroll;
                        transform.rotation = Quaternion.identity;
                        currentState = SlimeAnimationState.Idle;
                    }
                }
                else // Patroll
                {
                    if (waypoints.Length == 0 || waypoints[0] == null) return;

                    agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        currentState = SlimeAnimationState.Idle;
                        Invoke(nameof(WalkToNextDestination), 2f);
                    }
                }

                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;

            case SlimeAnimationState.Jump:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");
                break;

            case SlimeAnimationState.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;

            case SlimeAnimationState.Damage:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0")
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;

                StopAgent();
                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", 0);
                SetFace(faces.damageFace);
                break;
        }
    }

    private void SetFace(Texture tex)
    {
        if (faceMaterial != null)
            faceMaterial.SetTexture("_MainTex", tex);
    }

    private void StopAgent()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        animator.SetFloat("Speed", 0);
    }

    private void WalkToNextDestination()
    {
        currentState = SlimeAnimationState.Walk;
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        SetFace(faces.WalkFace);
    }

    public void CancelGoNextDestination() => CancelInvoke(nameof(WalkToNextDestination));

    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            float distanceOrg = Vector3.Distance(transform.position, originPos);
            walkType = (distanceOrg > 1f) ? WalkType.ToOrigin : WalkType.Patroll;
            currentState = SlimeAnimationState.Idle;
        }

        if (message.Equals("AnimationAttackEnded") || message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }
    }

    void OnAnimatorMove()
    {
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}
