using System.Collections;
using UnityEngine;

namespace Core.CA
{
    abstract public class CellsGrid
    {
        protected readonly Vector2Int gridSize;

        // protected readonly List<Cell> cells = new List<Cell>();
        // protected readonly List<Cell> enabledCells = new List<Cell>();
        // protected readonly List<Cell> freeCells = new List<Cell>();
        protected GridState currentGridState;
        protected GridState previousGridState = null;

        // private int iteration = 0;

        public event System.Action<Cell> cellItemSpawned;
        public event System.Action<Cell> cellItemDestroyed;

        public CellsGrid(
            Vector2Int gridSize
        )
        {
            this.gridSize = gridSize;
        }

        public void Init()
        {
            this.currentGridState = new GridState(
                gridSize: gridSize,
                iteration: 0
            );
            this.currentGridState.FillGrid();
        }

        abstract protected CellItem CreateCellItem(int iteration);
        abstract protected void OnInitialIteration();
        abstract protected void OnTick();

        public void Tick()
        {
            // Debug.Log($"iteration: {currentGridState.iteration} | enabledCells: {currentGridState.enabledCells.Count}");
            if (currentGridState.iteration == 0)
            {
                OnInitialIteration();
            }
            else
            {
                OnTick();
            }

            this.previousGridState = currentGridState;
            this.currentGridState = currentGridState.Clone(iteration: currentGridState.iteration + 1);

        }

        protected void SpawnCellItem(Cell cell)
        {
            // if (cell.Item != null) throw new System.Exception($"Cannot spawn cell item inside already filled cell ({cell.x},{cell.y})");
            if (currentGridState.enabledCells.Contains(cell)) throw new System.Exception($"Cannot spawn cell item inside already filled cell ({cell.x},{cell.y})");

            var item = CreateCellItem(currentGridState.iteration);

            cell.SetItem(item);

            currentGridState.enabledCells.Add(cell);
            currentGridState.freeCells.Remove(cell);

            cellItemSpawned?.Invoke(cell);
        }

        protected void DestroyCellItem(Cell cell)
        {
            // if (cell.Item == null) throw new System.Exception("Cannot destroy cell item inside empty cell ({cell.x},{cell.y})");
            if (currentGridState.freeCells.Contains(cell)) throw new System.Exception("Cannot destroy cell item inside empty cell ({cell.x},{cell.y})");

            cell.SetItem(null);

            currentGridState.enabledCells.Remove(cell);
            currentGridState.freeCells.Add(cell);

            cellItemDestroyed?.Invoke(cell);
        }
    }
}