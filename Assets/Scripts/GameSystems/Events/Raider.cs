using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raider : MonoBehaviour
{
    public float timeToShoot = 3f;

    private float shootTimer = 0f;
    private bool canShoot;

    public void OnBecomeShootableKeyframe()
    {

    }

    public void GrappleKeyframe()
    {
        AudioController.Instance.PlaySound2D("crunch_short_2");
    }

    public void HandKeyframe()
    {
        AudioController.Instance.PlaySound2D("crunch_blip");
    }

    public void AimKeyframe()
    {
        AudioController.Instance.PlaySound2D("crunch_blip");
    }

    public void OnPlayerCanBeKilledKeyframe()
    {
        canShoot = true;
    }

    private void Update()
    {
        if (canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer > timeToShoot)
            {
                shootTimer = 0f;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        var gunshot = AudioController.Instance.PlaySound2D("gunshot");
        DontDestroyOnLoad(gunshot.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dialogue");
    }
}
