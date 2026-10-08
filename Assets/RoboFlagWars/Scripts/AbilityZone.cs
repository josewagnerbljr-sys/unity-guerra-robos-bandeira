using UnityEngine;

namespace RoboFlagWars
{
    /// <summary>Zona de efeito (mijada / nuvem de tinta): atrasa e causa dano continuo nos inimigos.</summary>
    public class AbilityZone : MonoBehaviour
    {
        public RobotController owner;
        public Team team;
        public float radius, endTime, slow, dps;
        public bool ink;

        public static AbilityZone Create(RobotController owner, Vector3 pos, float radius, float duration,
                                         float slow, float dps, bool ink, Color color)
        {
            var go = new GameObject(ink ? "NuvemDeTinta" : "PocaDeMijo");
            pos.y = 0f;
            go.transform.position = pos;
            var z = go.AddComponent<AbilityZone>();
            z.owner = owner; z.team = owner.team; z.radius = radius; z.endTime = Time.time + duration;
            z.slow = slow; z.dps = dps; z.ink = ink;
            var mat = Mats.Make(color, 0f, ink ? 0.9f : 0.95f);
            if (ink)
            {
                for (int i = 0; i < 7; i++)
                {
                    var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(s.GetComponent<Collider>());
                    s.transform.SetParent(go.transform, false);
                    Vector2 o = Random.insideUnitCircle * radius * 0.6f;
                    s.transform.localPosition = new Vector3(o.x, 1.0f + Random.value, o.y);
                    float sc = radius * Random.Range(0.6f, 1.0f);
                    s.transform.localScale = new Vector3(sc, sc * 0.8f, sc);
                    s.GetComponent<Renderer>().sharedMaterial = mat;
                }
            }
            else
            {
                var d = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Object.Destroy(d.GetComponent<Collider>());
                d.transform.SetParent(go.transform, false);
                d.transform.localPosition = new Vector3(0, 0.03f, 0);
                d.transform.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
                d.GetComponent<Renderer>().sharedMaterial = mat;
            }
            return z;
        }

        void Update()
        {
            if (Time.time > endTime) { Destroy(gameObject); return; }
            var gm = GameManager.Instance;
            if (gm == null) return;
            foreach (var r in gm.Robots)
            {
                if (r.Dead || r.team == team) continue;
                Vector3 d = r.transform.position - transform.position; d.y = 0;
                if (d.magnitude > radius) continue;
                r.ApplySlow(slow, 0.3f);
                if (ink) r.inkedUntil = Time.time + 0.5f;
                r.dotAcc += dps * Time.deltaTime;
                if (r.dotAcc >= 1f)
                {
                    int dmg = Mathf.FloorToInt(r.dotAcc);
                    r.dotAcc -= dmg;
                    r.TakeDamage(dmg, owner);
                }
            }
        }
    }
}
