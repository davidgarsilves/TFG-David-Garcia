using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ExergameLoader : MonoBehaviour
{
    public GameObject esfera;
    public Text textoRepeticiones;
    public Text textoPuntuacion;
    public Text textoTiempo;
    public Text textoMensaje;

    private static string articulaciones;
    private static float[] positionCamera;
    private static float[] rotationCamera;

    private int repeticiones = 0;
    private int puntuacion = 0;
    private float tiempo;
    private int esferasActivadas = 0;
    private Boolean repeticionCompleta = false;

    private const string exergameDataFileName = "legRaise.json";
    private const string levelFileName = "legRaiseLvl1.json";
    private Exergame exergame = new Exergame(); 
    private ExergameLvl level = new ExergameLvl();
    private List<GameObject> posiciones = new List<GameObject>();

    void Awake()
    {
        string exergameFilePath = Path.Combine("Assets/Ejercicios", exergameDataFileName);
        string levelFilePath = Path.Combine("Assets/Ejercicios", levelFileName);
        
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
            articulaciones = level.Gameplay[0].Involved_joint;
            tiempo = level.Clock.Countdown;
            textoTiempo.text = tiempo.ToString();

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
        if (posiciones[esferasActivadas].GetComponent<Renderer>().material.GetColor("_Color") == Color.green & repeticionCompleta == false)
        {
            if (esferasActivadas == level.Trajectories.Positions.Count-1)
            {
                repeticiones += level.Gameplay[0].Repetition_increment;
                textoRepeticiones.text = repeticiones.ToString();
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
            posiciones[i].GetComponent<Renderer>().material.color = Color.white;
            posiciones[i].GetComponent<SphereCollider>().enabled = false;
        }
        posiciones[0].GetComponent<SphereCollider>().enabled = true;
    }

    void IncrementarPuntuacion(bool activated)
    {
        if (activated == true) {
            puntuacion += level.Gameplay[0].Score_increment;
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
