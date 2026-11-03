using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class DetailInfo
//{
//    private Transform clone;
//    private ImageGet imageGet;
//    private Texture2D staticTex;
//    private RenderTexture dynamicTex;

//    public DetailInfo(GameObject obj, int wideth, int height, Vector3 staticCameraOffset, Vector3 dynamiCameraOffset)
//    {
//        Animator anim = obj.GetComponent<Animator>();
//        if(anim == null)
//        {
//            Debug.Log("该物体缺少动画组件");
//            return;
//        }

//        imageGet = new ImageGet(dynamiCameraOffset);
//        clone = GameObject.Instantiate(obj).transform;

//        staticTex = ImageGet.Photograph(clone, staticCameraOffset);
//        dynamicTex = new RenderTexture(wideth, height, -1);
//        IsUsing(false);
//    }

//    public Texture2D GetStaticTex()
//    {
//        return staticTex;
//    }

//    public RenderTexture GetDynamicTex()
//    {
//        //IsUsing(true);
//        //imageGet.GetDynamicTex(clone, dynamicTex);
//        //return dynamicTex;
//        return null;
//    }
//    public void DestoryCamera()
//    {
//        imageGet.DestoryCamera();
//    }

//    public void ReleaseResources()
//    {
//        IsUsing(false);
//    }

//    private void IsUsing(bool isUseing)
//    {
//        clone.gameObject.SetActive(isUseing);
//        imageGet.IsUsing(isUseing);
//    }
//}
