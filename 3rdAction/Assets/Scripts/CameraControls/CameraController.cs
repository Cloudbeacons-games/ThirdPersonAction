using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region Fields
    float currentXAngle;
    public float currentYAngle;
    public bool isCameraLocked = false;
    

    [Range(0f, 90f)] public float upperVerticalLimit = 35f;
    [Range(0f, 90f)] public float lowerVerticalLimit = 35f;

    public float cameraSpeed = 50f;
    public bool smoothCameraRotation;
    [Range(1f, 50f)] public float cameraSmmothingFactor = 25f;
    private float cameraLockedRange = 15f;

    Transform cameraTransform;
    [SerializeField]Camera cam;
    [SerializeField] FindEnemyNearPlayer findEnemy;
    [SerializeField] InputReader input;
    [SerializeField] Image imgEnemyMarker;
    public LayerMask ignoreLayer;
    CountdownTimer searchForEnemy;

    private float searchTimer = 0.5f;
    private bool noEnemyMarked = true;

    Transform enemyToLook = null;
    Transform enemyToMark = null;

    #endregion


    public Vector3 GetUpDirection()=> cameraTransform.up;
    public Vector3 GetFacingDirection()=> cameraTransform.forward;

    private void Awake()
    {
        cameraTransform = transform;
        searchForEnemy = new CountdownTimer(searchTimer);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentXAngle = cameraTransform.localRotation.eulerAngles.x;
        currentYAngle = cameraTransform.localRotation.eulerAngles.y;
    }

    private void Start()
    {
        input.LockInCamera += HandleLockCamera;
        searchForEnemy.Start();
    }

    private void HandleLockCamera(bool isLockInPressed)
    {
        if (isLockInPressed && !isCameraLocked)
        {
            LockCameraToNearestTarget();
        }
        else if(isLockInPressed && isCameraLocked)
        {
            CenterCameraAfterUnlock();
            isCameraLocked = false;
        }
    }

    private void CenterCameraAfterUnlock()
    {
        currentXAngle = transform.localRotation.eulerAngles.x;
        currentYAngle = transform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        if (isCameraLocked && enemyToLook == null)
        {
            isCameraLocked = false;
            CenterCameraAfterUnlock();
        }
        
        if (!isCameraLocked)
            RotateCamera(input.LookDirection.x, -input.LookDirection.y);
        else
            RotateCameraLocked();

        if (searchForEnemy.IsFinished&&!isCameraLocked)
        {
            CheckForEnemyToMark();
        }

        if(!noEnemyMarked)
        {
            if(isCameraLocked)
                MarkEnemy(enemyToLook, isCameraLocked);
            else if(enemyToMark!=null)
                MarkEnemy(enemyToMark, isCameraLocked);
        }

        
    }

    private void CheckForEnemyToMark()
    {
        enemyToMark = findEnemy.FindCloseestEnemy();
        bool visible = enemyToMark != null ? CheckIfLineOfSight(enemyToMark):false ;

        if(enemyToMark != null&&visible)
        {
            imgEnemyMarker.enabled = true;
            noEnemyMarked = false;
        }
        else if(enemyToMark == null)
        {
            imgEnemyMarker.enabled = false;
        }


        searchForEnemy.Reset();
        searchForEnemy.Start();


    }

    private void MarkEnemy(Transform enemy,bool isLocked)
    {
        imgEnemyMarker.color = isLocked? Color.red : Color.white;
        Vector3 dir = enemy.position - transform.position;
        if (dir.magnitude<cameraLockedRange&& CheckIfLineOfSight(enemy))
            imgEnemyMarker.transform.position = cam.WorldToScreenPoint(enemy.position);
        else
        {
            imgEnemyMarker.enabled = false;
            noEnemyMarked = true;
        }
    }

    private void LockCameraToNearestTarget()
    {
        enemyToLook = findEnemy.FindCloseestEnemy();
        if(enemyToLook != null)
        {
            isCameraLocked = CheckIfLineOfSight(enemyToLook);
        }    
        
    }

    private void RotateCameraLocked()
    {
        bool Visible = CheckIfLineOfSight(enemyToLook);
        Vector3 dir = enemyToLook.position - transform.position;
        if (dir.magnitude > cameraLockedRange || !Visible)
        {
            currentXAngle = transform.localRotation.eulerAngles.x;
            currentYAngle = transform.localRotation.eulerAngles.y;
            isCameraLocked = false;
        }
            

        //cameraTransform.localRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.LookRotation(dir).eulerAngles;
        rotation.x = 12f;//Mathf.Clamp(rotation.x, -upperVerticalLimit, lowerVerticalLimit);

        cameraTransform.localRotation = Quaternion.Euler(rotation);
    }

    private bool CheckIfLineOfSight(Transform enemy)
    {
        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, (enemy.position - cameraTransform.position), out hit,100f, ~ignoreLayer);
        return hit.transform == enemy||hit.transform.tag == "Enemy";
    }

    private void RotateCamera(float horizontalInput, float verticalInput)
    {
        if(smoothCameraRotation)
        {
            horizontalInput = Mathf.Lerp(0, horizontalInput,Time.deltaTime*cameraSmmothingFactor);
            verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime*cameraSmmothingFactor);
        }

        currentXAngle += verticalInput * cameraSpeed * Time.deltaTime;
        currentYAngle += horizontalInput * cameraSpeed * Time.deltaTime;

        currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

        cameraTransform.localRotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);
    }
}
