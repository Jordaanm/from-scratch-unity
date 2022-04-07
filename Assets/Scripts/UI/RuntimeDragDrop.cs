using System.Collections.Generic;
using System.Threading.Tasks;
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
        private Vector2 mousePos;

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
            KillGhost();
            //CancelDrag();
        }

        void HandleMouseMoveSource(MouseMoveEvent evt)
        {
            UpdateMousePos(evt.localMousePosition);
            if (ghost != null)
            {
                MoveGhost();
            }
        }

        public void UpdateMousePos(Vector2 newPos)
        {
            this.mousePos = newPos;
        }
        
        void MoveGhost()
        {
            if (ghost == null)
            {
                return;
            }
            
            Debug.LogFormat("Ghost:: Mouse Move {0}, {1}", mousePos.x.ToString(), mousePos.y.ToString());
            Debug.LogFormat("GG {0}, {1}", ghost.style.left.ToString(), ghost.style.top.ToString());
            ghost.style.left = new StyleLength(mousePos.x - ghost.worldBound.width / 2);
            ghost.style.top = new StyleLength(mousePos.y - ghost.worldBound.height / 2);
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

        private async void HandleMouseDownTarget(
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
                ghost.pickingMode = PickingMode.Ignore;
                ghost.style.visibility = Visibility.Hidden;
                AttachGhost(ghost);

                //Add a delay so the bounds/size of the element are known, allowing us to properly center it over the cursor.
                await Task.Delay(1);
                ghost.style.visibility = Visibility.Visible;
                MoveGhost();
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
                ghost = null;
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