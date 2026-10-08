using System.Collections.Generic;
using UnityEngine;

namespace RoboFlagWars
{
    public class RobotRig
    {
        public Transform model, eyeL, eyeR, knife;
        public List<Renderer> renderers = new List<Renderer>();
        public float height = 2f, radius = 0.5f, speed = 6f;
        public int hp = 100;
    }

    /// <summary>
    /// Monta os 4 robos com primitivas + materiais PBR.
    /// Para usar modelos proprios: crie prefabs em Assets/Resources/Robots/{Humanoid,Canine,Cephalopod,Chameleon}.prefab
    /// com filhos chamados "EyeL", "EyeR" (e "Knife" no humanoide).
    /// </summary>
    public static class RobotBuilder
    {
        static GameObject P(RobotRig rig, PrimitiveType t, string name, Vector3 pos, Vector3 scale, Material m,
                            Transform parent = null, Vector3? euler = null)
        {
            var g = GameObject.CreatePrimitive(t);
            g.name = name;
            var c = g.GetComponent<Collider>();
            if (c != null) Object.DestroyImmediate(c);
            g.transform.SetParent(parent != null ? parent : rig.model, false);
            g.transform.localPosition = pos;
            g.transform.localScale = scale;
            if (euler.HasValue) g.transform.localEulerAngles = euler.Value;
            var r = g.GetComponent<Renderer>();
            r.sharedMaterial = m;
            rig.renderers.Add(r);
            return g;
        }

        static void Stats(RobotRig rig, RobotType type)
        {
            switch (type)
            {
                case RobotType.Humanoid: rig.height = 2.3f; rig.radius = 0.45f; rig.speed = 6f; rig.hp = 120; break;
                case RobotType.Canine: rig.height = 1.3f; rig.radius = 0.5f; rig.speed = 8.5f; rig.hp = 85; break;
                case RobotType.Cephalopod: rig.height = 2.2f; rig.radius = 0.6f; rig.speed = 5.5f; rig.hp = 100; break;
                default: rig.height = 1.3f; rig.radius = 0.5f; rig.speed = 6.8f; rig.hp = 90; break;
            }
        }

        public static RobotRig Build(RobotType type, Team team, Transform root)
        {
            var rig = new RobotRig();
            var model = new GameObject("Model");
            model.transform.SetParent(root, false);
            rig.model = model.transform;
            Stats(rig, type);

            var prefab = Resources.Load<GameObject>("Robots/" + type);
            if (prefab != null)
            {
                var inst = Object.Instantiate(prefab, rig.model);
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localRotation = Quaternion.identity;
                foreach (var c in inst.GetComponentsInChildren<Collider>()) Object.Destroy(c);
                foreach (var tr in inst.GetComponentsInChildren<Transform>(true))
                {
                    if (tr.name == "EyeL") rig.eyeL = tr;
                    if (tr.name == "EyeR") rig.eyeR = tr;
                    if (tr.name == "Knife") rig.knife = tr;
                }
                rig.renderers.AddRange(inst.GetComponentsInChildren<Renderer>());
                return rig;
            }

            Color tc = TeamUtil.Col(team);
            var teamMat = Mats.Make(tc, 0.7f, 0.75f);
            var metal = Mats.Make(new Color(0.55f, 0.57f, 0.6f), 0.95f, 0.8f);
            var dark = Mats.Make(new Color(0.12f, 0.12f, 0.14f), 0.8f, 0.5f);
            var eye = Mats.Make(Color.Lerp(tc, Color.white, 0.5f), 0f, 0.9f, tc * 3f);

            switch (type)
            {
                case RobotType.Humanoid: Humanoid(rig, teamMat, metal, dark, eye); break;
                case RobotType.Canine: Canine(rig, teamMat, metal, dark, eye); break;
                case RobotType.Cephalopod: Cephalopod(rig, teamMat, metal, dark, eye); break;
                default: Chameleon(rig, teamMat, metal, dark, eye); break;
            }
            return rig;
        }

