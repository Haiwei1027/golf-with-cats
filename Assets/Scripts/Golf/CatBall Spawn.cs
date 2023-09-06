using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that's responsible for holding data on where to spawn the cats and the balls
/// </summary>
public class CatBallSpawn : MonoBehaviour
{
    [SerializeField] private int colourId;
    public Vector3 ballOffset;

    private static Dictionary<int, CatBallSpawn> spawns;

    public static CatBallSpawn GetSpawn(int colourId)
    {
        return spawns[colourId];
    }

    private void Awake()
    {
        if (spawns == null) { spawns = new Dictionary<int, CatBallSpawn>(); }
        spawns.Add(colourId, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = PlayerColour.Get(colourId);

        Gizmos.DrawWireCube(transform.position + Vector3.up / 2f, Vector3.one);
        Gizmos.DrawWireSphere(transform.position + transform.TransformVector(ballOffset), 0.2f);
    }
}
