using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseManager : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
