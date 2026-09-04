/******************************************************************************
 * Author: Brad Dixon
 * File Name: WaveData.cs
 * Creation Date: 9/3/2026
 * Last Modified: 9/3/2026
 * Brief: Data class that stores information for each wave
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveData
{
    public List<GameObject> enemyTypes = new List<GameObject>();
    public int enemyCount;
    public int pointCostToSpawn;
}
