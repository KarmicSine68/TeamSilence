using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public int RoomCost;
    public int RotateAmount;
    public Door DoorForward;
    public Door DoorBackwards;
    public Door DoorOffshoot;
    public GameObject roomEntranceSpawn;
    public GameObject roomBacktrackSpawn;
    public GameObject roomBacktrackOffshootSpawn;
    public EnemySpawner RoomSpawner;
    [SerializeField] private int Points;
    [SerializeField] private int Waves;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEncounter()
    {
        if(RoomSpawner != null)
            RoomSpawner.StartEncounter(Points, Waves);
    }

}
