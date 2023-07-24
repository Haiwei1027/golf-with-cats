using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that handles sending synchronised object data
/// </summary>
public class HologramTransmitter : MonoBehaviour
{
    //Note: After research its better to send everything in a big tcp packet rather than lots of small ones as
    //TCP will automatically combine small packets and they will each have redundent headers which adds overhead.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
