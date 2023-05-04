using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.CA
{
    public class CellItem
    {
        public readonly int birthIteration;

        public CellItem(int birthIteration)
        {
            this.birthIteration = birthIteration;
        }
    }

    // interface ICellItem
    // {
    //     int Age { get; }
    //     void IncrementAge();
    // }

    public class Cell
    {
        public readonly int x;
        public readonly int y;

        private CellItem item; public CellItem Item => item;

        public Cell(int x, int y, CellItem item)
        {
            this.x = x;
            this.y = y;
            this.item = item;
        }

        public void SetItem(CellItem item)
        {
            this.item = item;
        }
    }

    public class GridState
    {
        public Vector2Int gridSize;
        public int iteration;

        public readonly List<Cell> cells;
        public readonly List<Cell> enabledCells;
        public readonly List<Cell> freeCells;

        public GridState(
            Vector2Int gridSize,
            int iteration = 0,
            List<Cell> cells = null,
            List<Cell> enabledCells = null,
            List<Cell> freeCells = null
        )
        {
            this.gridSize = gridSize;
            this.iteration = iteration;
            this.cells = cells ?? new List<Cell>();
            this.enabledCells = enabledCells ?? new List<Cell>();
            this.freeCells = freeCells ?? new List<Cell>();
        }

        public GridState Clone(int iteration)
        {
            var grid = new GridState(
                gridSize: gridSize,
                iteration: iteration,
                cells: new List<Cell>(cells),
                enabledCells: new List<Cell>(enabledCells),
                freeCells: new List<Cell>(freeCells)
            );

            return grid;
        }

        public void FillGrid()
        {
            if (cells.Count != 0) throw new System.Exception("cells list is not empty");

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    var cell = new Cell(
                        x: x,
                        y: y,
                        item: null
                    );

                    cells.Add(cell);
                    freeCells.Add(cell);
                }
            }
        }

        public Cell GetCellByPosition(int x, int y)
        {
            // Debug.Log($"{cells.Count} | {x},{y} | {y * gridSize.x + x} | ");

            return cells[y * gridSize.x + x];
        }

        public List<Cell> NeighborCells(Cell cell)
        {
            var positions = new List<Vector2Int>(){
                // top
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                // right center
                new Vector2Int(1, 0),
                // bottom
                new Vector2Int(1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1),
                // left center
                new Vector2Int(-1, 0)
            };

            var resultCells = new List<Cell>();

            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];

                // filter out unexisting cells outside the edges of the grid
                if (
                    (cell.x == 0 && pos.x == -1) ||
                    (cell.x == gridSize.x - 1 && pos.x == 1) ||
                    (cell.y == 0 && pos.y == -1) ||
                    (cell.y == gridSize.y - 1 && pos.y == 1)
                )
                {
                    continue;
                }

                var nbCell = GetCellByPosition(cell.x + pos.x, cell.y + pos.y);

                resultCells.Add(nbCell);
            }

            return resultCells;
        }

        public List<Cell> EnabledNeighborCells(Cell cell)
        {
            var nbCells = NeighborCells(cell);

            var resultCells = new List<Cell>();

            for (int i = 0; i < nbCells.Count; i++)
            {
                var nbCell = nbCells[i];

                // if (nbCell.Item != null)
                if (enabledCells.Contains(nbCell))
                {
                    resultCells.Add(nbCell);
                }
            }

            return resultCells;
        }
    }

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