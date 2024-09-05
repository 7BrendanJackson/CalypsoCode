using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//All the camera movement for the game
public class CameraMovement : MonoBehaviour
{
    Controls controls;

    [SerializeField]
    bool
        isRotating,
        panBorder;

    [SerializeField]
    float
        minMoveSpeed,
        maxMoveSpeed,
        sprintSpeedMod = 2,
        snapToFollowSpeed,
        panBorderThickness = 10,
        targetZoomAmount,
        currentZoomAmount,
        minZoomAmount = 100,
        maxZoomAmount = 1,
        zoomIncrement,
        zoomSpeed,
        rotateSensitivity,
        verticalMin,
        verticalMax;

    Vector2 moveDir;
    Transform parentTransform;

    //Don't think this will be needed in final build
    [SerializeField]
    Transform
        followTarget,
        debugFollowTarget;

    [SerializeField] AudioSource windSource;

    private void Awake()
    {
        //Init. Controls
        controls = new Controls();
    }

    //Bind Controls
    private void OnEnable()
    {
        controls.Enable();
        controls.Camera.RotateAround.performed += _ => StartCameraRotate();
        controls.Camera.RotateAround.canceled += _ => StopCameraRotate();
        controls.Camera.Zoom.performed += _ => SetZoom();
    }

    //Unbind Controls
    private void OnDisable()
    {
        controls.Disable();
        controls.Camera.RotateAround.performed -= _ => StartCameraRotate();
        controls.Camera.RotateAround.canceled -= _ => StopCameraRotate();
        controls.Camera.Zoom.performed -= _ => SetZoom();
    }

    void Start()
    {
        parentTransform = transform.parent;

        SetZoom();
    }

    void Update()
    {
        Move();
        GoToZoomLevel();
        RotateCamera();
        PanBorder();
    }

    private void LateUpdate()
    {
        //Follow or move to a specific target
        if (followTarget != null)
        {
            Vector3 targetPos = followTarget.position;
            parentTransform.position = Vector3.MoveTowards(parentTransform.position, new Vector3(targetPos.x, 0, targetPos.z), snapToFollowSpeed * Time.deltaTime);
        }
        //Plays a windy sound when the camera is high up in the sky
        windSource.volume = Mathf.Lerp(-1, .5f, transform.position.y / 20);
    }

    //Basic keyboard 2 axis movement
    private void Move()
    {
        if (controls.Camera.KeyboardMove.inProgress)
        {
            //Stop the camera following a target
            followTarget = null;

            moveDir = controls.Camera.KeyboardMove.ReadValue<Vector2>();

            //Move speed and direction based on sprint key (shift), directional buttons pushed, camera rotation and camera zoom level
            parentTransform.position += (controls.Camera.Sprint.inProgress ? sprintSpeedMod : 1)
                * Mathf.Lerp(minMoveSpeed, maxMoveSpeed, ZoomInverseLerp()) * Time.deltaTime *
                ((moveDir.x * parentTransform.right) + (moveDir.y * parentTransform.forward).normalized);
        }
    }

    //Inverse lerp for determining movement speed based on camera zoom level
    float ZoomInverseLerp()
    {
        Vector3 a = parentTransform.position + ((transform.position - parentTransform.position).normalized * minZoomAmount) - parentTransform.position;
        Vector3 b = transform.position - parentTransform.position;
        return Vector3.Dot(b, a) / Vector3.Dot(a, a);
    }


    //Used for middle mouse orbiting movement (Starting)
    void StartCameraRotate()
    {
        isRotating = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Used for middle mouse orbiting movement (Finishing)
    void StopCameraRotate()
    {
        isRotating = false;
        Cursor.lockState = CursorLockMode.None;
    }

    //Set the current zoom level based on scroll wheel
    void SetZoom()
    {
        float zoomInput = controls.Camera.Zoom.ReadValue<float>();

        if (zoomInput > 0) //Zoom in
        {
            targetZoomAmount -= zoomIncrement;

        }
        else if (zoomInput < 0) //Zoom out
        {
            targetZoomAmount += zoomIncrement;
        }
        //Sets the desired position to then be smoothly interpolated to in GoToZoomLevel()
        targetZoomAmount = Mathf.Clamp(targetZoomAmount, maxZoomAmount, minZoomAmount);
    }

    //Smoothly moves to the desired zoom level
    //Could switch later on to enumerator to avoid every frame
    void GoToZoomLevel()
    {
        if (currentZoomAmount != targetZoomAmount)
        {
            float travelDistance = Mathf.Clamp(Mathf.Abs(targetZoomAmount - currentZoomAmount), .1f, zoomSpeed);
            currentZoomAmount = Mathf.MoveTowards(currentZoomAmount, targetZoomAmount, zoomSpeed * Time.deltaTime * travelDistance);
            Vector3 maxZoomPos = parentTransform.position + ((transform.position - parentTransform.position).normalized * minZoomAmount);
            transform.position = Vector3.Lerp(parentTransform.position, maxZoomPos, currentZoomAmount / minZoomAmount);
        }
    }

    //Orbits the camera around the fulcrum
    void RotateCamera()
    {
        if (!isRotating) return;

        //X=Up/Down (pos,neg)
        //Y=Left/Right
        Vector2 rotateInput = controls.Camera.MouseMove.ReadValue<Vector2>();
        parentTransform.Rotate(new Vector3(0, rotateInput.x * Time.deltaTime * rotateSensitivity, 0), Space.World);
        float calcRot = transform.rotation.eulerAngles.x + -rotateInput.y * Time.deltaTime * rotateSensitivity;

        //Clamp Vertical Rotation
        if (calcRot > verticalMin && calcRot < verticalMax)
            transform.RotateAround(parentTransform.position, transform.right, -rotateInput.y * Time.deltaTime * rotateSensitivity);

        //Set final rotation
        transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, 0, 90), transform.eulerAngles.y, transform.eulerAngles.z);
    }


    //Pans the camera when the mouse is on the border
    //Need to check for window focus loss and disable this if true

    void PanBorder()
    {
        //Disables the code because this is a pain to deal with in the editor
        if (!panBorder) return;

        Vector2 mousePos = controls.Camera.MousePosition.ReadValue<Vector2>();
        Vector2 dir = Vector2.zero;

        //Allows for diagonal movement
        if (mousePos.x < panBorderThickness)  //Left
        {
            dir += Vector2.left;
        }
        if (mousePos.x > Screen.width - panBorderThickness) //Right
        {
            dir += Vector2.right;
        }
        if (mousePos.y < panBorderThickness) //Down
        {
            dir += Vector2.down;
        }
        if (mousePos.y > Screen.height - panBorderThickness) //Up
        {
            dir += Vector2.up;
        }

        if (dir != Vector2.zero)
        {
            //Pan in desired direction
            parentTransform.position += Mathf.Lerp(minMoveSpeed, maxMoveSpeed, ZoomInverseLerp()) * Time.deltaTime * ((dir.x * parentTransform.right) + (dir.y * parentTransform.forward).normalized);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Set Follow")]
    void DebugSetFollow()
    {
        SetFollowTarget(debugFollowTarget);
    }
    void SetFollowTarget(Transform toFollow)
    {
        followTarget = toFollow;
        if (toFollow.TryGetComponent<BuildingMaster>(out BuildingMaster bm))
        {
            targetZoomAmount = bm.followZoomAmount;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.parent.position, transform.position + (transform.position - transform.parent.position).normalized * minZoomAmount);
    }
#endif
}
