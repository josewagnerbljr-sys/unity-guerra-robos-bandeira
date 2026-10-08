using UnityEngine;

namespace RoboFlagWars
{
    public class Flag : MonoBehaviour
    {
        public Team team;
        public Vector3 basePos;
        public RobotController carrier;
        bool atBase = true;
        float droppedAt;
        Transform cloth;

        public static Flag Create(Team t, Vector3 pos)
        {
            var go = new GameObject("Bandeira " + TeamUtil.Name(t));
            go.transform.position = pos;
            var f = go.AddComponent<Flag>();
            f.team = t; f.basePos = pos;
            Color c = TeamUtil.Col(t);

            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Object.Destroy(pole.GetComponent<Collider>());
            pole.transform.SetParent(go.transform, false);
            pole.transform.localPosition = new Vector3(0, 1.6f, 0);
            pole.transform.localScale = new Vector3(0.08f, 1.6f, 0.08f);
            pole.GetComponent<Renderer>().sharedMaterial = Mats.Make(new Color(0.8f, 0.8f, 0.8f), 0.9f, 0.8f);

            var cl = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(cl.GetComponent<Collider>());
            cl.transform.SetParent(go.transform, false);
            cl.transform.localPosition = new Vector3(0.58f, 2.75f, 0);
            cl.transform.localScale = new Vector3(1.1f, 0.7f, 0.05f);
            cl.GetComponent<Renderer>().sharedMaterial = Mats.Make(c, 0f, 0.4f, c * 0.8f);
            f.cloth = cl.transform;

            var lg = new GameObject("Luz");
            lg.transform.SetParent(go.transform, false);
            lg.transform.localPosition = new Vector3(0, 3.5f, 0);
            var l = lg.AddComponent<Light>();
            l.type = LightType.Point; l.color = c; l.range = 9f; l.intensity = 2.2f;
            return f;
        }

        void Update()
        {
            if (cloth != null) cloth.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 4f) * 12f, 0);
            var gm = GameManager.Instance;
            if (gm == null || !gm.Playing) return;

            if (carrier != null)
            {
                transform.position = carrier.transform.position - carrier.transform.forward * 0.5f;
                transform.rotation = carrier.transform.rotation;
                Vector3 d = carrier.transform.position - gm.BaseOf(carrier.team); d.y = 0;
                if (d.magnitude < 3.5f)
                {
                    var c = carrier;
                    c.carriedFlag = null;
                    carrier = null;
                    gm.OnCapture(c);
                    ReturnToBase();
                }
                return;
            }

            if (!atBase && Time.time - droppedAt > 12f) { ReturnToBase(); return; }

            foreach (var r in gm.Robots)
            {
                if (r.Dead) continue;
                Vector3 d = r.transform.position - transform.position; d.y = 0;
                if (d.magnitude > 2.2f) continue;
                if (r.team != team)
                {
                    carrier = r; r.carriedFlag = this; atBase = false;
                    gm.Announce("Bandeira " + TeamUtil.Name(team) + " foi pega!");
                    r.Say(Dialogues.Pick(Dialogues.FlagTaken), true);
                    break;
                }
                else if (!atBase)
                {
                    gm.Announce("Bandeira " + TeamUtil.Name(team) + " voltou pra casa!");
                    ReturnToBase();
                    break;
                }
            }
        }

        public void Drop(Vector3 pos)
        {
            if (carrier != null) carrier.carriedFlag = null;
            carrier = null;
            pos.y = 0;
            transform.position = pos;
            transform.rotation = Quaternion.identity;
            droppedAt = Time.time;
            atBase = false;
            if (GameManager.Instance != null) GameManager.Instance.Announce("Bandeira " + TeamUtil.Name(team) + " caiu no chão!");
        }

        public void ReturnToBase()
        {
            carrier = null;
            atBase = true;
            transform.position = basePos;
            transform.rotation = Quaternion.identity;
        }
    }
}
