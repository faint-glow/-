using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothesController
{
    public static Vector3 AvataPhotographOffset = new Vector3(0, 0.75f, -1.2f);

    public static Vector3 BodyPhotographOffset = new Vector3(0, 1f, -2f);



    private IPlayerAble player;

    private AvatarImage avatarImage = new AvatarImage();
    private Dictionary<WearEquipType, SpriteRenderer> playerClothesDic = new Dictionary<WearEquipType, SpriteRenderer>();
    private Texture staticAvata;

    private Sprite defaultWeapon;
    public Sprite DefaultWeapon
    {
        get
        {
            if (defaultWeapon == null)
            {
                Sprite sprite = ResMgr.GetInstance().Load<Sprite>("Item/Equip/All_Sword_12");
                defaultWeapon = sprite;
            }
            return defaultWeapon;
        }
    }
    public PlayerClothesController(IPlayerAble player)
    {
        this.player = player;
        InitialPlayerClothes();
        EventCenter.GetInstance().AddEventListener<GameObject>(EventCenterEnum.´´½¨Íæ¼Ò.ToString(), (GameObject player) =>
        {
            InitialPlayerClothes();
        });
        
    }

    private void InitialPlayerClothes()
    {
        GameObject avatarObj = player.GetAnimator().gameObject;
        playerClothesDic[WearEquipType.Weapon] = avatarObj.transform.Find("LeftHand/Main/Weapon").GetComponent<SpriteRenderer>();

        WearAllClothes();
        Photograph();
    }

    private void WearAllClothes()
    {
        foreach (var item in playerClothesDic)
        {
            WearClothes(item.Key);
        }
    }

    public void WearClothes(WearEquipType equipType)
    {
        if (!playerClothesDic.ContainsKey(equipType) || playerClothesDic[equipType] == null)
            return;
        Equip equip = player.GetEquip(equipType);

        if (equip == null)
        {
            Undress(equipType);
            return;
        }
        playerClothesDic[equipType].sprite = equip.itemSprite;
        Photograph();
    }
    public void Undress(WearEquipType equipType)
    {
        if (!playerClothesDic.ContainsKey(equipType))
            return;
        playerClothesDic[equipType].sprite = DefaultWeapon;
        Photograph();
    }
    public Sprite GetDefaultClothes(WearEquipType equipType)
    {
        if (equipType == WearEquipType.Weapon)
            return DefaultWeapon;

        return null;
    }

    private void Photograph()
    {
        GameObject avatarObj = player.GetAnimator().gameObject;
        staticAvata = AvatarImage.GetStaticTex(avatarObj, AvataPhotographOffset);
    }
    public Texture GetStaticAvatar(bool isReBuild = false)
    {
        if(staticAvata == null || isReBuild == true)
            Photograph();
        return staticAvata;
    }
    public RenderTexture GetDynamicAvatar()
    {
        GameObject avatarObj = player.GetAnimator().gameObject;
        return avatarImage.GetDynamicTex(avatarObj, BodyPhotographOffset);
    }
    public void AvatarUsingEnd()
    {
        avatarImage.UsingEnd();
    }
}
