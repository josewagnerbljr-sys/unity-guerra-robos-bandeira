using UnityEngine;

namespace RoboFlagWars
{
    [RequireComponent(typeof(RobotController))]
    public class RobotAI : MonoBehaviour
    {
        public bool defender;
        RobotController rc, target;
        float nextThink, strafeUntil, strafeSign = 1f, avoidUntil, avoidSign = 1f, targetDist;
        bool engaging;
        Vector3 dest, jitter;
        float phase;

        void Awake() { rc = GetComponent<RobotController>(); phase = Random.value * 6.28f; }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.Playing || rc.Dead) { rc.moveWorld = Vector3.zero; rc.wantFire = false; return; }
            if (Time.time >= nextThink) { nextThink = Time.time + 0.25f; Think(gm); }
            Move();
            Aim();
        }

        void Think(GameManager gm)
        {
            target = null; targetDist = 999f;
            jitter = Random.insideUnitSphere * 1.0f;
            foreach (var r in gm.Robots)
            {
                if (r.Dead || r.team == rc.team) continue;
                float d = Vector3.Distance(r.transform.position, transform.position);
                float see = r.camoUntil > Time.time ? 4f : 28f;
                if (rc.inkedUntil > Time.time) see = Mathf.Min(see, 3f);
                if (d >= see || d >= targetDist) continue;
                RaycastHit hit;
                bool blocked = Physics.Linecast(rc.EyeMid, r.transform.position + Vector3.up * 1f, out hit, ~0, QueryTriggerInteraction.Ignore)
                               && hit.collider.GetComponentInParent<RobotController>() != r;
                if (!blocked) { target = r; targetDist = d; }
            }

            Team me = rc.team, foe = TeamUtil.Other(me);
            Flag ownFlag = gm.FlagOf(me), enemyFlag = gm.FlagOf(foe);
            Vector3 inward = me == Team.Red ? Vector3.right : Vector3.left;

            if (rc.carriedFlag != null) dest = gm.BaseOf(me);
            else if (ownFlag.carrier != null) dest = ownFlag.carrier.transform.position;
            else if (defender)
            {
                dest = gm.BaseOf(me) + inward * 9f + new Vector3(0, 0, Mathf.Sin(Time.time * 0.3f + phase) * 8f);
                if (target != null && targetDist < 16f) dest = target.transform.position;
            }
            else dest = enemyFlag.transform.position;

            engaging = target != null && targetDist < 9f && rc.hp > 35 && rc.carriedFlag == null;

            if (target != null)
            {
                switch (rc.type)
                {
                    case RobotType.Humanoid: if (targetDist < 2.4f) rc.wantSpecial = true; break;
                    case RobotType.Canine: if (targetDist < 7f) rc.wantSpecial = true; break;
                    case RobotType.Cephalopod: if (targetDist < 9f) rc.wantSpecial = true; break;
                    default: if ((rc.carriedFlag != null || rc.hp < 45) && rc.camoUntil < Time.time) rc.wantSpecial = true; break;
                }
            }
            else if (rc.type == RobotType.Chameleon && rc.carriedFlag != null && rc.camoUntil < Time.time) rc.wantSpecial = true;
        }

        void Move()
        {
            Vector3 dir = Vector3.zero;
            if (engaging && target != null)
            {
                Vector3 to = target.transform.position - transform.position; to.y = 0;
                if (Time.time > strafeUntil) { strafeUntil = Time.time + Random.Range(0.8f, 2f); strafeSign = Random.value < 0.5f ? -1f : 1f; }
                dir = Vector3.Cross(Vector3.up, to.normalized) * strafeSign;
                if (rc.type == RobotType.Humanoid && to.magnitude > 2f) dir += to.normalized * 0.8f;
            }
            else
            {
                Vector3 to = dest - transform.position; to.y = 0;
                if (to.sqrMagnitude > 1.5f) dir = to.normalized;
            }

            if (dir.sqrMagnitude > 0.01f)
            {
                dir.Normalize();
                RaycastHit h;
                if (Physics.SphereCast(transform.position + Vector3.up * 0.7f, 0.4f, dir, out h, 2.2f, ~0, QueryTriggerInteraction.Ignore)
                    && h.collider.GetComponentInParent<RobotController>() == null)
                {
                    if (Time.time > avoidUntil) { avoidUntil = Time.time + 0.8f; avoidSign = Random.value < 0.5f ? -1f : 1f; }
                }
                if (Time.time < avoidUntil) dir = Quaternion.Euler(0, 75f * avoidSign, 0) * dir;
            }
            rc.moveWorld = dir;
            if (dir.sqrMagnitude > 0.01f && target == null) rc.lookDir = dir;
        }

        void Aim()
        {
            if (target == null) { rc.wantFire = false; return; }
            Vector3 tp = target.transform.position + Vector3.up * 1.0f + jitter;
            rc.aimPoint = tp;
            Vector3 l = tp - transform.position; l.y = 0;
            rc.lookDir = l;
            rc.wantFire = Vector3.Angle(transform.forward, l) < 20f && targetDist < 28f;
        }
    }
}
