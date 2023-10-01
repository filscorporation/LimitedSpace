using System;
using System.Collections.Generic;
using SteelCustom.GameActions;

namespace SteelCustom
{
    public abstract class SelectableObject : VisualObject
    {
        public event Action OnChanged;
        
        public bool IsSelected { get; private set; }
        
        public abstract string Name { get; }
        public abstract List<GameAction> GameActions { get; }

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

        protected void CallOnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}