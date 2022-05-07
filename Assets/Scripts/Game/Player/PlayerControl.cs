using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private MovementCharacteristics _characteristics;
    [SerializeField] private FloatingJoystick _joystick;

    private float _vertical, _horizontal, _run;

    //private readonly string STR_VERTICAL = "Vertical";
    //private readonly string STR_HORIZONTAL = "Horizontal";
    private readonly string STR_SPEED = "isWalk";

    //private readonly string STR_RUN = "Run";
    private readonly string STR_JUMP = "Jump";



    private CharacterController _controller;
    private Animator _animator;

    private Vector3 _direction;
    private Quaternion _look;

    private Vector3 TargetRotate => _direction;
    private bool Idle => _horizontal == 0.0f && _vertical == 0.0f;


    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        Cursor.visible = _characteristics.VisibleCursor;
    }

    private void Update()
    {
        if (Input.touchSupported)
        {
            if (Input.touchCount == 1)
            {
                Movement();
                Rotate();
            }
        }
        else
        {
            Movement();
            Rotate();
        }
    }

    private void Movement()
    {
        if (_controller.isGrounded)
        {
            _horizontal = _joystick.Horizontal; //_horizontal = Input.GetAxis(STR_HORIZONTAL);
            _vertical = _joystick.Vertical; //_vertical = Input.GetAxis(STR_VERTICAL);


            //_run = Input.GetAxis(STR_RUN);

            _direction = _camera.TransformDirection(_horizontal, 0, _vertical).normalized;

            _animator.SetBool(STR_SPEED, _vertical != 0 || _horizontal != 0 );
            //PlayAnimation();
            Jump();
        }

        _direction.y -= _characteristics.Gravity * Time.deltaTime;

        float speed = /*_run **/ _characteristics.RunSpeed + _characteristics.MovementSpeed;
        Vector3 dir = _direction * speed * Time.deltaTime;

        dir.y = _direction.y;
        _controller.Move(dir);
    }

    private void Rotate()
    {
        if (Idle) return;

        Vector3 target = TargetRotate;
        target.y = 0;

        _look = Quaternion.LookRotation(target).normalized;

        float speed = _characteristics.AngularSpeed * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, _look, speed);

        //Debug.Log(_look.y);
        //_camera.rotation = Quaternion.RotateTowards(_camera.rotation, _look, speed);
    }

    private void Jump()
    {
        if (Input.GetButtonDown(STR_JUMP))
        {
            _animator.SetTrigger(STR_JUMP);
            _direction.y += _characteristics.JumpForce;
        }
    }
}
