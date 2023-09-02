using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for orchestrating events on the course
/// </summary>
public class CourseManager : MonoBehaviour
{
    [SerializeField] private ushort catPrefabId;
    [SerializeField] private ushort ballPrefabId;

    [SerializeField] GameObject gameUI;

    // Start is called before the first frame update
    void Start()
    {
        gameUI.SetActive(false);
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

        gameUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
