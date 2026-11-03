//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using System.Text.RegularExpressions;
//using UnityEngine.UI;

//public class Text_Change : TMP_Text
//{
//    /// 标记不换行的空格（换行空格Unicode编码为/u0020，不换行的/u00A0）
//    public static readonly string Non_breaking_space = "\u00A0";

//    /// 用于匹配标点符号（正则表达式）
//    private readonly string strPunctuation = @"\p{P}";

//    /// 用于存储text组件中的内容
//    private System.Text.StringBuilder TempText = null;

//    /// 用于存储text生成器中的内容
//    private IList<UILineInfo> TextLine;

//    //protected Text mytext;//Text组件
//    protected override void Awake()
//    {
//        base.Awake();
//        this.RegisterDirtyVerticesCallback(OnTextChange);
//    }
//    //解决换行空格问题
//    public void OnTextChange()
//    {
//        if (this.text.Contains(" "))
//        {
//            this.text = this.text.Replace(" ", Non_breaking_space);
//        }
//    }

//    protected override void OnPopulateMesh(VertexHelper toFill)
//    {
//        base.OnPopulateMesh(toFill);
//        StartCoroutine(ClearUpPunctuationMode(this));
//    }
//    //解决首字符出现标点问题
//    IEnumerator ClearUpPunctuationMode(TMP_Text _component)
//    {
//        //必须等当前帧跑完后，不然如果在运行text显示的时候对显示内容进行修改会报错
//        yield return new WaitForEndOfFrame();
//        //清除上一次添加的换行符号
//        _component.text = _component.text.Replace("\n", string.Empty);

//        Text text;
//        TextLine = _component.cachedTextGenerator.lines;
//        //需要改变的字符序号
//        int ChangeIndex = -1;
//        TempText = new System.Text.StringBuilder(_component.text);
//        for (int i = 1; i < TextLine.Count; i++)
//        {
//            //首位是否有标点
//            bool IsPunctuation = Regex.IsMatch(TempText[TextLine[i].startCharIdx].ToString(), strPunctuation);
//            //因为将换行空格都改成不换行空格后需要另外判断下如果首字符是不换行空格那么还是需要调整换行字符的下标
//            if (TempText[TextLine[i].startCharIdx].ToString() == Non_breaking_space)
//            {
//                IsPunctuation = true;
//            }

//            //没有标点就跳过本次循环
//            if (!IsPunctuation)
//            {
//                continue;
//            }
//            else
//            {
//                //有标点时保存当前下标
//                ChangeIndex = TextLine[i].startCharIdx;
//                //下面这个循环是为了判断当已经提前一个字符后当前这个的首字符还是标点时做的继续提前字符的处理
//                while (IsPunctuation)
//                {
//                    ChangeIndex = ChangeIndex - 1;
//                    if (ChangeIndex < 0) break;

//                    IsPunctuation = Regex.IsMatch(TempText[ChangeIndex].ToString(), strPunctuation);
//                    //因为将换行空格都改成不换行空格后需要另外判断下如果首字符是不换行空格那么还是需要调整换行字符的下标
//                    if (TempText[ChangeIndex].ToString() == Non_breaking_space)
//                    {
//                        IsPunctuation = true;
//                    }

//                }
//                if (ChangeIndex < 0) continue;

//                if (TempText[ChangeIndex - 1] != '\n')
//                    TempText.Insert(ChangeIndex, "\n");
//            }

//        }
//        //Debug.Log(TextLine.Count);
//        _component.text = TempText.ToString();

//    }
//}
