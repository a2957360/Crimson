using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : Singleton<CameraManager>
{
    #region Variables

    #region Components
    public Camera curcamera;
    #endregion

    #region Editor Variables
    public float _scrollingDistance = 2.5f;
    public float _cameraSpeed = 0.5f;
    #endregion

    #region Camera Variables

    public GameObject _targetToFollow;

    public GameObject targetCharacter;

    //[HideInInspector]
    public bool _canFreeMove = true;
    //[HideInInspector]
    public Vector2 _targetPos;
    [HideInInspector]
    public bool _fastMove = false;

    public Vector2 _camPos;
    Vector2 _offset;
    float _z;
    // camera size depending on screen size and aspect ratio
    public float _cameraHeight;
    public float _cameraWidth;

    float _mapWidth;
    float _mapHeight;

    float _mapCenterX;
    float _mapCenterY;

    float _cameraUpperY;
    float _cameraLowerY;
    float _cameraUpperX;
    float _cameraLowerX;

    int _borderWidth;

    GameObject CombatTargetposition;
    public float ZoomSpeed;
    public float Shakedis;
    float ZoomIntime;
    float ZoomInStoptime;
    float Shaketime;
    int shakenum = 1;
    float ZoomOutStoptime;
    float ZoomOuttime;
    public float ZoomSize;

    public bool showmap = false;
    public float zoomin = 2;
    public float zoomout = 2;
    #endregion
    #endregion

    #region Core GameFlow

    void Start()
    {
        curcamera = GetComponent<Camera>();

        _z = transform.position.z;
        _offset.Set(0.0f, 0.0f);

        _cameraHeight = GetComponent<Camera>().orthographicSize * 2;
        _cameraWidth = _cameraHeight * ((float)Screen.width / (float)Screen.height);
        _borderWidth = GameManager.Instance._borderWidth;
        _mapWidth = GameManager.Instance._mapWidth + _borderWidth;
        _mapHeight = GameManager.Instance._mapHeight + _borderWidth;
        _mapCenterX = GameManager.Instance._mapCenterX;
        _mapCenterY = GameManager.Instance._mapCenterY;

        _cameraUpperY = _mapCenterY + _mapHeight * 0.5f - _cameraHeight * 0.5f;
        _cameraLowerY = _mapCenterY - _mapHeight * 0.5f + _cameraHeight * 0.5f;
        _cameraUpperX = _mapCenterX + _mapWidth * 0.5f - _cameraWidth * 0.5f;
        _cameraLowerX = _mapCenterX - _mapWidth * 0.5f + _cameraWidth * 0.5f;
        ZoomSize = curcamera.orthographicSize;
        //AttackCameraMove();
    }

    void LateUpdate()
    {
        if (showmap != false)
        {
            ShowWholeMap();
        }
        if (CombatTargetposition != null)
        {
            AttackCameraMove();
        }
        else
        {
            MoveCamera();
        }
    }

    #endregion

    #region Functions

    public void MoveCamera()
    {
        if (targetCharacter != null)
        {
            _targetToFollow.transform.position = targetCharacter.transform.position;
        }
        if (_targetToFollow != null)
            _targetPos = _targetToFollow.transform.position;

        _camPos = transform.position;

        _targetPos.x = Mathf.Clamp(_targetPos.x, _cameraLowerX, _cameraUpperX);
        _targetPos.y = Mathf.Clamp(_targetPos.y, _cameraLowerY, _cameraUpperY);

        _camPos += 5.0f * (_targetPos - _camPos - _offset) * Time.deltaTime * _cameraSpeed;
        //// Hysteresis
        //if (_fastMove)
        //    _camPos += 5.0f * (_targetPos - _camPos - _offset) * Time.deltaTime;
        //else
        //    // regular lerp
        //    _camPos = Vector3.Lerp(_camPos, _targetPos, Time.deltaTime * 1.5f);
        transform.position = new Vector3(_camPos.x, _camPos.y, _z);
    }

    void AttackCameraMove()
    {
        if (CombatTargetposition != null)
        {
            if (ZoomIntime > 0)
            {
                //_camPos = transform.position;
                //Vector2 combattarget = CombatTargetposition.transform.position;
                //_camPos += 5.0f * (combattarget - _camPos - _offset) * Time.deltaTime * ZoomSpeed;
                //transform.position = new Vector3(_camPos.x, _camPos.y, _z);
                curcamera.orthographicSize -= ZoomSpeed * 2 * Time.deltaTime;

                ZoomIntime -= Time.deltaTime;
            }
            else if (ZoomInStoptime > 0)
            {
                ZoomInStoptime -= Time.deltaTime;
            }
            else if (Shaketime > 0)
            {
                shake();
                Shaketime -= Time.deltaTime;
            }
            else if (ZoomOuttime > 0)
            {
                //_camPos = transform.position;
                //Vector2 OriginalCamera = new Vector2(PlayerInputManager.Instance._cursorGridX, PlayerInputManager.Instance._cursorGridY);
                //_camPos += 5.0f * (OriginalCamera - _camPos - _offset) * Time.deltaTime * ZoomSpeed;
                //transform.position = new Vector3(_camPos.x, _camPos.y, _z);
                transform.position = new Vector3(CombatManager.Instance._BGImage.transform.position.x, CombatManager.Instance._BGImage.transform.position.y, transform.position.z);
                if (3 - curcamera.orthographicSize < 0.1f)
                {
                    curcamera.orthographicSize = 3;
                }
                else
                {
                    curcamera.orthographicSize += ZoomSpeed * Time.deltaTime;
                }

                ZoomOuttime -= Time.deltaTime;
            }
            else
            {
                ResetCombatAnimation();
            }

        }

    }

    void shake()
    {
        switch (shakenum)
        {
            case 1:
                transform.position += new Vector3(Shakedis, 0, 0);
                shakenum++;
                break;
            case 2:
                transform.position += new Vector3(Shakedis, 0, 0);
                shakenum++;
                break;
            case 3:
                transform.position += new Vector3(-Shakedis, 0, 0);
                shakenum++;
                break;
            case 4:
                transform.position += new Vector3(-Shakedis, 0, 0);
                shakenum++;
                break;
            case 5:
                transform.position += new Vector3(0, Shakedis, 0);
                shakenum++;
                break;
            case 6:
                transform.position += new Vector3(0, Shakedis, 0);
                shakenum++;
                break;
            case 7:
                transform.position += new Vector3(0, -Shakedis, 0);
                shakenum++;
                break;
            case 8:
                transform.position += new Vector3(0, -Shakedis, 0);
                shakenum = 1;
                break;
        }

    }

    public void StarCombatAnimation(GameObject defender, float time)
    {
        ZoomIntime = time * 0.1f;
        ZoomInStoptime = time * 0.25f;
        Shaketime = time * 0.35f;
        ZoomOuttime = time * 0.3f;
        CombatTargetposition = defender;
    }

    void ResetCombatAnimation()
    {
        CombatTargetposition = null;
        ZoomIntime = 0;
        ZoomInStoptime = 0;
        Shaketime = 0;
        ZoomOutStoptime = 0;
        ZoomOuttime = 0;
    }

    public void FollowTarget(GameObject target)
    {
        CancelInvoke("UnfollowTarget");
        targetCharacter = target;
        _canFreeMove = false;
        _fastMove = true;
    }

    public void FollowTargetByVector(Vector2 target)
    {
        CancelInvoke("UnfollowTarget");
        Destroy(GameObject.Find("New Game Object"));
        targetCharacter = new GameObject();
        targetCharacter.transform.position = target;
        PlayerInputManager.Instance._cursorGridX = (int)target.x;
        PlayerInputManager.Instance._cursorGridY = (int)target.y;
        _canFreeMove = false;
        _fastMove = true;
        Invoke("UnfollowTarget",1);
    }

    public void UnfollowTarget()
    {
        targetCharacter = null;
        _fastMove = false;
        _canFreeMove = true;
    }

    public void StarShowMap()
    {
        showmap = true;
    }

    public void ShowWholeMap()
    {
        int x = (int)((GameManager.Instance._mapWidth - 1) * 0.5f);
        int y = (int)((GameManager.Instance._mapHeight - 1) * 0.5f);

        FollowTargetByVector(new Vector2(x,y));

        while (zoomout > 0)
        {
            if (5 - curcamera.orthographicSize < 0.1f)
            {
                curcamera.orthographicSize = 5;
            }
            else
            {
                curcamera.orthographicSize += ZoomSpeed * 2 * Time.deltaTime;
            }
            zoomout -= Time.deltaTime;
            return;
        }
        while (zoomin > 0)
        {
            if (curcamera.orthographicSize - 3 < 0.1f)
            {
                curcamera.orthographicSize = 3;
            }
            else
            {
                curcamera.orthographicSize -= ZoomSpeed * Time.deltaTime;
            }

            zoomin -= Time.deltaTime;
            return;
        }
        FollowTargetByVector(GameManager.Instance._heros[0]._position);
        if (zoomin < 0 && zoomout < 0)
        {
            showmap = false;
            zoomout = 2;
            zoomin = 2;
        }
    }

    public void ChangeZoom()
    {

        if (ZoomSize < 5)
        {
            curcamera.orthographicSize += 1;
            ZoomSize++;
        }
        else
        {
            curcamera.orthographicSize = 3;
            ZoomSize = 3;
        }
        _cameraHeight = curcamera.orthographicSize * 2;
        _cameraWidth = _cameraHeight * ((float)Screen.width / (float)Screen.height);
        _cameraUpperY = _mapCenterY + _mapHeight * 0.5f - _cameraHeight * 0.5f;
        _cameraLowerY = _mapCenterY - _mapHeight * 0.5f + _cameraHeight * 0.5f;
        _cameraUpperX = _mapCenterX + _mapWidth * 0.5f - _cameraWidth * 0.5f;
        _cameraLowerX = _mapCenterX - _mapWidth * 0.5f + _cameraWidth * 0.5f;
        //CombatManager.Instance.setBGSize();
    }

        //public void MoveCameraUp()
        //{
        //    if (_canFreeMove)
        //    {
        //        _fastMove = false;
        //        _targetPos = new Vector2(transform.position.x, transform.position.y + _scrollingDistance);
        //    }
        //}

        //public void MoveCameraDown()
        //{
        //    if (_canFreeMove)
        //    {
        //        _targetPos = new Vector2(transform.position.x, transform.position.y - _scrollingDistance);
        //    }
        //}

        //public void MoveCameraLeft()
        //{
        //    if (_canFreeMove)
        //    {
        //        _targetPos = new Vector2(transform.position.x - _scrollingDistance, transform.position.y);
        //    }
        //}

        //public void MoveCameraRight()
        //{
        //    if (_canFreeMove)
        //    {
        //        _targetPos = new Vector2(transform.position.x + _scrollingDistance, transform.position.y);
        //    }
        //}

        #endregion

        #region Helpers

        #endregion
    }
