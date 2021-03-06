using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Inventory;
using static getObj;

public class FirePistol : MonoBehaviour
{
    public GameObject projectile; // on stockera le prefab Balle
    public Transform posTir; // Position de la Balle
    public float force; // puissance de tir
    public AudioClip sonDeTir; // Son de tir

    public GameObject FPSController;

    public GameObject impactEffect;
    public GameObject muzzleFlash;

    private bool isShooting = false;

    private GameObject[] spawnPoints;

    public GameObject ennemy;

    void Start() {
        this.muzzleFlash.SetActive(false);
        this.spawnPoints = GameObject.FindGameObjectsWithTag("ennemySpawnPoint");
    }

    private int getBullet()
    {
        int nb = FPSController.GetComponent<Player2>().inventory.get(IItems.BALLE);
        return nb;
    }

    private void removeABullet()
    {
        FPSController.GetComponent<Player2>().inventory.remove(IItems.BALLE);
        return;
    }

    IEnumerator fireLoop()
    {
        float speed = 0.1f;

        switch (FPSController.GetComponent<Player2>().currentWeapon)
        {
            case "ak":
                speed = 0.1f;
                break;
            case "famas":
                speed = 0.08f;
                break;
            default:
                break;
        }

        while (this.isShooting)
        {
            this.shoot();
            if (this.getBullet() > 0)
            {
                yield return new WaitForSeconds(speed);
            }
            else
            {
                this.isShooting = false;
                break;
            }
        }
    }

    void shoot() {
        GetComponent<AudioSource>().PlayOneShot(sonDeTir);
        this.removeABullet();
        RaycastHit hit;
        int distance = 50;

        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        GameObject obj = getObj.obj(ray, distance);

        if (Physics.Raycast(ray, out hit, distance)) {
            if (hit.collider.gameObject.tag == "Zombie")
            {
                if (hit.collider.gameObject.GetComponent<Zombie>().health <= 0) {
                    return;
                }
                hit.collider.gameObject.GetComponent<Zombie>().health -= 30;
                if (hit.collider.gameObject.GetComponent<Zombie>().health <= 0)
                {
                    FPSController.GetComponent<Player2>().totalKills++;
                    FPSController.GetComponent<Player2>().currentEnnemy--;
                    hit.collider.gameObject.GetComponent<AudioSource>().PlayOneShot(hit.collider.gameObject.GetComponent<Zombie>().deathSound);
                    if (FPSController.GetComponent<Player2>().currentEnnemy == 0) {
                        FPSController.GetComponent<Player2>().vague++;
                        
                        int nb = FPSController.GetComponent<Player2>().ennemyAmount * FPSController.GetComponent<Player2>().vague;
                        for (int i = 0; i < nb; i++)
                        {
                            GameObject spawnPoint = FPSController.GetComponent<Player2>().spawnPoints[UnityEngine.Random.Range(0, FPSController.GetComponent<Player2>().spawnPoints.Length)];
                            GameObject z = Instantiate(ennemy, spawnPoint.transform.position, Quaternion.identity);
                        }
                        FPSController.GetComponent<Player2>().currentEnnemy = nb;

                    }
                }
                return;
            }
            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.identity);
            Destroy(impact, 2f); // on d??truit l'effet apr??s 2s
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.isShooting = false;
        }
        if (Input.GetMouseButtonDown(0) && this.getBullet() > 0)
        {
            this.isShooting = true;
            StartCoroutine(this.fireLoop());
        }

        this.muzzleFlash.SetActive(this.isShooting);

        // if (this.isShooting && this.getBullet() != 0)// qd on clique 
        // {
            // this.removeABullet();
            // Instanciation (cr??ation) du projectile
            // GameObject go = Instantiate(projectile, posTir.position, Quaternion.identity);
            // Instantiate prend comme param??tre l'objet ?? instancier, la position et la rotation (ici elle est nulle)
            // on a acc??s aux composants de go qui est un game object

            // on propulse le projectile
            // go.GetComponent<Rigidbody>().AddForce(posTir.forward * force); // on applique une force au rigidbody en face

            // on d??truit le prjectile apr??s 10s
            // Destroy(go, 10);

            // On lance le son de tir
            // GetComponent<AudioSource>().PlayOneShot(sonDeTir);

            // chargeur--;
        // }

        if (Input.GetKeyUp("r"))
        {
            // chargeur = 10;
        }
        
    }
}
