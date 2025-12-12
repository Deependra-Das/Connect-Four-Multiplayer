using System.Collections.Generic;
using UnityEngine;

namespace ConnectFourMultiplayer.VFX
{
    public class TrailSpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject _centerPivot;
        [SerializeField] private GameObject[] _trailPrefabs;
        [SerializeField] private int _numberOfPlanets;
        [SerializeField] private float _orbitRadiusMin;
        [SerializeField] private float _orbitRadiusMax;
        [SerializeField] private float _orbitSpeedMin;
        [SerializeField] private float _orbitSpeedMax;

        private List<GameObject> _spawnedTrails = new List<GameObject>();

        void Start()
        {
            SpawnTrails();
        }

        void SpawnTrails()
        {
            float angleIncrement = 2 * Mathf.PI / _numberOfPlanets;

            for (int i = 0; i < _numberOfPlanets; i++)
            {
                GameObject selectedTrailPrefab = _trailPrefabs[i % _trailPrefabs.Length];
                float orbitRadius = Random.Range(_orbitRadiusMin, _orbitRadiusMax);
                float orbitSpeed = Random.Range(_orbitSpeedMin, _orbitSpeedMax);
                float startAngle = i * angleIncrement;

                GameObject newTrailObject = Instantiate(selectedTrailPrefab);
                newTrailObject.transform.parent = _centerPivot.transform;

                float x = Mathf.Cos(startAngle) * orbitRadius;
                float y = Mathf.Sin(startAngle) * orbitRadius;
                newTrailObject.transform.position = _centerPivot.transform.position + new Vector3(x, y, 0f);

                TrailOrbit orbit = newTrailObject.GetComponent<TrailOrbit>();
                orbit.centerPivot = _centerPivot;
                orbit.orbitSpeed = orbitSpeed;
                orbit.orbitRadius = orbitRadius;
                orbit.angle = startAngle;

                _spawnedTrails.Add(newTrailObject);
            }
        }

        void OnDestroy()
        {
            foreach (GameObject planet in _spawnedTrails)
            {
                if (planet != null)
                {
                    Destroy(planet);
                }
            }
            _spawnedTrails.Clear();
        }
    }
}
