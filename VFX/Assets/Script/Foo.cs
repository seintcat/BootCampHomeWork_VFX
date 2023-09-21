using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foo : MonoBehaviour
{
    public bool what;

    // Start is called before the first frame update
    void Start()
    {
        if(what)
        {
            Time.timeScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.deltaTime + ", " + Time.time);
    }
}
