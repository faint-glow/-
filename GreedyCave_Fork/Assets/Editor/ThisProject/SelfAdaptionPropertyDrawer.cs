using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//继承此类的条件(如果出了问题，第一时间检查这里)
//编辑对象也就是类似于（[CustomPropertyDrawer(typeof(Effect))]）中Effect的位置的类必须包含一下几个变量，且访问权限和变量名，以及变量类型都必须一样
//编辑对象必须继承SelfAdaptationClassMgr，当然，由于内部实现，你也可以直接把SelfAdaptationClassMgr中的所有属性添加到目标类中，而不继承该类

//注：SerializedPropertyHeightInfo的内部结构为，且内部结构的各种东西都必须一样
//public class SelfAdaptationClassMgr
//{
//    public bool isInitial;
//    public bool isSpread;//是否展开
//    public float allControlsHeight;//其他控件
//    public List<SerializedPropertyHeightInfo> serializedPropertyHeightInfos;//属性
//}
public class SelfAdaptionPropertyDrawer : PropertyDrawer
{
    //如果被编辑的对象，在列表中出现多个，也就是元素数大于等于2
    //实测全局变量由这多个对象共同使用
    //那么为什么这两个属性可以在这里被共同使用呢？
    //OnGUI方法里都对这两个属性赋予自己的初始值，也就是说，每个对象使用这两个的时候，都是一个全新的值
    private Rect currentRect;
    private float offset_X;
    private float lastOffset_Y;
    private float lastPropertyHeight;
    private bool isHorzontal;
    protected SerializedProperty currentProperty;
    protected Dictionary<SerializedProperty, string> propertyHideDic;
    /// <summary>
    /// 使用此方法获取的Rect可以自动判断合理位置，并且可以在GetPropertyHeight()中返回合理的值，达到自适应
    /// </summary>
    /// <param name="targetProperty">需要放置的目标属性</param>
    /// <param name="offset_x">目标位置的x偏移，一般为0就是合理的，不用管</param>
    /// <param name="offset_y">目标位置的y偏移，一般为0就是合理的，不用管</param>
    /// <returns></returns>
    public Rect GetRectVertical(SerializedProperty targetProperty,float offset_x = 0, float offset_y = 0)
    {
        if (currentRect == null)
            currentRect = new Rect();

        if (this.isHorzontal == true)
        {
            currentRect.y += lastPropertyHeight + lastOffset_Y;
        }
        this.offset_X = 0;
        this.isHorzontal = false;//一旦使用数值方向的Rect，就代表上一次横向排列结束
        lastOffset_Y = offset_y;
        lastPropertyHeight = EditorGUI.GetPropertyHeight(targetProperty);

        Rect newRect = new Rect(currentRect);
        newRect.x += offset_x;
        newRect.y += 10 + lastOffset_Y;

        currentRect.y += 10 + lastPropertyHeight + lastOffset_Y;

        SerializedProperty serializedPropertyHeightInfos = currentProperty.FindPropertyRelative("serializedPropertyHeightInfos");
        serializedPropertyHeightInfos.arraySize += 1;//新增一个元素
        SerializedProperty element = serializedPropertyHeightInfos.GetArrayElementAtIndex(serializedPropertyHeightInfos.arraySize - 1);
        element.FindPropertyRelative("serializedPropertyName").stringValue = targetProperty.name;
        element.FindPropertyRelative("y").floatValue = offset_y;

        return newRect;
    }

