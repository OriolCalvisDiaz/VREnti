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
        public float closeDistance = 5;                 //  Radius of movment sensibility
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
        public Queue<Vector3> m_controller2 = new Queue<Vector3>();
        public Queue<Vector3> m_controller3 = new Queue<Vector3>();
        public Queue<Vector3> m_controller4 = new Queue<Vector3>();

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

        public bool reset;

        public int state = 0;
        public int state_reading = 0;

        Vector3 printLeft;// = Left.transform.localPosition;
        Vector3 printRight;// = Right.transform.localPosition;
        Vector3 printHead;// = Head.transform.localEulerAngles;
        Vector3 printUser;// = m_PlayerPosition;

        void Start()
        {
            m_Reset();

        }

        public void m_Reset()
        {
            PF.PathStart();
            m_PlayerPosition = Vector3.zero;

            m_WaitTime = startWaitTime;
            m_TimeToRotate = timeToRotate;
            tmp_r = Right.transform;
            tmp_l = Left.transform;

            m_CurrentWaypointIndex = 0;

            printLeft = Left.transform.localPosition;
            printRight = Right.transform.localPosition;
            printHead = Head.transform.localEulerAngles;
            printUser = m_PlayerPosition;

            state = 0;

            state_reading = 0;

            animator.GetComponent<Animator>().SetInteger("battle", 0);
            animator.GetComponent<Animator>().SetInteger("moving", 1);
            mo_count = 0;

            m_controller1.Clear();
            m_controller2.Clear();
            m_controller3.Clear();
            m_controller4.Clear();


        }


        private void Update()
        {

            switch (state)
            {
                case 0: //Patrol
                    animator.GetComponent<Animator>().SetInteger("battle", 0); 
                    animator.GetComponent<Animator>().SetInteger("moving", 1);

                    Patroling();
                    EnviromentView();

                    break;
                case 1: //PlayerInRange

                    GetMovement();
                    Patroling();

                    break;
                case 2: //Detected
                    animator.GetComponent<Animator>().SetInteger("battle", 7);

                    StartCoroutine("Roar");

                    break;
                case 3: //Attack

                    Chasing();

                    m_controller1.Clear();
                    m_controller2.Clear();
                    m_controller3.Clear();
                    m_controller4.Clear();


                    break;
                case 4: //Ignore
                    animator.GetComponent<Animator>().SetInteger("battle", 0);
                    animator.GetComponent<Animator>().SetInteger("moving", 1);

                    i_Patroling();
                    StartCoroutine("Fade");

                    m_controller1.Clear();
                    m_controller2.Clear();
                    m_controller3.Clear();
                    m_controller4.Clear();
                    break;
                case 5: //Chase

                  //  Chasing();

                    break;
                case 6: //Death

                    m_Reset();

                    break;
                default:
                    break;
            }
        }

        IEnumerator Roar()
        {
            yield return new WaitForSeconds(1.0f);
            state = 3;
        }

        IEnumerator Fade()
        {
            yield return new WaitForSeconds(1.0f);
            state = 0;
        }

        IEnumerator Attacking()
        {
            animator.GetComponent<Animator>().SetInteger("battle", 2);
            animator.GetComponent<Animator>().SetInteger("moving", 2);
            yield return new WaitForSeconds(1.14f);
            state = 7;
        }

        void GetMovement()
        {
            Vector3 LeftPos = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 RightPos = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 LeftRot = Left.transform.eulerAngles;
            Vector3 RightRot = Right.transform.eulerAngles;

            switch (state_reading)
            { 
                case 0: 


                Vector3 HeadRot = Head.transform.localEulerAngles;
                Vector3 User = m_PlayerPosition;

                m_controller1.Enqueue(LeftPos);
                m_controller2.Enqueue(RightPos);
                m_controller3.Enqueue(LeftRot);
                m_controller4.Enqueue(RightRot);

                if (m_controller1.Count == 20 * 2)
                {
                    state_reading = 1;
                }
                else
                {
                    for (int i = 0; i < 20 * 2; i++)
                    {

                        if (m_controller1.Count > 20 * 2)
                        {
                            m_controller1.Dequeue();//Si no borra, no hay suficientes seguir leyendo
                        }
                        if (m_controller2.Count > 20 * 2)
                        {
                            m_controller2.Dequeue();//Si no borra, no hay suficientes seguir leyendo
                        }
                        if (m_controller3.Count > 20 * 2)
                        {
                            m_controller3.Dequeue();//Si no borra, no hay suficientes seguir leyendo
                        }
                        if (m_controller4.Count > 20 * 2)
                        {
                            m_controller4.Dequeue();//Si no borra, no hay suficientes seguir leyendo
                        }

                        if (m_controller1.Count == 20 * 2)
                        {
                            state_reading = 1;
                            break;
                        }
                        if (m_controller2.Count == 20 * 2)
                        {
                            state_reading = 1;
                            break;
                        }
                        if (m_controller3.Count == 20 * 2)
                        {
                            state_reading = 1;
                            break;
                        }
                        if (m_controller4.Count == 20 * 2)
                        {
                            state_reading = 1;
                            break;
                        }

                        if (m_controller1.Count < 20 * 2)
                        {
                            break;
                        }
                        if (m_controller2.Count < 20 * 2)
                        {
                            break;
                        }
                        if (m_controller3.Count < 20 * 2)
                        {
                            break;
                        }
                        if (m_controller4.Count < 20 * 2)
                        {
                            break;
                        }
                    }
                }

                break;
                case 1: //
                    mo_count = 0;

                   // mo_count = GetCount(m_controller1.Count, m_controller1, LeftPos);
                   // mo_count += GetCount(m_controller2.Count, m_controller2, RightPos);
                    mo_count += GetCount(m_controller3.Count, m_controller3, LeftRot);
                    mo_count += GetCount(m_controller4.Count, m_controller4, RightRot);

                    if (mo_count >= 1)
                    {
                        state = 2;//Detected
                        state_reading = 2;
                    }
                    else
                    {
                        state = 4;//Ignore
                        state_reading = 0;
                    }
                break;
            case 2:
                    if (state == 1)//PlayerInRange
                    { 
                        state = 0;//Patrol
                        state_reading = 0;
                    }

                    break;
            default:
                break;
            }
        
        }

        private int GetCount(int _limit, Queue<Vector3> _q, Vector3 v_tmp)
        {
            Vector3 tmp_II = v_tmp.normalized;

            for (int i = 0; i < _limit; i++)
            {
                Vector3 tmp_I = _q.Dequeue();

                Vector3 offset = tmp_I.normalized - tmp_II;
                float sqrLen = offset.sqrMagnitude;
                if (sqrLen > closeDistance * closeDistance)
                    mo_count++;
                tmp_II = tmp_I.normalized;
            }

            return mo_count;

        }

        private void Chasing()
        {

            agent.SetDestination(m_PlayerPosition);

            Move(speedRun);

            animator.GetComponent<Animator>().SetInteger("battle", 1);
            animator.GetComponent<Animator>().SetInteger("moving", 1);

            agent.destination = m_PlayerPosition;

            if (agent.remainingDistance <= 2.0f)
            {
                StartCoroutine("Attacking");
            }


        }

        private void i_Patroling()
        {
            playerLastPosition = Vector3.zero;

            Move(speedWalk);
            m_TimeToRotate -= Time.deltaTime;

            PF.PathUpdate();
            PF.PathSpeed(speedWalk);

        }

        private void Patroling()
        {
            playerLastPosition = Vector3.zero;

            Move(speedWalk);

            m_TimeToRotate -= Time.deltaTime;
            PF.PathSpeed(speedWalk);

            PF.PathUpdate();

        }

        void Move(float speed)
        {
            PF.PathSpeed(speed);

        }

        void CaughtPlayer()
        {
            //d(0);
        }

        void EnviromentView()
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

                        if (Vector3.Distance(transform.position, player.position) > (viewRadius + 1))
                        {
                            state = 4;//Ignore
                            break;
                        }
                        else
                        {
                            m_PlayerPosition = player.transform.position;
                            state = 1;//PlayerInRange
                            break;
                        }

                    }
                    else
                        state = 0;//Patrol

                }
            }
        }
    }
}
