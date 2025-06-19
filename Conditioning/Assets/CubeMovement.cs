using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 0.05f;
    public Vector3 startPosition = new Vector3(-0.2f, 0.1f, 0.5f);
    public Vector3 endPosition = new Vector3(0.2f, 0.1f, 0.5f);

    void Start()
    {
        StartCoroutine(MoveCube());
    }

    IEnumerator MoveCube()
    {
        while (true)
        {
            // move the object to position (0.012, 0.028, 0.377) in 1 second
            transform.position = Vector3.Lerp(transform.position, new Vector3(2.012f, 0.028f, 0.377f), Time.deltaTime * 1f);

            // wait for 1 second
            yield return new WaitForSeconds(1);

            // bring the cube back to original position saved in the start function
            //transform.position = startPosition;

            // wait for 1 second
            yield return new WaitForSeconds(1);

            // move the object to position (0.012, 0.028, 0.377) in 1 second
            transform.position = Vector3.Lerp(transform.position, new Vector3(1.12f, 0.028f, 0.377f), Time.deltaTime * 1f);

            // wait for 1 second before looping again
            yield return new WaitForSeconds(1);
        }
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 0.05f; // Speed of the cube's movement
    public float distance = 0.05f; // Distance of the cube's movement
    public Vector3 startPosition = new Vector3(-0.2,0.1,0.5); // Start position of the cube
    public Vector3 endPosition = new Vector3(0.2,0.1,0.5); // End position of the cube

    
    // Start is called before the first frame update
    void Start()
    {
        // save  the start position of the cube
        Vector3 Position = transform.position;
        //startPosition = Position;
        
    }

    // Update is called once per frame
    void Update()
    {   
        // calculate the 
        // move the object to position (0.012, 0.028, 0.377) in 1 second
        transform.position = Vector3.Lerp(transform.position, new Vector3(0.012f, 0.028f, 0.377f), Time.deltaTime * 1f);

        //wait for 1 second
        yield return new WaitForSeconds(1);

        //bring the cube back to original position saved in the start function
        //
        transform.position = startPosition;

        //wait for 1 second
        yield return new WaitForSeconds(1);

        // move the object to position (0.012, 0.028, 0.377) in 1 second
        transform.position = Vector3.Lerp(transform.position, new Vector3(0.212f, 0.028f, 0.377f), Time.deltaTime * 1f);

    }
}

*/
