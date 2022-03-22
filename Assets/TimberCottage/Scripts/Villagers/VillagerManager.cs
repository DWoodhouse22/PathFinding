using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    /// <summary>
    /// Class to handle management of villagers
    /// </summary>
    public class VillagerManager : MonoBehaviour
    {
        [SerializeField] private VillagerBase villagerBasePrefab;
        [SerializeField] private VillagerBuilder villagerBuilderPrefab;
        [SerializeField] private VillagerCarrier villagerCarrierPrefab;

        [SerializeField] private Transform villagerPoolTransform;
        [SerializeField] private Transform builderPoolTransform;
        [SerializeField] private Transform carrierPoolTransform;
        
        private List<VillagerBase> _availableVillagers;
        private List<VillagerBuilder> _builders;
        private List<VillagerCarrier> _carriers;

        private int _totalSpawnedBaseVillagers;
        private int _totalSpawnedBuilders;
        private int _totalSpawnedCarriers;
        
        public enum EVillagerType
        {
            Undefined = 0,
            Base,
            Builder,
            Carrier
        }
        
        private void Awake()
        {
            _availableVillagers = new List<VillagerBase>();
            _builders = new List<VillagerBuilder>();
            _carriers = new List<VillagerCarrier>();
            _villagerRequests = new Queue<VillagerRequest>();
        }

        /// <summary>
        /// Spawn a basic villager at location
        /// </summary>
        /// <param name="spawnLocation">World position</param>
        /// <param name="rotation">Rotation</param>
        public void SpawnVillager(Vector3 spawnLocation, Quaternion rotation)
        {
            VillagerBase v = Instantiate(villagerBasePrefab, spawnLocation, rotation, villagerPoolTransform);
            v.name = $"BaseVillager{_totalSpawnedBaseVillagers}";
            _availableVillagers.Add(v);
            _totalSpawnedBaseVillagers++;
        }

        /// <summary>
        /// Reallocate a basic villager as a builder
        /// </summary>
        public void AllocateBuilder()
        {
            if (_availableVillagers.Count <= 0)
            {
                Debug.Log("No villagers available");
                return;
            }
            
            VillagerBase villager = _availableVillagers.First();
            Transform villagerTransform = villager.transform;
            VillagerBuilder builder = Instantiate(villagerBuilderPrefab, villagerTransform.position, 
                villagerTransform.rotation, builderPoolTransform);
            builder.name = $"Builder{_totalSpawnedBuilders}";
            _totalSpawnedBuilders++;
            _availableVillagers.Remove(villager);
            _builders.Add(builder);
            Destroy(villager.gameObject);

            DebugPrintVillagerCounts();
        }

        /// <summary>
        /// Reallocate a basic villager as a carrier
        /// </summary>
        public void AllocateCarrier()
        {
            if (_availableVillagers.Count <= 0)
            {
                Debug.Log("No villagers available");
                return;
            }

            VillagerBase villager = _availableVillagers.First();
            Transform villagerTransform = villager.transform;
            VillagerCarrier carrier = Instantiate(villagerCarrierPrefab, villagerTransform.position, 
                villagerTransform.rotation, carrierPoolTransform);
            carrier.name = $"Carrier{_totalSpawnedCarriers}";
            _totalSpawnedCarriers++;
            _availableVillagers.Remove(villager);
            _carriers.Add(carrier);
            Destroy(villager.gameObject);

            DebugPrintVillagerCounts();
        }

        public void DeAllocateBuilder()
        {
            if (_builders.Count <= 0)
            {
                Debug.Log("No builders available");
                return;
            }

            VillagerBuilder builder = _builders.First();
            _builders.Remove(builder);
            Transform builderTransform = builder.transform;
            SpawnVillager(builderTransform.position, builderTransform.rotation);
            Destroy(builder.gameObject);
            
            DebugPrintVillagerCounts();
        }

        public void DeAllocateCarrier()
        {
            if (_carriers.Count <= 0)
            {
                Debug.Log("No carriers available");
                return;
            }

            VillagerCarrier carrier = _carriers.First();
            _carriers.Remove(carrier);
            Transform carrierTransform = carrier.transform;
            SpawnVillager(carrierTransform.position, carrierTransform.rotation);
            Destroy(carrier.gameObject);

            DebugPrintVillagerCounts();
        }

        private Queue<VillagerRequest> _villagerRequests;
        /// <summary>
        /// Request a villager for a job
        /// </summary>
        /// <param name="villagerType">Type of villager requested</param>
        /// <param name="callback">Callback fired when villager is available</param>
        public void RequestVillager(EVillagerType villagerType, Action<VillagerBase> callback)
        {
            Debug.Log($"Requesting {villagerType}");
            VillagerRequest request = new VillagerRequest(villagerType, callback);
            _villagerRequests.Enqueue(request);
        }

        private void Update()
        {
            // This is fine in update for now.. should probably be a coroutine?
            if (_villagerRequests.Count == 0)
            {
                return;
            }

            VillagerRequest request = _villagerRequests.Peek();
            switch (request.VillagerType)
            {
                case EVillagerType.Carrier:
                    RequestCarrier(request);
                    break;
                case EVillagerType.Builder:
                    break;
                case EVillagerType.Base:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RequestCarrier(VillagerRequest request)
        {
            VillagerCarrier carrier = _carriers.Find(x => x.IsBusy == false);
            if (carrier == null)
            {
                return;
            }
            
            _carriers.Remove(carrier);
            request.Callback(carrier);
            carrier.AssignJob();
            _villagerRequests.Dequeue();
        }

        public void ReturnVillagerToThePool(VillagerBase villager)
        {
            switch (villager.VillagerType)
            {
                case EVillagerType.Carrier:
                    villager.UnAssignJob();
                    _carriers.Add(villager as VillagerCarrier);
                    break;
                case EVillagerType.Builder:
                    villager.UnAssignJob();
                    _builders.Add(villager as VillagerBuilder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly struct VillagerRequest
        {
            public EVillagerType VillagerType { get; }
            public Action<VillagerBase> Callback { get; }

            public VillagerRequest(EVillagerType villagerType, Action<VillagerBase> callback)
            {
                VillagerType = villagerType;
                Callback = callback;
            }
        }

        private void DebugPrintVillagerCounts()
        {
            // Debug.Log($"available:{_availableVillagers.Count}\tBuilders:{_builders.Count}" +
            //           $"\tCarriers:{_carriers.Count}\tAll{_allVillagers.Count}");
        }
    }
}