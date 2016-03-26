using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    public GameObject Owner;

    public float Speed = 50;
    public float Damage = 200;

    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = gameObject.transform.forward * Speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.gameObject.tag.Equals(Owner.tag))
        {
            return;
        }

        //explode!

        GameManager.Instance.Explode(collision.contacts[0].point, Owner, Damage);
        Object.Destroy(gameObject);
    }
}