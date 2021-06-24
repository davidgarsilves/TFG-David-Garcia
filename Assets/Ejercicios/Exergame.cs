[System.Serializable]
public class Camera_setup
{
    public string Id_camera = "";
    public float[] Position = {0, 0, 0};
    public float[] Rotation = {0, 0, 0};
}

[System.Serializable]
public class Score
{
    public bool Activated = false;
}

[System.Serializable]
public class Exergame
{
    public string Name = "";
    public string Description = "";
    public Camera_setup Camera_setup = new Camera_setup();
    public Score Score = null;
    public int Levels = 0;
    public string Type = "";
}
