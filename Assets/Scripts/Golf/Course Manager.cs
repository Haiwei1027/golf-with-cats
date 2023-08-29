using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseManager : MonoBehaviour
{
    [SerializeField] private ushort catPrefabId;
    [SerializeField] private ushort ballPrefabId;
    // Start is called before the first frame update
    void Start()
    {
        Resident.onStartGame += StartCourse;
    }

    public void StartCourse()
    {
        int colourId = Resident.Instance.record.ColourId;
        CatBallSpawn spawn = CatBallSpawn.GetSpawn(colourId);
        if (spawn == null) { Debug.LogError($"No Spawn for {colourId}"); return; }

        HologramSystem.Instantiate(catPrefabId, spawn.transform.position, spawn.transform.rotation);
        HologramSystem.Instantiate(ballPrefabId, spawn.transform.position+spawn.ballOffset, spawn.transform.rotation);

        HologramSystem.Instantiate(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
