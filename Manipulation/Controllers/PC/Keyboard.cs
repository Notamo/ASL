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
            if(Input.GetKeyDown(KeyCode.DownArrow)
                || Input.GetKeyDown(KeyCode.S))
            {
                MoveBehavior.Down();
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)
                || Input.GetKeyDown(KeyCode.W))
            {
                MoveBehavior.Up();
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)
                || Input.GetKeyDown(KeyCode.A))
            {
                MoveBehavior.Left();
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)
                || Input.GetKeyDown(KeyCode.D))
            {
                MoveBehavior.Right();
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