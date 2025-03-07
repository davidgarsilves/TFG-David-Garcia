﻿using UnityEngine;

public class ObjetoMovil : MonoBehaviour
{
    private string joint;
    private GameObject articulacion = null;
    private static bool botellaAgarrada = false;

    // Start is called before the first frame update
    void Start()
    {
        joint = ExergameLoader.getJoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (botellaAgarrada == true)
            transform.position = articulacion.transform.position;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == joint)
        {
            articulacion = collision.gameObject;
            botellaAgarrada = true;
        }
    }

    public static void setBotellaAgarrada(bool agarrada)
    {
        botellaAgarrada = agarrada;
    }
}
