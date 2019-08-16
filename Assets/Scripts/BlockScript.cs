using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour {

    public static float DropSpeed = 1f;
    public Vector2 NewPos;
    bool _doOnce, _doOnce1;

    public Button RotateButton;
    public Button LeftButton;
    public Button RightButton;
    public Button DownButton;

    void Start ()
    {

        StartCoroutine(BlockDrop());
        NewPos = gameObject.transform.position;

        //Adding listeners for UI buttons so player can move tetriminos on mobile
        RotateButton.onClick.AddListener(() => RotatePiece());
        LeftButton.onClick.AddListener(MoveLeft);
        RightButton.onClick.AddListener(MoveRight);
        DownButton.onClick.AddListener(MoveDown);

    }

    //Methods for moving tetriminos on mobile device
    #region Touch Controls

// These methods move the gameObject then check to see if that new transform is in the grid, if not, Undo 
    public void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);
        if (IsInGrid())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    public void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        if (IsInGrid())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    public void MoveDown()
    {

        StartCoroutine(QuickDrop());


        if (IsInGrid())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(0, 1, 0);


        }
    }

    public void RotatePiece()
    {
        Debug.Log("1");
        transform.Rotate(0, 0, -90);

        if (IsInGrid())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);

        }
        else
        {
            transform.Rotate(0, 0, 90);
        }
    }
    #endregion

    //Methods for moving tetriminos on Keyboard
    void Update () {

        // These methods move the gameObject then check to see if that new transform is in the grid, if not, Undo 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {           
            transform.position += new Vector3(1, 0, 0);
            if (IsInGrid())
            {
                FindObjectOfType<GameManager>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (IsInGrid())
            {
                FindObjectOfType<GameManager>().UpdateGrid(this);

            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            StartCoroutine(QuickDrop());
            

            if (IsInGrid())
            {
                FindObjectOfType<GameManager>().UpdateGrid(this);

            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

      
            }
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            if (IsInGrid())
            {
                FindObjectOfType<GameManager>().UpdateGrid(this);

            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }

    }

    
    //Returns true if gameObject is in valid position in Grid
    bool IsInGrid()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<GameManager>().Round(mino.position);
            if (FindObjectOfType<GameManager>().CheckIsInsideGrid(pos) == false)
            {
              
                return false;
            }

            if (FindObjectOfType<GameManager>().GetTransform(pos) != null && FindObjectOfType<GameManager>().GetTransform(pos).parent != transform)
            {

                return false;
            }
            
        }


            return true;
    }

    //Allows player to drop tetrimino faster
    IEnumerator QuickDrop()
    {
        transform.position += new Vector3(0, -1, 0);

        if (IsInGrid())
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);

        }
        else
        {
            transform.position += new Vector3(0, 1, 0);


        }
        
        yield return new WaitForSeconds(.1f);
        if (Input.GetKey(KeyCode.DownArrow))
        {
            StartCoroutine(QuickDrop());
        }
    }

    //Drops the active block steadily
    IEnumerator BlockDrop()
    {

        yield return new WaitForSeconds(DropSpeed);

        NewPos = gameObject.transform.position;

        NewPos.y -= 1;
        gameObject.transform.position = NewPos;
        if(NewPos.y >= 1)
        {
            StartCoroutine(BlockDrop());

        }
        
        if (IsInGrid() && _doOnce == false && _doOnce1 == false)
        {
            FindObjectOfType<GameManager>().UpdateGrid(this);

        }
        else
        {
           //Destroys script so player can no longer control this block 
            _doOnce = true;
            transform.position += new Vector3(0, 1, 0);
            enabled = false;
            FindObjectOfType<GameManager>().DeleteRow();
            FindObjectOfType<GameManager>().SpawnNextBlock();
            gameObject.tag = "Block";
            Destroy(this);
            StopCoroutine(BlockDrop());

        }

        //checks to see if block is on bottom of grid
        if(NewPos.y <= 0 && _doOnce == false)
        {
            _doOnce1 = true;
            Invoke("SubtleDelay", 1);

        }

    }

    //Gives players a small window to still move block after it has hit the bottom most position
    public void SubtleDelay()
    {
        enabled = false;
        FindObjectOfType<GameManager>().DeleteRow();
        FindObjectOfType<GameManager>().SpawnNextBlock();
        Destroy(this);
    }
}
