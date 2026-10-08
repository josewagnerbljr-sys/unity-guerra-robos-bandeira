using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoboFlagWars
{
    [RequireComponent(typeof(CharacterController))]
    public class RobotController : MonoBehaviour
    {
        public Team team;
        public RobotType type;
        public string robotName;
        public bool isPlayer;
        public int maxHp = 100, hp;
        public float baseSpeed = 6f;
        public float fireInterval = 0.2f;

        // entradas (preenchidas por PlayerInput ou RobotAI)
        [HideInInspector] public Vector3 moveWorld, lookDir = Vector3.forward, aimPoint;
        [HideInInspector] public bool wantFire, wantSpecial;
        [HideInInspector] public Flag carriedFlag;

        public bool Dead { get; private set; }
        [HideInInspector] public float slowUntil, slowFactor = 1f, inkedUntil, camoUntil, stunUntil, dotAcc;
        public float SpecialCooldownLeft { get { return Mathf.Max(0f, specialReadyAt - Time.time); } }
        public Vector3 EyeMid
        {
            get
            {
                if (rig != null && rig.eyeL != null && rig.eyeR != null) return (rig.eyeL.position + rig.eyeR.position) * 0.5f;
                return transform.position + Vector3.up * 1.5f;
            }
        }

        CharacterController cc;
        RobotRig rig;
        Transform model;
        SpeechBubble bubble;
        LineRenderer lineL, lineR;
        readonly Dictionary<Renderer, Material[]> original = new Dictionary<Renderer, Material[]>();
        Material camoMat;
        bool camoApplied;
        float vy, nextFire, specialReadyAt, nextHoleAt, nextSpeech, respawnAt, spawnProtectUntil, laserOff, tilt;

        public void Init(Team t, RobotType ty, bool player, Vector3 spawn)
        {
            team = t; type = ty; isPlayer = player;
            robotName = Dialogues.Name(ty);
            name = robotName + " (" + TeamUtil.Name(t) + ")";
            cc = GetComponent<CharacterController>();
            rig = RobotBuilder.Build(ty, t, transform);
            model = rig.model;
            cc.radius = rig.radius;
            cc.height = Mathf.Max(rig.height, rig.radius * 2f + 0.05f);
            cc.center = new Vector3(0, cc.height * 0.5f, 0);
            cc.stepOffset = 0.3f;
            maxHp = rig.hp; baseSpeed = rig.speed; hp = maxHp;
            foreach (var r in rig.renderers) original[r] = r.sharedMaterials;
            camoMat = Mats.Make(new Color(0.45f, 0.5f, 0.35f), 0f, 0.05f);
            lineL = MakeLaser(); lineR = MakeLaser();
            bubble = SpeechBubble.Create(transform, cc.height + 0.7f);
            cc.enabled = false; transform.position = spawn; cc.enabled = true;
            lookDir = team == Team.Red ? Vector3.right : Vector3.left;
            transform.rotation = Quaternion.LookRotation(lookDir);
            GameManager.Instance.Robots.Add(this);
            Say(Dialogues.Pick(Dialogues.Spawn));
        }

        LineRenderer MakeLaser()
        {
            var go = new GameObject("Laser");
            go.transform.SetParent(transform, false);
            var l = go.AddComponent<LineRenderer>();
            Color c = Color.Lerp(TeamUtil.Col(team), Color.white, 0.35f);
            l.positionCount = 2; l.useWorldSpace = true;
            l.startWidth = 0.08f; l.endWidth = 0.04f;
            l.material = Mats.Line(c); l.startColor = c; l.endColor = c;
            l.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            l.enabled = false;
            return l;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || rig == null) return;

            if (Dead)
            {
                if (gm.Playing && Time.time >= respawnAt) Respawn();
                return;
            }

            bool stunned = Time.time < stunUntil;
            float spd = baseSpeed * (Time.time < slowUntil ? slowFactor : 1f) * (carriedFlag != null ? 0.9f : 1f);
            Vector3 mv = (stunned || !gm.Playing) ? Vector3.zero : Vector3.ClampMagnitude(moveWorld, 1f) * spd;

            if (cc.isGrounded && vy < 0) vy = -2f;
            vy -= 25f * Time.deltaTime;
            cc.Move((mv + Vector3.up * vy) * Time.deltaTime);

            Vector3 ld = lookDir; ld.y = 0;
            if (ld.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(ld), 12f * Time.deltaTime);

            // animacao simples: tombo no buraco + balanco ao andar
            tilt = Mathf.MoveTowards(tilt, stunned ? 78f : 0f, 320f * Time.deltaTime);
            float bob = (type == RobotType.Cephalopod ? 0.08f * Mathf.Sin(Time.time * 3f) + 0.1f
                                                        : Mathf.Abs(Mathf.Sin(Time.time * 11f)) * 0.06f * Mathf.Clamp01(mv.magnitude));
            model.localPosition = new Vector3(0, bob, 0);
            model.localRotation = Quaternion.Euler(tilt, 0, 0);

            if (!stunned && gm.Playing && Time.time > nextHoleAt && cc.isGrounded)
            {
                foreach (var h in Hole.All)
                {
                    Vector3 d = transform.position - h.transform.position; d.y = 0;
                    if (d.magnitude < h.radius) { Stumble(); break; }
                }
            }

            if (wantFire && !stunned && gm.Playing && Time.time >= nextFire) Fire();
            if (wantSpecial)
            {
                wantSpecial = false;
                if (!stunned && gm.Playing && Time.time >= specialReadyAt) UseSpecial();
            }

            if (Time.time > laserOff && lineL.enabled) { lineL.enabled = false; lineR.enabled = false; }
            UpdateCamo();
        }

        // ---------------- combate ----------------
        void Fire()
        {
            nextFire = Time.time + fireInterval;
            Vector3 origin = EyeMid;
            Vector3 dir = aimPoint - origin;
            if (dir.sqrMagnitude < 0.01f) dir = transform.forward;
            dir.Normalize();
            if (Vector3.Dot(dir, transform.forward) < 0.3f) dir = transform.forward;

            var hits = Physics.RaycastAll(origin, dir, 70f, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            Vector3 end = origin + dir * 70f;
            foreach (var h in hits)
            {
                var rc = h.collider.GetComponentInParent<RobotController>();
                if (rc == this) continue;
                end = h.point;
                if (rc != null && rc.team != team && !rc.Dead) rc.TakeDamage(9, this);
                break;
            }
            Vector3 a0 = rig.eyeL != null ? rig.eyeL.position : origin;
            Vector3 a1 = rig.eyeR != null ? rig.eyeR.position : origin;
            lineL.SetPosition(0, a0); lineL.SetPosition(1, end);
            lineR.SetPosition(0, a1); lineR.SetPosition(1, end);
            lineL.enabled = true; lineR.enabled = true;
            laserOff = Time.time + 0.07f;
        }

        public void TakeDamage(int dmg, RobotController attacker)
        {
            if (Dead || Time.time < spawnProtectUntil || dmg <= 0) return;
            hp -= dmg;
            if (hp <= 0) Die(attacker);
        }

        void Die(RobotController attacker)
        {
            Dead = true;
            var gm = GameManager.Instance;
            if (carriedFlag != null)
            {
                var f = carriedFlag;
                f.Drop(transform.position);
                Say(Dialogues.Pick(Dialogues.FlagLost), true);
            }
            if (attacker != null && attacker != this)
            {
                gm.AddKill(attacker.team);
                attacker.Say(Dialogues.Pick(Dialogues.Kill), true);
            }
            Say(Dialogues.Pick(Dialogues.Death), true);
            model.gameObject.SetActive(false);
            lineL.enabled = false; lineR.enabled = false;
            cc.enabled = false;
            respawnAt = Time.time + 4f;
        }

        void Respawn()
        {
            transform.position = GameManager.Instance.SpawnPoint(team);
            cc.enabled = true;
            hp = maxHp; Dead = false; vy = 0; tilt = 0;
            stunUntil = 0; camoUntil = 0; inkedUntil = 0; slowUntil = 0;
            model.gameObject.SetActive(true);
            spawnProtectUntil = Time.time + 2f;
            Say(Dialogues.Pick(Dialogues.Spawn));
        }

        public void ApplySlow(float factor, float dur)
        {
            slowFactor = factor;
            slowUntil = Time.time + dur;
        }

        void Stumble()
        {
            stunUntil = Time.time + 1.6f;
            nextHoleAt = stunUntil + 2f;
            TakeDamage(5, null);
            Say(Dialogues.Pick(Dialogues.Stumble), true);
        }

        // ---------------- habilidades especiais ----------------
        void UseSpecial()
        {
            switch (type)
            {
                case RobotType.Humanoid:
                    specialReadyAt = Time.time + 6f;
                    StartCoroutine(Stab());
                    break;
                case RobotType.Canine:
                    specialReadyAt = Time.time + 9f;
                    Pee();
                    break;
                case RobotType.Cephalopod:
                    specialReadyAt = Time.time + 10f;
                    Ink();
                    break;
                default:
                    specialReadyAt = Time.time + 12f;
                    camoUntil = Time.time + 6f;
                    Say(Dialogues.Pick(Dialogues.Special(type)), true);
                    break;
            }
        }

        IEnumerator Stab()
        {
            Say(Dialogues.Pick(Dialogues.Special(type)), true);
            float t = 0; bool hit = false;
            while (t < 0.35f)
            {
                t += Time.deltaTime;
                if (rig.knife != null) rig.knife.localRotation = Quaternion.Euler(0, Mathf.Lerp(-80f, 80f, t / 0.35f), 0);
                if (!hit && t > 0.12f) { hit = true; DoStab(); }
                yield return null;
            }
            if (rig.knife != null) rig.knife.localRotation = Quaternion.identity;
        }

        void DoStab()
        {
            foreach (var r in GameManager.Instance.Robots)
            {
                if (r.Dead || r.team == team) continue;
                Vector3 to = r.transform.position - transform.position; to.y = 0;
                if (to.magnitude < 2.7f && Vector3.Angle(transform.forward, to) < 75f) r.TakeDamage(50, this);
            }
        }

        void Pee()
        {
            Say(Dialogues.Pick(Dialogues.Special(type)), true);
            LampPost best = null; float bd = 3.5f;
            foreach (var p in LampPost.All)
            {
                float d = Vector3.Distance(p.transform.position, transform.position);
                if (d < bd) { bd = d; best = p; }
            }
            Vector3 pos = best != null ? best.transform.position : transform.position - transform.forward * 0.6f;
            AbilityZone.Create(this, pos, best != null ? 5.5f : 2.8f, 7f, 0.5f, 6f, false, new Color(1f, 0.88f, 0.1f));
        }

        void Ink()
        {
            Say(Dialogues.Pick(Dialogues.Special(type)), true);
            AbilityZone.Create(this, transform.position + transform.forward * 3.5f, 4f, 5f, 0.55f, 4f, true, new Color(0.03f, 0.03f, 0.05f));
        }

        void UpdateCamo()
        {
            var gm = GameManager.Instance;
            bool camo = Time.time < camoUntil;
            bool friendly = isPlayer || (gm.Player != null && gm.Player.team == team);
            bool near = gm.Player != null && Vector3.Distance(gm.Player.transform.position, transform.position) < 4f;
            bool visible = !camo || friendly || near;
            if (camo != camoApplied)
            {
                camoApplied = camo;
                foreach (var kv in original)
                {
                    if (kv.Key == null) continue;
                    if (camo)
                    {
                        var arr = new Material[kv.Value.Length];
                        for (int i = 0; i < arr.Length; i++) arr[i] = camoMat;
                        kv.Key.sharedMaterials = arr;
                    }
                    else kv.Key.sharedMaterials = kv.Value;
                }
            }
            foreach (var kv in original) if (kv.Key != null && kv.Key.enabled != visible) kv.Key.enabled = visible;
        }

        public void Say(string line, bool feed = false)
        {
            if (!feed && Time.time < nextSpeech) return;
            nextSpeech = Time.time + 2.5f;
            if (bubble != null) bubble.Show(line, 3f, team);
            if (feed && GameManager.Instance != null) GameManager.Instance.Feed(robotName + ": " + line, team);
        }
    }
}
