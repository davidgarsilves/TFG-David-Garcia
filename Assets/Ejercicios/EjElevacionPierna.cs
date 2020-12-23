using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EjElevacionPierna : MonoBehaviour
{

    public GameObject esferaRodillaInicial;
    public GameObject esferaPieInicial;
    public GameObject esferaRodillaFinal;
    public GameObject esferaPieFinal;
    public Text textoRepeticiones;
    public Text textoPuntuacion;

    private Boolean nuevaRepeticion = false;
    private Boolean mediaRepeticion = false;
    private int repeticiones = 0;
    private int puntuacion = 0;

    // Start is called before the first frame update
    void Start()
    {
        esferaRodillaInicial = Instantiate(esferaRodillaInicial);
        esferaRodillaInicial.transform.position = new Vector3((float)-0.25, (float)0.8, (float)1.5);
        esferaPieInicial = Instantiate(esferaPieInicial);
        esferaPieInicial.transform.position = new Vector3((float)-0.83, (float)0.8, (float)1.5);
        esferaRodillaFinal = Instantiate(esferaRodillaFinal);
        esferaRodillaFinal.transform.position = new Vector3((float)-0.2, (float)1.2, (float)1.5);
        esferaRodillaFinal.GetComponent<SphereCollider>().enabled = false;
        esferaPieFinal = Instantiate(esferaPieFinal);
        esferaPieFinal.transform.position = new Vector3((float)-0.7, (float)1.4, (float)1.5);
        esferaPieFinal.GetComponent<SphereCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (esferaRodillaInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & esferaPieInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & mediaRepeticion == false)
        {
            mediaRepeticion = true;
            puntuacion = puntuacion + 50;
            textoPuntuacion.text = puntuacion.ToString();
            esferaRodillaFinal.GetComponent<SphereCollider>().enabled = true;
            esferaPieFinal.GetComponent<SphereCollider>().enabled = true;
        }

        if (esferaRodillaInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & esferaPieInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green &
            esferaRodillaFinal.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & esferaPieFinal.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & nuevaRepeticion == false)
        {
            nuevaRepeticion = true;
            repeticiones++;
            textoRepeticiones.text = repeticiones.ToString();
            puntuacion = puntuacion + 50;
            textoPuntuacion.text = puntuacion.ToString();
            Invoke("ReiniciarEsferas", 2.0f);
        }
    }

    void ReiniciarEsferas()
    {
        nuevaRepeticion = false;
        mediaRepeticion = false;
        esferaRodillaInicial.GetComponent<Renderer>().material.color = Color.white;
        esferaPieInicial.GetComponent<Renderer>().material.color = Color.white;
        esferaRodillaFinal.GetComponent<Renderer>().material.color = Color.white;
        esferaPieFinal.GetComponent<Renderer>().material.color = Color.white;
        esferaRodillaFinal.GetComponent<SphereCollider>().enabled = false;
        esferaPieFinal.GetComponent<SphereCollider>().enabled = false;
    }
}
