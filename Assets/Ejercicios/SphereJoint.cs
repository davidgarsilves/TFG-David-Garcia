using UnityEngine;

public class SphereJoint : MonoBehaviour
{
    private string joint;
    public GameObject explosion;

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
            if (GetComponent<Renderer>().material.color != new Color(64f / 255f, 236f / 255f, 57f / 255f))
            {
                GameObject particulasExplosion;
                particulasExplosion = Instantiate(explosion);
                particulasExplosion.transform.position = gameObject.transform.position;
                Destroy(particulasExplosion, 2.1f);
                GetComponent<Renderer>().material.color = new Color(64f / 255f, 236f / 255f, 57f / 255f);
            }
        }
    }

}