    public Rect GetRectHorizontal(SerializedProperty targetProperty, float width, float offset_x = 0, float offset_y = 0)
    {
        //如果上一次是竖直方向的，那么这一次还是竖直方向，但是必须要设置宽度
        if (currentRect == null)
            currentRect = new Rect();
        if (isHorzontal == false)
        {
            Rect thisRect = GetRectVertical(targetProperty, 0, offset_y);
            this.currentRect.y -= (lastPropertyHeight + lastOffset_Y);

            this.isHorzontal = true;
            thisRect.width = width;
            thisRect.x += this.offset_X + offset_x;
            this.offset_X = width + offset_x  + 10 + GetLabelWidth();
            
            return thisRect;
        }
        Rect newRect = new Rect(currentRect);

        newRect.width = width;
        newRect.x += this.offset_X + offset_x;
        newRect.y += offset_y;
        this.offset_X += offset_x + width + 10 + GetLabelWidth();
        float currentPropertyHeight = EditorGUI.GetPropertyHeight(targetProperty);
        if (currentPropertyHeight > lastPropertyHeight)
        {
            currentRect.y += (currentPropertyHeight - lastPropertyHeight);//水平方向时，比较当前属性高度和上一个高度，选择最大的使用
            lastPropertyHeight = currentPropertyHeight;
        }
        if(offset_y > lastOffset_Y)
        {
            SerializedProperty serializedPropertyHeightInfos = currentProperty.FindPropertyRelative("serializedPropertyHeightInfos");
            SerializedProperty element = serializedPropertyHeightInfos.GetArrayElementAtIndex(serializedPropertyHeightInfos.arraySize - 1);
            element.FindPropertyRelative("serializedPropertyName").stringValue = targetProperty.name;
            element.FindPropertyRelative("y").floatValue = offset_y;
            lastOffset_Y = offset_y;
        }
        return newRect;
    }
    public Rect GetRectFreeVertical(float height, float offset_x = 0, float offset_y = 0)
    {
        if (currentRect == null)
            currentRect = new Rect();

        if (this.isHorzontal == true)
        {
            currentRect.y += lastPropertyHeight + lastOffset_Y;
        }
        this.offset_X = 0;
        this.isHorzontal = false;//一旦使用数值方向的Rect，就代表上一次横向排列结束
        lastOffset_Y = offset_y;
        lastPropertyHeight = height;

        Rect newRect = new Rect(currentRect);
        newRect.x += offset_x;
        newRect.y += 10 + lastOffset_Y;

        currentRect.y += 10 + lastPropertyHeight + lastOffset_Y;

        SerializedProperty allControlsHeight = currentProperty.FindPropertyRelative("allControlsHeight");
        allControlsHeight.floatValue += 10 + height;

        return newRect;
    }
    public Rect GetRectFreeHorizontal(float height, float width, float offset_x = 0, float offset_y = 0)
    {
        //如果上一次是竖直方向的，那么这一次还是竖直方向，但是必须要设置宽度
        if (currentRect == null)
            currentRect = new Rect();
        if (isHorzontal == false)
        {
            Rect thisRect = GetRectFreeVertical(height, 0, offset_y);
            this.currentRect.y -= (lastPropertyHeight + lastOffset_Y);

            this.isHorzontal = true;
            thisRect.width = width;
            thisRect.x += this.offset_X + offset_x;
            this.offset_X = width + offset_x + 10 + GetLabelWidth();

            return thisRect;
        }
        Rect newRect = new Rect(currentRect);

        newRect.width = width;
        newRect.x += this.offset_X + offset_x;
        newRect.y += offset_y;
        this.offset_X += offset_x + width + 10 + GetLabelWidth();
        float currentPropertyHeight = height;
        if (currentPropertyHeight > lastPropertyHeight)
        {
            currentRect.y += (currentPropertyHeight - lastPropertyHeight);//水平方向时，比较当前属性高度和上一个高度，选择最大的使用
            lastPropertyHeight = currentPropertyHeight;
        }
        if (offset_y > lastOffset_Y)
        {
            SerializedProperty allControlsHeight = currentProperty.FindPropertyRelative("allControlsHeight");
            allControlsHeight.floatValue += 10 + height - lastPropertyHeight;
            lastOffset_Y = offset_y;
        }
        return newRect;
    }

