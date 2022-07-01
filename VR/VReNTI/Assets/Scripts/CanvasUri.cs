using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasUri : MonoBehaviour
{
    public Queue<Vector3> m_controller1 = new Queue<Vector3>();
    public Queue<Vector3> m_controller2 = new Queue<Vector3>();
    public Queue<Vector3> m_controller3 = new Queue<Vector3>();
    public Queue<Vector3> m_controller4 = new Queue<Vector3>();

    public int state = 0;
    public int state_reading = 0;

    public Text t_Left;
    public Text t_Right;
    public Text t_Head;
    public Text t_Player;
    public Text t_Count;

    public int mo_count;

    public GameObject Left;
    public GameObject Right;
    public GameObject Head;
    public Transform m_PlayerPosition;                       //  Last position of the player when the player is seen by the enemy

    Vector3 printLeft;// = Left.transform.localPosition;
    Vector3 printRight;// = Right.transform.localPosition;
    Vector3 printHead;// = Head.transform.localEulerAngles;
    Vector3 printUser;// = m_PlayerPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMovement();
    }
    void GetMovement()
    {
        if (state_reading == 0 || state_reading == 1)
        {
            Vector3 LeftPos = Left.transform.localPosition;
            Vector3 RightPos = Right.transform.localPosition;
            Vector3 LeftRot = Left.transform.localEulerAngles;
            Vector3 RightRot = Right.transform.localEulerAngles;
            Vector3 HeadRot = Head.transform.localEulerAngles;
            Vector3 User = m_PlayerPosition.position;

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
                for (int i = 0; i < 1000; i++)
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
        }

        if (state_reading == 1)
        {

            Vector3 t_printLeft = Left.transform.localPosition;
            Vector3 t_printRight = Right.transform.localPosition;
            Vector3 t_LeftRot = Left.transform.localEulerAngles;
            Vector3 t_RightRot = Right.transform.localEulerAngles;

            Vector3 Limite = new Vector3(20f, 20f, 20f);

            if (m_controller1.Count >= 20 * 2)
            {
                for (int i = 0; i < m_controller1.Count; i++)
                {
                    Vector3 tmp_I = m_controller1.Dequeue();
                    bool closeEnough = Mathf.Approximately(t_printLeft.x, tmp_I.x)
                        && Mathf.Approximately(t_printLeft.y, tmp_I.y)
                        && Mathf.Approximately(t_printLeft.z, tmp_I.z);
                    if (!closeEnough)
                        mo_count++;
 
                }

            }
            if (m_controller2.Count >= 20 * 2)
            {
                for (int i = 0; i < m_controller2.Count; i++)
                {
                    Vector3 tmp_I = m_controller2.Dequeue();
                    bool closeEnough = Mathf.Approximately(t_printRight.x, tmp_I.x)
                        && Mathf.Approximately(t_printRight.y, tmp_I.y)
                        && Mathf.Approximately(t_printRight.z, tmp_I.z);
                    if (!closeEnough)
                        mo_count++;
                }

            }
            if (m_controller3.Count >= 20 * 2)
            {
                for (int i = 0; i < m_controller3.Count; i++)
                {
                    Vector3 tmp_I = m_controller3.Dequeue();
                    bool closeEnough = Mathf.Approximately(t_LeftRot.x, tmp_I.x)
                        && Mathf.Approximately(t_LeftRot.y, tmp_I.y)
                        && Mathf.Approximately(t_LeftRot.z, tmp_I.z);
                    if (!closeEnough)
                        mo_count++;

                }

            }
            if (m_controller4.Count >= 20 * 2)
            {
                for (int i = 0; i < m_controller4.Count; i++)
                {
                    Vector3 tmp_I = m_controller4.Dequeue();
                    bool closeEnough = Mathf.Approximately(t_RightRot.x, tmp_I.x)
                        && Mathf.Approximately(t_RightRot.y, tmp_I.y)
                        && Mathf.Approximately(t_RightRot.z, tmp_I.z);
                    if (!closeEnough)
                        mo_count++;

                }

            }


            if (mo_count >= 1)
            {
                state = 2;
            }
            else
            {
                state = 0;
                //StartCoroutine(DisguideReset());
            }

            t_Count.text = mo_count.ToString();
            t_Left.text = t_printLeft.x + " | " + t_printLeft.y + " | " + t_printLeft.z;
            t_Right.text = t_printRight.x + " | " + t_printRight.y + " | " + t_printRight.z;
            t_Head.text = t_LeftRot.x + " | " + t_LeftRot.y + " | " + t_LeftRot.z;
            t_Player.text = t_RightRot.x + " | " + t_RightRot.y + " | " + t_RightRot.z;
        }

    }
}
