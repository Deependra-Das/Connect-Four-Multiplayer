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
        [SerializeField] private float _angularVelocity;

        private List<GameObject> _spawnedTrails = new List<GameObject>();

        void Start()
        {
            SpawnTrails();
        }

        void SpawnTrails()
        {
            float radiusIncrement = (_numberOfPlanets > 1)
          ? (_orbitRadiusMax - _orbitRadiusMin) / (_numberOfPlanets - 1)
          : 0f;

            for (int i = 0; i < _numberOfPlanets; i++)
            {
                GameObject selectedTrailPrefab = _trailPrefabs[i % _trailPrefabs.Length];

                float orbitRadius = _orbitRadiusMin + i * radiusIncrement;
                float startAngle = Mathf.PI; // LEFT SIDE

                GameObject newTrailObject = Instantiate(selectedTrailPrefab);
                newTrailObject.transform.SetParent(_centerPivot.transform);

                float x = Mathf.Cos(startAngle) * orbitRadius;
                float y = Mathf.Sin(startAngle) * orbitRadius;
                newTrailObject.transform.position = new Vector3(x, y, 0f);

                TrailOrbit orbit = newTrailObject.GetComponent<TrailOrbit>();
                orbit.centerPivot = _centerPivot;
                orbit.orbitRadius = orbitRadius;
                orbit.angle = startAngle;
                orbit.angularVelocity = _angularVelocity;

                _spawnedTrails.Add(newTrailObject);
            }
        }

        void OnDestroy()
        {
            foreach (GameObject trail in _spawnedTrails)
            {
                if (trail != null)
                {
                    Destroy(trail);
                }
            }
            _spawnedTrails.Clear();
        }
    }
}
