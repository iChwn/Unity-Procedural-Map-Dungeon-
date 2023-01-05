using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject[] randomItems, randomEnemies, roundedEdges;
    public GameObject floorPrefab, wallPrefab, tilePrefab, exitPrefab;
    [HideInInspector] public float minX, maxX, minY, maxY;
    public int totalFloorCount;
    [Range(0, 100)] public int itemSpawnPercent;
    [Range(0, 100)] public int enemySpawnPercent;
    List<Vector3> floorList = new List<Vector3>();
    LayerMask floorMask, wallMask;
    public bool useRoundedEdges; 
    Vector2 hitSize;

    void Start() {
        Vector2 hitSize = Vector2.one * 0.8f;
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
        RandomWalker();
    }

    void Update() {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RandomWalker() {
        Vector3 currentPosition = Vector3.zero;
        while(floorList.Count < totalFloorCount) {
            switch(Random.Range(1,5)) {
                case 1: currentPosition += Vector3.up; break;
                case 2: currentPosition += Vector3.right; break;
                case 3: currentPosition += Vector3.down; break;
                case 4: currentPosition += Vector3.left; break;
            }

            bool inFloorList = false;
            for(int i = 0; i < floorList.Count; i++) {
                if(Vector3.Equals(currentPosition, floorList[i])) {
                    inFloorList = true;
                    break;
                }

            }
            if(!inFloorList) {
                floorList.Add(currentPosition);
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

        for(int x = (int)minX - 2; x <= (int)maxX + 2; x++) {
            for(int y = (int)minY - 2; y <= (int)maxY + 2; y++) {
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, floorMask);
                if(hitFloor) {
                    if(!Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 1])) {
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, floorMask);
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, floorMask);
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, floorMask);
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, floorMask);
                       
                        GenerateItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                        GenerateEnemies(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                    }
                }
                GenerateRoundedEdges(x, y);       
            }   
        }
    }

    void GenerateRoundedEdges(int x, int y) {
        if(useRoundedEdges) {
            Collider2D hitWall = Physics2D.OverlapBox(new Vector2(x, y), hitSize, 0, wallMask);
            Debug.Log(hitWall);
            if(hitWall) {
                Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1), hitSize, 0, wallMask);
                Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize, 0, wallMask);
                Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize, 0, wallMask);
                Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize, 0, wallMask);
                int bitVal = 0;
                if(!hitTop) bitVal += 1;
                if(!hitRight) bitVal += 2;
                if(!hitBottom) bitVal += 4;
                if(!hitLeft) bitVal += 8;
                if(bitVal > 0) {
                    GameObject goEdge = Instantiate(roundedEdges[bitVal], new Vector2(x, y), Quaternion.identity) as GameObject;
                    goEdge.name = roundedEdges[bitVal].name;
                    goEdge.transform.SetParent(hitWall.transform);
                }
            }
        }
    }

    void GenerateItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft) {
        if((hitTop || hitRight || hitBottom || hitLeft) && !(hitTop && hitBottom) && !(hitLeft && hitRight)) {
            int roll = Random.Range(1, 101);
            if(roll <= itemSpawnPercent) {
                int itemIndex = Random.Range(0, randomItems.Length);
                GameObject goItem = Instantiate(randomItems[itemIndex], hitFloor.transform.position, Quaternion.identity) as GameObject;
                goItem.name = randomItems[itemIndex].name;
                goItem.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void GenerateEnemies(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft) {
        if(hitTop && hitRight && hitBottom && hitLeft) {
            int roll = Random.Range(1, 101);
            if(roll <= enemySpawnPercent) {
                int enemyIndex = Random.Range(0, randomEnemies.Length);
                GameObject goEnemy = Instantiate(randomEnemies[enemyIndex], hitFloor.transform.position, Quaternion.identity) as GameObject;
                goEnemy.name = randomEnemies[enemyIndex].name;
                goEnemy.transform.SetParent(hitFloor.transform);
            }
        }
    }

    void ExitDoorway() {
        Vector3 doorPosition = floorList[floorList.Count - 1];
        GameObject goDoor = Instantiate(exitPrefab, doorPosition, Quaternion.identity) as GameObject;
        goDoor.name = exitPrefab.name;
        goDoor.transform.SetParent(transform);
    }
}
