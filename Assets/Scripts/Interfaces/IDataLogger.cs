using UnityEngine;

public interface IDataLogger
{
    // Driving distance logging
    void LogDrivingDistance(DrivingDistanceData data);

    // Target recognition logging
    void LogTargetRecognition(TargetRecognitionData data);

    // User input logging
    void LogUserInput(UserInputData data);

    // Event timing logging
    void LogEvent(EventTimeData data);

    void LogMovement(MovementData data);

    //void LogTotalDrivingDistance(TotalDrivingDistanceData data);





    // Optional lifecycle methods (VERY useful)
    void InitializeSession(string participantId); // is it necessary?
    void CloseSession();
}