    public Rect GetRect_Height(float height, float y = 10)
    {
        currentRect.height = height;
        currentRect.y += y;
        Rect newRect = new Rect(currentRect);

        SerializedProperty allControlsHeight = currentProperty.FindPropertyRelative("allControlsHeight");
        allControlsHeight.floatValue += 10 + height;
        currentRect.y += height;
        return newRect;
    }

    //变量名拥有多少的宽度
    public virtual float GetLabelWidth()
    {
        return 150;
    }
    //折叠框的显示名字
    public virtual string GetSpreadName(SerializedProperty property)
    {
        return "重写GetSpreadName，可以修改";
    }
    //序列化的时候会调用无参构造函数，可以在无参构造函数赋予初始值，或者定义变量的时候设置初始值
    //但是，序列化在某些情况下，是没有办法让属性初始值为我们设定好的初始值
    //1.对象所属类没有无参构造函数，此时定义变量时设置的初始值就没用，在有参构造函数里设置初始值是无效的，因为根本不会被调用
    //2.对象存在于列表之中，当对象是列表元素的时候，随便你怎么搞，就是没有初始值(数组同理)
    //所以可以用这个方法来处理设置初始值，效果目前和包含无参构造函数一致
    //注意：如果对象是数组元素，那么新增数组元素的时候，新增的数组元素是完全复制上一个数组元素，包括已经初始化的标签
    //所以，新增的元素不会调用这个方法。不过无需担心，因为在游戏开发过程中，这样的效果反而是更棒的

