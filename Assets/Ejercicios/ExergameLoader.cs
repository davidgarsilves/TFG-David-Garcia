using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExergameLoader : MonoBehaviour
{
    public GameObject esfera;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoRepeticiones;
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoIncrementoPuntos;
    public Text textoMensaje;

    public AudioSource tocarEsfera;

    private static string articulacionPrincipal;
    private static float[] positionCamera;
    private static float[] rotationCamera;
    private static List<string> articulaciones;

    private int repeticiones = 0;
    private int puntuacion = 0;
    private float tiempo;
    private int esferasActivadas = 0;
    private Boolean repeticionCompleta = false;

    private const string exergameDataFileName = "manoArriba.json";
    private const string levelFileName = "manoArribaLvl1.json";
    private Exergame exergame = new Exergame(); 
    private ExergameLvl level = new ExergameLvl();
    private List<GameObject> posiciones = new List<GameObject>();


    void Awake()
    {
        string exergameFilePath = Path.Combine("Assets/Ejercicios/Exergames", exergameDataFileName);
        string levelFilePath = Path.Combine("Assets/Ejercicios/Exergames", levelFileName);
        
        if (File.Exists(exergameFilePath) & File.Exists(levelFilePath))
        {
            string dataExergameAsJson = File.ReadAllText(exergameFilePath);
            exergame = JsonUtility.FromJson<Exergame>(dataExergameAsJson);
            UnityEngine.Debug.Log("Successfully loaded data exergame file.");

            string dataLevelAsJson = File.ReadAllText(levelFilePath);
            level = JsonUtility.FromJson<ExergameLvl>(dataLevelAsJson);
            UnityEngine.Debug.Log("Successfully loaded data level file.");

            positionCamera = exergame.Camera_setup.Position;
            rotationCamera = exergame.Camera_setup.Rotation;
            articulacionPrincipal = level.Gameplay[0].Involved_joint;
            articulaciones = level.Gameplay[0].Joints;
            tiempo = level.Clock.Countdown;
            textoTiempo.text = tiempo.ToString();
            textoDescripcion.text = exergame.Description;

            for (int i = 0; i < level.Trajectories.Positions.Count; i++)
            {
                posiciones.Add(Instantiate(esfera));
                posiciones[i].transform.position = new Vector3(level.Trajectories.Positions[i].X, level.Trajectories.Positions[i].Y, level.Trajectories.Positions[i].Z);
                posiciones[i].GetComponent<SphereCollider>().enabled = false;
            }
            posiciones[0].GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            Debug.LogError("Cannot load exergame data!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempo >= 0 & repeticiones < level.Max_number_repetitions.Repetitions) { 
            tiempo -= Time.deltaTime;
            textoTiempo.text = tiempo.ToString("f0");
            textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
            Partida();
        }
        else
        {
            textoMensaje.text = "Juego terminado";

            for (int i = 0; i < posiciones.Count; i++)
                posiciones[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    void Partida()
    {
        if (posiciones[esferasActivadas].GetComponent<Renderer>().material.GetColor("_Color") == new Color(64f / 255f, 236f / 255f, 57f / 255f) & repeticionCompleta == false)
        {
            if (esferasActivadas == level.Trajectories.Positions.Count-1)
            {
                repeticiones += level.Gameplay[0].Repetition_increment;
                textoRepeticiones.text = repeticiones.ToString() + " / "+ level.Max_number_repetitions.Repetitions.ToString();
                IncrementarPuntuacion(exergame.Score.Activated);
                Invoke(nameof(ReiniciarEsferas), 2.0f);
                esferasActivadas = 0;
                repeticionCompleta = true;
            }
            else
            {
                esferasActivadas++;
                IncrementarPuntuacion(exergame.Score.Activated);
                posiciones[esferasActivadas].GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    void ReiniciarEsferas()
    {
        repeticionCompleta = false;
        for (int i = 0; i < posiciones.Count; i++)
        {
            posiciones[i].GetComponent<Renderer>().material.color = new Color(255f / 255f, 229f / 255f, 0f / 255f);
            posiciones[i].GetComponent<SphereCollider>().enabled = false;
        }
        posiciones[0].GetComponent<SphereCollider>().enabled = true;
    }

    void IncrementarPuntuacion(bool activated)
    {
        if (activated == true) {
            puntuacion += level.Gameplay[0].Score_increment;
            textoPuntuacion.text = puntuacion.ToString();

            textoIncrementoPuntos.text = "+ "+level.Gameplay[0].Score_increment.ToString();
            StartCoroutine(FadeTextToZeroAlpha(2f, textoIncrementoPuntos));
            tocarEsfera.Play();
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public static string getJoint()
    {
        return articulacionPrincipal;
    }

    public static float[] getPositionCamera()
    {
        return positionCamera;
    }

    public static float[] getRotationCamera()
    {
        return rotationCamera;
    }
    
    public static List<string> getJointList()
    {
        return articulaciones;
    }
}
