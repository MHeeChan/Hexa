using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
            testStart();
        
        if (Input.GetKeyUp(KeyCode.C))
            testClear();

        if (Input.GetKeyUp(KeyCode.K))
        {
            HexGrid.Instance.hexGrid[3][0].setBlockType(BlockType.Spinner);
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            Debug.Log("clear test");
            HexGrid.totalMission = 9;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("fail test");
            HexGrid.totalCount = 19;
        }
    }

    void testStart()
    {
        Debug.Log("testStart");
        foreach (var i in HexGrid.Instance.hexGrid )
        {
            foreach (var j in i)
            {
                BlockType randomType = (BlockType)Random.Range((int)BlockType.Blue, (int)BlockType.Purple + 1);
                j.setBlockType(randomType);
            }
        }
    }
    
    void testClear()
    {
        Debug.Log("testStart");
        foreach (var i in HexGrid.Instance.hexGrid )
        {
            foreach (var j in i)
            {
                j.setBlockType(BlockType.None);
            }
        }
    }

    void testHardCode()
    {
        // Debug.Log("testStart");
        // for (int i = 0; i < HexGrid.Instance.hexGrid.Count; i++)
        // {
        //     for (int j = 0; j < HexGrid.Instance.hexGrid[i].Count; j++)
        //     {
        //         HexGrid.Instance.hexGrid[i][j].setBlockType(BlockType.Blue);
        //     }
        // }
    }
}
