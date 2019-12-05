using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState { INTRO_MYA, INTRO_BULLETS, INTRO_FINAL, DEBATE, LOSS, WIN }

//public enum characters { MAKOTO_NAEGI, SAYAKA_MAIZONO, KIYOTAKA_ISHIMARU, JUNKO_ENOSHIMA, SAKURA_OGAMI, KYOKO_KIRIGIRI, MONDO_OWADA, AOI_ASAHINA, YASUHIRO_HAGAKURE, CHIHIRO_FUJISAKI, BYAKUYA_TOGAMI, CELESTIA_LUDENBERG, LEON_KUWATA, TOKO_FUKAWA, HIFUMI_YAMADA }
public enum Character { MAKOTO, SAYAKA, TAKA, JUNKO, SAKURA, KYOKO, MONDO, HINA, HIRO, CHIHIRO, BYAKUYA, CELESTE, LEON, TOKO, HIFUMI }

public class DebateController : MonoBehaviour
{
    int WIDTH = 800, HEIGHT = 600;

    [SerializeField]
    GameState state = GameState.INTRO_MYA;

    public GameObject myaObject;
    float myaFast = 3000, myaSlow = 50, myaCenterSize = 50, myaMaxX = 0;

    public GameObject gunObject;
    Vector3 gunCornerPosition;
    Transform gunOuterRing;
    Transform gunCylinder;
    float gunSpeed = 3000, gunMinX = -700, gunMaxX = -400, gunBulletMinX = 210, gunWaitTime = 2, gunScaleMult = 1.25f, gunRotZ = 0, gunExtraRotZ = 60, gunRotSpeed = 500;

    public GameObject[] truthBullets;
    Vector3 bulletScale;
    int bulletIndex = 0;

    int activeText = -1;

    NonstopDebateText[] text;

    float timer = 0;

    public GameObject cursor;
    Crosshair crosshair;

    Transform camMainRotator, camRandomOffset, camLookAt, camSlide;
    float lastCameraAngle, newCameraAngle;
    float lastCameraTilt, newCameraTilt;
    Vector3 lastRandomOffset, newRandomOffset, slide;
    float cameraLerp = 0;

    public GameObject shotText;
    bool firing = false;
    Vector3 fireTarget;
    float fireTime = 0.0f;
    public float fireDelay = 2.5f;
    int fireDirection = 1;

    public AudioSource spinSound;
    public AudioSource gunshotSound;
    public AudioSource reflectSound;
    public AudioSource breakSound;

    public GameObject breakParticles;

    bool won = false;
    float winTimeDelay = 1;
    public GameObject victoryScreen;
    public float victoryScreenShake = 10;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        /*
        TextMeshPro tmp = truthBulletPrefab.GetComponentInChildren<TextMeshPro>();
        tmp.SetText(truthBullets[0]);
        tmp.ForceMeshUpdate();
        float width = tmp.textBounds.size.x;
        foreach (Transform child in tmp.transform)
        {
            if (child.name == "Center") child.localScale *= width;
            else if (child.name == "Tip") child.localPosition *= width;
        }
        */
        myaMaxX = -myaObject.transform.localPosition.x;

        gunCornerPosition = gunObject.transform.localPosition;
        gunObject.transform.localPosition = new Vector3(gunMinX, 0, 0);
        gunObject.transform.localScale *= gunScaleMult;
        gunRotZ = gunObject.transform.localEulerAngles.z;
        foreach(Transform child in gunObject.transform)
        {
            if (child.name == "Outer")
            {
                gunOuterRing = child;
                gunOuterRing.localScale *= gunScaleMult;
            }
            else if(child.name == "Cylinder")
            {
                gunCylinder = child;
            }
        }

        foreach(GameObject bullet in truthBullets)
        {
            bullet.transform.localPosition += new Vector3(1000, 0, 0);
        }
        bulletScale = truthBullets[0].transform.localScale;

        text = GetComponentsInChildren<NonstopDebateText>(true);
        foreach (var t in text) t.gameObject.SetActive(false);
        //NextText();

        timer = 0;

        crosshair = cursor.GetComponent<Crosshair>();
        cursor.SetActive(false);

