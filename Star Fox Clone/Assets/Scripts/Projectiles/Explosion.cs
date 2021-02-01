using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float timeBeforeDespawn = 2f;

    public void Instantiate(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }

    void Start()
    {
        StartCoroutine(despawn());
    }

    IEnumerator despawn()
    {
        yield return new WaitForSeconds(timeBeforeDespawn);
        Destroy(gameObject);
    }
}
