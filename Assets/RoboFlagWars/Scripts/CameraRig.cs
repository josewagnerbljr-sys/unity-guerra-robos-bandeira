using UnityEngine;

namespace RoboFlagWars
{
    /// <summary>Camera 3a pessoa. A mira (centro da tela) define o ponto-alvo; os lasers dos olhos convergem nele.</summary>
    public class CameraRig : MonoBehaviour
    {
        public Transform target;
        public Camera cam;
        public float distance = 5.2f, height = 1.9f;
        float yaw, pitch = 14f, orbit;

        public Vector3 FlatForward { get { return Quaternion.Euler(0, yaw, 0) * Vector3.forward; } }
        public Vector3 FlatRight { get { return Quaternion.Euler(0, yaw, 0) * Vector3.right; } }

        public void SetYaw(float y) { yaw = y; }
        public void AddLook(Vector2 d)
        {
            yaw += d.x * 0.12f;
            pitch = Mathf.Clamp(pitch - d.y * 0.1f, -12f, 55f);
        }

        void LateUpdate()
        {
            if (target == null)
            {
                orbit += Time.deltaTime * 0.08f;
                transform.position = new Vector3(Mathf.Sin(orbit) * 55f, 24f, Mathf.Cos(orbit) * 48f);
                transform.LookAt(new Vector3(0, 2, 0));
                return;
            }
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            Vector3 pivot = target.position + Vector3.up * height;
            Vector3 desired = pivot - rot * Vector3.forward * distance + rot * Vector3.right * 0.7f;
            Vector3 dir = desired - pivot;
            float len = dir.magnitude;
            dir /= len;
            float best = len;
            foreach (var h in Physics.SphereCastAll(pivot, 0.25f, dir, len, ~0, QueryTriggerInteraction.Ignore))
            {
                if (h.distance <= 0.0001f) continue;
                if (h.collider.GetComponentInParent<RobotController>() != null) continue;
                if (h.distance < best) best = h.distance;
            }
            transform.position = pivot + dir * Mathf.Max(0.6f, best - 0.1f);
            transform.rotation = rot;
        }

        public Vector3 ComputeAimPoint(RobotController self, out bool enemy)
        {
            enemy = false;
            var ray = new Ray(transform.position, transform.forward);
            var hits = Physics.SphereCastAll(ray, 0.4f, 80f, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var h in hits)
            {
                if (h.distance < distance * 0.8f) continue;
                var rc = h.collider.GetComponentInParent<RobotController>();
                if (rc == self) continue;
                if (rc != null && !rc.Dead && rc.team != self.team)
                {
                    enemy = true;
                    return rc.transform.position + Vector3.up * 1.0f;
                }
                return h.point;
            }
            return ray.origin + ray.direction * 70f;
        }
    }
}
