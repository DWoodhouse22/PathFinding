using System.Collections;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerCarrierBehaviour : MonoBehaviour, IVillagerBehaviour
    {
        public IEnumerator BehaviourRoutine { get; set; }
        public IEnumerator BehaviourCoroutine()
        {
            yield return null;
        }

        public void StartBehaviour()
        {
            Debug.Log("Carrier behaviour not defined");
        }

        public void StopBehaviour()
        {
            Debug.Log("Carrier behaviour not defined");
        }
    }
}