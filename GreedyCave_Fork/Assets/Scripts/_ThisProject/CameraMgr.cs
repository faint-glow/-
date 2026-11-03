using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class CameraMgr : BaseManager<CameraMgr>
{
    private Camera _camera;
    private CinemachineBrain _cinemaChineBrain;//CinemachineBrain实际上是通过CinemachineVirtualCamera控制Camera，也就是说，如果没有Camera，就什么都没用
    private CinemachineVirtualCamera _mainVirtalCamera;//实际上是控制Camera，并不是真正的Camear

    //像这种CinemachineComponent有几个类型使用virtalCamera.AddCinemachineComponent就可以选择对应的CinemachineComponent
    //如果同时添加多个相同类型的，选择最后一次添加的生效,如果想换一个需要重新AddCinemachineComponent
    //建议把上一个DestroyCinemachineComponent,然后在Add
    private CinemachineFramingTransposer virtualCameraBody;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    public CinemachineBlendDefinition.Style defaultBlendType = CinemachineBlendDefinition.Style.EaseInOut;
    public CameraMgr()
    {
        GetCamera();
        GetCinemachineBrain();
        GetMainVirtalCamera();
        EventCenter.GetInstance().AddEventListener<Scene>(EventCenterEnum.切换场景后.ToString(),(scene)=>
        {
            //https://blog.csdn.net/qq_41807260/article/details/87343570 //相机渲染层级知识
            //这是主相机，不渲染小地图物体，下方代码是在原来的基础上，删除对SmallMapObject层级的渲染
            //GetCamera().cullingMask &= ~(1 << 7);
            GetCamera().cullingMask &= ~(1 << LayerMask.NameToLayer("SmallMapObject"));
        });
    }
    public Camera GetCamera()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        if (_camera != null)
        {
            PostProcessLayer postprocessLayer = _camera.GetComponent<PostProcessLayer>();
            if (postprocessLayer == null)
            {
                GameObject.Destroy(_camera.gameObject);
                _camera = null;
            }
        }
        if (_camera == null)
        {
            GameObject cameraObject = ResMgr.GetInstance().Load<GameObject>("Camera/Main Camera");//new GameObject("Main Camera");
            _camera = cameraObject.GetComponent<Camera>();
            _camera.tag = "MainCamera";//保证场景中有MainCamera
            _camera.cullingMask &=~( 1 << 7);
        }
        return _camera;
    }
    public CinemachineBrain GetCinemachineBrain()
    {
        _cinemaChineBrain = GetCamera().gameObject.GetComponent<CinemachineBrain>();
        if (_cinemaChineBrain == null)
            _cinemaChineBrain = GetCamera().gameObject.AddComponent<CinemachineBrain>();
        return _cinemaChineBrain;
    }
    public CinemachineVirtualCamera GetMainVirtalCamera()
    {
        _mainVirtalCamera = GetCinemachineBrain().ActiveVirtualCamera as CinemachineVirtualCamera;//如果场景中没有CinemachineBrain，虚拟相机是没有用的，所以获取虚拟相机，就必须要确保CinemachineBrain存在
        if (_mainVirtalCamera == null)
            _mainVirtalCamera = CreateNormalCamera(out virtualCameraBody);

        return _mainVirtalCamera;
    }
    public void Follow(GameObject obj)
    {
        GetMainVirtalCamera().Follow = obj.transform;
    }
    public void SeBlendCameraType(CinemachineBlendDefinition.Style type)
    {
        GetCinemachineBrain().m_DefaultBlend.m_Style = type;
    }
    public CinemachineVirtualCamera CreateNormalCamera(out CinemachineFramingTransposer virtualCameraBody)
    {
        GameObject virtalCameraObj = new GameObject("CM vcam");
        CinemachineVirtualCamera virtalCamera = virtalCameraObj.AddComponent<CinemachineVirtualCamera>();
        virtalCamera.transform.position = new Vector3(0, 0, -10);
        virtalCamera.Priority = 100;
        virtalCamera.m_Lens.OrthographicSize = 5;
        virtalCamera.m_Lens.NearClipPlane = 0.3f;
        virtalCamera.m_Lens.FarClipPlane = 1000;
        virtualCameraBody = virtalCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (virtualCameraBody == null)
            virtualCameraBody = virtalCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        virtualCameraBody.m_ScreenY = 0.6f;
        return virtalCamera;
    }

}
