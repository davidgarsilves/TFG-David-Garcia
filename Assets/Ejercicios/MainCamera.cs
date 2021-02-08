using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private float[] positionCamera;
    private float[] rotationCamera;

    // Start is called before the first frame update
    void Start()
    {
        positionCamera = ExergameLoader.getPositionCamera();
        rotationCamera = ExergameLoader.getRotationCamera();
        transform.position = new Vector3(positionCamera[0], positionCamera[1], positionCamera[2]);
        transform.rotation = Quaternion.Euler(rotationCamera[0], rotationCamera[1], rotationCamera[2]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
