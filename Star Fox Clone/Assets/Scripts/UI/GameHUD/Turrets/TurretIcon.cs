using ProtocFiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretIcon : MonoBehaviour
{
    //Destroyed
    [SerializeField] Image bg;
    [SerializeField] Image fill;
    [SerializeField] Image full;
    //Missles
    [SerializeField] Image reloadFill;
    [SerializeField] Text missles;
    //Heat
    [SerializeField] Transform HeatWarning;

    float respawnTime;
    float reloadTime;
    //float timeLeft;

    public void Initiate(TurretMount turretMount, Sprite sprite)
    {
        LinkToTurretMount(turretMount);
        linkToTurret(turretMount.MyTurret);
        
        //HP Icon
        bg.sprite = sprite;
        fill.sprite = sprite;
        full.sprite = sprite;

        respawnTime = turretMount.RespawnTime;

        if (turretMount.MyTurretType == TurretClass_P.Msl)
        {
            missles.gameObject.SetActive(true);
            reloadTime = turretMount.MyTurret.Data.cooldown;
        }
    }

    void LinkToTurretMount(TurretMount turretMount)
    {
        turretMount.TurretDestroyed += Destroyed;
    }

    void linkToTurret(Turret turret)
    {
        turret.ConnectToUI(this);
    }

    private void Update()
    {
        #region Destruction
        if (fill.fillAmount < 1)
        {
            fill.fillAmount += Time.deltaTime / respawnTime;
        }
        else if (!full.gameObject.activeSelf)
        {
            full.gameObject.SetActive(true);
        }
        #endregion

        #region Missles
        if (reloadFill.fillAmount < 1)
        {
            reloadFill.fillAmount += Time.deltaTime / reloadTime;
        }
        else if(reloadFill.gameObject.activeSelf)
        {
            reloadFill.gameObject.SetActive(false);
        }
        #endregion
    }

    public void Destroyed()
    {
        fill.fillAmount = 0;
        full.gameObject.SetActive(false);
    }

    #region Heat
    
    public void ShowHeatShutdown(bool value)
    {
        //Play sound
        StopCoroutine("BlinkingLights");
        warningActive = value;
        HeatWarning.gameObject.SetActive(value);
    }

    bool warningActive;

    public void ShowHeatWarning(bool value)
    {
        if (value == warningActive) return;
        warningActive = value;


        if (warningActive)
            StartCoroutine("BlinkingLights");
        else
        {
            StopCoroutine("BlinkingLights");
            HeatWarning.gameObject.SetActive(false);
        }
    }

    IEnumerator BlinkingLights()
    {
        //Play sound

        HeatWarning.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        StartCoroutine("BlinkingLights");
    }

    #endregion

    #region missles

    public void LoadingMissle()
    {
        reloadFill.fillAmount = 0;
        reloadFill.gameObject.SetActive(true);
    }

    public void SetMissles(int amount)
    {
        missles.text = amount.ToString();
    }
    #endregion

}
