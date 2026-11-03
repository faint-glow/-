using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleValuePanel : MapPanel
{
    public static string prefabsPath = "WorldUI/BattleValuePanel";
    private TMP_Text valueText;
    public override void ShowMeFirst()
    {
        valueText = GetPanel("Text (TMP)").GetUI<TMP_Text>();
    }

    public void ShowDamageValue(float value,int dir,Color color,Vector3 targetPos,int textSize = 36)
    {
        string valueStr = "";
        if (value >= 1 || value <= -1)
            valueStr = ((int)value).ToString();
        else
            valueStr = value.ToString("0.0");
        valueText.text = valueStr;
        transform.position = targetPos;
        valueText.transform.localPosition = new Vector3(0, 0, 0);
        valueText.color = color;
        valueText.fontSize = textSize;
        StartCoroutine(DamageValueAnim(valueText.transform, dir, HidePanel,300));//世界UI缩小一百倍，这里进行速度补偿
    }

    public void ShowAddStateValue(float value, Color color, Vector3 targetPos)
    {
        string valueStr = "";
        if (value >= 1 || value <= -1)
            valueStr = ((int)value).ToString();
        else
            valueStr = value.ToString("0.0");//保留一位小数，小数点后面有几个0，就保留几位
        valueText.text = valueStr;
        transform.position = targetPos;
        valueText.transform.localPosition = new Vector3(0, 0, 0);
        valueText.color = color;
        StartCoroutine(AddBloodValueAnim(valueText.transform, HidePanel, 300));//世界UI缩小一百倍，这里进行速度补偿
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="trans">谁进行动画</param>
    /// <param name="action">动画结束的方法</param>
    /// <param name="speed">动画进行的速度</param>
    /// <param name="deltaX">x向基础值</param>
    /// <param name="deltaY">y向基础值</param>
    /// <param name="attenX">x向衰减因子</param>
    /// <param name="attenY">y向衰减因子</param>
    /// <returns></returns>
    IEnumerator DamageValueAnim(Transform trans,int dir,Action action = null,float speed = 3,float deltaX = 30f, float deltaY = 50f, float attenX = 0.4f, float attenY = 0.5f)
    {
        Vector3 target = new Vector3(0, 0, 0);
        deltaX = deltaX * dir;
        for (int i = 0; i < 6; i++)
        {
            target.x = target.x + deltaX;
            target.y = deltaY;

            if (i % 2 != 0)
                target.y = 0;

            while (true)
            {
                if (MoveToWorld(trans, target, speed) == true)
                    break;
                yield return null;
            }

            if (i % 3 == 0)
            {
                deltaX = deltaX * (1 - attenX);
                deltaY = deltaY * (1 - attenY);
            }
        }
        yield return new WaitForSeconds(0.1f);
        action();

    }
    IEnumerator AddBloodValueAnim(Transform trans, Action action = null, float speed = 3)
    {
        Vector3 targetPos = new Vector3(0, 80, 0);
        while (true)
        {
            if(MoveToWorld(trans, targetPos, speed))
                break;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        action();
    }
    public static bool MoveToWorld(Transform targetTrans, Vector3 targteLocalPos,float speed)
    {
        targetTrans.localPosition = Vector3.MoveTowards(targetTrans.localPosition, targteLocalPos, Time.deltaTime * speed);
        if (targetTrans.localPosition == targteLocalPos)
            return true;
        return false;
    }
    public void HidePanel()
    {
        WorldUIManager.GetInstance().HideWorldPoorPanel("WorldUI/BattleValuePanel", this);
    }
}
