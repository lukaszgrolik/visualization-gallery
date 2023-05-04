using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public sealed class CellItem_GameOfLife : CellItem
    {
        private int colorsCount;
        private int colorGenerationSize;

        public void Setup(Core.Automata.GameOfLife.CellItem cellItem, int colorsCount, int colorGenerationSize)
        {
            base.Setup(cellItem);

            this.colorsCount = colorsCount;
            this.colorGenerationSize = colorGenerationSize;

            this.meshRend = GetComponentInChildren<MeshRenderer>();
            var tempMat = new Material(this.meshRend.sharedMaterial);

            this.meshRend.sharedMaterial = tempMat;

            SetColor();
        }

        void SetColor()
        {
            var cellItem = this.cellItem as Core.Automata.GameOfLife.CellItem;

            var hue = (float)(cellItem.birthIteration / colorGenerationSize) % colorsCount / colorsCount;
            var color = Color.HSVToRGB(hue, .5f, .5f);

            meshRend.sharedMaterial.SetColor("_Color", color);
        }
    }
}
