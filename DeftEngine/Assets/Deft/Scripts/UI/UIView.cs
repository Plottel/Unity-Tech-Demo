using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deft.UI
{
    public class UIView : Manager
    {
        private List<UIPanel> panelList;
        private Dictionary<Type, List<UIPanel>> panelLookup;

        void Awake()
        {
            panelList = new List<UIPanel>();
            panelLookup = new Dictionary<Type, List<UIPanel>>();
        }

        public void AddPanel(Canvas canvasPrefab, UIPanel panelPrefab, string name)
        {
            Type t = panelPrefab.GetType();
            var newCanvas = Instantiate(canvasPrefab, transform);
            var newPanel = Instantiate(panelPrefab, newCanvas.transform);

            newPanel.canvas = newCanvas;
            newPanel.IsVisible = false;
            newPanel.name = name;
            newCanvas.name = string.Concat("Canvas--", newPanel.name);

            List<UIPanel> panelLookupEntry;
            if (!panelLookup.TryGetValue(t, out panelLookupEntry))
            {
                panelLookupEntry = new List<UIPanel>();
                panelLookup.Add(t, panelLookupEntry);
            }

            panelList.Add(newPanel);
            panelLookupEntry.Add(newPanel);
        }

        public T GetPanel<T>() where T : UIPanel
        {
            if (panelLookup.TryGetValue(typeof(T), out List<UIPanel> panelLookupEntry))
                return panelLookupEntry[0] as T;
            return null;
        }

        public T GetPanel<T>(string name) where T : UIPanel
        {
            if (panelLookup.TryGetValue(typeof(T), out List<UIPanel> panelLookupEntry))
            {
                foreach (var panel in panelLookupEntry)
                {
                    if (panel.name == name)
                        return panel as T;
                }

                return null;
            }

            return null;
        }
    }
}

