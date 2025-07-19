using UnityEngine;

public class camarafollow : MonoBehaviour
{
    public GameObject camarafolloe;
    public GameObject camara;
    public Vector3 offset;
    void LateUpdate()
    {
        camara.transform.position = camarafolloe.transform.position + offset ;
        
    }

}
