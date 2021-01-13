using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretIcon : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Image fill;
    [SerializeField] Image full;

    [SerializeField] Transform HeatWarning;

    float respawnTime;
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
        if (fill.fillAmount < 1)
        {
            fill.fillAmount += Time.deltaTime / respawnTime;
        }
        else if (!full.gameObject.activeSelf)
        {
            full.gameObject.SetActive(true);
        }
    }

    public void Destroyed()
    {
        fill.fillAmount = 0;
        full.gameObject.SetActive(false);
    }

    #region Heat
    
    public void ShowHeatShutdown(bool value)
    {
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

        HeatWarning.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        HeatWarning.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        warningActive = false;
    }

    #endregion

    #region missles
    /*
    public void AddMisslesToHUD(int maxMissles)
    {
        MissleStatus.gameObject.SetActive(true);

        missleIcons = new List<Image>();
        for (int i = 0; i <maxMissles; i++)
        {
            Image ri = Instantiate(rocketIconPrefab);
            missleIcons.Add(ri);
            ri.transform.SetParent(MissleIconParent);
        }
    }

    public void SetMissles(int amount)
    {
        foreach (Image im in missleIcons)
        {
            im.gameObject.SetActive(false);
        }

        for (int i = 0; i < amount; i++)
        {
            missleIcons[i].gameObject.SetActive(true);
        }
    }
    */
    #endregion


}
