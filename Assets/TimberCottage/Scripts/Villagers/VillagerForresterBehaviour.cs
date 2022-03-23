using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerForresterBehaviour : MonoBehaviour, IVillagerBehaviour
    {
        public IEnumerator BehaviourRoutine { get; set; }
        public IEnumerator BehaviourCoroutine()
        {
            yield return null;
        }

        public void StartBehaviour()
        {
            
        }

        public void StopBehaviour()
        {
            
        }
    }
}