using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretIcon : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Image fill;
    [SerializeField] Image full;
    [SerializeField] Image reloadFill;
    [SerializeField] Text missles;

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

        if (turretMount.MyTurretType == TurretType.Missiles)
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

    public void ShowHeatWarning()
    {
        if (warningActive) return;
        warningActive = true;
        StartCoroutine("BlinkingLights");
    }

    IEnumerator BlinkingLights()
    {
        //Play sound
        for (int i = 0; i < 20; i++)
        {
            HeatWarning.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            HeatWarning.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

        warningActive = false;
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
