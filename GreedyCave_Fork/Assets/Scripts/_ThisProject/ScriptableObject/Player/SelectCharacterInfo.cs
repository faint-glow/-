using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyDictionary/Player/SelectCharacterInfo", fileName = "NewSelectCharacterInfo")]
public class SelectCharacterInfo : ScriptableObject,IGetAble
{
    //不动数据
    public GameObject prefabs;
    public Gender gender;

    //动态数据，会变动，但是会临时获取，不需要保存也可以
    private AvatarImage avatarImage = new AvatarImage();
    public Texture GetStaticAvatar()
    {
        return AvatarImage.GetStaticTex(prefabs, new Vector3(0, 0.75f, -1.6f));
    }
}