        static void Humanoid(RobotRig r, Material team, Material metal, Material dark, Material eye)
        {
            P(r, PrimitiveType.Cube, "Torso", new Vector3(0, 1.3f, 0), new Vector3(0.8f, 0.95f, 0.45f), team);
            P(r, PrimitiveType.Cube, "Peito", new Vector3(0, 1.45f, 0.24f), new Vector3(0.55f, 0.4f, 0.08f), metal);
            P(r, PrimitiveType.Cube, "Cabeca", new Vector3(0, 2.0f, 0), new Vector3(0.5f, 0.45f, 0.45f), metal);
            r.eyeL = P(r, PrimitiveType.Sphere, "EyeL", new Vector3(-0.12f, 2.03f, 0.22f), new Vector3(0.13f, 0.13f, 0.09f), eye).transform;
            r.eyeR = P(r, PrimitiveType.Sphere, "EyeR", new Vector3(0.12f, 2.03f, 0.22f), new Vector3(0.13f, 0.13f, 0.09f), eye).transform;
            P(r, PrimitiveType.Cylinder, "Antena", new Vector3(0, 2.35f, 0), new Vector3(0.03f, 0.12f, 0.03f), dark);
            P(r, PrimitiveType.Cube, "PernaE", new Vector3(-0.2f, 0.45f, 0), new Vector3(0.28f, 0.9f, 0.3f), dark);
            P(r, PrimitiveType.Cube, "PernaD", new Vector3(0.2f, 0.45f, 0), new Vector3(0.28f, 0.9f, 0.3f), dark);
            P(r, PrimitiveType.Cube, "BracoE", new Vector3(-0.55f, 1.3f, 0), new Vector3(0.22f, 0.8f, 0.25f), metal);
            P(r, PrimitiveType.Cube, "BracoD", new Vector3(0.55f, 1.3f, 0), new Vector3(0.22f, 0.8f, 0.25f), metal);
            var pivot = new GameObject("Knife");
            pivot.transform.SetParent(r.model, false);
            pivot.transform.localPosition = new Vector3(0.6f, 1.0f, 0.25f);
            r.knife = pivot.transform;
            P(r, PrimitiveType.Cube, "Lamina", new Vector3(0, 0, 0.45f), new Vector3(0.05f, 0.09f, 0.9f),
              Mats.Make(new Color(0.85f, 0.87f, 0.9f), 1f, 0.95f), r.knife);
            P(r, PrimitiveType.Cube, "Cabo", new Vector3(0, 0, -0.08f), new Vector3(0.07f, 0.07f, 0.22f), dark, r.knife);
        }

        static void Canine(RobotRig r, Material team, Material metal, Material dark, Material eye)
        {
            P(r, PrimitiveType.Cube, "Corpo", new Vector3(0, 0.75f, 0), new Vector3(0.6f, 0.5f, 1.3f), team);
            P(r, PrimitiveType.Cube, "Cabeca", new Vector3(0, 1.05f, 0.8f), new Vector3(0.45f, 0.4f, 0.45f), metal);
            P(r, PrimitiveType.Cube, "Focinho", new Vector3(0, 0.96f, 1.12f), new Vector3(0.25f, 0.2f, 0.3f), dark);
            P(r, PrimitiveType.Sphere, "Nariz", new Vector3(0, 1.0f, 1.28f), new Vector3(0.1f, 0.08f, 0.08f), dark);
            P(r, PrimitiveType.Cube, "OrelhaE", new Vector3(-0.16f, 1.34f, 0.72f), new Vector3(0.1f, 0.24f, 0.08f), dark);
            P(r, PrimitiveType.Cube, "OrelhaD", new Vector3(0.16f, 1.34f, 0.72f), new Vector3(0.1f, 0.24f, 0.08f), dark);
            r.eyeL = P(r, PrimitiveType.Sphere, "EyeL", new Vector3(-0.13f, 1.14f, 1.02f), new Vector3(0.11f, 0.11f, 0.08f), eye).transform;
            r.eyeR = P(r, PrimitiveType.Sphere, "EyeR", new Vector3(0.13f, 1.14f, 1.02f), new Vector3(0.11f, 0.11f, 0.08f), eye).transform;
            float[] xs = { -0.22f, 0.22f };
            float[] zs = { -0.5f, 0.5f };
            foreach (var x in xs)
                foreach (var z in zs)
                    P(r, PrimitiveType.Cube, "Pata", new Vector3(x, 0.25f, z), new Vector3(0.15f, 0.5f, 0.15f), dark);
            P(r, PrimitiveType.Cube, "Rabo", new Vector3(0, 1.0f, -0.78f), new Vector3(0.08f, 0.08f, 0.5f), metal, null, new Vector3(-35f, 0, 0));
        }

