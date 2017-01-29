using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using WiimoteApi;

public class WiimoteHandler : MonoBehaviour
{
    public int MovespeedMultiplier;
    public float Gravity = 20.0F;
    public Text Out;
    public RawImage Icon;
    public InputDataType Mode;
    public int Accel;
    public int Axis = 1;
    public int ThrowThreshold = 1000;

    private Wiimote wiimote;
    private Vector3 _moveDirection = Vector3.zero;


    public void Start()
    {
        Mode = InputDataType.REPORT_BUTTONS_ACCEL_EXT16;
        SearchWiimotes();
    }

    public void Update()
    {
        //Icon.gameObject.SetActive(WiimoteManager.HasWiimote());
        WiimoteManager.FindWiimotes();
        wiimote = WiimoteManager.Wiimotes[0];
        if (!EnvironmentManager.Instance.Player) return;
        if (!WiimoteManager.HasWiimote()) { return; }

        wiimote.ReadWiimoteData();
        
        //if (wiimote.Button.a)
        //    EnvironmentManager.Instance.Player.GetComponent<ThirdPersonCharacter>().Move(new Vector3(0,1,0), false, true);

        //if (wiimote.Button.b)
        //    EnvironmentManager.Instance.Player.Soak();
        
        // Get Nunchuck Data
        if (wiimote.current_ext == ExtensionController.NUNCHUCK)
        {
            var dat = wiimote.Nunchuck;
            Accel = dat.accel[Axis];
            
            // Throw Handling
            int force = Accel - ThrowThreshold;
            if (force > 0)
                EnvironmentManager.Instance.Player.Throw();
            
            //// Aim Handling
            //if (wiimote.Nunchuck.c)
            //{
            //    EnvironmentManager.Instance.Player.AimTarget();
            //}
            //var joy = dat.GetStick01();
            
            //float x = -0.5f + (float) joy[0];
            //float z = -0.5f + (float) joy[1];


            //x = Normalize(x);
            //z = Normalize(z);
            //MoveCharacter(new Vector3(x, 0, z));
            
        }
    }

    void MoveCharacter(Vector3 movement)
    {
        ThirdPersonCharacter controller = EnvironmentManager.Instance.Player.GetComponent<ThirdPersonCharacter>();
        
        _moveDirection = movement;
        _moveDirection = EnvironmentManager.Instance.Player.transform.TransformDirection(_moveDirection);
        _moveDirection *= MovespeedMultiplier;
        
        controller.Move(_moveDirection * Time.deltaTime, false, false);
    }

    public void SearchWiimotes()
    {
        WiimoteManager.FindWiimotes();
        InitWiimotes();
    }

    void InitWiimotes()
    {
        wiimote = WiimoteManager.Wiimotes[0];
        Debug.Log(wiimote.current_ext);
    }

    public void SetMode()
    {
        if (!WiimoteManager.HasWiimote()) return;
        wiimote.SendDataReportMode(Mode);
    }

    void FinishedWithWiimotes()
    {
        foreach (Wiimote remote in WiimoteManager.Wiimotes)
        {
            WiimoteManager.Cleanup(remote);
        }
    }

    float Normalize(float value, float tolerance = 0.2f)
    {
        if (Mathf.Abs(value) < tolerance)
        {
            return 0;
        }
        return value;
    }
}