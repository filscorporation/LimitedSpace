using System.Collections;
using Steel;

namespace SteelCustom
{
    public class VisualObject : ScriptComponent
    {
        private Material _material;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _resetCoroutine;
        private float _alpha;
        private Color _color;

        protected void ReplaceShader()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (_material == null)
            {
                Shader shader = ResourcesManager.GetShader("custom_shader.glsl");
                _material = ResourcesManager.CreateMaterial(shader);
            }
            _spriteRenderer.Material = _material;

            ResetOverride();
        }

        protected void OverrideColor(Color color, float delay)
        {
            OverrideColor(color);
            if (_resetCoroutine != null)
                StopCoroutine(_resetCoroutine);
            _resetCoroutine = StartCoroutine(ResetOverrideDelayed(delay));
        }

        protected void OverrideColor(Color color)
        {
            _color = color;
            
            var props = _spriteRenderer.CustomMaterialProperties;
            props.SetColor("override_color", new Color(_color.R, _color.G, _color.B, _alpha));
            props.SetInt("override_color_flag", 1);
            _spriteRenderer.CustomMaterialProperties = props;
        }

        protected void OverrideAlpha(float alpha)
        {
            _alpha = alpha;
            
            var props = _spriteRenderer.CustomMaterialProperties;
            props.SetColor("override_color", new Color(_color.R, _color.G, _color.B, _alpha));
            props.SetInt("override_alpha_flag", 1);
            _spriteRenderer.CustomMaterialProperties = props;
        }

        protected void ResetOverride()
        {
            _color = Color.White;
            _alpha = 1;
            
            var props = _spriteRenderer.CustomMaterialProperties;
            props.SetInt("override_color_flag", 0);
            props.SetInt("override_alpha_flag", 0);
            props.SetColor("override_color", Color.White);
            _spriteRenderer.CustomMaterialProperties = props;
        }

        protected void ResetOverrideColor()
        {
            _color = Color.White;
            
            var props = _spriteRenderer.CustomMaterialProperties;
            props.SetInt("override_color_flag", 0);
            _spriteRenderer.CustomMaterialProperties = props;
        }

        private IEnumerator ResetOverrideDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            ResetOverride();
            _resetCoroutine = null;
        }
    }
}