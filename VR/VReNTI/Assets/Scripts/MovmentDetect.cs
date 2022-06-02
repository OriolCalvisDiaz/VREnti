using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathCreation.Examples
{
    public class MovmentDetect : MonoBehaviour
    {
        public PathCreator pathCreator00;
        public PathCreator pathCreator01;
        public Transform PJ;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //pathCreator01.path.SetPoint(1f).transform.position = new Vector3(PJ.position.x, PJ.position.y, PJ.position.z);
        }
    }
}