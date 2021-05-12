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

    private const string exergameDataFileName = "elevacionManoDerecha.json";

    private Exergame exergame = new Exergame(); 
    private ExergameLvl level = new ExergameLvl();
    private List<GameObject> posiciones = new List<GameObject>();


    void Awake()
    {
        string exergameFilePath = Path.Combine("Assets/Ejercicios/Exergames", exergameDataFileName);
        
        if (File.Exists(exergameFilePath))
        {
            string dataExergameAsJson = File.ReadAllText(exergameFilePath);
            exergame = JsonUtility.FromJson<Exergame>(dataExergameAsJson);
            UnityEngine.Debug.Log("Successfully loaded data exergame file.");

            positionCamera = exergame.Camera_setup.Position;
            rotationCamera = exergame.Camera_setup.Rotation;
            textoDescripcion.text = exergame.Description;

            CargarDatosPartida(exergame.Name + "Lvl" + Math.Ceiling(Decimal.Divide(exergame.Levels, 2))+ ".json");
        }
        else
        {
            Debug.LogError("Cannot load exergame data!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempo >= 0 & repeticiones < level.Max_number_repetitions.Repetitions) 
        { 
            tiempo -= Time.deltaTime;
            textoTiempo.text = tiempo.ToString("f0");
            textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
            Partida();

            if (Input.GetKeyUp(KeyCode.UpArrow))
                CambiarDificultad("arriba");
            if (Input.GetKeyUp(KeyCode.DownArrow))
                CambiarDificultad("abajo");
        }
        else
        {
            textoMensaje.text = "Juego terminado";

            for (int i = 0; i < posiciones.Count; i++)
                posiciones[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    void CargarDatosPartida(String nivel)
    {
        string levelFileName = nivel;
        string levelFilePath = Path.Combine("Assets/Ejercicios/Exergames", levelFileName);
        if (File.Exists(levelFilePath))
        {
            string dataLevelAsJson = File.ReadAllText(levelFilePath);
            level = JsonUtility.FromJson<ExergameLvl>(dataLevelAsJson);
            UnityEngine.Debug.Log("Successfully loaded data level file.");

            articulacionPrincipal = level.Gameplay[0].Involved_joint;
            articulaciones = level.Gameplay[0].Joints;
            tiempo = level.Clock.Countdown;
            textoTiempo.text = tiempo.ToString();

            for (int i = 0; i < level.Trajectories.Positions.Count; i++)
            {
                posiciones.Add(Instantiate(esfera));
                posiciones[i].transform.position = new Vector3(level.Trajectories.Positions[i].X, level.Trajectories.Positions[i].Y, level.Trajectories.Positions[i].Z);
                posiciones[i].transform.localScale = new Vector3(level.Scale, level.Scale, level.Scale);
                posiciones[i].GetComponent<SphereCollider>().enabled = false;
            }
            posiciones[0].GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            Debug.LogError("Cannot load level data!");
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

    void CambiarDificultad(String tecla)
    {
        if (tecla.Equals("arriba"))
        {
            if (level.Level == exergame.Levels)
            {
                Debug.Log("no se puede subir mas la dificultad");
            }
            else
            {
                for (int i = 0; i < posiciones.Count; i++)
                    Destroy(posiciones[i]);

                posiciones.Clear();

                CargarDatosPartida(exergame.Name + "Lvl" + (level.Level + 1) + ".json");
                esferasActivadas = 0;
                puntuacion = 0;
                textoPuntuacion.text = puntuacion.ToString();
                repeticiones = 0;
                textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
            }
        }
        if (tecla.Equals("abajo"))
        {
            if (level.Level == 1)
            {
                Debug.Log("no se puede bajar mas la dificultad");
            }
            else
            {
                for (int i = 0; i < posiciones.Count; i++)
                    Destroy(posiciones[i]);

                posiciones.Clear();

                CargarDatosPartida(exergame.Name + "Lvl" + (level.Level - 1) + ".json");
                esferasActivadas = 0;
                puntuacion = 0;
                textoPuntuacion.text = puntuacion.ToString();
                repeticiones = 0;
                textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
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
