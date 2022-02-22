using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour
{
    public enum FireMode{Auto, Burst, Single};
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleflash;

    bool triggerReleasedSinceLastShot;
    int shotsRemainningInBurst;

    private void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash> ();
        shotsRemainningInBurst = burstCount;
    }

    float nextShotTime;
    void Shoot ()
    {
        if (Time.time > nextShotTime)
        {
            if (fireMode==FireMode.Burst)
            {
                if(shotsRemainningInBurst == 0) {return;}
                shotsRemainningInBurst--;
            }

            else if (fireMode == FireMode.Single)
            {
                if(!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainningInBurst = burstCount;
    }
}
