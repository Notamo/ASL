using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;

namespace ASL.Manipulation.Controllers.PC
{
    public class Keyboard : MonoBehaviour
    {
        private MoveObject _moveBehavior;

        private void Awake()
        {
            _moveBehavior = gameObject.GetComponent<MoveObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.S))
            {
                MoveBehavior.Down();
            }
            if(Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.W))
            {
                MoveBehavior.Up();
            }
            if(Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.A))
            {
                MoveBehavior.Left();
            }
            if(Input.GetKey(KeyCode.RightArrow)
                || Input.GetKey(KeyCode.D))
            {
                MoveBehavior.Right();
            }

            if (Input.GetKey(KeyCode.R))
            {
                gameObject.GetComponent<CreateObject>().CreatePUNObject("Room2");
            }
        }

        public MoveObject MoveBehavior
        {
            get
            {
                return _moveBehavior;
            }
            set
            {
                if(value != null)
                {
                    _moveBehavior = value;
                }
            }
        }
    }
}