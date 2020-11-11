using UnityEngine;
using UnityEngine.UI;

public class TurretIcon : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Image fill;
    [SerializeField] Image full;
    float respawnTime;
    //float timeLeft;

    public void Initiate(TurretMount turretMount, Sprite sprite)
    {
        LinkToTurretMount(turretMount);

        bg.sprite = sprite;
        fill.sprite = sprite;
        full.sprite = sprite;

        respawnTime = turretMount.RespawnTime;
    }

    public void LinkToTurretMount(TurretMount turretMount)
    {
        turretMount.InformUIAboutDestruction.AddListener(Destroyed);
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
}
