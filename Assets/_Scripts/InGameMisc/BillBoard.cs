using UnityEngine;

namespace _Scripts
{
    public class BillBoard : MonoBehaviour
    {
        private Camera _camera;

        private void Start() => _camera = Camera.main;

        private void LateUpdate() => transform.forward = _camera.transform.forward;
    }
}
