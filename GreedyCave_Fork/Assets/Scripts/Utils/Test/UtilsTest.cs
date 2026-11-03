using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilsTest : MonoBehaviour
{
    public List<GameObject> objs;

    private void Awake()
    {

        //MonoMgr.Instance.AddEvent(TestMore);

        //AudioMgr.Instance.Play("");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        }
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        if (Input.GetMouseButtonDown(2))
        {
            
        }
        
    }

    public void EnableFalse()
    {
        for (int i = 0; i < objs.Count; i++)
        {
        }
    }
    public void EnableTrue()
    {

    }
    public void TestMore()
    {
        Debug.Log("game");
    }

    public void TestMore(int a)
    {
        Debug.Log(a);
    }
    
}
