using UnityEngine;

public class ObjetoFijo : MonoBehaviour
{
    private string objetoMovil;

    // Start is called before the first frame update
    void Start()
    {
        objetoMovil = ExergameLoader.getObjetoMovil();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == objetoMovil+"(Clone)")
            ExergameLoader.setSoltar(true);    
    }
}
