using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    enum State
    {
        Patrol,
        Chase,
        Run
    }
    State _currentState;

    enum Direction
    {
        Up,
        Left,
        Down,
        Right,
        Empty
    }
    Direction _facing;

    Vector3 MoveVector
    {
        get
        {
            switch (_facing)
            {
                case Direction.Up:
                    _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
                    _rb.transform.eulerAngles = new Vector3(0, -90, 0);
                    _rb.freezeRotation = true;
                    return new Vector3(0, 0, _movementSpeed);

                case Direction.Right:
                    _rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
                    _rb.transform.eulerAngles = new Vector3(0, 0, 0);
                    _rb.freezeRotation = true;
                    return new Vector3(_movementSpeed, 0, 0);

                case Direction.Left:
                    _rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
                    _rb.transform.eulerAngles = new Vector3(0, 180, 0);
                    _rb.freezeRotation = true;
                    return new Vector3(_movementSpeed * -1, 0, 0);

                case Direction.Down:
                    _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
                    _rb.transform.eulerAngles = new Vector3(0, 90, 0);
                    _rb.freezeRotation = true;
                    return new Vector3(0, 0, _movementSpeed * -1);
            }
            return Vector3.forward;
        }
    }

    CharacterController _characterController;

    Collider _coll;

    Rigidbody _rb;

    Transform _player;

    [HideInInspector] public int GridLocationX = 0;
    [HideInInspector] public int GridLocationZ = 0;
    [HideInInspector] public int EnemyIndex = 0;

    [SerializeField] float _movementSpeed = 5;
    [SerializeField] float _movementSpeedModifier;

    bool[] _hitCheck = new bool[5];
    RaycastHit[] _hits = new RaycastHit[5];
    [SerializeField] float _hitDistance = 10;

    float _timer = 3;
    [SerializeField] const float _timerDuration = 3;

    [SerializeField] float _runAwayDuration = 8;

    Vector3 _destination;
    Vector3 _tempDestination = Vector3.zero;
    float _destinationTimer;
    [SerializeField] float _maxDestinationTimer = 3.0f;

    void Start()
    {
        _currentState = State.Patrol;

        _characterController = GetComponent<CharacterController>();

        _coll = GetComponent<Collider>();

        _rb = GetComponent<Rigidbody>();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        _destination = new Vector3(0, -4, 0);

        _destinationTimer = _maxDestinationTimer;

        PatrolUpdate();
    }

    void FixedUpdate()
    {
        if (GameBehavior.Instance.CurrentState == GameState.Running)
        {
            _hitCheck[0] = Physics.Raycast(transform.position, transform.TransformDirection(MoveVector), out _hits[0], _hitDistance);
            _hitCheck[1] = Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 22.5f, 0) * MoveVector), out _hits[1], _hitDistance);
            _hitCheck[2] = Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, 45, 0) * MoveVector), out _hits[2], _hitDistance);
            _hitCheck[3] = Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, -22.5f, 0) * MoveVector), out _hits[3], _hitDistance);
            _hitCheck[4] = Physics.Raycast(transform.position, transform.TransformDirection(Quaternion.Euler(0, -45, 0) * MoveVector), out _hits[4], _hitDistance);

            int hitCount = 0;
            if (_currentState == State.Patrol)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (_hitCheck[i] && _hits[i].transform.gameObject.CompareTag("Player"))
                    {
                        Debug.Log(EnemyIndex + ": Hit detetcted");
                        hitCount++;
                    }
                }
                if (hitCount > 0)
                {
                    Debug.Log(EnemyIndex + ": Chasing");
                    _currentState = State.Chase;
                    _movementSpeed += _movementSpeedModifier;
                }
            }
            else if (_currentState == State.Chase)
            {
                foreach (RaycastHit rh in _hits)
                {
                    if (rh.transform.gameObject.CompareTag("Player"))
                    {
                        Debug.Log(EnemyIndex + ": Hit detetcted");
                        hitCount++;
                    }
                }

                if (hitCount <= 0)
                    _timer -= Time.deltaTime;
                else
                    _timer = _timerDuration;

                if (_timer <= 0)
                {
                    Debug.Log(EnemyIndex + ": Chase ended");
                    _currentState = State.Patrol;
                    _movementSpeed -= _movementSpeedModifier;
                }
            }

            _characterController.Move(MoveVector * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destination) <= 0.2f)
            {
                PatrolUpdate();
            }
        }
    }

    public void Flash()
    {
        if(_currentState != State.Run)
        {
            StartCoroutine(RunAway());
            Debug.Log("Running away!");
        }
    }

    IEnumerator RunAway()
    {
        _currentState = State.Run;
        _coll.enabled = false;

        ReverseDirection();

        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(new Vector3(2, 1, 2), new Vector3(0.5f, 0.25f, 0.5f), i);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(_runAwayDuration);

        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.25f, 0.5f), new Vector3(2, 1, 2), i);
            yield return new WaitForEndOfFrame();
        }

        _coll.enabled = true;
        _currentState = State.Patrol;
    }

    void PatrolUpdate()
    {
        if (_currentState == State.Patrol)
            _facing = UpdateDirection();
        else if (_currentState == State.Chase)
            _facing = ChaseDirection();
        else if (_currentState == State.Run)
            _facing = RunDirection();

        if(EnemyIndex == 0)
            Debug.Log("Enemy " + EnemyIndex + ": New direction: " + _facing);

        switch (_facing)
        {
            case Direction.Up:
                GridLocationZ++;
                break;

            case Direction.Right:
                GridLocationX++;
                break;

            case Direction.Left:
                GridLocationX--;
                break;

            case Direction.Down:
                GridLocationZ--;
                break;
        }

        _destination = new Vector3(GridLocationX * 8, -4, GridLocationZ * 8);
    }

    void ReverseDirection()
    {
        switch (_facing)
        {
            case Direction.Up:
                _facing = Direction.Down;
                GridLocationZ--;
                break;

            case Direction.Left:
                _facing = Direction.Right;
                GridLocationX++;
                break;

            case Direction.Right:
                _facing = Direction.Left;
                GridLocationX--;
                break;

            case Direction.Down:
                _facing = Direction.Up;
                GridLocationZ++;
                break;
        }
        _destination = new Vector3(GridLocationX * 8, -4, GridLocationZ * 8);
    }

    Direction ChaseDirection()
    {
        MazeNode mn = MazeBehavior.Instance.Map[GridLocationX, GridLocationZ];

        Direction playerDirectionX = _player.transform.position.x > transform.position.x ? Direction.Right : Direction.Left;
        Direction playerDirectionZ = _player.transform.position.z > transform.position.z ? Direction.Up : Direction.Down;

        Direction priorityDirection = _player.transform.right.magnitude > _player.transform.up.magnitude ? playerDirectionX : playerDirectionZ;
        Direction secondaryDirection = priorityDirection == playerDirectionX ? playerDirectionZ : playerDirectionX;

        Debug.Log(EnemyIndex + ": Priority: " + priorityDirection + "\nSecondary: " + secondaryDirection);

        Direction tempDirection = BasicDirectionCheck(priorityDirection, mn);
        if (tempDirection != Direction.Empty)
            return tempDirection;

        tempDirection = BasicDirectionCheck(secondaryDirection, mn);
        if (tempDirection != Direction.Empty)
            return tempDirection;

        return UpdateDirection();
    }

    Direction RunDirection()
    {
        MazeNode mn = MazeBehavior.Instance.Map[GridLocationX, GridLocationZ];

        Direction playerDirectionX = _player.transform.position.x < transform.position.x ? Direction.Right : Direction.Left;
        Direction playerDirectionZ = _player.transform.position.z < transform.position.z ? Direction.Up : Direction.Down;

        Direction priorityDirection = _player.transform.right.magnitude < _player.transform.up.magnitude ? playerDirectionX : playerDirectionZ;
        Direction secondaryDirection = priorityDirection == playerDirectionX ? playerDirectionZ : playerDirectionX;

        Debug.Log(EnemyIndex + ": Priority: " + priorityDirection + "\nSecondary: " + secondaryDirection);

        Direction tempDirection = BasicDirectionCheck(priorityDirection, mn);
        if (tempDirection != Direction.Empty)
            return tempDirection;

        tempDirection = BasicDirectionCheck(secondaryDirection, mn);
        if (tempDirection != Direction.Empty)
            return tempDirection;

        return UpdateDirection();
    }

    Direction BasicDirectionCheck(Direction dir, MazeNode mn)
    {
        switch (dir)
        {
            case Direction.Up:
                if (mn.OpenUp)
                    return Direction.Up;
                break;

            case Direction.Left:
                if (mn.OpenLeft)
                    return Direction.Left;
                break;

            case Direction.Down:
                if (mn.OpenDown)
                    return Direction.Down;
                break;

            case Direction.Right:
                if (mn.OpenRight)
                    return Direction.Right;
                break;
        }

        return Direction.Empty;
    }

    //Tests each direction on the map to see where travel is possible
    //Prioritizes going forward, left, or right; only goes backwards if there's no other option
    Direction UpdateDirection()
    {
        Debug.Log("Enemy " + EnemyIndex + " Facing: " + _facing);
        Debug.Log("Current position: x " + GridLocationX + ", z: " + GridLocationZ);
        MazeNode mn = MazeBehavior.Instance.Map[GridLocationX, GridLocationZ];
        int LRUSeed = Random.Range(0, 3);
        switch (_facing)
        {
            case Direction.Up:
                for(int i = 0; i < 3; i++)
                {
                    if(LRUSeed == 0)
                    {
                        if (mn.OpenRight)
                            return Direction.Right;

                        LRUSeed++;
                    }
                    else if(LRUSeed == 1)
                    {
                        if (mn.OpenLeft)
                            return Direction.Left;

                        LRUSeed++;
                    }
                    else
                    {
                        if (mn.OpenUp)
                            return Direction.Up;

                        LRUSeed = 0;
                    }
                }
                return Direction.Down;

            case Direction.Right:
                for (int i = 0; i < 3; i++)
                {
                    if (LRUSeed == 0)
                    {
                        if (mn.OpenDown)
                            return Direction.Down;

                        LRUSeed++;
                    }
                    else if(LRUSeed == 1)
                    {
                        if (mn.OpenUp)
                            return Direction.Up;

                        LRUSeed++;
                    }
                    else
                    {
                        if (mn.OpenRight)
                            return Direction.Right;

                        LRUSeed = 0;
                    }
                }
                return Direction.Left;

            case Direction.Left:
                for (int i = 0; i < 3; i++)
                {
                    if (LRUSeed == 0)
                    {
                        if (mn.OpenUp)
                            return Direction.Up;

                        LRUSeed++;
                    }
                    else if(LRUSeed == 1)
                    {
                        if (mn.OpenDown)
                            return Direction.Down;

                        LRUSeed++;
                    }
                    else
                    {
                        if (mn.OpenLeft)
                            return Direction.Left;

                        LRUSeed = 0;
                    }
                }
                return Direction.Right;

            case Direction.Down:
                for (int i = 0; i < 3; i++)
                {
                    if (LRUSeed == 0)
                    {
                        if (mn.OpenLeft)
                            return Direction.Left;

                        LRUSeed++;
                    }
                    else if(LRUSeed == 1)
                    {
                        if (mn.OpenRight)
                            return Direction.Right;

                        LRUSeed++;
                    }
                    else
                    {
                        if (mn.OpenDown)
                            return Direction.Down;

                        LRUSeed = 0;
                    }
                }
                return Direction.Up;
        }
        return Direction.Up;
    }
}
