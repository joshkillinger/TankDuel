using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public float PlayerNumber = 1;
    public float EnginePower = 1000;
    public float LookSpeed = 90;
    public float AimSpeed = 20;

    private int hp = 100;
    private int armor = 10;

    public int Kills = 0;
    public int Deaths = 0;

    private Rigidbody rigidbody;

    public WheelCollider[] Wheels;
    public Transform Turret;
    public GameObject Gun;
    public float MaxGunAngle;
    public float MinGunAngle;
    
    public Transform Muzzle;
    public GameObject Bullet;
    public float FireRate;
    private float lastFire;

    public bool DebugOn;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, -.25f, 0);
        lastFire = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Drive();
        Look();
        Aim();
        Fire();
    }

    void Drive()
    {
        float turn = Input.GetAxisRaw("P1Turn");// *Time.deltaTime;
        float drive = Input.GetAxisRaw("P1Drive") * -1;// *Time.deltaTime;
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

    private void Look()
    {
        float rotateInput = Input.GetAxisRaw("P1Rotate");
        float rotate = rotateInput * LookSpeed * Time.deltaTime * -1;

        if (rotate != 0)
        {
            Turret.Rotate(new Vector3(1,0, 0), rotate, Space.Self);
        }
    }

    private void Aim()
    {
        float aimInput = Input.GetAxisRaw("P1Aim") * -1;
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

    private void Fire()
    {
        if (Input.GetButtonDown("P1Fire"))
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

    public bool TakeDamage(int damage)
    {
        bool died = false;
        damage -= armor;
        damage = Mathf.Max(0, damage);
        hp -= damage;

        Debug.Log(gameObject.tag + " took " + damage + " damage! " + hp + " hp remaining.");

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