using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RoboFlagWars
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool Held, Clicked;
        public void OnPointerDown(PointerEventData e) { Held = true; Clicked = true; }
        public void OnPointerUp(PointerEventData e) { Held = false; }
        public void OnPointerExit(PointerEventData e) { Held = false; }
    }

    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform knob;
        public Vector2 Value;
        RectTransform rt;
        void Awake() { rt = (RectTransform)transform; }
        public void OnPointerDown(PointerEventData e) { OnDrag(e); }
        public void OnDrag(PointerEventData e)
        {
            Vector2 lp;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out lp))
            {
                Vector2 half = rt.rect.size * 0.5f;
                Value = Vector2.ClampMagnitude(new Vector2(lp.x / half.x, lp.y / half.y), 1f);
                knob.anchoredPosition = Vector2.Scale(Value, half) * 0.7f;
            }
        }
        public void OnPointerUp(PointerEventData e) { Value = Vector2.zero; knob.anchoredPosition = Vector2.zero; }
    }

    public class LookPad : MonoBehaviour, IDragHandler
    {
        public Vector2 Accum;
        public void OnDrag(PointerEventData e) { Accum += e.delta; }
    }

    /// <summary>Toda a interface (HUD, controles de celular, menu de escolha e fim de jogo) criada em codigo.</summary>
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance;

        GameManager gm;
        Font font;
        Text timerT, redT, greenT, feedT, bannerT, crossT, specialT, endT;
        Image inkImg, specFill;
        GameObject hud, selectPanel, endPanel;
        VirtualJoystick joy;
        LookPad look;
        HoldButton fireBtn, specBtn;
        float bannerUntil;
        readonly List<string> feed = new List<string>();
        static Sprite circle;

        public Vector2 MoveInput { get { return joy != null ? joy.Value : Vector2.zero; } }
        public bool FireHeld { get { return fireBtn != null && fireBtn.Held; } }
        public Vector2 ConsumeLook() { if (look == null) return Vector2.zero; var v = look.Accum; look.Accum = Vector2.zero; return v; }
        public bool ConsumeSpecial() { if (specBtn != null && specBtn.Clicked) { specBtn.Clicked = false; return true; } return false; }

        public static GameUI Create(GameManager gm)
        {
            var go = new GameObject("GameUI");
            var ui = go.AddComponent<GameUI>();
            ui.gm = gm;
            ui.Build();
            return ui;
        }

        void Awake() { Instance = this; }

        // ---------- helpers ----------
        static Sprite Circle()
        {
            if (circle != null) return circle;
            int n = 128;
            var t = new Texture2D(n, n, TextureFormat.RGBA32, false);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                    t.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01((1f - d) * 20f)));
                }
            t.Apply();
            circle = Sprite.Create(t, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f));
            return circle;
        }

        RectTransform Rect(string n, Transform p, Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax)
        {
            var go = new GameObject(n, typeof(RectTransform));
            var r = go.GetComponent<RectTransform>();
            r.SetParent(p, false);
            r.anchorMin = aMin; r.anchorMax = aMax; r.offsetMin = oMin; r.offsetMax = oMax;
            return r;
        }

        Image Img(string n, Transform p, Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax, Color c, Sprite s = null)
        {
            var r = Rect(n, p, aMin, aMax, oMin, oMax);
            var i = r.gameObject.AddComponent<Image>();
            i.color = c; i.sprite = s;
            return i;
        }

        Text Txt(string n, Transform p, string s, int size, TextAnchor a, Color c, Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax)
        {
            var r = Rect(n, p, aMin, aMax, oMin, oMax);
            var t = r.gameObject.AddComponent<Text>();
            t.font = font; t.text = s; t.fontSize = size; t.alignment = a; t.color = c;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var o = r.gameObject.AddComponent<Outline>();
            o.effectColor = new Color(0, 0, 0, 0.9f);
            return t;
        }

        Button MakeButton(string n, Transform p, Vector2 aMin, Vector2 aMax, Vector2 oMin, Vector2 oMax, Color c, string label, int size, System.Action onClick)
        {
            var img = Img(n, p, aMin, aMax, oMin, oMax, c);
            var b = img.gameObject.AddComponent<Button>();
            b.targetGraphic = img;
            b.onClick.AddListener(() => onClick());
            Txt("L", img.transform, label, size, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one, new Vector2(10, 10), new Vector2(-10, -10));
            return b;
        }

        // ---------- construcao ----------
        void Build()
        {
            font = UiFont.Get();
            if (EventSystem.current == null)
            {
                var es = new GameObject("EventSystem").AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                var t = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (t != null)
                {
                    var comp = es.gameObject.AddComponent(t);
                    var m = t.GetMethod("AssignDefaultActions");
                    if (m != null) m.Invoke(comp, null);
                }
                else es.gameObject.AddComponent<StandaloneInputModule>();
#else
                es.gameObject.AddComponent<StandaloneInputModule>();
#endif
            }

            var cgo = new GameObject("Canvas");
            cgo.transform.SetParent(transform, false);
            var canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = cgo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            cgo.AddComponent<GraphicRaycaster>();
            var root = (RectTransform)cgo.transform;
            Vector2 z = Vector2.zero, o = Vector2.one;

            inkImg = Img("Tinta", root, z, o, z, z, new Color(0, 0, 0, 0));
            inkImg.raycastTarget = false;

            var hudR = Rect("HUD", root, z, o, z, z);
            hud = hudR.gameObject;

            // area de olhar (metade direita)
            var lookImg = Img("OlharPad", hudR, new Vector2(0.4f, 0), o, z, z, new Color(0, 0, 0, 0));
            look = lookImg.gameObject.AddComponent<LookPad>();

            // textos do HUD
            timerT = Txt("Tempo", hudR, "05:00", 76, TextAnchor.UpperCenter, Color.white, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-200, -110), new Vector2(200, -10));
            redT = Txt("PlacarV", hudR, "", 34, TextAnchor.UpperLeft, TeamUtil.Col(Team.Red) + new Color(0.15f, 0.15f, 0.15f), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -150), new Vector2(520, -10));
            greenT = Txt("PlacarG", hudR, "", 34, TextAnchor.UpperRight, TeamUtil.Col(Team.Green) + new Color(0.15f, 0.15f, 0.15f), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-520, -150), new Vector2(-20, -10));
            feedT = Txt("Feed", hudR, "", 26, TextAnchor.UpperLeft, Color.white, new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -430), new Vector2(900, -170));
            bannerT = Txt("Aviso", hudR, "", 58, TextAnchor.MiddleCenter, new Color(1, 0.9f, 0.3f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-800, 170), new Vector2(800, 300));
            crossT = Txt("Mira", hudR, "+", 80, TextAnchor.MiddleCenter, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-40, -40), new Vector2(40, 40));

            // joystick
            var joyImg = Img("Joystick", hudR, z, z, new Vector2(60, 60), new Vector2(420, 420), new Color(1, 1, 1, 0.18f), Circle());
            joy = joyImg.gameObject.AddComponent<VirtualJoystick>();
            var knob = Img("Knob", joyImg.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-70, -70), new Vector2(70, 70), new Color(1, 1, 1, 0.45f), Circle());
            knob.raycastTarget = false;
            joy.knob = knob.rectTransform;

            // botao de fogo
            var fireImg = Img("Fogo", hudR, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-330, 70), new Vector2(-90, 310), new Color(0.9f, 0.15f, 0.1f, 0.55f), Circle());
            fireBtn = fireImg.gameObject.AddComponent<HoldButton>();
            Txt("L", fireImg.transform, "LASER", 38, TextAnchor.MiddleCenter, Color.white, z, o, z, z);

            // botao de especial com recarga
            var spImg = Img("Especial", hudR, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-560, 150), new Vector2(-370, 340), new Color(0.2f, 0.5f, 0.95f, 0.55f), Circle());
            specBtn = spImg.gameObject.AddComponent<HoldButton>();
            specFill = Img("Recarga", spImg.transform, z, o, z, z, new Color(0, 0, 0, 0.6f), Circle());
            specFill.type = Image.Type.Filled; specFill.fillMethod = Image.FillMethod.Radial360; specFill.fillAmount = 0f; specFill.raycastTarget = false;
            specialT = Txt("L", spImg.transform, "ESPECIAL", 28, TextAnchor.MiddleCenter, Color.white, z, o, z, z);

            hud.SetActive(false);

            // painel de escolha do robo
            var selR = Rect("Escolha", root, z, o, z, z);
            selectPanel = selR.gameObject;
            Img("Fundo", selR, z, o, z, z, new Color(0, 0, 0, 0.72f));
            Txt("Titulo", selR, "GUERRA DOS ROBÔS NORDESTINOS", 70, TextAnchor.MiddleCenter, new Color(1, 0.85f, 0.25f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-900, 270), new Vector2(900, 400));
            Txt("Sub", selR, "Você é o time VERMELHO: pegue a bandeira VERDE e leve pra sua base! Escolha seu robô:", 32, TextAnchor.MiddleCenter, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-900, 190), new Vector2(900, 260));
            for (int i = 0; i < 4; i++)
            {
                var ty = (RobotType)i;
                float x0 = -790 + i * 400;
                MakeButton("Robo" + i, selR, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(x0, -170), new Vector2(x0 + 380, 130),
                    new Color(0.15f, 0.2f, 0.3f, 0.95f), Dialogues.Name(ty).ToUpper() + "\n\n" + Dialogues.TypeDesc(ty), 26,
                    () => { selectPanel.SetActive(false); hud.SetActive(true); gm.StartMatch(ty); });
            }
            Txt("Dica", selR, "Joystick à esquerda  |  Arraste à direita para mirar  |  LASER sai dos olhos  |  Cuidado com os buracos!", 28, TextAnchor.MiddleCenter, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-900, -320), new Vector2(900, -240));
            selectPanel.SetActive(false);

            // painel de fim de jogo
            var endR = Rect("Fim", root, z, o, z, z);
            endPanel = endR.gameObject;
            Img("Fundo", endR, z, o, z, z, new Color(0, 0, 0, 0.8f));
            endT = Txt("Resultado", endR, "", 50, TextAnchor.MiddleCenter, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-900, -100), new Vector2(900, 330));
            MakeButton("Reiniciar", endR, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-250, -330), new Vector2(250, -200),
                new Color(0.1f, 0.5f, 0.2f, 0.95f), "JOGAR DE NOVO", 44,
                () => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            endPanel.SetActive(false);
        }

        public void ShowSelect() { selectPanel.SetActive(true); }
        public void ShowEnd(string msg) { endT.text = msg; endPanel.SetActive(true); }
        public void Banner(string s) { if (bannerT == null) return; bannerT.text = s; bannerUntil = Time.time + 3f; }
        public void Feed(string s)
        {
            feed.Add(s);
            if (feed.Count > 5) feed.RemoveAt(0);
            if (feedT != null) feedT.text = string.Join("\n", feed.ToArray());
        }

        void Update()
        {
            if (gm == null || timerT == null) return;
            float tl = Mathf.Max(0f, gm.timeLeft);
            timerT.text = string.Format("{0:00}:{1:00}", (int)(tl / 60f), (int)(tl % 60f));
            timerT.color = tl < 30f ? new Color(1f, 0.3f, 0.25f) : Color.white;
            redT.text = "VERMELHO\nKills: " + gm.killsRed + "   Bandeiras: " + gm.capRed;
            greenT.text = "VERDE\nKills: " + gm.killsGreen + "   Bandeiras: " + gm.capGreen;

            var c = bannerT.color; c.a = Time.time < bannerUntil ? 1f : Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 2f);
            bannerT.color = c;

            var p = gm.Player;
            if (p != null)
            {
                float cd = p.SpecialCooldownLeft;
                specialT.text = Dialogues.SpecialName(p.type) + (cd > 0.05f ? "\n" + Mathf.CeilToInt(cd) + "s" : "");
                specFill.fillAmount = cd > 0.05f ? Mathf.Clamp01(cd / 12f) : 0f;
                float inkA = Time.time < p.inkedUntil ? 0.65f : 0f;
                var ic = inkImg.color; ic.a = Mathf.MoveTowards(ic.a, inkA, Time.deltaTime * 2f); inkImg.color = ic;
                bool en = gm.PlayerInput != null && gm.PlayerInput.targetingEnemy;
                crossT.color = en ? new Color(1f, 0.2f, 0.2f) : Color.white;
            }
        }
    }
}
