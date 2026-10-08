using UnityEngine;

namespace RoboFlagWars
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public float matchSeconds = 300f;
        public Team PlayerTeam = Team.Red;
        public System.Collections.Generic.List<RobotController> Robots = new System.Collections.Generic.List<RobotController>();
        public bool Playing, Ended;
        public float timeLeft;
        public int killsRed, killsGreen, capRed, capGreen;
        public RobotController Player;
        public PlayerInput PlayerInput;
        public Flag redFlag, greenFlag;
        public Vector3 redBase = new Vector3(-42, 0, 0), greenBase = new Vector3(42, 0, 0);

        Team? firstCapturer;
        CameraRig rig;

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            timeLeft = matchSeconds;
            ArenaBuilder.Build(this);
            SetupCamera();
            GameUI.Create(this);
        }

        void Start() { GameUI.Instance.ShowSelect(); }

        void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }
            cam.fieldOfView = 65f;
            cam.farClipPlane = 300f;
            cam.nearClipPlane = 0.2f;
            if (RenderSettings.skybox != null) cam.clearFlags = CameraClearFlags.Skybox;
            else { cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.62f, 0.78f, 0.95f); }
            rig = cam.gameObject.GetComponent<CameraRig>();
            if (rig == null) rig = cam.gameObject.AddComponent<CameraRig>();
            rig.cam = cam;
        }

        public Vector3 BaseOf(Team t) { return t == Team.Red ? redBase : greenBase; }
        public Flag FlagOf(Team t) { return t == Team.Red ? redFlag : greenFlag; }

        public Vector3 SpawnPoint(Team t)
        {
            Vector3 inward = t == Team.Red ? Vector3.right : Vector3.left;
            return BaseOf(t) + inward * 6f + new Vector3(0, 0.1f, Random.Range(-10f, 10f));
        }

        public void StartMatch(RobotType chosen)
        {
            foreach (Team t in new[] { Team.Red, Team.Green })
            {
                foreach (RobotType ty in System.Enum.GetValues(typeof(RobotType)))
                {
                    var go = new GameObject("Robo");
                    var rc = go.AddComponent<RobotController>();
                    bool isP = (t == PlayerTeam && ty == chosen);
                    rc.Init(t, ty, isP, SpawnPoint(t));
                    if (isP)
                    {
                        Player = rc;
                        var pi = go.AddComponent<PlayerInput>();
                        pi.rc = rc; pi.cam = rig;
                        PlayerInput = pi;
                        rig.target = go.transform;
                        rig.SetYaw(t == Team.Red ? 90f : -90f);
                    }
                    else
                    {
                        var ai = go.AddComponent<RobotAI>();
                        ai.defender = (ty == RobotType.Humanoid || ty == RobotType.Cephalopod);
                        rc.fireInterval = 0.42f;
                    }
                }
            }
            timeLeft = matchSeconds;
            Playing = true;
            Announce("Começa a peleja, cabra!");
        }

        void Update()
        {
            if (!Playing) return;
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f) { timeLeft = 0f; End(); }
        }

        public void AddKill(Team t)
        {
            if (t == Team.Red) killsRed++; else killsGreen++;
        }

        public void OnCapture(RobotController r)
        {
            if (r.team == Team.Red) capRed++; else capGreen++;
            if (!firstCapturer.HasValue) firstCapturer = r.team;
            Announce("Time " + TeamUtil.Name(r.team) + " resgatou a bandeira!");
            r.Say(Dialogues.Pick(Dialogues.FlagCaptured), true);
        }

        public void Announce(string s) { if (GameUI.Instance != null) GameUI.Instance.Banner(s); }

        public void Feed(string text, Team t)
        {
            if (GameUI.Instance == null) return;
            GameUI.Instance.Feed(string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(TeamUtil.Col(t)), text));
        }

        void End()
        {
            Playing = false; Ended = true;
            Team? w = null;
            string reason = "";
            if (killsRed > killsGreen) { w = Team.Red; reason = "mais kills"; }
            else if (killsGreen > killsRed) { w = Team.Green; reason = "mais kills"; }
            else if (firstCapturer.HasValue) { w = firstCapturer; reason = "empate nas kills, desempate pela bandeira"; }

            string msg = "TEMPO ESGOTADO!\n\nVERMELHO " + killsRed + " kills (bandeiras: " + capRed + ")\nVERDE " + killsGreen + " kills (bandeiras: " + capGreen + ")\n\n";
            if (!w.HasValue) msg += "EMPATE, oxente! Ninguém ganhou nem perdeu.";
            else
            {
                msg += "Vitória do time " + TeamUtil.Name(w.Value) + " (" + reason + ")\n";
                msg += (w.Value == PlayerTeam) ? "Arretado, cabra da peste! Você ganhou!" : "Eita, perdemo! Mas foi uma peleja bonita.";
            }
            GameUI.Instance.ShowEnd(msg);
        }
    }
}