        static void Cephalopod(RobotRig r, Material team, Material metal, Material dark, Material eye)
        {
            P(r, PrimitiveType.Sphere, "Cabeca", new Vector3(0, 1.5f, 0), new Vector3(0.95f, 1.25f, 0.95f), team);
            P(r, PrimitiveType.Sphere, "Capacete", new Vector3(0, 1.75f, -0.05f), new Vector3(0.7f, 0.6f, 0.7f), metal);
            P(r, PrimitiveType.Sphere, "OlhoBaseE", new Vector3(-0.28f, 1.45f, 0.36f), new Vector3(0.28f, 0.28f, 0.2f), dark);
            P(r, PrimitiveType.Sphere, "OlhoBaseD", new Vector3(0.28f, 1.45f, 0.36f), new Vector3(0.28f, 0.28f, 0.2f), dark);
            r.eyeL = P(r, PrimitiveType.Sphere, "EyeL", new Vector3(-0.28f, 1.45f, 0.46f), new Vector3(0.18f, 0.18f, 0.1f), eye).transform;
            r.eyeR = P(r, PrimitiveType.Sphere, "EyeR", new Vector3(0.28f, 1.45f, 0.46f), new Vector3(0.18f, 0.18f, 0.1f), eye).transform;
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                Vector3 outward = new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a));
                var t = P(r, PrimitiveType.Capsule, "Tentaculo", outward * 0.3f + new Vector3(0, 0.6f, 0), new Vector3(0.14f, 0.5f, 0.14f), dark);
                t.transform.localRotation = Quaternion.AngleAxis(-25f, Vector3.Cross(Vector3.up, outward));
            }
        }

        static void Chameleon(RobotRig r, Material team, Material metal, Material dark, Material eye)
        {
            P(r, PrimitiveType.Sphere, "Corpo", new Vector3(0, 0.7f, 0), new Vector3(0.7f, 0.8f, 1.3f), team);
            P(r, PrimitiveType.Sphere, "Cabeca", new Vector3(0, 0.85f, 0.75f), new Vector3(0.5f, 0.45f, 0.55f), metal);
            P(r, PrimitiveType.Sphere, "CopaOlhoE", new Vector3(-0.28f, 1.05f, 0.8f), new Vector3(0.32f, 0.32f, 0.32f), dark);
            P(r, PrimitiveType.Sphere, "CopaOlhoD", new Vector3(0.28f, 1.05f, 0.8f), new Vector3(0.32f, 0.32f, 0.32f), dark);
            r.eyeL = P(r, PrimitiveType.Sphere, "EyeL", new Vector3(-0.29f, 1.07f, 0.94f), new Vector3(0.18f, 0.18f, 0.1f), eye).transform;
            r.eyeR = P(r, PrimitiveType.Sphere, "EyeR", new Vector3(0.29f, 1.07f, 0.94f), new Vector3(0.18f, 0.18f, 0.1f), eye).transform;
            float[] xs = { -0.4f, 0.4f };
            float[] zs = { -0.4f, 0.4f };
            foreach (var x in xs)
                foreach (var z in zs)
                    P(r, PrimitiveType.Cube, "Pata", new Vector3(x, 0.25f, z), new Vector3(0.15f, 0.5f, 0.15f), dark);
            for (int i = 0; i < 7; i++)
            {
                float s = Mathf.Lerp(0.34f, 0.1f, i / 6f);
                P(r, PrimitiveType.Sphere, "Cauda", new Vector3(0, 0.6f + 0.1f * i, -0.85f - 0.28f * Mathf.Sin(i * 0.5f)), new Vector3(s, s, s), metal);
            }
        }
    }
}