        camSlide = transform.parent;
        camLookAt = camSlide.parent;
        camRandomOffset = camLookAt.parent;
        camMainRotator = camRandomOffset.parent;

        shotText.GetComponent<ShotText>().trigger += ShotTextHit;
    }

    void NextText()
    {
        if(activeText >= 0) text[activeText].Finished -= NextText;
        activeText++;
        if (activeText >= text.Length) activeText = 0;
        text[activeText].Finished += NextText;
        text[activeText].gameObject.SetActive(true);

        lastCameraAngle = newCameraAngle;
        newCameraAngle = 360 * ((float)text[activeText].speaker / 16);
        lastRandomOffset = newRandomOffset;
        newRandomOffset = Random.insideUnitSphere;
        lastCameraTilt = newCameraTilt;
        newCameraTilt = Random.Range(-10, 10);
        slide = Random.insideUnitSphere / 10f;
        cameraLerp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        switch(state)
        {
            case GameState.INTRO_MYA:
                if (myaObject.transform.localPosition.x < -myaCenterSize)
                {
                    myaObject.transform.localPosition += new Vector3(myaFast, 0, 0) * Time.deltaTime;
                    if (myaObject.transform.localPosition.x > -myaCenterSize)
                        myaObject.transform.localPosition = new Vector3(-myaCenterSize, 0, 0);
                }
                else if (myaObject.transform.localPosition.x <= myaCenterSize)
                    myaObject.transform.localPosition += new Vector3(myaSlow, 0, 0) * Time.deltaTime;
                else if (myaObject.transform.localPosition.x < myaMaxX)
                    myaObject.transform.localPosition += new Vector3(myaFast, 0, 0) * Time.deltaTime;
                else state++;
                break;
            case GameState.INTRO_BULLETS:
                if (gunObject.transform.localPosition.x < gunMaxX)
                {
                    gunObject.transform.localPosition += new Vector3(gunSpeed, 0, 0) * Time.deltaTime;
                    if (gunObject.transform.localPosition.x > gunMaxX)
                        gunObject.transform.localPosition = new Vector3(gunMaxX, 0, 0);
                }
                else if (bulletIndex < truthBullets.Length)
                {
                    if (gunExtraRotZ < 60) gunExtraRotZ += gunRotSpeed * Time.deltaTime;
                    else gunExtraRotZ = 60;
                    gunCylinder.transform.localRotation = Quaternion.Euler(0, 0, gunRotZ + gunExtraRotZ);

                    truthBullets[bulletIndex].transform.localPosition -= new Vector3(gunSpeed, 0, 0) * Time.deltaTime;
                    if (truthBullets[bulletIndex].transform.localPosition.x < gunBulletMinX)
                    {
                        gunExtraRotZ = 0;
                        truthBullets[bulletIndex].transform.localPosition = new Vector3(gunBulletMinX, truthBullets[bulletIndex].transform.localPosition.y, 0);
                        bulletIndex++;
                        spinSound.Play();
                    }
                }
                else if (timer < gunWaitTime)
                {
                    if (gunExtraRotZ < 60) gunExtraRotZ += gunRotSpeed * Time.deltaTime;
                    else gunExtraRotZ = 60;
                    gunCylinder.transform.localRotation = Quaternion.Euler(0, 0, gunRotZ + gunExtraRotZ);

                    timer += Time.deltaTime;
                }
                else
                {
                    state++;
                }
                break;
            case GameState.INTRO_FINAL:
                if (gunObject.transform.localPosition.x > gunMinX)
                {
                    gunObject.transform.localPosition -= new Vector3(gunSpeed, 0, 0) * Time.deltaTime;
                    foreach (GameObject bullet in truthBullets)
                    {
                        bullet.transform.localScale -= Vector3.one * Time.deltaTime * 5;
                        if (bullet.transform.localScale.x < 0)
                            bullet.transform.localScale = Vector3.zero;
                    }
                }
                else
                {
                    gunOuterRing.transform.localScale /= gunScaleMult;
                    gunObject.transform.localScale /= gunScaleMult;
                    gunObject.transform.localPosition = gunCornerPosition;

                    foreach (GameObject bullet in truthBullets)
                    {
                        bullet.transform.localScale = bulletScale;
                        bullet.transform.localPosition = new Vector3(-1000, 0, 0);
                    }

                    bulletIndex = 0;
                    truthBullets[bulletIndex].transform.localPosition = new Vector3(gunBulletMinX, 10, 0);
                    state++;
                    cursor.SetActive(true);
                    NextText();
                }
                break;
            case GameState.DEBATE:
                if (Input.GetMouseButtonDown(0) && !firing)
                {
                    gunshotSound.Play();

                    firing = true;
                    fireTime = fireDelay;
                    fireDirection = 1;
                    shotText.GetComponentInChildren<TextMeshPro>().SetText(truthBullets[bulletIndex].GetComponentInChildren<TextMeshProUGUI>().text);
                    shotText.transform.localPosition = transform.position +
                        transform.forward * -30 +
                        transform.right * 10 +
                        transform.up * 3;

                    if (crosshair.hover) shotText.transform.LookAt(crosshair.rayHit.point);
                    else shotText.transform.LookAt(crosshair.ray.origin + crosshair.ray.direction * 100);
                }

                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    gunExtraRotZ = 0;
                    truthBullets[bulletIndex].transform.localPosition = new Vector3(-1000, 0, 0);
                    bulletIndex++;
                    if (bulletIndex >= truthBullets.Length) bulletIndex = 0;
                    truthBullets[bulletIndex].transform.localPosition = new Vector3(gunBulletMinX, 10, 0);
                    spinSound.Play();
                }
                if (gunExtraRotZ < 60) gunExtraRotZ += gunRotSpeed * Time.deltaTime;
                else gunExtraRotZ = 60;
                gunCylinder.transform.localRotation = Quaternion.Euler(0, 0, gunRotZ + gunExtraRotZ);

                if (cameraLerp < 1)
                {
                    camMainRotator.localRotation = Quaternion.Euler(0, Mathf.Lerp(lastCameraAngle, newCameraAngle, cameraLerp), 0);
                    camRandomOffset.localPosition = Vector3.Lerp(lastRandomOffset, newRandomOffset, cameraLerp);
                    camRandomOffset.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(lastCameraTilt, newCameraTilt, cameraLerp));
                    //camSlide.localPosition = Vector3.Lerp(camSlide.localPosition, Vector3.zero, cameraLerp);
                }
                else camSlide.localPosition += slide * Time.deltaTime;
                cameraLerp += Time.deltaTime;
                break;
            case GameState.WIN:
                if (timer < winTimeDelay) timer += Time.deltaTime;
                else
                {
                    if (victoryScreen.transform.localPosition.x > 30) victoryScreen.transform.localPosition -= new Vector3(2000, -2000, 0) * Time.deltaTime;
                    else victoryScreen.transform.localPosition = Vector3.zero + Random.insideUnitSphere * victoryScreenShake;
                }
                break;
        }

        if(state != GameState.DEBATE && state != GameState.WIN)
        {
            newCameraAngle -= 30 * Time.deltaTime;
            camMainRotator.localRotation = Quaternion.Euler(0, newCameraAngle, -10);
        }

        if (firing)
        {
            gunObject.transform.localScale = Vector3.one + 2 * Vector3.one * fireTime / fireDelay;
            shotText.transform.localPosition += shotText.transform.forward * 30 * Time.deltaTime * fireDirection;
            fireTime -= Time.deltaTime;
            if (fireTime < 0)
            {
                firing = false;
                shotText.transform.localPosition = new Vector3(0, -100, 0);
            }
        }
    }

    void ShotTextHit(Collider other)
    {
        if (won) return;

        if(other.gameObject.tag == "Main Weak Point" && bulletIndex == 0)
        {
            if(bulletIndex == 0)
            {
                won = true;
                breakSound.Play();
                Instantiate(breakParticles, shotText.transform.position, new Quaternion());
                state = GameState.WIN;
                timer = 0;
            }
        }
        else if(other.gameObject.tag != "Untagged")
        {
            other.transform.parent.GetComponent<NonstopDebateText>().tempShake = 0.25f;
            reflectSound.Play();
            fireDirection = -1;
        }
        else
        {
            reflectSound.Play();
            fireDirection = -1;
        }
    }
}
