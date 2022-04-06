using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class RuntimeDragDrop
    {
        VisualElement eventSource;
        VisualElement root;
        VisualElement ghost;

        bool isDragging = false;
        string currentDragType;
        object currentDragData;

        List<RunDndDragCase> dragCaseHandlers = new List<RunDndDragCase>();
        List<RunDndDropCase> dropCaseHandlers = new List<RunDndDropCase>();

        static System.Predicate<object> defaultPredicate = (object x) => true;

        public static RuntimeDragDrop Initialise(VisualElement root, VisualElement source = null)
        {
            if (source == null)
            {
                source = root;
            }

            var runDnd = new RuntimeDragDrop();

            return runDnd
                .SetEventSource(source)
                .SetRoot(root);
        }

        public RuntimeDragDrop SetEventSource(VisualElement source)
        {
            if (eventSource != null)
            {
                CleanupEventSource();
            }

            this.eventSource = source;
            source.RegisterCallback<MouseUpEvent>(HandleMouseUpSource);
            source.RegisterCallback<MouseLeaveEvent>(HandleMouseLeaveSource);
            source.RegisterCallback<MouseMoveEvent>(HandleMouseMoveSource);
            return this;
        }

        private void CleanupEventSource()
        {
            eventSource.UnregisterCallback<MouseUpEvent>(HandleMouseUpSource);
            eventSource.UnregisterCallback<MouseLeaveEvent>(HandleMouseLeaveSource);
        }

        public RuntimeDragDrop SetRoot(VisualElement root)
        {
            this.root = root;
            return this;
        }

        #region Case Handlers

        public RuntimeDragDrop AddDragCase(RunDndDragCase dragCase)
        {
            if (!dragCaseHandlers.Contains(dragCase))
            {
                dragCaseHandlers.Add(dragCase);
            }

            return this;
        }

        public RuntimeDragDrop AddDropCase(RunDndDropCase dropCase)
        {
            if (!dropCaseHandlers.Contains(dropCase))
            {
                dropCaseHandlers.Add(dropCase);
            }

            return this;
        }

        #endregion

        #region Canvas Mouse Event Handlers

        void HandleMouseLeaveSource(MouseLeaveEvent evt)
        {
            CancelDrag();
        }

        void HandleMouseUpSource(MouseUpEvent evt)
        {
            //CancelDrag();
        }

        void HandleMouseMoveSource(MouseMoveEvent evt)
        {
            if (ghost != null)
            {
                MoveGhost(evt.localMousePosition);
            }
        }

        void MoveGhost(Vector2 localMousePosition)
        {
            ghost.style.left = new StyleLength(localMousePosition.x - ghost.worldBound.width / 2);
            ghost.style.top = new StyleLength(localMousePosition.y - ghost.worldBound.height / 2);
        }

        #endregion

        #region Drag Mouse Event Handlers

        public void MakeDraggable(
            VisualElement element,
            string type,
            System.Func<object> getDataCallback,
            System.Predicate<object> canDrag = null
        )
        {
            element.RegisterCallback<MouseDownEvent>(evt => HandleMouseDownTarget(evt, type, getDataCallback, canDrag));
        }

        private void HandleMouseDownTarget(
            MouseDownEvent evt,
            string type,
            System.Func<object> getDataCallback,
            System.Predicate<object> canDrag
        )
        {

            if (isDragging)
            {
                return;
            }

            var data = getDataCallback();
            if (canDrag != null && !canDrag(data))
            {
                return;
            }

            //Find Matching Case
            var matchingCase = dragCaseHandlers.Find(x => x.matchesCase(type, data));

            if (matchingCase != null)
            {
                //Make Active
                isDragging = true;
                currentDragType = type;
                currentDragData = data;
                // Create Ghost
                ghost = matchingCase.makeGhost(currentDragType, currentDragData);
                ghost.style.position = Position.Absolute;
                AttachGhost(ghost);
                ghost.pickingMode = PickingMode.Ignore;
                MoveGhost(evt.localMousePosition);
            }
        }

        #endregion

        #region Drop Mouse Event Handlers

        public void MakeDropTarget(VisualElement element, string type, System.Func<object> getDataCallback)
        {
            element.RegisterCallback<MouseUpEvent>(evt => HandleMouseUpDropTarget(evt, type, getDataCallback));
        }

        private void HandleMouseUpDropTarget(MouseUpEvent evt, string type, System.Func<object> getDataCallback)
        {
            if (!isDragging)
            {
                return;
            }

            var data = getDataCallback();

            //Find Matching Case
            var matchingCase = dropCaseHandlers.Find(x => x.canReceive(currentDragType, type, currentDragData, data));

            if (matchingCase != null)
            {
                // Kill Ghost
                KillGhost();

                //Perform Drop Operation
                matchingCase.handleDrop(currentDragType, type, currentDragData, data);

                //Make Inactive
                isDragging = false;
                currentDragType = null;
                currentDragData = null;
            }
        }

        #endregion

        private void AttachGhost(VisualElement ghost)
        {
            root.Add(ghost);
        }

        private void KillGhost()
        {
            if (ghost != null)
            {
                ghost.RemoveFromHierarchy();
            }
        }

        private void CancelDrag()
        {
            isDragging = false;
            currentDragData = null;
            currentDragType = null;
            KillGhost();
        }
    }
    
    public class RunDndDragCase
    {
        public delegate bool MatchCase(string type, object data);
        public delegate VisualElement MakeGhost(string type, object data);

        public MatchCase matchesCase;
        public MakeGhost makeGhost;

        public RunDndDragCase(MatchCase matchesCase, MakeGhost makeGhost) {
            this.matchesCase = matchesCase;
            this.makeGhost = makeGhost;
        }
    }

    public class RunDndDropCase
    {
        public delegate bool CanReceive(string dragType, string dropType, object dragData, object dropData);
        public delegate void HandleDrop(string dragType, string dropType, object dragData, object dropData);

        public CanReceive canReceive;
        public HandleDrop handleDrop;

        public RunDndDropCase(CanReceive canReceive, HandleDrop handleDrop) {
            this.canReceive = canReceive;
            this.handleDrop = handleDrop;
        }
    }
}