using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject TeleportSpot;
    public bool left;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && TeleportSpot == null)
        {
            Debug.Log("you win!");
            FindAnyObjectByType<MapGenerator>().WinText.SetActive(true);
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = TeleportSpot.transform.position;
        }
    }
}
