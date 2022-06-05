using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using UnityEngine.UI;

namespace PathCreation.Examples
{
    public class Patrol : MonoBehaviour
    {
        public PathFollower PF;
        public float startWaitTime = 4;                 //  Wait time of every action
        public float timeToRotate = 2;                  //  Wait time when the enemy detect near the player without seeing
        public float speedWalk = 6;                     //  Walking speed, speed in the nav mesh agent
        public float speedRun = 9;                      //  Running speed

        //private InputDevice targetDevice;

        public float viewRadius = 30;                   //  Radius of the enemy view
        public float viewAngle = 90;                    //  Angle of the enemy view
        public LayerMask playerMask;                    //  To detect the player with the raycast
        public LayerMask obstacleMask;                  //  To detect the obstacules with the raycast
        public float meshResolution = 1.0f;             //  How many rays will cast per degree
        public int edgeIterations = 4;                  //  Number of iterations to get a better performance of the mesh filter when the raycast hit an obstacule
        public float edgeDistance = 0.5f;               //  Max distance to calcule the a minumun and a maximum raycast when hits something

        public Animator animator;
        public NavMeshAgent agent;

        public int m_CurrentWaypointIndex;                     //  Current waypoint where the enemy is going to

        public Queue<Vector3> m_controller1 = new Queue<Vector3>();
        public Vector3 m_controller2;
        public Vector3 m_controller3;
        public int mo_count;

        public GameObject Left;
        public GameObject Right;
        public GameObject Head;

        Transform tmp_r;
        Transform tmp_l;

        public Vector3 playerLastPosition = Vector3.zero;      //  Last position of the player when was near the enemy
        public Vector3 m_PlayerPosition;                       //  Last position of the player when the player is seen by the enemy

        public float m_WaitTime;                               //  Variable of the wait time that makes the delay
        public float m_TimeToRotate;                           //  Variable of the wait time to rotate when the player is near that makes the delay
        public bool m_playerInRange;                           //  If the player is in range of vision, state of chasing
        public bool m_PlayerNear;                              //  If the player is near, state of hearing
        public bool m_IsPatrol;                                //  If the enemy is patrol, state of patroling
        public bool m_CaughtPlayer;                            //  if the enemy has caught the player
        public bool m_detected;
        public bool m_attack;
        public bool m_disguide;
        public bool reset;

        void Start()
        {
            m_Reset();
            InvokeRepeating("GetMovement", 2.0f, 0.2f);

        }

        public void m_Reset()
        {
            PF.PathStart();
            m_PlayerPosition = Vector3.zero;
            m_IsPatrol = true;
            m_CaughtPlayer = false;
            m_playerInRange = false;
            m_PlayerNear = false;
            m_attack = false;
            m_detected = false;
            m_disguide = false;
            m_WaitTime = startWaitTime;
            m_TimeToRotate = timeToRotate;
            tmp_r = Right.transform;
            tmp_l = Left.transform;

            m_CurrentWaypointIndex = 0;

        }


        private void Update()
        {

            if (m_IsPatrol || m_PlayerPosition == Vector3.zero || m_disguide)
            {
                if(!m_playerInRange)
                    StartCoroutine(AnimationPatrol());

                Patroling();
                if (m_PlayerPosition == Vector3.zero || m_PlayerNear || m_playerInRange || !m_disguide)
                {
                    m_Reset();
                }
            }
            if (!m_IsPatrol || m_playerInRange && m_attack)
            {
                Chasing();
            }
            if(m_playerInRange && !m_attack && !m_disguide)        
                StartCoroutine(AnimationSeeing());

            if (!m_disguide)
                EnviromentView();

        }

        void GetMovement()
        {
            if (m_playerInRange && !m_attack)
            {
                Vector3 LeftPos = Left.transform.localPosition;
                Vector3 RightPos = Right.transform.localPosition;
                Vector3 LeftRot = Left.transform.localEulerAngles;
                Vector3 RightRot = Right.transform.localEulerAngles;

                m_controller1.Enqueue(LeftPos);
                m_controller1.Enqueue(RightPos);
                m_controller1.Enqueue(LeftRot);
                m_controller1.Enqueue(RightRot);

                mo_count = 0;

                Vector3 print = new Vector3();

                if (m_controller1.Count >= 10 * 4)
                {

                    for (int i = 0; i < m_controller1.Count; i++)
                    {
                        print -= m_controller1.Dequeue();

                        if (print != Vector3.zero)
                        {
                            mo_count++;
                        }
                    }


                    if (mo_count >= 4)
                    {
                        m_attack = true;
                    }
                    else
                    {
                        m_playerInRange = false;
                        m_IsPatrol = true;
                        m_disguide = true;
                        StartCoroutine(DisguideReset());

                    }
                }
            }
            else
            {
                m_controller1.Clear();
            }
        }


        private void Chasing()
        {
            if (!m_CaughtPlayer && m_attack)
            {
                agent.SetDestination(m_PlayerPosition);

                if (m_playerInRange)
                {
                    Move(speedRun);

                    StartCoroutine(AnimationChasing());

                    agent.destination = m_PlayerPosition;

                    if (agent.remainingDistance <= 2.0f)
                    {
                        StartCoroutine(AnimationAttack());
                    }
                }
            }
        }

        private void Patroling()
        {
            if (m_PlayerNear)
            {
                if (m_TimeToRotate <= 0)
                {
                    Move(speedWalk);
                }
                else
                {
                    m_TimeToRotate -= Time.deltaTime;
                }
            }
            else
            {
                playerLastPosition = Vector3.zero;
                PF.PathUpdate();
            }
        }

        void Move(float speed)
        {
            PF.PathSpeed(speed);

        }

        void CaughtPlayer()
        {
            Move(0);
        }

        IEnumerator DisguideReset()
        {
            yield return new WaitForSeconds(2.5f);
            m_disguide = false;
        }

        IEnumerator AnimationAttack()
        {
            animator.GetComponent<Animator>().SetInteger("battle", 2);
            animator.GetComponent<Animator>().SetInteger("moving", 2);
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator AnimationChasing()
        {
            animator.GetComponent<Animator>().SetInteger("battle", 1);
            animator.GetComponent<Animator>().SetInteger("moving", 1);
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator AnimationPatrol()
        {
            animator.GetComponent<Animator>().SetInteger("battle", 0);
            animator.GetComponent<Animator>().SetInteger("moving", 1);
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator AnimationSeeing()
        {
            animator.GetComponent<Animator>().SetInteger("battle", 7);
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator Catch()
        {
            Transform player;           

            Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);   //  Make an overlap sphere around the enemy to detect the playermask in the view radius

            for (int i = 0; i < playerInRange.Length; i++)
            {
                player = playerInRange[i].transform;
                Vector3 dirToPlayer = (player.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
                {
                    float dstToPlayer = Vector3.Distance(transform.position, player.position);          //  Distance of the enmy and the player
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                    {

                        m_IsPatrol = false;                 //  Change the state to chasing the player
                        m_playerInRange = true;             //  The player has been seeing by the enemy and then the nemy starts to chasing the player
 
                            if (Vector3.Distance(transform.position, player.position) > (viewRadius + 1))
                                m_playerInRange = false;                //  Change the sate of chasing
                            else
                                m_PlayerPosition = player.transform.position;
                    }
                    else
                        m_playerInRange = false;

                }
            }
            yield return new WaitForSeconds(0.5f);

        }


        void EnviromentView()
        {
            StartCoroutine(Catch());
        }
    }
}
