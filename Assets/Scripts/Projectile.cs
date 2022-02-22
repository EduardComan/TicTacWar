using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public Color trailColour;
    float speed = 10;
    float damage = 1;
    float lifetime = 3;
    float skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, lifetime); //distruge gloantele dupa nr de secunde cu care a fost initializat variabila lifetime

        Collider[] initialCollision = Physics.OverlapSphere(transform.position, 1f, collisionMask); //daca se spawneaza gloantele intr-un obiect
        if(initialCollision.Length > 0)
        {
            OnHitObject(initialCollision[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColour);

    }

    public void SetSpeed (float newSpeed)
    {
        speed = newSpeed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions (float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth,  collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }
    void OnHitObject (Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit (damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
