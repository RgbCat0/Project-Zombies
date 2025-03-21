using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    public class CamMover : MonoBehaviour
    {
        [SerializeField]
        private Transform camHolder;
        void LateUpdate()
        {
        transform.position = camHolder.position;
        }
    }
}
