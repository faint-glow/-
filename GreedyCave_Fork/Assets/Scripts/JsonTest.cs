using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JsonTest : MonoBehaviour
{
    Coroutine coroutine;
    public void Start()
    {
        //coroutine = StartCoroutine(Aaa());
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Coroutine newCoroutine = StartCoroutine(Aaa());
        //    Debug.Log(coroutine == newCoroutine);
        //}
    }
    IEnumerator Aaa()
    {
        yield return null;
        Debug.Log("finish");
        coroutine = null;
        Debug.Log(coroutine);
    }

    public void Test()
    {
        TextAsset s = Resources.Load<TextAsset>("data");

        Json obj = JsonUtility.FromJson<Json>(s.text);
        Debug.Log(obj.manyClass[1].BagCounts[0].count);
    }
}
[System.Serializable]
public class Json
{
    public JsonClass[] manyClass;
}
[System.Serializable]
public class JsonClass
{
    public string PersonName;
    public int PersonAge;
    public JsonClassMore[] BagCounts;
    public int PersonSalary;
}
[System.Serializable]
public class JsonClassMore
{
    public int count;
}
