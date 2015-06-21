using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponType weaponType;

    public float meleeRange;
    public float meleeRate;

    public float weaponDamage;

    public float rangedForce;
    public float rangedDropRate;
    public float rangedLoadTime;
    public float rangedAmmo;

    public GameObject projectileObject;

    private float attackTimer = Mathf.Infinity;

    private RaycastHit hit;

    public enum WeaponType { Melee, Ranged, Hibrid }

    private void Update ()
    {
        if (weaponType == WeaponType.Melee)
        {
            Ray hitRay = new Ray(transform.parent.position, transform.parent.forward);

            if (Input.GetMouseButtonDown(0) && attackTimer >= meleeRate)
            {
                if (Physics.Raycast(hitRay, out hit, meleeRange))
                {
                    hit.transform.SendMessage
                                  (
                                      "TakeDamage", 
                                      weaponDamage, 
                                      SendMessageOptions.DontRequireReceiver
                                  );

                    // Play weapon hit animation.
                }

                Animation animation = this.GetComponent<Animation>();

                animation.Play("Axe Attack Animation");

                // Play weapon attack sound.

                attackTimer = 0f;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }
        else if (weaponType == WeaponType.Ranged)
        {
            if (Input.GetMouseButtonDown(0) && rangedAmmo > 0)
            {
                GameObject projectile = (GameObject)Instantiate
                                                    (
                                                        projectileObject,
                                                        transform.position,
                                                        transform.rotation
                                                    );

                Projectile projectileComponent = projectile.GetComponent<Projectile>();

                projectileComponent.damageAmount = weaponDamage;

                GameObject projectileTip = null;

                foreach (Transform child in projectile.transform)
                {
                    if (child.name.Contains("Tip"))
                    {
                        projectileTip = child.gameObject;
                        break;
                    }
                }

                projectileTip.GetComponent<Rigidbody>().AddForce
                                                        (
                                                            projectile.transform.forward * rangedForce,
                                                            ForceMode.Impulse
                                                        );
            }
        }
        else if (weaponType == WeaponType.Hibrid)
        {

        }
    }
}