using System.Collections.Generic;

[System.Serializable]
public class Clock
{
    public bool Activated = false;
    public int Countdown = 0;
}

[System.Serializable]
public class Max_number_repetitions
{
    public bool Activated = false;
    public int Repetitions = 0;
}

[System.Serializable]
public class Positions
{
    public float X = 0;
    public float Y = 0;
    public float Z = 0;
}

[System.Serializable]
public class Trajectory
{
    public string Id_trajectory = "";
    public List<Positions> Positions = null;
}

[System.Serializable]
public class Gameplay
{
    public string Id_trajectory = "";
    public string Involved_joint = "";
    public List<string> Joints = null;
    public int Repetition_increment = 0;
    public int Score_increment = 0;
}

[System.Serializable]
public class ExergameLvl
{
    public int Level = 0;
    public string ExergameName = "";
    public Clock Clock = null;
    public Max_number_repetitions Max_number_repetitions = null;
    public Trajectory Trajectories = null;
    public Gameplay[] Gameplay = null;
}
