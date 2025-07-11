using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingScript : MonoBehaviour
{
    [SerializeField] GameObject Cube; // The object to move
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        Cube.transform.position += new Vector3(1, 0, 1); // Set initial position of the cube
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        // Vector3.zero : Vector3(0,0,0)와 동일합니다.
        // Vector3.Dsitance(A,B) : A와 B의 사이의 거리를 반환합니다.
        Debug.Log($"거리는 : {distance}");
    }
}
