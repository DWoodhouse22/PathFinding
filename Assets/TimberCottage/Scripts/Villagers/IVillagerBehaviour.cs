using System.Collections;

namespace TimberCottage.Pathfinding
{
    public interface IVillagerBehaviour
    {
        public IEnumerator BehaviourRoutine { set; get; }
        public IEnumerator BehaviourCoroutine();

        public void StartBehaviour();
        public void StopBehaviour();
    }
}