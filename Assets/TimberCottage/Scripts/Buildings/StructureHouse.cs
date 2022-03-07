using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  TimberCottage.Pathfinding
{
    public class StructureHouse : StructureBase
    {
        [SerializeField] private Transform unitExit;

        private readonly int _numVillagersToSpawn = 5;
        private readonly float _delayBetweenVillagerSpawn = 1f;
        private IEnumerator _onConstructedRoutine;

        public override void OnPlaced()
        {
            base.OnPlaced();
        }

        public override void OnConstructed()
        {
            base.OnConstructed();

            _onConstructedRoutine = OnConstructedCoroutine();
            StartCoroutine(_onConstructedRoutine);
        }

        private IEnumerator OnConstructedCoroutine()
        {
            int villagersSpawned = 0;
            while (villagersSpawned < _numVillagersToSpawn)
            {
                _villagerManager.SpawnVillager(unitExit.position, unitExit.rotation);
                villagersSpawned++;
                yield return new WaitForSeconds(_delayBetweenVillagerSpawn);
            }
        }
    }
}
