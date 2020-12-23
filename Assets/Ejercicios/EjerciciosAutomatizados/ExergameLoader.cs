using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ExergameLoader : MonoBehaviour
{
    public GameObject esferaInicial;
    public GameObject esferaFinal;
    public Text textoRepeticiones;
    public Text textoPuntuacion;
    public Text textoTiempo;
    public Text textoMensaje;
    public static string articulaciones; 
    public static float[] positionCamera;
    public static float[] rotationCamera;

    private Boolean nuevaRepeticion = false;
    private Boolean mediaRepeticion = false;
    private int repeticiones = 0;
    private int puntuacion = 0;
    private float tiempo;

    private const string gameDataFileName = "exergame.json";
    public Exergames exergame = new Exergames();
    
    void Awake()
    {
        string filePath = Path.Combine("Assets/Ejercicios/EjerciciosAutomatizados", gameDataFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            exergame = JsonUtility.FromJson<Exergames>(dataAsJson);
            UnityEngine.Debug.Log("Successfully loaded data exergame file.");

            positionCamera = exergame.Camera_setup.Position;
            rotationCamera = exergame.Camera_setup.Rotation;
            articulaciones = exergame.Gameplay[0].Involved_joint;
            tiempo = exergame.Clock.Countdown;
            textoTiempo.text = tiempo.ToString();

            esferaInicial = Instantiate(esferaInicial);
            esferaInicial.transform.position = new Vector3(exergame.Trajectories[0].Start_position[0], exergame.Trajectories[0].Start_position[1], exergame.Trajectories[0].Start_position[2]);
            esferaFinal = Instantiate(esferaFinal);
            esferaFinal.transform.position = new Vector3(exergame.Trajectories[0].End_position[0], exergame.Trajectories[0].End_position[1], exergame.Trajectories[0].End_position[2]);
            esferaFinal.GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            Debug.LogError("Cannot load exergame data!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempo >= 0 & repeticiones < exergame.Max_number_repetitions) { 
            tiempo -= Time.deltaTime;
            textoTiempo.text = tiempo.ToString("f0");
            Partida();
        }
        else
        {
            textoMensaje.text = "Juego terminado";
            esferaInicial.GetComponent<SphereCollider>().enabled = false;
            esferaFinal.GetComponent<SphereCollider>().enabled = false;
        }
    }

    void Partida()
    {
        if (esferaInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & mediaRepeticion == false)
        {
            mediaRepeticion = true;
            IncrementarPuntuacion(exergame.Score.Activated);
            esferaFinal.GetComponent<SphereCollider>().enabled = true;
        }

        if (esferaInicial.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & esferaFinal.GetComponent<Renderer>().material.GetColor("_Color") == Color.green & nuevaRepeticion == false)
        {
            nuevaRepeticion = true;
            repeticiones = repeticiones + exergame.Gameplay[0].Repetition_increment;
            textoRepeticiones.text = repeticiones.ToString();
            IncrementarPuntuacion(exergame.Score.Activated);
            Invoke("ReiniciarEsferas", 2.0f);
        }
    }

    void ReiniciarEsferas()
    {
        nuevaRepeticion = false;
        mediaRepeticion = false;
        esferaInicial.GetComponent<Renderer>().material.color = Color.white;
        esferaFinal.GetComponent<Renderer>().material.color = Color.white;
        esferaFinal.GetComponent<SphereCollider>().enabled = false;
    }

    void IncrementarPuntuacion(bool activated)
    {
        if (activated == true) {
            puntuacion = puntuacion + exergame.Gameplay[0].Score_increment;
            textoPuntuacion.text = puntuacion.ToString();
        }
    }

    public static string getJoint()
    {
        return articulaciones;
    }
    public static float[] getPositionCamera()
    {
        return positionCamera;
    }
    public static float[] getRotationCamera()
    {
        return rotationCamera;
    }
}

