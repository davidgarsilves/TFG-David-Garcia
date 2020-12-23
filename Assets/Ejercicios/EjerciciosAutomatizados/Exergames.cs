[System.Serializable]
public class Camera_setup
{
    public string Id_camera = "";
    public float[] Position = {0, 0, 0};
    public float[] Rotation = {0, 0, 0};
}

[System.Serializable]
public class Clock
{
    public bool Activated = false;
    public int Countdown = 0;
}

[System.Serializable]
public class Score
{
    public bool Activated = false;
}

[System.Serializable]
public class Trajectory
{
    public string Id_trajectory = "";
    public float[] Start_position = {0, 0, 0};
    public float[] End_position = {0, 0, 0};
}

[System.Serializable]
public class Gameplay
{
    public string Id_trajectory = "";
    public string Involved_joint = "";
    public int Repetition_increment = 0;
    public int Score_increment = 0;
}

[System.Serializable]
public class Exergames
{
    public string Description = "";
    public Camera_setup Camera_setup = new Camera_setup();
    public Clock Clock = null;
    public Score Score = null;
    public int Max_number_repetitions = 0;
    public Trajectory[] Trajectories = null;
    public Gameplay[] Gameplay = null;
}
