using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake _instance;

    public static CameraShake Instance
    {
        get
        {
            if (_instance == null)
            {
                var instances = FindObjectsOfType<CameraShake>();
                if (instances.Length > 1) Debug.LogError("Multiple Instances of CameraShake in scene");
                else if (instances.Length == 0) Debug.LogError("No instances of CameraShake in scene");
                else _instance = instances[0];
            }

            return _instance;
        }
    }

    private float _trauma;

    public static float Trauma { get { return Instance._trauma; } set { Instance._trauma = value; } }

    private float _seed;

    private float _elapsedTime;

    public static Vector3 TargetPos;

    // Use this for initialization
    private void Start()
    {
        _seed = Random.Range(0, 999999);
    }

    // Update is called once per frame
    private void Update()
    {
        _trauma -= Time.deltaTime;
        _trauma = Mathf.Clamp(_trauma, 0, 1);

        var xPos = GetPerlin(_seed, 100, 1);
        var yPos = GetPerlin(_seed + 1, 100, 1);
        var rot = GetPerlin(_seed + 2, 100, 10);
        transform.position = new Vector3(TargetPos.x + xPos, TargetPos.y + yPos, TargetPos.z);
        transform.eulerAngles = new Vector3(0, 0, rot);
        _elapsedTime += Time.deltaTime;
    }

    private float GetPerlin(float newSeed, float frequency, float strength)
    {
        var noise = Mathf.PerlinNoise(newSeed + _elapsedTime * frequency, newSeed + _elapsedTime * frequency);
        noise = noise * 2 - 1;
        return noise * Mathf.Pow(_trauma, 2) * strength;
    }

}
