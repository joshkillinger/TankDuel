using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public int PlayerNumber = 1;
    private string playerPrefix;
    public float EnginePower = 1000;
    public float LookSpeed = 90;
    public float AimSpeed = 20;

    public static int maxHP = 30;
    private int hp = 0;
    private int armor = 10;

    private int kills = 0;
    public int Kills
    {
        get { return kills; }
        set
        {
            kills = value;
            scoreText.text = "Score: " + kills.ToString();
        }
    }
    public int Deaths = 0;

    private Rigidbody rigidBody;

    public WheelCollider[] Wheels;
    public Transform Turret;
    public GameObject Gun;
    public float MaxGunAngle;
    public float MinGunAngle;

    public GameObject PlayerCamera;
    private Camera playerCamera;
    private Text scoreText;
    private LineRenderer reticle;

    public Transform Muzzle;
    public GameObject Bullet;
    public float FireRate;
    private float lastFire;

    public bool DebugOn;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = new Vector3(0, -.25f, 0);
        lastFire = 0;

        int players = GameManager.Instance.NumberOfPlayers;
        
        Rect camRect = new Rect();
        camRect.width = players > 2 ? .5f: 1f;
        camRect.height = .5f;

        switch (PlayerNumber)
        {
            case 1:
                camRect.x = 0;
                camRect.y = .5f;
                break;

            case 2:
                camRect.x = players < 3 ? 0f : .5f;
                camRect.y = players < 3 ? 0f : .5f;
                break;

            case 3:
                camRect.x = 0f;
                camRect.y = 0f;
                break;

            case 4:
                camRect.x = .5f;
                camRect.y = 0f;
                break;

            default:
                Debug.LogError("Invalid player number of " + PlayerNumber + " detected!");
                break;
        }

        playerCamera = PlayerCamera.GetComponent<Camera>();
        playerCamera.rect = camRect;

        playerPrefix = "P" + PlayerNumber;

        scoreText = gameObject.GetComponentInChildren<Text>();

        SetUpTargetingReticle();

        hp = maxHP;
    }


    private void SetUpTargetingReticle()
    {
        reticle = gameObject.GetComponentInChildren<LineRenderer>();
        
        //reticle.SetVertexCount(100);

        Vector3[] positions = new Vector3[100];
        
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i].x = 0;
            float t = i * .25f;
            t = t * t;
            positions[i].y = -.5f * 9.81f * t;
            positions[i].z = i * 50 * .25f;
        }
        reticle.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
            return;

        Drive();
        Look();
        Aim();
        Fire();
    }

    /// <summary>
    /// Controls the movement of the tank based on input
    /// </summary>
    void Drive()
    {
        float turn = Input.GetAxisRaw(playerPrefix + "Turn");// *Time.deltaTime;
        float drive = Input.GetAxisRaw(playerPrefix + "Drive") * -1;// *Time.deltaTime;
        float leftDrive = Mathf.Clamp(drive + turn, -1, 1) * EnginePower;
        float rightDrive = Mathf.Clamp(drive - turn, -1, 1) * EnginePower;

        int i = 0;
        for (i = 0; i < 2; i++)
        {
            Wheels[i].motorTorque = leftDrive;
        }
        for (i = 2; i < 4; i++)
        {
            Wheels[i].motorTorque = rightDrive;
        }

        if (DebugOn)
        {
            Debug.Log("Left = " + leftDrive + " Right = " + rightDrive);
        }
    }

    /// <summary>
    /// Controls the rotation of the turret based on input
    /// </summary>
    private void Look()
    {
        float rotateInput = Input.GetAxisRaw(playerPrefix + "Rotate");
        float rotate = rotateInput * LookSpeed * Time.deltaTime * -1;

        if (rotate != 0)
        {
            Turret.Rotate(new Vector3(1,0, 0), rotate, Space.Self);
        }
    }

    /// <summary>
    /// Controls the height of the cannon based on input
    /// </summary>
    private void Aim()
    {
        float aimInput = Input.GetAxisRaw(playerPrefix + "Aim") * -1;
        float aim = aimInput * AimSpeed * Time.deltaTime;
        float currentAim = Gun.transform.localEulerAngles.y;
        aim += currentAim;
        aim = Mathf.Clamp(aim, MinGunAngle, MaxGunAngle);
        Gun.transform.localEulerAngles = new Vector3(Gun.transform.localEulerAngles.x, aim, Gun.transform.localEulerAngles.z);

        if (DebugOn)
        {
            Debug.Log("Aim = " + aim + " LocalEulerAngles = " + Gun.transform.localEulerAngles);
        }
    }

    /// <summary>
    /// Fires a bullet on input if reload is finished
    /// </summary>
    private void Fire()
    {
        if (Input.GetButtonDown(playerPrefix + "Fire"))
        {
            if ((lastFire + FireRate) <= Time.time)
            {
                lastFire = Time.time;

                //shoot
                GameObject bullet = GameObject.Instantiate<GameObject>(Bullet);
                bullet.transform.position = Muzzle.position;
                bullet.transform.rotation = Quaternion.LookRotation(Muzzle.position - Gun.transform.position);
                bullet.GetComponent<BulletScript>().Owner = gameObject;
            }
        }
    }

    /// <summary>
    /// Takes <paramref name="damage"/> points of damage, modified by armor
    /// </summary>
    /// <param name="damage">Points of damage to take</param>
    /// <returns>True if tank was destroyed</returns>
    public bool TakeDamage(int damage, Vector3 source)
    {
        if (hp <= 0) return false;

        bool died = false;
        damage -= armor;
        damage = Mathf.Max(0, damage);
        hp -= damage;

        Debug.Log(gameObject.tag + " took " + damage + " damage! " + hp + " hp remaining.");

        rigidBody.AddExplosionForce(damage * 100.0f, source, 0f, 2.0f, ForceMode.Impulse);

        if (hp <= 0)
        {
            //we died
            died = true;
            Deaths++;
            Debug.Log(gameObject.tag + " died!");
        }

        return died;
    }
}