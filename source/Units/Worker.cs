using Steel;
using SteelCustom.MapSystem;

namespace SteelCustom.Units
{
    public class Worker : Unit
    {
        protected override string SpritePath => _isMale ? "worker_m.png" : "worker_f.png";

        private bool _isMale;

        public override void Init(Tile tile)
        {
            _isMale = Random.NextFloat(0, 1) < 0.5f;
            
            base.Init(tile);
        }
    }
}