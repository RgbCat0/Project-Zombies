using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    public class CamMover : MonoBehaviour
    {
        public static Transform CamHolder;

        void LateUpdate()
        {
            if (CamHolder is not null) // Check if CamHolder is assigned
                transform.position = CamHolder.position;
        }
    }
}
