using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
class Config
 {
    gridSize
    gridLineSize
    gridColor
    cameraViewSize
    cellItemSize
    tickInterval
    algo
}
*/

abstract class Bla
{

}

class Bla_X : Bla
{

}

class X
{
    static public void From()
    {
        var bla = new Bla_X();

        var x = new X();
        x.Test(bla);
    }

    public void Test(Bla bla)
    {

    }
}

namespace Main
{

    // public class CellularAutomata : CellularAutomata<Core.Automata.GameOfLife.CellItem> {}
    // public class CellularAutomata : CellularAutomata { }

    public class CellularAutomata : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = Vector2Int.one * 25;

        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellsContainer;
        [SerializeField] private Transform cellOrigin;
        [SerializeField] private Transform surfaceContainer;
        [SerializeField] private MeshRenderer gridRend;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cellItemSize = .5f; public float CellItemSize => cellItemSize;

        [SerializeField] private float tickInterval = .05f;

        [SerializeField] private CellularAutomataAlgo algo;
        // private CellularAutomataAlgo_TestAutomaton algo_testAutomata;
        // private CellularAutomataAlgo_GameOfLife algo_gameOfLife;

        private Core.CA.CellsGrid cellsGrid;
        private float lastTickTime = -Mathf.Infinity;

        // private Dictionary<CellItem, GameObject> dict_cellItem_gameObject = new Dictionary<CellItem, GameObject>();
        private List<CellItem> cellItems = new List<CellItem>();
        private Dictionary<Core.CA.Cell, CellItem> dict_cell_cellItem = new Dictionary<Core.CA.Cell, CellItem>();

        // Start is called before the first frame update
        void Start()
        {
            CreateCellsGrid();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) == false && Input.GetKeyDown(KeyCode.Space))
            {
                CreateCellsGrid();
            }

            if (Time.time >= lastTickTime + tickInterval)
            {
                cellsGrid.Tick();

                for (int i = 0; i < cellItems.Count; i++)
                {
                    cellItems[i].OnIteration(cellsGrid.Iteration);
                }

                lastTickTime = Time.time;
            }
        }

        void Clear()
        {
            foreach (var item in dict_cell_cellItem)
            {
                Destroy(item.Value.gameObject);
            }

            cellItems.Clear();
            dict_cell_cellItem.Clear();
        }

        void CreateCellsGrid()
        {
            if (dict_cell_cellItem.Count > 0) Clear();

            // algo.Init(
            //     cellularAutomata: this
            // );

            cellsGrid = algo.CreateCellsGrid(
                gridSize: gridSize
            );
            cellsGrid.Init();

            cellsGrid.cellItemSpawned += OnCellItemSpawned;
            cellsGrid.cellItemDestroyed += OnCellItemDestroyed;
        }

        void OnCellItemSpawned(Core.CA.Cell cell)
        {
            // var cellItem = cell.Item as Core.Automata.GameOfLife.CellItem;

            var offset = new Vector3(.5f, 0, .5f);
            var pos = cellOrigin.position + new Vector3(cell.x * cellSize, 0, cell.y * cellSize) + offset;

            var cellItemObj = Instantiate(cellPrefab, pos, Quaternion.identity, cellsContainer);
            cellItemObj.transform.localScale *= cellItemSize;

            var cellItemScript = algo.OnCellItemSpawned(cellItemObj, cell, this);

            cellItems.Add(cellItemScript);
            dict_cell_cellItem.Add(cell, cellItemScript);
        }

        void OnCellItemDestroyed(Core.CA.Cell cell)
        {
            Destroy(dict_cell_cellItem[cell].gameObject);

            cellItems.Remove(dict_cell_cellItem[cell]);
            dict_cell_cellItem.Remove(cell);
        }

        void OnValidate()
        {
            if (gridSize.x != gridSize.y)
            {
                gridSize.y = gridSize.x;
                Debug.LogWarning("Currently grid must be a square"); // @todo update grid shader to handle rectangular grids (separate rows and columns)
            }

            if (gridSize.x != 0 && gridSize.y != 0)
            {
                surfaceContainer.localScale = new Vector3(gridSize.x / 10f, 1, gridSize.y / 10f);

                var props = new MaterialPropertyBlock();
                props.SetFloat("_CellsPerRow", gridSize.x);
                var lineSize = 0.1f;
                props.SetFloat("_LineSize", lineSize / gridSize.x);

                gridRend.SetPropertyBlock(props);
            }

        }
    }
}
