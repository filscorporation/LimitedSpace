using Steel;

namespace SteelCustom
{
    public abstract class SelectableObject : ScriptComponent
    {
        public bool IsSelected { get; private set; }

        public void Select()
        {
            IsSelected = true;

            OnSelectedChanged();
        }

        public void Deselect()
        {
            IsSelected = false;

            OnSelectedChanged();
        }

        protected virtual void OnSelectedChanged() { }
    }
}