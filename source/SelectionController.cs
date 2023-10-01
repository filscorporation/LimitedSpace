using System;
using System.Collections.Generic;
using Steel;

namespace SteelCustom
{
    public class SelectionController : ScriptComponent
    {
        public event Action OnSelectedChanged;
        
        public SelectableObject Hovered { get; set; }
        public List<SelectableObject> SelectedObjects => new List<SelectableObject>(_selectedObjects);
        
        private readonly List<SelectableObject> _selectedObjects = new List<SelectableObject>();
        
        public override void OnUpdate()
        {
            if (UI.IsPointerOverUI())
                return;
            
            if (GameController.Instance.BuilderController.HasDraft)
                return;
            
            if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
            {
                if (Hovered != null)
                    Select(Hovered);
                else
                    DeselectAll();
            }
        }

        private void Select(SelectableObject selectableObject)
        {
            foreach (SelectableObject selectedObject in _selectedObjects)
                selectedObject.Deselect();
            _selectedObjects.Clear();

            selectableObject.Select();
            _selectedObjects.Add(selectableObject);
            
            OnSelectedChanged?.Invoke();
            
            Log.LogInfo($"Selected {selectableObject}");
        }

        private void DeselectAll()
        {
            foreach (SelectableObject selectedObject in _selectedObjects)
                selectedObject.Deselect();
            _selectedObjects.Clear();
            
            OnSelectedChanged?.Invoke();
        }
    }
}