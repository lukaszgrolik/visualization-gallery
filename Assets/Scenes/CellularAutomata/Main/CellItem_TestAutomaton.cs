using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public sealed class CellItem_TestAutomaton : CellItem
    {
        // private Core.Automata.TestAutomaton.CellItem cellItem;
        private float maxSize;
        private float ageGrowth;

        public void Setup(Core.Automata.TestAutomaton.CellItem cellItem, float maxSize, float ageGrowth)
        {
            base.Setup(cellItem);

            this.maxSize = maxSize;
            this.ageGrowth = ageGrowth;

            cellItem.aged += OnAged;
            cellItem.energyChanged += OnEnergyChanged;

            this.meshRend = GetComponentInChildren<MeshRenderer>();
            var tempMat = new Material(this.meshRend.sharedMaterial);

            this.meshRend.sharedMaterial = tempMat;

            SetColor();
        }

        void OnAged(Core.CA.CellItem cellItem)
        {
            transform.localScale = transform.localScale.With(y: transform.localScale.y + ageGrowth);

            SetColor();
        }

        void OnEnergyChanged(Core.Automata.TestAutomaton.CellItem cellItem)
        {
            var newScale = Mathf.Lerp(0, maxSize, cellItem.EnergyProgress);
            // Debug.Log($"newScale: {newScale} | cellItem.EnergyProgress: {cellItem.EnergyProgress}");

            transform.localScale = (Vector3.one * newScale).With(y: transform.localScale.y);
        }

        void SetColor()
        {
            var cellItem = this.cellItem as Core.Automata.TestAutomaton.CellItem;

            var hue = (1f / 3f) * (1 - cellItem.Progress);
            var color = Color.HSVToRGB(hue, .5f, .5f);

            meshRend.sharedMaterial.SetColor("_Color", color);
        }
    }
}
