using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenConsole : MonoBehaviour
{

    private static ScreenConsole instance;

    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void Write(string text)
    {
        if (instance.textMesh == null)
        {
            instance.textMesh = instance.GetComponent<TextMeshProUGUI>();
        }
        instance.textMesh.text += text + "\n";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
