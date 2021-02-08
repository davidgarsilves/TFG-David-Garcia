using UnityEngine;

public class SphereJoint : MonoBehaviour
{
    private string joint;

    // Start is called before the first frame update
    void Start()
    {
        joint = ExergameLoader.getJoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == joint)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
    }

}
