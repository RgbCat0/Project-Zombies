using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    public class CamMover : MonoBehaviour
    {
        public static Transform CamHolder;

        void LateUpdate()
        {
            transform.position = CamHolder.position;
        }
    }
}
