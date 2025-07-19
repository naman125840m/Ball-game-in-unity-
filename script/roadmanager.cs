using UnityEngine;

public class roadmanager : MonoBehaviour
{
    public GameObject roads;
    public float roadlenth;
    public float roadmax ;
   

    public void Start()

    {
        for (int i = 0; i < 2; i++)
        {
            GameObject road = Instantiate(roads);
            road.transform.position = new Vector3(-14, 0,roadlenth + roadmax * 106 );
           

            roadmax ++;
        }
    }
    public void roadspam()
    {
        GameObject road = Instantiate(roads);
        road.transform.position = new Vector3(-14, 0, roadlenth + roadmax * 106);


        roadmax++;
    }
}
