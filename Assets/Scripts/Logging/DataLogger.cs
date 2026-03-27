using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger : IDataLogger
{
    private static DataLogger _instance;
    private static readonly object _lock = new object();
    private string participantId;


    // Singleton property
    public static DataLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataLogger();
                    }
                }
            }
            return _instance;
        }
    }

    private string folderPath;
    private readonly Dictionary<string, int> nextNoByFile = new Dictionary<string, int>();

    // 1. Update headers to include filetime
    private readonly Dictionary<string, string> headerByFile = new Dictionary<string, string>()
{
{ "Driving_distance.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,TotalDriveDistance,End_pos_x,End_pos_y,End_pos_z,Targ_x,Targ_y,Targ_z" },
        { "Target_rec.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,Drive_distance,Rec_pos_x,Rec_pos_y,Rec_pos_z,Targ_x,Targ_y,Targ_z,Forward_x,Forward_y,Forward_z" },
    { "Event_time_data.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,event,timeRemaining" },
{ "Movement_data.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,Pos_x,Pos_y,Pos_z,Dir_x,Dir_y,Dir_z,DroneCapt,TotalTravelledDistance" },
    { "Total_driving_distance.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,Duration,Total_distance,Final_pos_x,Final_pos_y,Final_pos_z" },
    { "Drone_flight.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,Pos_x,Pos_y,Pos_z,IsFlying" },
        { "User_input_log.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,Input,Correct,Completed,TargetNumber" },
        { "Collision_log.csv", "No,participant_id,timestamp,filetimeTS,partID,taskID,collidedObject,playerPosX,playerPosY,playerPosZ" },



};


    // Private constructor prevents direct instantiation
    private DataLogger() { }

    // Reset method for testing or new sessions
    public void ResetInstance()
    {
        lock (_lock)
        {
            CloseSession();
            nextNoByFile.Clear();
            folderPath = null;
        }
    }

    // Your existing methods remain unchanged...
    public void InitializeSession(string participantId)
    {
        this.participantId = participantId;
        string logsRoot = Path.Combine(Application.persistentDataPath, "Logs");

        folderPath = Path.Combine(logsRoot, $"Participant_{participantId}");
        Directory.CreateDirectory(folderPath);

        nextNoByFile.Clear();

        foreach (var kvp in headerByFile)
        {
            string file = kvp.Key;
            string fullPath = Path.Combine(folderPath, file);

            EnsureHeader(fullPath, kvp.Value);

            int lineCount = SafeCountLines(fullPath);
            int dataRows = Math.Max(0, lineCount - 1);
            nextNoByFile[file] = dataRows + 1;
        }

        Debug.Log($"Logging to: {folderPath}");
    }

    public void CloseSession()
    {
        // Nothing required for AppendAllText.
        // (If you later switch to buffered streams, flush/close here.)
    }



    public void LogDroneFlight(DroneFlightData d)
    {
        string file = "Drone_flight.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{d.Pos_x},{d.Pos_y},{d.Pos_z},{d.IsFlying}");
    }


    public void LogTargetRecognition(TargetRecognitionData d)
    {
        string file = "Target_rec.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{d.Drive_distance},{d.Rec_pos_x},{d.Rec_pos_y},{d.Rec_pos_z},{d.Targ_x},{d.Targ_y},{d.Targ_z},{d.Forward_x},{d.Forward_y},{d.Forward_z}");
    }

    public void LogDrivingDistance(DrivingDistanceData d)
    {
        string file = "Driving_distance.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{d.TotalDriveDistance},{d.End_pos_x},{d.End_pos_y},{d.End_pos_z},{d.Targ_x},{d.Targ_y},{d.Targ_z}");
    }



    public void LogUserInput(UserInputData d)
    {
        string file = "User_input_log.csv";
        FillCommonAuto(d, file);

        string inputValue = d.Input == int.MinValue ? "none" : d.Input.ToString();

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{inputValue},{d.Correct},{d.Completed},{d.TargetNumber}");
    }

    public void LogCollision(CollisionData d)
    {
        string file = "Collision_log.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{d.collidedObject},{d.playerPosX},{d.playerPosY},{d.playerPosZ}");
    }





    public void LogEvent(EventTimeData d)
    {
        string file = "Event_time_data.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID},{d.eventType},{d.timeRemaining}");
    }



    public void LogMovement(MovementData d)
    {
        string file = "Movement_data.csv";
        FillCommonAuto(d, file);

        Append(file,
            $"{d.No},{participantId},{d.timestamp},{d.filetimeTS},{d.partID},{d.taskID}," +
            $"{d.Pos_x},{d.Pos_y},{d.Pos_z}," +
            $"{d.Dir_x},{d.Dir_y},{d.Dir_z}," +
            $"{d.DroneCapt},{d.TotalTravelledDistance}");
    }


    // ...

    private void FillCommonAuto(CommonData d, string file)
    {
        if (d.No <= 0)
            d.No = GetNextNo(file);

        if (d.timestamp <= 0)
            d.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Set filetimeTS (100-nanosecond intervals since 1601-01-01)
        d.filetimeTS = DateTime.UtcNow.ToFileTimeUtc();
    }

    private int GetNextNo(string file)
    {
        if (!nextNoByFile.TryGetValue(file, out int next))
        {
            // If someone logs before InitializeSession, we still try to function
            next = 1;
            nextNoByFile[file] = next;
        }

        nextNoByFile[file] = next + 1;
        return next;
    }

    private void Append(string file, string line)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("DataLogger not initialized. Call InitializeSession(participantId) first.");
            return;
        }

        if (!headerByFile.TryGetValue(file, out string header))
        {
            Debug.LogError($"No header configured for file: {file}");
            return;
        }

        string fullPath = Path.Combine(folderPath, file);
        EnsureHeader(fullPath, header);
        File.AppendAllText(fullPath, line + "\n");
    }

    private void EnsureHeader(string fullPath, string headerLine)
    {
        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, headerLine + "\n");
            return;
        }

        // If file exists but is empty, write header
        var info = new FileInfo(fullPath);
        if (info.Length == 0)
        {
            File.WriteAllText(fullPath, headerLine + "\n");
        }
    }

    private int SafeCountLines(string fullPath)
    {
        try
        {
            int count = 0;
            using (var sr = new StreamReader(fullPath))
            {
                while (sr.ReadLine() != null) count++;
            }
            return count;
        }
        catch
        {
            // If something goes wrong, assume only header
            return 1;
        }
    }
}



//how to use it: logger.LogEvent(new EventTimeData
//{
//    partID = 1,
//    condition = Condition.AUTO,
//    taskID = TaskID.T1,
//    eventType = TaskEvent.task_start
//});