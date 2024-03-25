using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 270;
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public Transform bulletSpawnPoint; // The position where the bullet spawns
    public float bulletSpeed = 100f; // Speed of the bullet
    public int reloadSpeed = 3;
    public bool isRifle;



    //Variables that change throughout code
    bool _canShoot;
    int _currentAmmoInClip;
    int _ammoInReserve;


    //Muzzle Flash
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    //Aiming the Gun
    public Vector3 normalLoacalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSmoothing = 10;

    //Reloading the gun
    public Vector3 targetReloadPosition;

    //Mouse Settings
    public float mouseSensitivity = 1;
    Vector2 _currentRotation;
    public float weaponSwayAmount = 10;

    //Weapon Recoil
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    //Only assign if randomized recoil = false
    public Vector2[] recoilPattern;

    //Shooting the gun
    public Vector3 targetShootRotation;
    


    //Weapon Sounds
    public AudioSource weaponFire;
    public AudioSource weaponReload;
    public AudioSource shufflingAim; //ADS Sound Effect
    public AudioSource hitMarker;


    private void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }

    private void Update()
    {
        DetermineAim();
        DetermineRotation();

        // Shooting
        if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            StartCoroutine(ShootGun());
        }
        // Reloading
        else if (Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            int amountNeeded = clipSize - _currentAmmoInClip;
            StartCoroutine(ReloadGun(amountNeeded));
        }
    }


    IEnumerator ReloadGun(int amountNeeded)
    {
        // Play reload sound
        weaponReload.enabled = true;
        weaponReload.Play();



        float elapsedTime = 0f;
        float reloadDuration = 2f;

        Vector3 initialGunPosition = transform.localPosition;
        while (elapsedTime < reloadDuration)
        {
            // Smoothly interpolate between initial and target position
            transform.localPosition = Vector3.Lerp(initialGunPosition, targetReloadPosition, elapsedTime / reloadDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Wait for reload time
        yield return new WaitForSeconds(reloadSpeed - reloadDuration);

        // Reload ammo
        if (amountNeeded > _ammoInReserve)
        {
            _currentAmmoInClip += _ammoInReserve;
            _ammoInReserve = 0;
        }
        else
        {
            _currentAmmoInClip += amountNeeded;
            _ammoInReserve -= amountNeeded;
        }

        // Reset gun position
        transform.localPosition = normalLoacalPosition;

        // Stop reload sound
        weaponReload.enabled = false;
    }


    void DetermineRotation()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseAxis *= mouseSensitivity;
        _currentRotation += mouseAxis;

        _currentRotation.y = Mathf.Clamp(_currentRotation.y, -90, 90); //limit up and down look radius

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;

        transform.root.localRotation = Quaternion.AngleAxis(_currentRotation.x, Vector3.up);
        transform.parent.localRotation = Quaternion.AngleAxis(-_currentRotation.y, Vector3.right);
    }

    void DetermineAim()
    {
        Vector3 target = normalLoacalPosition;
        if (Input.GetMouseButton(1)) target = aimingLocalPosition;

        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);

        transform.localPosition = desiredPosition;
    }

   

    void DetermineRecoil()
    {
        //Physical Gun Recoil
        transform.localPosition -= Vector3.forward * 0.1f;

        //Camera Recoil
        if (randomizeRecoil)
        {
            float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(xRecoil, yRecoil);

            _currentRotation += recoil;
        }
        else
        {
            int currentStep = clipSize + 1 - _currentAmmoInClip;
            currentStep = Mathf.Clamp(currentStep, 0, recoilPattern.Length - 1);

            _currentRotation += recoilPattern[currentStep];
        }
    }

    IEnumerator ShootGun()
    {
        DetermineRecoil();
        StartCoroutine(MuzzleFlash());

        // Instantiate the bullet prefab
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Set the tag of the bullet to "knockDown"
        bullet.tag = "knockDown";

        // Set the bullet velocity
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.parent.forward * bulletSpeed; // Set bullet speed here

        // Destroy the bullet after 5 seconds
        Destroy(bullet, 3f); // Adjust the time as needed

        RayCastForEnemy();
        weaponFire.enabled = true;
        weaponFire.Play(); // Play shooting sound
        if (isRifle)
        {
            yield return new WaitForSeconds(1.1f);

        // Store the current gun rotation before changing it
        Quaternion previousGunRotation = transform.localRotation;

        // Smoothly interpolate to the target rotation
        float elapsedTime = 0f;
        float rotationDuration = 0.2f; // Adjust the duration of rotation
        while (elapsedTime < rotationDuration)
        {
            transform.localRotation = Quaternion.Slerp(previousGunRotation, Quaternion.Euler(targetShootRotation), elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the gun rotation after shooting
        transform.localRotation = Quaternion.Euler(targetShootRotation);

        // Wait for a short duration
        yield return new WaitForSeconds(fireRate);

        // Check if additional rotation is enabled for this gun
        
            // Smoothly interpolate back to the original rotation
            elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, previousGunRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset gun rotation to previous rotation
            transform.localRotation = previousGunRotation;
        }
        if (!isRifle)
        {
            yield return new WaitForSeconds(fireRate);

        }
        _canShoot = true;
    }





    IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }

   
  



    void RayCastForEnemy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, 1 << LayerMask.NameToLayer("Enemy")))
        {
            try
            {
                Debug.Log("Hit an Enemy");
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.AddForce(transform.parent.transform.forward * 10);
                hitMarker.enabled = true;
                hitMarker.Play(); // Play hitmarker sound
            }
            catch { }
        }
    }

}
