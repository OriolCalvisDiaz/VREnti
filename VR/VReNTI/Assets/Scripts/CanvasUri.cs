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
            m_controller1.Enqueue(LeftRot);
            m_controller2.Enqueue(RightPos);
            m_controller2.Enqueue(RightRot);
            m_controller3.Enqueue(HeadRot);
            m_controller4.Enqueue(User);

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

                    if (m_controller1.Count < 20 * 2)
                    {
                        break;
                    }
                    if (m_controller2.Count < 20 * 2)
                    {
                        break;
                    }

                }
            }
        }

        if (state_reading == 1)
        {

            Vector3 t_printLeft = printLeft;
            Vector3 t_printRight = printRight;

            Vector3 Limite = new Vector3(20f, 20f, 20f);


            if (m_controller1.Count >= 20 * 2)
            {
                for (int i = 0; i < m_controller1.Count; i++)
                {
                    Vector3 tmp_I = m_controller1.Dequeue();
                    printLeft = new Vector3(Mathf.Abs(printLeft.x - tmp_I.x), Mathf.Abs(printLeft.y - tmp_I.y), Mathf.Abs(printLeft.z - tmp_I.z));
                }
                if (m_controller2.Count >= 20 * 2)
                {
                    for (int i = 0; i < m_controller2.Count; i++)
                    {
                        Vector3 tmp_I = m_controller2.Dequeue();
                        printRight = new Vector3(Mathf.Abs(printRight.x - tmp_I.x), Mathf.Abs(printRight.y - tmp_I.y), Mathf.Abs(printRight.z - tmp_I.z));
                    }

                }
            }


            printLeft = new Vector3(Mathf.Clamp(printLeft.x, 0, 100), Mathf.Clamp(printLeft.y, 0, 100), Mathf.Clamp(printLeft.z, 0, 100));
            printRight = new Vector3(Mathf.Clamp(printRight.x, 0, 100), Mathf.Clamp(printRight.y, 0, 100), Mathf.Clamp(printRight.z, 0, 100));


            if (Mathf.Abs(printLeft.x - t_printLeft.x) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (Mathf.Abs(printLeft.y - t_printLeft.y) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (Mathf.Abs(printLeft.z - t_printLeft.z) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (Mathf.Abs(printRight.x - t_printRight.x) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (Mathf.Abs(printRight.y - t_printRight.y) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (Mathf.Abs(printRight.z - t_printRight.z) >= 1)
                mo_count += 2;
            else
                mo_count -= 1;

            if (mo_count < 0)
                mo_count = 0;

            if (mo_count < 0)
                mo_count = 0;

            t_Count.text = mo_count.ToString();
            t_Left.text = printLeft.x + " | " + printLeft.y + " | " + printLeft.z;
            t_Right.text = printRight.x + " | " + printRight.y + " | " + printRight.z;
            t_Head.text = printHead.x + " | " + printHead.y + " | " + printHead.z;
            t_Player.text = printUser.x + " | " + printUser.y + " | " + printUser.z;
        }

    }
}
