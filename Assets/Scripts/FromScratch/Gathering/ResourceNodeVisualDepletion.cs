using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FromScratch.Gathering
{
    [System.Serializable]
    public struct ResourceDepletionView
    {
        public int maximumStock;

        [ChildGameObjectsOnly]
        public GameObject view;
    }
    
    public class ResourceNodeVisualDepletion: MonoBehaviour
    {
        public GameObject defaultView;

        public List<ResourceDepletionView> views = new List<ResourceDepletionView>();

        private ResourceNode resourceNode;
        public void Awake()
        {
            resourceNode = GetComponent<ResourceNode>();
            resourceNode.onGathered.AddListener(OnDeplete);
        }

        private void Start()
        {
            ShowDefaultView();
        }

        private void ShowDefaultView()
        {
            //Hide all
            foreach (var resourceDepletionView in views)
            {
                resourceDepletionView.view.SetActive(false);
            }
            //Show Default
            defaultView.SetActive(true);
        }

        private void ShowView(GameObject view)
        {
            defaultView.SetActive(false);

            foreach (var resourceDepletionView in views)
            {
                if (resourceDepletionView.view == view)
                {
                    view.SetActive(true);
                }
                else
                {
                    view.SetActive(false);
                }
            }
        }

        private GameObject DetermineView(int stock)
        {
            foreach (var view in views)
            {
                if (stock <= view.maximumStock)
                {
                    return view.view;
                }
            }
            return null;
        }

        private void OnDeplete()
        { 
            Debug.LogFormat("OnDeplete {0}", resourceNode.remainingStock.ToString());
            GameObject viewToShow = DetermineView(resourceNode.remainingStock);
            if (viewToShow == null)
            {
                ShowDefaultView();
            }
            else
            {
                ShowView(viewToShow);
            }
        }
    }
}