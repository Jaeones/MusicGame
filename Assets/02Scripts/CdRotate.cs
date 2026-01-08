using UnityEngine;

public class CdRotate : MonoBehaviour
{
    [SerializeField]
    float rotateSp = 60f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotateSp*Time.deltaTime);
    }
}
