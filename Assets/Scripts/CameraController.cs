using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineFreeLook freeLookCamera;
    PlayerController player;
    public float m_SplineCurvature;
    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        Singleton<CinemachineBrain>.Instance.m_ShowDebugText = true;
        CinemachineCore.GetInputAxis = GetAxisCustom;
        // Lưu lại góc quay ban đầu của camera
        startRotation = freeLookCamera.transform.rotation;
        
#if UNITY_EDITOR
        freeLookCamera.m_XAxis.m_MaxSpeed = 1000f;
        freeLookCamera.m_YAxis.m_MaxSpeed = 5f;
#elif UNITY_ANDROID || UNITY_IOS
freeLookCamera.m_XAxis.m_MaxSpeed = 150f;
freeLookCamera.m_YAxis.m_MaxSpeed = 1f;
#endif

    }

    public void SetFollowCharacter(Transform characterTransform)
    {
        freeLookCamera.Follow = characterTransform;
        freeLookCamera.LookAt = characterTransform;
        //targetLookAt = transformCamera;
        player = characterTransform.gameObject.GetComponent<PlayerController>();
        //endRotation = Quaternion.LookRotation(targetLookAt.position - freeLookCamera.transform.position);
    }
    public void RemoveFollowCharacter()
    {
        freeLookCamera.Follow = null;
    }

    public void MoveCameraUp()
    {
        freeLookCamera.m_YAxis.Value = Mathf.Lerp(freeLookCamera.m_YAxis.Value, 1, 0.5f * Time.deltaTime); //nâng cao camera khi xài skill
    }
    public float GetAxisCustom(string axisName)
    {

#if UNITY_EDITOR
        if (player == null) return 0f;
        if (axisName == "Mouse X")
        {
            if (Input.GetKey("mouse 0") && player.GetCurrentState() == 0
                && Input.mousePosition.x > Screen.width / 2)
            {
                return UnityEngine.Input.GetAxis("Mouse X");

            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetKey("mouse 0") && player.GetCurrentState() == 0
                && Input.mousePosition.x > Screen.width / 2)
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
#elif UNITY_ANDROID || UNITY_IOS
        
        if(player == null ||player.GetCurrentState() != 0) return 0f;
        int touchX; int touchY;
        for (int i = 0; i < Input.touchCount; i++)
        {
        Touch touch = Input.GetTouch(i);
        if (axisName == "Mouse X")
        {
            if (touch.position.x > Screen.width / 2)
            {
                 return touch.deltaPosition.x *0.1f;
            }
            else 
            {
                touchX=0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (touch.position.x > Screen.width / 2)
            {
                return touch.deltaPosition.y *0.1f;
            }
            else 
            {
                touchY=0;
            }
        }
        
        }
        return 0;
       
#elif UNITY_STANDALONE
        Debug.Log("Chạy trên PC (Windows, macOS, Linux)");
#else
        Debug.Log("Nền tảng khác");

#endif
    }

    public Transform targetLookAt;  // Tham chiếu đến đối tượng mà camera sẽ nhìn vào
    public float transitionTime = 0.01f;  // Thời gian chuyển đổi (s)

    private Quaternion startRotation;  // Vị trí quay ban đầu của camera
    private Quaternion endRotation;    // Vị trí quay mục tiêu của camera
    private float startTime;           // Thời gian bắt đầu chuyển đổi

    

    void Update()
    {
    }

    // Gọi hàm này khi muốn bắt đầu chuyển đổi
    public void StartTransition()
    {
        startTime = Time.time;  // Ghi lại thời gian bắt đầu
    }
}
