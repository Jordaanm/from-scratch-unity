using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UI
{
    public interface IUserInterfaceLayer
    {
        VisualElement Root {
            get;
        }

        VisualElement BuildContents();

        void OnClose();
        void OnOpen();

        void SetLayerManager(LayerManager layerManager);
    }

    public class LayerDetails
    {
        public VisualElement element;
        public IUserInterfaceLayer instance;

        public LayerDetails(VisualElement element, IUserInterfaceLayer instance) {
            this.element = element;
            this.instance = instance;
        }
    }

    public class LayerManager
    {
        protected List<LayerDetails> m_layerDetails = new List<LayerDetails>();
        protected List<IUserInterfaceLayer> m_layers = new List<IUserInterfaceLayer>();
        protected VisualElement m_layerHost;

        public LayerManager(VisualElement host) {
            m_layerHost = host;
            LayerManager.RegisterLayerHost(host, this);
        }

        public VisualElement AddLayer(IUserInterfaceLayer instance, bool bringToFront = true) {

            VisualElement element = new VisualElement();
            element.AddToClassList("layer");
            m_layerHost.Add(element);
            element.pickingMode = PickingMode.Ignore;

            if (bringToFront) {
                element.BringToFront();
            }

            instance.SetLayerManager(this);

            if(instance.Root == null) {
                instance.BuildContents();
            }

            element.Add(instance.Root);

            m_layerDetails.Add(new LayerDetails(
                element,
                instance
            ));

            instance.OnOpen();


            return element;
        }

        public void RemoveLayer(IUserInterfaceLayer instance) {
            LayerDetails layerDetails = m_layerDetails.Find(x => x.instance == instance);
            while (layerDetails != null) {
                //Remove from Hierarchy
                layerDetails.element.RemoveFromHierarchy();

                //Remove from Layers
                m_layerDetails.Remove(layerDetails);

                layerDetails.instance.OnClose();

                layerDetails = m_layerDetails.Find(x => x.instance == instance);
            }
        }

        public VisualElement HostElement {
            get { return m_layerHost; }
        }

        public static Dictionary<VisualElement, LayerManager> layerContext = new Dictionary<VisualElement, LayerManager>();

        public static void RegisterLayerHost(VisualElement view, LayerManager layerManager) {
            layerContext.Add(view, layerManager);
        }

        public static LayerManager Find(VisualElement view) {
            LayerManager layerManager = null;
            while(view != null) {

                if(layerContext.TryGetValue(view, out layerManager)) {
                    return layerManager;
                }

                view = view.parent;
            }

            return null;
        }
    }

}