    private void InitialPropertyHandler(SerializedProperty property)
    {
        property.FindPropertyRelative("isSpread").boolValue = true;
        InitialProperty(property);
    }
    public virtual void InitialProperty(SerializedProperty property)
    {

    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        currentProperty = property;
        offset_X = 0;
        lastOffset_Y = 0;
        isHorzontal = false;
        lastPropertyHeight = 0;
        SerializedProperty isInitial = currentProperty.FindPropertyRelative("isInitial");

        if (isInitial.boolValue == false)
        {
            InitialPropertyHandler(property);
            isInitial.boolValue = true;
        }
        SerializedProperty allControlsHeight = currentProperty.FindPropertyRelative("allControlsHeight");
        SerializedProperty serializedPropertyHeightInfos = currentProperty.FindPropertyRelative("serializedPropertyHeightInfos");
        SerializedProperty isSpread = currentProperty.FindPropertyRelative("isSpread");

        EditorGUIUtility.labelWidth = GetLabelWidth();
        position.height = EditorGUIUtility.singleLineHeight;
        currentRect = position;

        allControlsHeight.floatValue = 0;
        serializedPropertyHeightInfos.ClearArray();

        isSpread.boolValue = EditorGUI.Foldout(currentRect, isSpread.boolValue, GetSpreadName(currentProperty));
        currentRect.y += 10;
        propertyHideDic = new Dictionary<SerializedProperty, string>();
        if (isSpread.boolValue == true)
        {
            SelfAdaption_OnGUI(position, currentProperty, label);
        }
    }
    /// <summary>
    /// 此方法无内容物，可以直接删掉base.SelfAdaption_OnGUI(...)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public virtual void SelfAdaption_OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
    }

    /// <summary>
    /// 添加需要特殊处理的属性
    /// </summary>
    /// <param name="property">目标属性</param>
    /// <param name="content">如果值为"",则会隐藏该属性，如果不为"",则该属性显示的名字即为content</param>
    private void AddSpecialProperty(SerializedProperty property, string content = "")
    {
        if (propertyHideDic.ContainsKey(property) && propertyHideDic[property] == "")
            return;
        propertyHideDic[property] = content;
    }
    public void AddHideProperty(SerializedProperty property)
    {
        AddSpecialProperty(property, "");
    }
    public void ChangePropertyDisplayName(SerializedProperty property, string content)
    {
        AddSpecialProperty(property, content);
    }
    /// <summary>
    /// 显示一个自动计算位置并且判断是否隐藏和显示名字的属性
    /// </summary>
    /// <param name="property"></param>
    /// <param name="displayName">属性显示的名字,""为使用默认名字，</param>
    /// <param name="action">伴随显示，如果property隐藏，这个方法就不会执行</param>
    public void ShowPropertyVertical(SerializedProperty property,string displayName = "", float offset_X = 0, float offset_Y = 0, System.Action action = null)
    {
        if (propertyHideDic.ContainsKey(property))
        {
            if (propertyHideDic[property] == "")
                return;
            EditorGUI.PropertyField(GetRectVertical(property, offset_X, offset_Y), property, new GUIContent(propertyHideDic[property]), true);
            if (action != null)
                action.Invoke();
            return;
        }
        if(displayName != "")
            EditorGUI.PropertyField(GetRectVertical(property, offset_X, offset_Y), property,new GUIContent(displayName), true);
        else
            EditorGUI.PropertyField(GetRectVertical(property, offset_X, offset_Y), property, true);
        if (action != null)
            action.Invoke();
    }

    /// <summary>
    /// 显示一个自动计算位置并且判断是否隐藏和显示名字的属性
    /// </summary>
    /// <param name="property"></param>
    /// <param name="displayName">属性显示的名字,""为使用默认名字，</param>
    /// <param name="action">伴随显示，如果property隐藏，这个方法就不会执行</param>
    public void ShowPropertyHorizontal(SerializedProperty property,float width, string displayName = "",float offset_X = 0,float offset_Y = 0, System.Action action = null)
    {
        if (propertyHideDic.ContainsKey(property))
        {
            if (propertyHideDic[property] == "")
                return;
            EditorGUI.PropertyField(GetRectHorizontal(property, width, offset_X, offset_Y), property, new GUIContent(propertyHideDic[property]), true);
            if (action != null)
                action.Invoke();
            return;
        }
        if (displayName != "")
            EditorGUI.PropertyField(GetRectHorizontal(property, width, offset_X, offset_Y), property, new GUIContent(displayName), true);
        else
            EditorGUI.PropertyField(GetRectHorizontal(property, width, offset_X, offset_Y), property, true);
        if (action != null)
            action.Invoke();
    }

    public string ShowTextArea(string content, int height = 20)
    {
        Rect textAreaRect = GetRect_Height(height);
        EditorGUIUtility.AddCursorRect(textAreaRect, MouseCursor.Text);//鼠标悬停在textAreaRect这个范围时，变成文本类型的鼠标，也就是工字型
        return GUI.TextArea(textAreaRect, content);
    }

    public virtual float SelfAdaption_GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //SerializedProperty newPropertty = property.FindPropertyRelative("isSpread");
        if (property.FindPropertyRelative("isSpread").boolValue == false)
            return 20;
        float height = 0;
        SerializedProperty propertyInfoList = property.FindPropertyRelative("serializedPropertyHeightInfos");
        SerializedProperty allControlsHeight = property.FindPropertyRelative("allControlsHeight");

        //这里没有使用一个公共float,而且记录所有属性的名字重新计算高度的原因是：此处的value折叠时总是因为调用顺序无法刷新高度，这样可以解决
        //虽然看起来性能及其操蛋，但是实测不会有什么卡顿
        for (int i = 0; i < propertyInfoList.arraySize; i++)
        {
            SerializedProperty element = propertyInfoList.GetArrayElementAtIndex(i);
            string propertyName = element.FindPropertyRelative("serializedPropertyName").stringValue;
            float y = element.FindPropertyRelative("y").floatValue;

            SerializedProperty targetProperty = property.FindPropertyRelative(propertyName);
            height += 10 + EditorGUI.GetPropertyHeight(targetProperty) + y;
        }

        height += allControlsHeight.floatValue;
        height += 20;//这里必须要要加一点数值，否则直接体验来自unity的恶意。（一打开与此脚本有关的窗口，直接一直加载不出来）。
        height = Mathf.Clamp(height, 20, float.MaxValue);

        return height;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = SelfAdaption_GetPropertyHeight(property, label);
        return height;
    }
}
