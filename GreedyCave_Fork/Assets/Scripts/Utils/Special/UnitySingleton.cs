using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySingleton<T> : MonoBehaviour where T : Component
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = (T)obj.AddComponent(typeof(T));
                    //obj.hideFlags = HideFlags.DontSave;
                    //GameObject.DestroyImmediate(obj);
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this as T;
        else
            Destroy(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        //GameObject.DestroyImmediate(obj);
    }
}
