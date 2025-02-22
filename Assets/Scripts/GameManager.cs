using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public enum PlanetType
{
    EarthLike = 0,
    Frozen = 1,
    Lava = 2,
    Venom = 3,
    Vecumm = 4,
}
public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            if (instance == null)
            {
                GameObject obj = new("GameManager");
                instance = obj.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    [SerializeField] private UIManager ui;
    [SerializeField] private Transform ship;
    [SerializeField] private GameObject warpEffect;
    [SerializeField] private List<GameObject> planetsPrefabs;

    [SerializeField] private float targetDistance;

    private Transform currentPlanet;
    private Transform newPlanet;

    private PlanetStatus currentStatus;
    private PlanetStatus newStatus;

    public bool isWarping = false;
    private bool isFinishingWarp = false;
    private float deceleration = 10f;

    private float rotationSpeed = 1f;
    private float rotationThreshold = 0.1f;
    private bool isRotating = false;


    [SerializeField] private float currentSpeed = 0f;
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


    [SerializeField] ParticleSystem effect1;
    [SerializeField] ParticleSystem effect2;

    public ShipStatus shipStatus;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        shipStatus = new ShipStatus(100, 100, 0);
    }
    private void Start()
    {
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }

   
        //shipStatus.OnPopulationChanged += UpdatePopulation;
        //shipStatus.OnMalfunctionsChanged += UpdateMalfunctions;


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
        if (isWarping && shipStatus.Energy < 0)
        {
            ui.StartFading();
            ui.SetEnergyText();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ui.StartFading();
        //    ui.SetPopuText();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ui.StartFading();
        //    ui.SetMalfText();
        //}
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

        UpdateEffects(effect1);
        UpdateEffects(effect2);


        
        if (effect2 != null)
        {
            effect2.gameObject.SetActive(true);
        }

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
            var energyCost = Random.Range(10, 15);
            StartCoroutine(ConsumeEnergyOverTime(energyCost));
        }
    }
    public void OnWrapButtonClick()
    {
        if (isWarping || isRotating) return;


        if (currentPlanet != null)
        {
            currentPlanet.gameObject.GetComponent<PlanetInfoDisplay>().ClosePlane();
        }

        

        isRotating = true;
        currentSpeed = 0f;

        noiseOffsetX = Random.Range(0, 100f);
        noiseOffsetY = Random.Range(0, 100f);


        var predictedPosition = ship.position + (ship.forward * (targetDistance + maxWarpSpeed * 2));

        float randomX = Random.Range(-targetDistance, targetDistance);
        float randomY = Random.Range(-targetDistance, targetDistance);
        float randomZ = Random.Range(-targetDistance, targetDistance);

        var finalPosition = predictedPosition + (ship.right * randomX) + (ship.up * randomY) + (ship.forward * randomZ);

        var tempPlanet = PlanetStatus.GenerateRandomPlanet();
        var type = tempPlanet.GetPlanetTypy();
        var planetPrefab = planetsPrefabs[(int)type];
        newPlanet = Instantiate(planetPrefab, finalPosition, Quaternion.identity).transform;

        var display = newPlanet.gameObject.GetComponent<PlanetInfoDisplay>();
      

        display.SetPlanetData(tempPlanet);

        display.landButton.onClick.AddListener(() => ConsumePopuAndMalf());
        display.landButton.onClick.AddListener(() => display.CloseLandButton());
        newPlanet.gameObject.SetActive(true);
        StartCoroutine(WarpSequence());
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
        if (effect1 != null)
        {
            effect1.gameObject.SetActive(true);
        }
        isWarping = true;
    }


    private IEnumerator FinishWarp()
    {
        if (isFinishingWarp) yield break;
        isFinishingWarp = true;
        while (currentSpeed > 4f)
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 4.93f);
            if (currentSpeed < 4.94f)
            {
                if (effect1 != null)
                {
                    effect1.gameObject.SetActive(false);
                }
                if (effect2 != null)
                {
                    effect2.gameObject.SetActive(false);
                }

                currentSpeed = 0;
                break;
            }
            yield return null;
        }

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

    private void UpdateEffects(ParticleSystem ps)
    {
        if (ps == null)
        {
            return;
        }

        var main = ps.main;
        main.startSize = Mathf.Lerp(0.1f, 1.0f, currentSpeed / maxWarpSpeed);

        var emission = ps.emission;
        emission.rateOverTime = Mathf.Lerp(10f, 120f, currentSpeed / maxWarpSpeed);

        main.startLifetime = Mathf.Lerp(1.5f, 2f, currentSpeed / maxWarpSpeed);
    }
    private IEnumerator ConsumeEnergyOverTime(float totalEnergyToLose)
    {
        float energyLost = 0f;

        while (isWarping && energyLost < totalEnergyToLose && shipStatus.Energy > 0)
        {
            float speedFactor = Mathf.Max(0.1f, currentSpeed / maxWarpSpeed);
            float energyLossPerStep = 1f;
            shipStatus.Energy -= energyLossPerStep; 
            energyLost += energyLossPerStep;

            float waitTime = Mathf.Lerp(1.2f, 1.8f, speedFactor);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void ConsumePopuAndMalf()
    {
        var popuCost = Random.Range(10, 50);
        var malfCost = Random.Range(10, 50);

        shipStatus.Population -= popuCost;
        shipStatus.Malfunctions += malfCost;

        if (shipStatus.Population < 0)
        {
            ui.StartFading();
            ui.SetPopuText();

        }
        else if (shipStatus.Malfunctions >= 100)
        {
            ui.StartFading();
            ui.SetMalfText();
        }
        
    }
}
