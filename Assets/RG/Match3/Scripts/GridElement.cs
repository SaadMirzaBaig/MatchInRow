using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElement : MonoBehaviour {

    public GridElement RightGridElement;

    public GridElement downGridElement;

    public GridElement upGridElement;

    private Tile tile;

    private int randomNum;

    public int myId;

    private bool hasTile;

    // RETURN THE ID IF THE GRID ELEMENT HAS A TILE
    public void SetID() {

        if ( hasTile ) {

            tile = transform.GetChild(0).GetComponent<Tile>();
            myId = tile.id;
        }

    }

    // RETURNS IF GRID ELEMENT HAS A TILE OR NOT
    public bool HasTile() {

        return (transform.childCount != 0);
    }

    // ON CLICK
    private void OnMouseDown() {

        if (hasTile && myId == 0)
        {

            RemoveTileAndArrange();
        }
    }


    public void SpawnTile(List<GameObject> Tile) {


        //CONTINUE IF IT DOESNT HAVE ANY TILE
        if ( !hasTile )
        {

            randomNum = Random.Range(0, 4);
            Instantiate(Tile[randomNum], transform.position, Quaternion.identity, transform);

            hasTile = true;

            SetID();
        }

        // REPLACE THE TILE WITH CLICKABLE OBJECT 
        else {

            RemoveTile();
            randomNum = Random.Range(0, 2);
            Instantiate(Tile[randomNum], transform.position, Quaternion.identity, transform);

            hasTile = true;
            StartCoroutine(WaitAndGetId());

        }
    }



    //REMOVE TILE AND ARRANGE THE GRID
    public void RemoveTileAndArrange() {

        if(upGridElement != null) {
            StartCoroutine(RemoveWaitToArrange());

        }
        else {

            RemoveTile();
        }

    }

    //REMOVE THE TILE
    public void RemoveTile() {

        Destroy(transform.GetChild(0).gameObject);
        
        // SET THE TILE STATUS TO FALSE
        hasTile = false;


    }

    public void SwapTiles() {

        //CHECK IF THE UPER NEIGHBOUR ISN'T NULL
        if ( upGridElement != null ) {
            
            //SWAP THE TILE WITH UPER NEIGHBOUR ONLY IF IT HAS CHILD

            if ( !hasTile && upGridElement.hasTile) {

                //SWAP THE IDs WITH EACH OTHER              
                myId = upGridElement.myId;
                upGridElement.myId = 0;

                // CHANGE THE PARENT OF THE TILE 
                upGridElement.transform.GetChild(0).SetParent(transform);    
                
                //RESET THE LOCAL POSITION OF THE CHILD
                transform.GetChild(0).localPosition = Vector3.zero;       

                //SWAP THE TILE STATUS
                hasTile = true;
                upGridElement.hasTile = false;

            }
            else if(!hasTile && !upGridElement.hasTile)
            {
                hasTile = false;
                upGridElement.hasTile = false;
            }

        }

    }

    IEnumerator WaitAndDestroy() {

        yield return new WaitForEndOfFrame();

        Destroy(transform.GetChild(0).gameObject);

        // SET THE TILE STATUS TO FALSE
        hasTile = false;
    }

    IEnumerator WaitAndGetId() {

        yield return new WaitForFixedUpdate();
        SetID();
    }

    IEnumerator RemoveWaitToArrange() {

        Destroy(transform.GetChild(0).gameObject);
        hasTile = false;

        yield return new WaitForEndOfFrame();

        BroadcastManager.ArrangeGrid?.Invoke();
    }

}
