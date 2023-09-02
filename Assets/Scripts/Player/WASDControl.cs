using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDControl : MonoBehaviour
{
    Vector3 dir;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<HologramTransceiver>().isOwner) return;
        name = "Owner";

        dir.z = Input.GetKey(KeyCode.S) ? -1 : (Input.GetKey(KeyCode.W) ? 1 : 0);
        dir.x = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
    }

    private void FixedUpdate()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }
}
