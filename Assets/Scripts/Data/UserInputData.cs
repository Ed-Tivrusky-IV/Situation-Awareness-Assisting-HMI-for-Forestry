using System;
using UnityEngine;

[Serializable]
public class UserInputData : CommonData
{
    // Two-number input entered by participant
    public int Input;

    // Whether the submitted input is correct
    public bool Correct;

    // Whether the user successfully completed before time ends
    public bool Completed;

    public int TargetNumber;

}