using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDrops : StationaryWeapon
{
    protected override IEnumerator Fire()
    {
        startReloading();

        //Debugging
        yield return new WaitForSeconds(3f);

        GameObject b = GameObject.Instantiate(data.bombData.visuals);
        b.transform.position = transform.position;
        b.transform.rotation = transform.rotation;
        b.layer = 11;

        EnemyBomb bomb = b.AddComponent<EnemyBomb>();
        bomb.Initialize(data.bombData);

        yield return new WaitForSeconds(0.1f);
    }

    protected override void aim()
    {
        return;
    }
}
