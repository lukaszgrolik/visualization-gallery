using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public abstract class CellItem : MonoBehaviour
    {
        protected Core.CA.CellItem cellItem;

        protected MeshRenderer meshRend;

        public void Setup(Core.CA.CellItem cellItem)
        {
            this.cellItem = cellItem;

            // OnSetup();
        }

        abstract public void OnIteration(int iteration);

        // protected abstract void OnSetup();
    }
}
