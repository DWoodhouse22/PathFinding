using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TimberCottage.Pathfinding
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private Transform meshRoot;

        private MeshFilter[] _meshes;
        private IEnumerator _growTreeRoutine;
        private bool _isGrown;
        private readonly float _timeToMature = 30f;
        private readonly Vector3 _startingScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        private void Awake()
        {
            meshRoot.transform.localScale = _startingScale;
            _meshes = meshRoot.GetComponentsInChildren<MeshFilter>();
            DisplayRandomMesh();
        }

        private void DisplayRandomMesh()
        {
            foreach (MeshFilter mesh in _meshes)
            {
                mesh.gameObject.SetActive(false);
            }

            int r = Random.Range(0, _meshes.Length);
            _meshes[r].gameObject.SetActive(true);
        }

        private void Start()
        {
            StartGrowth();
        }

        private void StartGrowth()
        {
            if (_growTreeRoutine != null || _isGrown)
            {
                return;
            }

            _growTreeRoutine = TreeGrowthRoutine();
            StartCoroutine(_growTreeRoutine);
        }

        private void OnFullyGrown()
        {
            _isGrown = true;
        }

        private IEnumerator TreeGrowthRoutine()
        {
            float timeElapsed = 0f;
            while (timeElapsed <= _timeToMature)
            {
                yield return new WaitForEndOfFrame();
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / _timeToMature;
                meshRoot.localScale = Vector3.Lerp(_startingScale, Vector3.one, t);
            }
            
            meshRoot.localScale = Vector3.one;
            _growTreeRoutine = null;
            OnFullyGrown();
        }
    }
}