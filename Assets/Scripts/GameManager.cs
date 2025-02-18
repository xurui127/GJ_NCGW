using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlanetType
{
    Normal = 0,
    Frozen = 1,
    Lava = 2,
    Venom = 3,
    Vecumm = 4,
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform ship;
    [SerializeField] private GameObject warpEffect;
    [SerializeField] private List<GameObject> planetsPrefabs;

    [SerializeField] private float targetDistance;

    private Transform currentPlanet;
    private Transform newPlanet;

    private bool isWarping = false;
    private bool isFinishingWarp = false;
    private float warpSpeed = 50f;
    private float deceleration = 5f;


    private void Update()
    {
        if (isWarping)
        {
            ship.position += ship.forward * warpSpeed * Time.deltaTime;

            if (newPlanet != null && Vector3.Distance(ship.position, newPlanet.position) < 100f)
            {
                StartCoroutine(FinishWarp());
            }
        }
    }

    public void OnWrapButtonClick()
    {
        if (isWarping) return;

        isWarping = true;

        var predictedPosition = ship.position + (ship.forward * (targetDistance + warpSpeed * 2f));
        newPlanet = Instantiate(planetsPrefabs[RandomPlanet()], predictedPosition, Quaternion.identity).transform;

        newPlanet.gameObject.SetActive(true);
        //StartCoroutine(WarpSequence());
    }

    private int RandomPlanet()
    {
        return Random.Range(0, planetsPrefabs.Count);
    }

    private IEnumerator WarpSequence()
    {
        yield return new WaitForSeconds(2f);

        if (newPlanet != null)
        {
            newPlanet.gameObject.SetActive(true);
        }
        //warpEffect.gameObject.SetActive(false);
    }

    private IEnumerator FinishWarp()
    {
        if (isFinishingWarp) yield break;
        isFinishingWarp = true;
        while (warpSpeed > 0)
        {
            warpSpeed -= deceleration * Time.deltaTime;
            yield return null;
        }

        warpSpeed = 0;
        isWarping = false;

        if (currentPlanet != null)
        {
            Destroy(currentPlanet.gameObject);
        }
        if (newPlanet != null)
        {

            currentPlanet = newPlanet;
        }
        isFinishingWarp = false;
    }
}
