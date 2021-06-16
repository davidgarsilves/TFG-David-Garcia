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
    public TextMeshProUGUI textoCambioDificultad;
    public Image postura;
    public GameObject textoEscena;
    public Image preparacionEjercicio;
    public TextMeshProUGUI textoNombre;
    public Image preparacionPostura;
    public TextMeshProUGUI textoPreparacionMensaje;
    public TextMeshProUGUI textoTiempoPreparacion;
    public TextMeshProUGUI textoMensajeFinal;
    

    public AudioSource tocarEsfera;
    public AudioSource sonidoCuentaAtras;
    public AudioSource sonidoCuentaAtrasYa;

    private static string articulacionPrincipal;
    private static float[] positionCamera;
    private static float[] rotationCamera;
    private static List<string> articulaciones;

    private int repeticiones = 0;
    private int puntuacion = 0;
    private float tiempoJuego;
    private int esferasActivadas = 0;
    private Boolean repeticionCompleta = false;
    private float tiempo; //nuevooooooooooooooo
    private float tiempoComiemzo = 10f;

    private const string exergameDataFileName = "Elevacion de mano derecha.json";

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

            textoNombre.text = exergame.Name;
            preparacionPostura.GetComponent<Image>().sprite = Resources.Load<Sprite>("normal");
            textoPreparacionMensaje.text = "Imita la postura de la imagen para realizar el ejercicio";

            positionCamera = exergame.Camera_setup.Position;
            rotationCamera = exergame.Camera_setup.Rotation;
            
            textoDescripcion.text = exergame.Description;
            postura.GetComponent<Image>().sprite = Resources.Load<Sprite>("normal"); // si quiero que tenga los bordes redondeados recortarlos pero luego mantener la forma cuadrada, igual que el circulo cuando cambias el script

            CargarDatosPartida(exergame.Name + " Lvl " + Math.Ceiling(Decimal.Divide(exergame.Levels, 2))+ ".json"); //posiblemente haya que hacer dos metodos, este sea el de cargar los datos del nive
                                                                                                                   //y en un metodo anterior cargar los datos de la partida junton con la posicion de la camara y la foto de la postura
            postura.enabled = false;
            textoDescripcion.enabled = false;
            textoRepeticiones.enabled = false;
            textoPuntuacion.enabled = false;
            textoTiempo.enabled = false;
            tiempoJuego += tiempoComiemzo;
            for (int i = 0; i < textoEscena.transform.childCount; i++)
                textoEscena.transform.GetChild(i).gameObject.SetActive(false);
            Destroy(preparacionEjercicio, tiempoComiemzo);
            for (int i = 0; i < preparacionEjercicio.transform.childCount; i++)
                Destroy(preparacionEjercicio.transform.GetChild(i).gameObject, tiempoComiemzo);

            tiempo = Time.time;

        }
        else
        {
            Debug.LogError("Cannot load exergame data!");
        }
    }



    // Update is called once per frame
    void Update()
    {
        ContadorPreparacionEjercicio();

        if (Time.time > tiempo + tiempoComiemzo)
        {
            postura.enabled = true;
            textoDescripcion.enabled = true;
            textoRepeticiones.enabled = true;
            textoPuntuacion.enabled = true;
            textoTiempo.enabled = true;
           
            for (int i = 0; i < textoEscena.transform.childCount; i++)
                textoEscena.transform.GetChild(i).gameObject.SetActive(true);
            //textoTiempo.enabled = true;
            posiciones[0].GetComponent<SphereCollider>().enabled = true;
        }

        if (tiempoJuego >= 0 & repeticiones < level.Max_number_repetitions.Repetitions) 
        {
            tiempoJuego -= Time.deltaTime;
            textoTiempo.text = tiempoJuego.ToString("f0");
            textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
            Partida();

            if (Input.GetKeyUp(KeyCode.UpArrow))
                CambiarDificultad("arriba");
            if (Input.GetKeyUp(KeyCode.DownArrow))
                CambiarDificultad("abajo");
        }
        else
        {
            textoMensajeFinal.text = "Partida terminada";

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
            tiempoJuego = level.Clock.Countdown;
            textoTiempo.text = tiempoJuego.ToString();

            for (int i = 0; i < level.Trajectories.Positions.Count; i++)
            {
                posiciones.Add(Instantiate(esfera));
                posiciones[i].transform.position = new Vector3(level.Trajectories.Positions[i].X, level.Trajectories.Positions[i].Y, level.Trajectories.Positions[i].Z);
                posiciones[i].transform.localScale = new Vector3(level.Scale, level.Scale, level.Scale);
                posiciones[i].GetComponent<SphereCollider>().enabled = false;
            }
            //posiciones[0].GetComponent<SphereCollider>().enabled = true;
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
                textoCambioDificultad.text = "No se puede aumentar más la dificultad";
                StartCoroutine(FadeOutCR(textoCambioDificultad, -1.3f));
            }
            else
            {
                textoCambioDificultad.text = "Dificultad aumentada";
                StartCoroutine(FadeOutCR(textoCambioDificultad, -1.3f));

                for (int i = 0; i < posiciones.Count; i++)
                    Destroy(posiciones[i]);

                posiciones.Clear();

                CargarDatosPartida(exergame.Name + " Lvl " + (level.Level + 1) + ".json");
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
                textoCambioDificultad.text = "No se puede reducir más la dificultad";
                StartCoroutine(FadeOutCR(textoCambioDificultad, -1.3f));
            }
            else
            {
                textoCambioDificultad.text = "Dificultad reducida";
                StartCoroutine(FadeOutCR(textoCambioDificultad, -1.3f));

                for (int i = 0; i < posiciones.Count; i++)
                    Destroy(posiciones[i]);

                posiciones.Clear();

                CargarDatosPartida(exergame.Name + " Lvl " + (level.Level - 1) + ".json");
                esferasActivadas = 0;
                puntuacion = 0;
                textoPuntuacion.text = puntuacion.ToString();
                repeticiones = 0;
                textoRepeticiones.text = repeticiones.ToString() + " / " + level.Max_number_repetitions.Repetitions.ToString();
            }
        }
    }
    private IEnumerator FadeOutCR(TextMeshProUGUI texto, float time)
    {
        float duration = 0.75f;
        float currentTime = time;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    void ContadorPreparacionEjercicio() {
        textoTiempoPreparacion.text = (tiempoComiemzo - Time.time).ToString("f0");
        if (textoTiempoPreparacion.text == "6" || textoTiempoPreparacion.text == "7")
            textoTiempoPreparacion.text = "3";
        else if (textoTiempoPreparacion.text == "4" || textoTiempoPreparacion.text == "5")
            textoTiempoPreparacion.text = "2";
        else if (textoTiempoPreparacion.text == "2" || textoTiempoPreparacion.text == "3")
            textoTiempoPreparacion.text = "1";
        else if (textoTiempoPreparacion.text == "0" || textoTiempoPreparacion.text == "1")
            textoTiempoPreparacion.text = "YA!";
        else
            textoTiempoPreparacion.text = "";

        if (Math.Round((tiempoComiemzo - Time.time), 1) == 7.6 || Math.Round((tiempoComiemzo - Time.time), 1) == 5.6 || Math.Round((tiempoComiemzo - Time.time), 1) == 3.6)
            sonidoCuentaAtras.Play();
        if (Math.Round((tiempoComiemzo - Time.time), 1) == 1.6)
            sonidoCuentaAtrasYa.Play();
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
            StartCoroutine(FadeOutCR(textoIncrementoPuntos, -0.75f));
            tocarEsfera.Play();
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
