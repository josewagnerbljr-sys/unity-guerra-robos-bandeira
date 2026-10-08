using System.Text;
using UnityEngine;

namespace RoboFlagWars
{
    public class SpeechBubble : MonoBehaviour
    {
        TextMesh tm, shadow;
        float hideAt;
        static Camera cam;

        public static SpeechBubble Create(Transform parent, float height)
        {
            var go = new GameObject("Fala");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0, height, 0);
            var sb = go.AddComponent<SpeechBubble>();
            sb.shadow = MakeText(go.transform, Color.black, new Vector3(0.012f, -0.012f, 0.01f));
            sb.tm = MakeText(go.transform, new Color(1f, 0.93f, 0.4f), Vector3.zero);
            sb.SetVisible(false);
            return sb;
        }

        static TextMesh MakeText(Transform p, Color c, Vector3 off)
        {
            var g = new GameObject("t");
            g.transform.SetParent(p, false);
            g.transform.localPosition = off;
            var t = g.AddComponent<TextMesh>();
            t.font = UiFont.Get();
            g.GetComponent<MeshRenderer>().sharedMaterial = t.font.material;
            t.anchor = TextAnchor.LowerCenter;
            t.alignment = TextAlignment.Center;
            t.fontSize = 56;
            t.characterSize = 0.07f;
            t.color = c;
            return t;
        }

        public void Show(string s, float dur, Team team)
        {
            string w = Wrap(s, 20);
            tm.text = w; shadow.text = w;
            hideAt = Time.time + dur;
            SetVisible(true);
        }

        void SetVisible(bool v) { tm.gameObject.SetActive(v); shadow.gameObject.SetActive(v); }

        void LateUpdate()
        {
            if (!tm.gameObject.activeSelf) return;
            if (Time.time > hideAt) { SetVisible(false); return; }
            if (cam == null) cam = Camera.main;
            if (cam != null) transform.rotation = cam.transform.rotation;
        }

        static string Wrap(string s, int max)
        {
            var sb = new StringBuilder();
            int line = 0;
            foreach (var word in s.Split(' '))
            {
                if (line + word.Length > max && line > 0) { sb.Append('\n'); line = 0; }
                else if (line > 0) { sb.Append(' '); line++; }
                sb.Append(word); line += word.Length;
            }
            return sb.ToString();
        }
    }
}
