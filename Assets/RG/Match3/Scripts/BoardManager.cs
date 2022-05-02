using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    private GridElement[,] GridElementComponent;                        //2D array to hold each grid object's element component

    private GameObject gridElement;                                     //to spawn empty game object in the grid

    [SerializeField] private GameObject Grid;                           //Main parent of grid


    //to initialize dynamic grid

    [Range(3,9)]
    [SerializeField] private int numberOfRows;
    
    [Range(3, 9)]
    [SerializeField] private int numberOfColumns;         

    [SerializeField] private List<GameObject> Tiles;                    //Clickable and non clickable objects


    private void OnEnable() {

        BroadcastManager.ArrangeGrid.AddListener(ArrangeGrid);
        BroadcastManager.InitializeGame.AddListener(InitializeGrid);
        BroadcastManager.ClearGrid.AddListener(ClearGrid);
    }

    private void OnDisable() {

        BroadcastManager.ArrangeGrid.RemoveListener(ArrangeGrid);
        BroadcastManager.InitializeGame.RemoveListener(InitializeGrid);
        BroadcastManager.ClearGrid.RemoveListener(ClearGrid);

    }

    private void Start() {

        InitializeGrid();
    }


    //INITIALZING MAIN GRID
    private void InitializeGrid() {

        GridElementComponent = new GridElement[numberOfColumns, numberOfRows];

        for (int column = 0 ; column < numberOfColumns ; column++) {
            for (int row = 0 ; row < numberOfRows ; row++) {
                SpawnGridElements(column, row);
            }
        }

        PopulateGridElement();

    }

    //SPAWNING EMPTY GRID ELEMENTS
    private void SpawnGridElements(int x, int y) {

        gridElement = new GameObject("x: " + x + "y: " + y);    //naming with respect to x and y position for readablity
        gridElement.transform.position = new Vector3(x, y);     //setting the positions

        gridElement.transform.SetParent(Grid.transform);        //Orgainizing Hierarchy

        gridElement.AddComponent<GridElement>();        // attaching Element properties with each grid element
        gridElement.AddComponent<BoxCollider2D>();      // adding collider for on click detection

        GridElementComponent[x, y] = gridElement.GetComponent<GridElement>();
    }


    //Populate the each grid element with its neiboughr's information
    private void PopulateGridElement() {

        //Spawning objects from DOWN to UP

        for (int column = 0 ; column < numberOfColumns ; column++)
        {
            for (int row = 0 ; row < numberOfRows ; row++)
            {
                GridElementComponent[column, row].SpawnTile(Tiles); //SPAWN THE TILE FOR EACH GRID ELEMENT

                AssignNeighbours(column, row);  // UPDATING NEIGHBOURS INFO FOR EACH TILE

            }

        }

        CheckForSimillarTilesOnStart();

    }


    private void AssignNeighbours(int column, int row) {

        //Assign below object if its not the first row of the grid
        if (row != 0) {
            GridElementComponent[column, row].downGridElement = GridElementComponent[column, row - 1];
        }

        //Assign left object if its not the last colum of the grid
        if ((column + 1) != numberOfColumns) {
            GridElementComponent[column, row].RightGridElement = GridElementComponent[column + 1, row];
        }

        //Assign above object if its not the last row of the grid
        if (row + 1 != numberOfRows) {
            GridElementComponent[column, row].upGridElement = GridElementComponent[column, row + 1];
        }
    }

    // PREVENT FORM 3 IN A ROW AT THE BEGINING OF THE GAME
    private void CheckForSimillarTilesOnStart() {

        int matchCountPurple = 1;
        int matchCountYellow = 1;

        for (int row = 0 ; row < numberOfRows ; row++) {

            //reset counter after every row check

            for (int column = 0 ; column < numberOfColumns ; column++) {

                //Tile with ID = 0 is CLICKABLE tile and it CAN BE IGNORED

                if (GridElementComponent[column, row].RightGridElement != null && GridElementComponent[column, row].myId > 0 ) {
                    // CHCEK MATCH WITH TILE ID = 1
                    if (GridElementComponent[column, row].myId == 1 && GridElementComponent[column, row].RightGridElement.myId == 1)
                    {
                        matchCountPurple++;

                        if (matchCountPurple == 3)
                        {
                            // REPLACE WITH A NEW TILE IF 3 IN A ROW
                            GridElementComponent[column, row].SpawnTile(Tiles);

                            matchCountPurple = 1;
                        }

                    }
                    // CHECK MATCH WITH TILE ID = 2
                    else if ( GridElementComponent[column, row].myId == 2 && GridElementComponent[column, row].RightGridElement.myId == 2 ){

                        matchCountYellow++;

                        if (matchCountYellow == 3){
                            // REPLACE WITH A NEW TILE IF 3 IN A ROW
                            GridElementComponent[column, row].SpawnTile(Tiles);
                            matchCountYellow = 1;
                        }

                    }

                }

                //reset match count meter every 3 in row check
                else {

                    matchCountPurple = 1;
                    matchCountYellow = 1;
                }

            }
        }
    }

    private void CheckMatchToScore() {

        // TEMPORARY HOLDS LAST KNOW COLUMN VALUE FOR PURPLE AND YELLOW TILE
        int tempHoldColumn = 0;
        int matchCount;


        for ( int row = 0 ; row < numberOfRows ; row++ ) {

            matchCount = 1;

            for ( int column = 0 ; column < numberOfColumns ; column++ ) {

                //CHECK IF IT IS NOT THE END OF THE GRID 
                // ID 1 IS FOR PURPLE TILE
                // ID 2 IS FOR YELLOW TILE

                if (GridElementComponent[column, row].RightGridElement != null) {

                    if (GridElementComponent[column, row].myId == 1 && GridElementComponent[column, row].RightGridElement.myId == 1 ||
                        GridElementComponent[column, row].myId == 2 && GridElementComponent[column, row].RightGridElement.myId == 2)
                    {
                        matchCount++;
                        tempHoldColumn = column;

                    }

                    else
                    {
                        if (matchCount > 2)
                        {
                            for (int i = 0 ; i < matchCount ; i++)
                            {
                                GridElementComponent[tempHoldColumn + 1, row].RemoveTile();
                                tempHoldColumn--;
                            }

                            StartCoroutine(WaitToArrange());

                        }

                        matchCount = 1;
                    }

                }
                else {

                    if (matchCount > 2)
                    {
                        for (int i = 0 ; i < matchCount ; i++)
                        {
                           GridElementComponent[tempHoldColumn + 1, row].RemoveTile();
                            tempHoldColumn--;
                        }

                        //ARRANGE THE GRID AFTER REMOVING THE MATCHING TILES
                        StartCoroutine(WaitToArrange());
                    }

                    matchCount = 1;

                }

            }
        }

    }


    //Arrange the grid after tiles being removed
    public void ArrangeGrid() {

        //Traversing grid from DOWN to UP

        for (int column = 0 ; column < numberOfColumns ; column++ ) {

            for (int row = 0 ; row < numberOfRows ; row++) {

                GridElementComponent[column, row].SwapTiles();

            }
        }

        CheckMatchToScore();
    }

    IEnumerator WaitToArrange() {

        yield return new WaitForEndOfFrame();

        ArrangeGrid();
    }


    private void ClearGrid() {

        int numberOfGridElements = Grid.transform.childCount;

        for ( int i = 0 ; i < numberOfGridElements ; i++ ) {

            Destroy(Grid.transform.GetChild(i).gameObject);

        }



    }
}
