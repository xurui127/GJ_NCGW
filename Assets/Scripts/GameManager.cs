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
    [SerializeField] private float warpSpeed = 50f;
    private float deceleration = 5f;

    private float rotationSpeed = 1f;
    private float rotationThreshold = 0.1f;
    private bool isRotating = false;


    private float currentSpeed = 0f;
    [SerializeField] private float maxWarpSpeed = 80f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float declerationDistance = 150f;

    [SerializeField] private float shakeAmount = 0.5f;
    [SerializeField] private float shakeSpeed = 5f;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraShakeAmount = 0.5f;
    [SerializeField] private float cameraShakeSpeed = 5f;
    private Vector3 originalCameraPosition;

    private float noiseOffsetX;
    private float noiseOffsetY;

    private void Start()
    {
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }
    }
    private void Update()
    {
        if (isRotating)
        {
            RotateTowardsTarget();
        }

        if (isWarping)
        {

            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (newPlanet == null) return;

        Vector3 directionToPlanet = (newPlanet.position - ship.position).normalized;
        Vector3 targetStopPosition = newPlanet.position - (directionToPlanet * 100f);

        // Ship shake 
        float dynamicShakeAmount = Mathf.Lerp(0f, shakeAmount, currentSpeed / maxWarpSpeed);
        float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed + noiseOffsetX, 0) - 0.5f) * dynamicShakeAmount;
        float shakeY = (Mathf.PerlinNoise(0, Time.time * shakeSpeed + noiseOffsetY) - 0.5f) * dynamicShakeAmount;
        Vector3 shakeOffset = ship.right * shakeX + ship.up * shakeY;

        ApplyCameraShake();
        float distanceToTarget = Vector3.Distance(ship.position, targetStopPosition);

        if (currentSpeed < maxWarpSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        if (distanceToTarget < declerationDistance)
        {
            float slowFactor = distanceToTarget / declerationDistance;
            currentSpeed = Mathf.Lerp(5f, maxWarpSpeed, slowFactor);
        }

        ship.position = Vector3.MoveTowards(ship.position, targetStopPosition, currentSpeed * Time.deltaTime) + shakeOffset;

        if (distanceToTarget < 100f)
        {
            StartCoroutine(FinishWarp());
            noiseOffsetX = 0;
            noiseOffsetY = 0;
        }
    }
    private void RotateTowardsTarget()
    {
        if (newPlanet == null) return;

        Vector3 directionToPlanet = (newPlanet.position - ship.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlanet);

        ship.rotation = Quaternion.Slerp(ship.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angleDifference = Quaternion.Angle(ship.rotation, targetRotation);
        if (angleDifference < rotationThreshold)
        {
            isRotating = false;
            isWarping = true;
        }
    }
    public void OnWrapButtonClick()
    {
        if (isWarping || isRotating) return;

        isRotating = true;
        currentSpeed = 0f;

        noiseOffsetX = Random.Range(0, 100f);
        noiseOffsetY = Random.Range(0, 100f);


        var predictedPosition = ship.position + (ship.forward * (targetDistance + maxWarpSpeed * 2));

        float randomX = Random.Range(-targetDistance, targetDistance);
        float randomY = Random.Range(-targetDistance, targetDistance);
        float randomZ = Random.Range(-targetDistance, targetDistance);

        var finalPosition = predictedPosition + (ship.right * randomX) + (ship.up * randomY) + (ship.forward * randomZ);

        newPlanet = Instantiate(planetsPrefabs[RandomPlanet()], finalPosition, Quaternion.identity).transform;
        newPlanet.gameObject.SetActive(true);

        StartCoroutine(WarpSequence());
    }

    private int RandomPlanet()
    {
        return Random.Range(0, planetsPrefabs.Count);
    }

    private IEnumerator WarpSequence()
    {
        Vector3 directionToPlanet = (newPlanet.position - ship.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlanet);

        while (Quaternion.Angle(ship.rotation, targetRotation) > rotationThreshold)
        {
            ship.rotation = Quaternion.RotateTowards(ship.rotation, targetRotation, rotationSpeed * 50f * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        isWarping = true;
    }


    private IEnumerator FinishWarp()
    {
        if (isFinishingWarp) yield break;
        isFinishingWarp = true;
        while (currentSpeed > 0)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            yield return null;
        }

        currentSpeed = 0;

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
    private void ApplyCameraShake()
    {
        if (cameraTransform == null) return;

        float dynamicCameraShake = Mathf.Lerp(0f, cameraShakeAmount, currentSpeed / maxWarpSpeed);

        float shakeX = (Mathf.PerlinNoise(Time.time * cameraShakeSpeed, 0) - 0.5f) * dynamicCameraShake;
        float shakeY = (Mathf.PerlinNoise(0, Time.time * cameraShakeSpeed) - 0.5f) * dynamicCameraShake;

        cameraTransform.localPosition = originalCameraPosition + new Vector3(shakeX, shakeY, 0);
    }
}
