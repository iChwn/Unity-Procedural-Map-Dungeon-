using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject floorPrefab, wallPrefab, tilePrefab, exitPrefab;
    [HideInInspector] public float minX, maxX, minY, maxY;
    public int totalFloorCount;
    List<Vector3> floorList = new List<Vector3>();

    void Start() {
        RandomWalker();
    }

    void Update() {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RandomWalker() {
        Vector3 currentPossition = Vector3.zero;
        floorList.Add(currentPossition);
        while(floorList.Count < totalFloorCount) {
        // Debug.Log(floorList.Count);
            switch(Random.Range(1,5)) {
                case 1: currentPossition += Vector3.up; break;
                case 2: currentPossition += Vector3.right; break;
                case 3: currentPossition += Vector3.down; break;
                case 4: currentPossition += Vector3.left; break;
            }
            bool inFloorList = false;
            for(int i = 0; i > floorList.Count; i++) {
                if(Vector3.Equals(currentPossition, floorList[i])) {
                    inFloorList = true;
                    break;
                }
            }
            if(!inFloorList) {
                floorList.Add(currentPossition);
            }
        }
        for(int i = 0; i < floorList.Count; i++) {
            GameObject goTile = Instantiate(tilePrefab, floorList[i], Quaternion.identity) as GameObject;
            goTile.name = tilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        
        StartCoroutine(DelayProgress());
    }

    IEnumerator DelayProgress() {
        while(floorList.Count > totalFloorCount) {
            yield return null;
        }
        ExitDoorway();
    }

    void ExitDoorway() {
        Vector3 doorPossition = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPossition, Quaternion.identity) as GameObject;
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}
