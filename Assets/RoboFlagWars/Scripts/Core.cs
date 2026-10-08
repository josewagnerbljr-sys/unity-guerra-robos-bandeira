using System.Collections.Generic;
using UnityEngine;

namespace RoboFlagWars
{
    public enum Team { Red, Green }
    public enum RobotType { Humanoid, Canine, Cephalopod, Chameleon }

    public static class TeamUtil
    {
        public static Color Col(Team t) { return t == Team.Red ? new Color(0.85f, 0.1f, 0.1f) : new Color(0.1f, 0.75f, 0.2f); }
        public static Team Other(Team t) { return t == Team.Red ? Team.Green : Team.Red; }
        public static string Name(Team t) { return t == Team.Red ? "VERMELHO" : "VERDE"; }
    }

    /// <summary>Materiais PBR criados em codigo (URP Lit ou Standard).</summary>
    public static class Mats
    {
        static Shader lit;

        public static Material Make(Color c, float metallic = 0.5f, float smooth = 0.6f, Color? emission = null)
        {
            if (lit == null)
            {
                lit = Shader.Find("Universal Render Pipeline/Lit");
                if (lit == null) lit = Shader.Find("Standard");
            }
            var m = new Material(lit);
            m.color = c;
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
            if (emission.HasValue && m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", emission.Value);
            }
            return m;
        }

        public static Material Line(Color c)
        {
            var m = new Material(Shader.Find("Sprites/Default"));
            m.color = c;
            return m;
        }

        public static Texture2D Noise(Color a, Color b, int size = 256, float scale = 8f)
        {
            var t = new Texture2D(size, size, TextureFormat.RGB24, true);
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float n = Mathf.PerlinNoise(x * scale / size, y * scale / size) * 0.6f
                            + Mathf.PerlinNoise(x * scale * 4f / size, y * scale * 4f / size) * 0.4f;
                    t.SetPixel(x, y, Color.Lerp(a, b, n));
                }
            t.wrapMode = TextureWrapMode.Repeat;
            t.Apply();
            return t;
        }
    }

    public static class UiFont
    {
        static Font f;
        public static Font Get()
        {
            if (f == null)
            {
#if UNITY_2022_2_OR_NEWER
                f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#else
                f = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif
                if (f == null) f = Font.CreateDynamicFontFromOSFont("Arial", 32);
            }
            return f;
        }
    }

    public class Hole : MonoBehaviour
    {
        public static readonly List<Hole> All = new List<Hole>();
        public float radius = 1.3f;
        void OnEnable() { All.Add(this); }
        void OnDisable() { All.Remove(this); }
    }

    public class LampPost : MonoBehaviour
    {
        public static readonly List<LampPost> All = new List<LampPost>();
        void OnEnable() { All.Add(this); }
        void OnDisable() { All.Remove(this); }
    }
}
