using System.Collections;
using UnityEngine;

namespace TimberCottage.Pathfinding
{
    public class VillagerBuilderBehaviour : MonoBehaviour, IVillagerBehaviour
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