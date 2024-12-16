using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoveGameScripts
{
    public class PositionProvider : MonoBehaviour
    {
        [SerializeField] private Transform startPositionerTransform;
        [SerializeField] private Transform linePositionerTransform;
        [SerializeField] private Transform finishPositionerTransform;

        [SerializeField, BoxGroup] private float startPosXOffset;
        [SerializeField, BoxGroup] private float startPosZOffset;
        [SerializeField, BoxGroup] private float linePosOffset;

        public Vector3 GetRandomStartPosition()
        {
            var randomX = Random.Range(-startPosXOffset, startPosXOffset);
            var randomZ = Random.Range(-startPosZOffset, startPosZOffset);
            
            var position = startPositionerTransform.position;
            position.x += randomX;
            position.z += randomZ;
            return position;
        }

        public Vector3 GetLinePosition(int index)
        {
            var position = linePositionerTransform.position;
            position.x -= linePosOffset * index;
            return position;
        }

        public Vector3 GetFinishPosition()
        {
            return finishPositionerTransform.position;
        }
    }
}
