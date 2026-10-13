using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    public static Manager _;
    public static Dictionary <string,GameObject> Prefabs
    {
        get { return _._Prefabs; }
        set { _._Prefabs = value; }
    }
    [SerializeField] public Dictionary <string, GameObject> _Prefabs;
    void Start()
    {
        _ = this;
    }

    void Update()
    {
        
    }
}
