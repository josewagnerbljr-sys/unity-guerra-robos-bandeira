using UnityEngine;

namespace RoboFlagWars
{
    public class PlayerInput : MonoBehaviour
    {
        public RobotController rc;
        public CameraRig cam;
        public bool targetingEnemy;

        void Update()
        {
            var gm = GameManager.Instance;
            var ui = GameUI.Instance;
            if (gm == null || ui == null || rc == null || cam == null) return;
            if (!gm.Playing) { rc.moveWorld = Vector3.zero; rc.wantFire = false; return; }

            Vector2 mv = ui.MoveInput;
            Vector2 look = ui.ConsumeLook();
            bool fire = ui.FireHeld;
            bool sp = ui.ConsumeSpecial();

#if ENABLE_LEGACY_INPUT_MANAGER
            // atalhos para testar no PC: WASD/setas, botao direito do mouse = olhar, ESPACO = laser, E = especial
            mv += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetMouseButton(1)) look += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 8f;
            fire |= Input.GetKey(KeyCode.Space);
            if (Input.GetKeyDown(KeyCode.E)) sp = true;
#endif
            cam.AddLook(look);
            mv = Vector2.ClampMagnitude(mv, 1f);
            Vector3 f = cam.FlatForward, r = cam.FlatRight;
            rc.moveWorld = f * mv.y + r * mv.x;
            rc.lookDir = f;
            rc.aimPoint = cam.ComputeAimPoint(rc, out targetingEnemy);
            rc.wantFire = fire;
            if (sp) rc.wantSpecial = true;
        }
    }
}
