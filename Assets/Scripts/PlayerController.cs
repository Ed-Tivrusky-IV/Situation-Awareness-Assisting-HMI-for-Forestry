using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

//TODO: KeepPlayerInBounds is not working, need to be updated.

public class PlayerController : MonoBehaviour
{
    //private PlayerControls controls;
    private PlayerInput playerinput;
    public float speed = 5f;
    public float turnSpeed = 20f;
    //private float verticalInput;
    //private float horizontalInput;
    private Vector3 localMove = Vector3.zero;
    private Vector3 worldMove = Vector3.zero;
    private Vector2 _moveInput;
    private float _twistInput = 0f;

    private readonly float xRange = 180f; //To be updated
    private readonly float zRange = 180f;

    private bool shouldStop = false;
    private bool ifOnMove = false;
    private bool backingUp = false;
    private int backupCounter = 0;
    private Rigidbody playerRb;

    private AudioSource engineAudioSource; // Reference to the AudioSource component for engine sound

    [SerializeField] private TaskTimer taskTimer; // Reference to the player model GameObject

    void Awake()
    {
        //controls = new PlayerControls();
        playerinput = GetComponent<PlayerInput>();
        playerinput.neverAutoSwitchControlSchemes = true; // Prevent automatic switching of control schemes

        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true; // Freeze rotation so physics doesn't mess with our rotation
        playerRb.linearVelocity = Vector3.zero; // Start with zero velocity

        engineAudioSource = GetComponent<AudioSource>(); // Get the AudioSource component for engine sound

        foreach (var device in InputSystem.devices)
        {
            if (device.description.manufacturer.Contains("Granite"))
            {
                Debug.Log($"Keeping device: {device.displayName}");
            }
            else if (device is HID)
            {
                InputSystem.DisableDevice(device);
                Debug.Log($"Disabled HID: {device.displayName}");
            }
        }

    }

    // The difference between Send Messages and Broadcast Messages is that Send Messages only send to the GameObject it is called on,
    // while Broadcast Messages send to all child GameObjects as well.
    public void OnMove(InputValue value)
    {
        //Debug.Log("Move input received");
        Vector2 move = value.Get<Vector2>();
        //float move = value.Get<float>();
        if (backingUp)
        {
            move = -move;
        }
        ifOnMove = (move.y != 0);
        _moveInput = move;
        _moveInput.y = SignEpsilon(_moveInput.y);
    }

    private float SignEpsilon(float value, float eps = 0.01f)
    {
        if (value > eps) return 1f;
        if (value < -eps) return -1f;
        return 0f; // or return a small epsilon value if you want to avoid returning zero
    }

    public void OnRotate(InputValue value)
    {
        //Debug.Log("Rotate input received");
        float twist = value.Get<float>();
        //_twistInput = -twist;
        _twistInput = SignEpsilon(-twist);
        //if (!shouldStop && ifOnMove)
        //if (!shouldStop)
        //{
        //    //transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * -_twistInput);
        //    Quaternion delta = Quaternion.Euler(0, _twistInput * turnSpeed * Time.fixedDeltaTime, 0);
        //    playerRb.MoveRotation(playerRb.rotation * delta);
        //    //Debug.Log($"Rotating player by {_twistInput}");
        //}
    }

    void FixedUpdate()
    {
        //if (!shouldStop)
        //{
        //    //transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * -_twistInput);
        //    Quaternion delta = Quaternion.Euler(0, _twistInput * turnSpeed * Time.fixedDeltaTime, 0);
        //    playerRb.MoveRotation(playerRb.rotation * delta);
        //    //Debug.Log($"Rotating player by {_twistInput}");

        //    localMove.Set(0, 0, _moveInput.y);
        //    //localMove.Set(0, 0, move); 
        //    worldMove = transform.TransformDirection(localMove);
        //    playerRb.linearVelocity = worldMove * speed;
        //    //Debug.Log($"Moving player by {worldMove}");

        //    KeepPlayerInBounds();
        //}
        if (!shouldStop)
        {
            Quaternion delta = Quaternion.Euler(0, _twistInput * turnSpeed * Time.fixedDeltaTime, 0);
            playerRb.MoveRotation(playerRb.rotation * delta);

            localMove.Set(0, 0, _moveInput.y);
            worldMove = transform.TransformDirection(localMove);
            playerRb.linearVelocity = worldMove * speed;

            KeepPlayerInBounds();

            // Play engine sound if moving, stop if not
            if (Mathf.Abs(_moveInput.y) > 0.01f)
            {
                if (!engineAudioSource.isPlaying)
                    engineAudioSource.Play();
            }
            else
            {
                if (engineAudioSource.isPlaying)
                    engineAudioSource.Stop();
            }
        }
        else
        {
            if (engineAudioSource.isPlaying)
                engineAudioSource.Stop();
        }
    }


    public void OnBackUp()
    {
        if (backupCounter % 2 == 0)
        {
            backingUp = true;
        }
        else
        {
            backingUp = false;
        }

        backupCounter += 1;
    }

    void KeepPlayerInBounds() 
    {
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }
        if (transform.position.z > zRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRange);
        }
        else if (transform.position.z < -zRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zRange);
        }
    }

    public void StopPlayer()
    {
        shouldStop = true;
        //Stop the timer too
        taskTimer.StopTimer();
    }

    public void StartPlayer()
    {
        shouldStop = false;
        //Start the timer too
        taskTimer.ResumeTimer();
    }
}
