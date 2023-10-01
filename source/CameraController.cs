using Steel;

namespace SteelCustom
{
    public class CameraController : ScriptComponent
    {
        public bool CanControl { get; set; }

        private const int GAP = 15;
        private const float SPEED = 8;
        
        public void Init()
        {
            RecenterCamera();

            CanControl = true;
        }

        public override void OnUpdate()
        {
            if (!CanControl)
                return;
            
            var mousePosition = Input.MousePosition;
            float delta = SPEED * Time.DeltaTime;
            
            if (mousePosition.X < GAP)
                MoveCamera(new Vector2(-delta, 0));
            if (mousePosition.X > Screen.Width - GAP)
                MoveCamera(new Vector2(delta, 0));
            if (mousePosition.Y < GAP)
                MoveCamera(new Vector2(0, -delta));
            if (mousePosition.Y > Screen.Height - GAP)
                MoveCamera(new Vector2(0, delta));
            
            if (Input.IsKeyPressed(KeyCode.A))
                MoveCamera(new Vector2(-delta, 0));
            if (Input.IsKeyPressed(KeyCode.D))
                MoveCamera(new Vector2(delta, 0));
            if (Input.IsKeyPressed(KeyCode.S))
                MoveCamera(new Vector2(0, -delta));
            if (Input.IsKeyPressed(KeyCode.W))
                MoveCamera(new Vector2(0, delta));

            if (Input.IsKeyJustPressed(KeyCode.F))
                RecenterCamera();
        }

        private void RecenterCamera()
        {
            var map = GameController.Instance.Map;
            Transformation.Position = new Vector3(map.Size * 0.5f, map.Size * 0.5f, Transformation.Position.Z);
        }

        private void MoveCamera(Vector2 offset)
        {
            Transformation.Position += (Vector3)offset;
        }
    }
}