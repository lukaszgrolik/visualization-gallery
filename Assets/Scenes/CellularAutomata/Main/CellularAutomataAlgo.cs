using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    // public interface ICellularAutomataAlgo
    // {

    // }

    // public abstract class CellularAutomataAlgo : MonoBehaviour, ICellularAutomataAlgo
    public abstract class CellularAutomataAlgo : MonoBehaviour
    {
        // protected CellularAutomata cellularAutomata;

        // public void Init(CellularAutomata cellularAutomata)
        // {
        //     this.cellularAutomata = cellularAutomata;
        // }

        public abstract Core.CA.CellsGrid CreateCellsGrid(Vector2Int gridSize);
        public abstract CellItem OnCellItemSpawned(GameObject cellItemObj, Core.CA.Cell cell, CellularAutomata cellularAutomata);
    }
}