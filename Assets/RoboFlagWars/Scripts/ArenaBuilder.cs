using UnityEngine;
using UnityEngine.Rendering;

namespace RoboFlagWars
{
    /// <summary>Monta a arena do sertao em codigo: chao, muros, bases, buracos, postes, cactos, luz e neblina.</summary>
    public static class ArenaBuilder
    {
        static GameObject Prim(PrimitiveType t, Transform parent, string name, Vector3 pos, Vector3 scale, Material m, bool collider)
        {
            var g = GameObject.CreatePrimitive(t);
            g.name = name;
            if (!collider) Object.Destroy(g.GetComponent<Collider>());
            g.transform.SetParent(parent, false);
            g.transform.localPosition = pos;
            g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = m;
            return g;
        }

        public static void Build(GameManager gm)
        {
            var root = new GameObject("Arena").transform;
            Lighting();

            var sand = Mats.Make(Color.white, 0f, 0.08f);
            sand.mainTexture = Mats.Noise(new Color(0.66f, 0.5f, 0.32f), new Color(0.86f, 0.72f, 0.5f), 256, 10f);
            sand.mainTextureScale = new Vector2(24, 15);
            Prim(PrimitiveType.Cube, root, "Chao", new Vector3(0, -0.5f, 0), new Vector3(120, 1, 76), sand, true);

            var adobe = Mats.Make(Color.white, 0f, 0.12f);
            adobe.mainTexture = Mats.Noise(new Color(0.5f, 0.3f, 0.2f), new Color(0.72f, 0.5f, 0.36f), 128, 6f);
            adobe.mainTextureScale = new Vector2(4, 1);
            Prim(PrimitiveType.Cube, root, "MuroN", new Vector3(0, 2, 39), new Vector3(124, 4, 2), adobe, true);
            Prim(PrimitiveType.Cube, root, "MuroS", new Vector3(0, 2, -39), new Vector3(124, 4, 2), adobe, true);
            Prim(PrimitiveType.Cube, root, "MuroL", new Vector3(61, 2, 0), new Vector3(2, 4, 80), adobe, true);
            Prim(PrimitiveType.Cube, root, "MuroO", new Vector3(-61, 2, 0), new Vector3(2, 4, 80), adobe, true);

            // coberturas
            Vector3[] cover = {
                new Vector3(0, 1.5f, 0), new Vector3(-18, 1.25f, 12), new Vector3(-18, 1.25f, -12),
                new Vector3(18, 1.25f, 12), new Vector3(18, 1.25f, -12), new Vector3(-10, 1f, -5), new Vector3(10, 1f, 5) };
            Vector3[] coverSize = {
                new Vector3(6, 3, 6), new Vector3(4, 2.5f, 4), new Vector3(4, 2.5f, 4),
                new Vector3(4, 2.5f, 4), new Vector3(4, 2.5f, 4), new Vector3(4, 2, 2), new Vector3(4, 2, 2) };
            for (int i = 0; i < cover.Length; i++) Prim(PrimitiveType.Cube, root, "Cobertura", cover[i], coverSize[i], adobe, true);
            Prim(PrimitiveType.Cube, root, "ParedeL", new Vector3(28, 1.25f, 0), new Vector3(2, 2.5f, 10), adobe, true);
            Prim(PrimitiveType.Cube, root, "ParedeO", new Vector3(-28, 1.25f, 0), new Vector3(2, 2.5f, 10), adobe, true);

            // buracos
            Vector3[] holes = {
                new Vector3(-12, 0, 10), new Vector3(12, 0, -10), new Vector3(-5, 0, -14), new Vector3(5, 0, 14),
                new Vector3(-22, 0, -4), new Vector3(22, 0, 4), new Vector3(0, 0, 9), new Vector3(0, 0, -9) };
            var holeMat = Mats.Make(new Color(0.02f, 0.02f, 0.02f), 0f, 0.05f);
            var rimMat = Mats.Make(new Color(0.35f, 0.22f, 0.14f), 0f, 0.05f);
            foreach (var h in holes)
            {
                Prim(PrimitiveType.Cylinder, root, "BuracoBorda", h + new Vector3(0, 0.01f, 0), new Vector3(3.2f, 0.01f, 3.2f), rimMat, false);
                var hg = Prim(PrimitiveType.Cylinder, root, "Buraco", h + new Vector3(0, 0.02f, 0), new Vector3(2.6f, 0.01f, 2.6f), holeMat, false);
                hg.AddComponent<Hole>().radius = 1.3f;
            }

            // bases e bandeiras
            var redMat = Mats.Make(TeamUtil.Col(Team.Red) * 0.8f, 0.3f, 0.5f);
            var greenMat = Mats.Make(TeamUtil.Col(Team.Green) * 0.8f, 0.3f, 0.5f);
            Prim(PrimitiveType.Cylinder, root, "BaseVermelha", gm.redBase + new Vector3(0, 0.075f, 0), new Vector3(10, 0.075f, 10), redMat, false);
            Prim(PrimitiveType.Cylinder, root, "BaseVerde", gm.greenBase + new Vector3(0, 0.075f, 0), new Vector3(10, 0.075f, 10), greenMat, false);
            gm.redFlag = Flag.Create(Team.Red, gm.redBase);
            gm.greenFlag = Flag.Create(Team.Green, gm.greenBase);

            // postes de luz (a mijada do cachorro e maior perto deles)
            Vector3[] posts = {
                new Vector3(-32, 0, 14), new Vector3(-32, 0, -14), new Vector3(32, 0, 14), new Vector3(32, 0, -14),
                new Vector3(-8, 0, 18), new Vector3(8, 0, -18) };
            var poleMat = Mats.Make(new Color(0.25f, 0.25f, 0.28f), 0.9f, 0.6f);
            var bulbMat = Mats.Make(new Color(1f, 0.9f, 0.6f), 0f, 0.9f, new Color(1f, 0.8f, 0.4f) * 2.5f);
            foreach (var p in posts)
            {
                var pole = Prim(PrimitiveType.Cylinder, root, "Poste", p + new Vector3(0, 2.5f, 0), new Vector3(0.25f, 2.5f, 0.25f), poleMat, true);
                pole.AddComponent<LampPost>();
                Prim(PrimitiveType.Sphere, root, "Lampada", p + new Vector3(0, 5.2f, 0), new Vector3(0.6f, 0.6f, 0.6f), bulbMat, false);
                var lg = new GameObject("LuzPoste");
                lg.transform.SetParent(root, false);
                lg.transform.position = p + new Vector3(0, 5f, 0);
                var l = lg.AddComponent<Light>();
                l.type = LightType.Point; l.range = 14f; l.intensity = 1.6f; l.color = new Color(1f, 0.85f, 0.55f);
            }

            // cactos (mandacaru)
            Vector3[] cactus = {
                new Vector3(-34, 0, 25), new Vector3(-20, 0, 28), new Vector3(-5, 0, 30), new Vector3(10, 0, 27),
                new Vector3(25, 0, 29), new Vector3(35, 0, 24), new Vector3(-34, 0, -25), new Vector3(-18, 0, -29),
                new Vector3(-2, 0, -28), new Vector3(14, 0, -30), new Vector3(28, 0, -27), new Vector3(35, 0, -22) };
            var cactusMat = Mats.Make(new Color(0.18f, 0.42f, 0.2f), 0f, 0.3f);
            foreach (var c in cactus)
            {
                float h = Random.Range(1.6f, 2.6f);
                var stem = Prim(PrimitiveType.Capsule, root, "Mandacaru", c + new Vector3(0, h, 0), new Vector3(0.6f, h, 0.6f), cactusMat, true);
                var a1 = Prim(PrimitiveType.Capsule, root, "Braco", c + new Vector3(0.6f, h * 1.1f, 0), new Vector3(0.3f, 0.6f, 0.3f), cactusMat, false);
                a1.transform.localRotation = Quaternion.Euler(0, 0, -25);
                var a2 = Prim(PrimitiveType.Capsule, root, "Braco", c + new Vector3(-0.55f, h * 0.9f, 0), new Vector3(0.3f, 0.5f, 0.3f), cactusMat, false);
                a2.transform.localRotation = Quaternion.Euler(0, 0, 25);
            }
        }

        static void Lighting()
        {
            var sun = new GameObject("Sol").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.92f, 0.78f);
            sun.intensity = 1.25f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(38, -35, 0);
            RenderSettings.sun = sun;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.6f, 0.7f, 0.9f);
            RenderSettings.ambientEquatorColor = new Color(0.7f, 0.6f, 0.5f);
            RenderSettings.ambientGroundColor = new Color(0.3f, 0.25f, 0.2f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.8f, 0.72f, 0.58f);
            RenderSettings.fogDensity = 0.006f;

            var skySh = Shader.Find("Skybox/Procedural");
            if (skySh != null)
            {
                var sky = new Material(skySh);
                sky.SetFloat("_AtmosphereThickness", 1.1f);
                sky.SetColor("_SkyTint", new Color(0.55f, 0.65f, 0.85f));
                RenderSettings.skybox = sky;
            }
            QualitySettings.shadowDistance = 90f;
        }
    }
}
