using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {

    #region Declaring Variables
    public static int GridWidth = 10;
    public static int GridHeight = 20;

    public Transform[,] Grid = new Transform [GridWidth, GridHeight];

    public int FullRow;
    public int AddTime;

    public int LinesToClear;
    public int ClearedLines;
    int _rollOverLines;

    public int Timer;
    public int CurrentLvl;

    public Text ToClear, Cleared, Level, TimerText;

    public GameObject GameOverPanel;
    public bool GameOver;
    #endregion

    //Resetting game timer & level requirements on game start
    void Start ()
    {
        Timer = 60;
        CurrentLvl = 1;
        LinesToClear = 3;
        StartCoroutine(TimerCounter());

	}
	
    //Runs checks intergral to gameplay on Update
	void Update ()
    {

        //If player meets the requiremnts for set level, start next level, increase speed
        if(ClearedLines >= LinesToClear)
        {
            if(ClearedLines > LinesToClear)
            {
                _rollOverLines = ClearedLines - LinesToClear;
            }

            ClearedLines = 0 + _rollOverLines;
            CurrentLvl++;
            LinesToClear += 2;

            if(BlockScript.DropSpeed != .2f)
            {
                BlockScript.DropSpeed -= .15f;
            }
            else
            {
                BlockScript.DropSpeed  = .1f;

            }
        }   

        //Reset tags on tetrimino gameObjects on lose condition
       if (GameOver == true)
        {
            foreach (GameObject mino in GameObject.FindGameObjectsWithTag("Block"))
            {

                mino.tag = "Untagged";
            }

            //If players presses any button ( besides the power button ;) ) on lose the game will restart
            if (Input.anyKeyDown)
            {
                GameOver = false;
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }
        }

       // Checks to see if the player has already lost, and if not checks to see if the player is currently losing.
       if(GameOver == false)
        {
            if (GameObject.FindGameObjectWithTag("Block") != null)
            {
                //Cylces through each "Mino" object to see if they are above the allowed grid height, if so, game over !
                foreach (GameObject mino in GameObject.FindGameObjectsWithTag("Block"))
                {

                    if (mino.transform.position.y >= GridHeight - 1)
                    {
                        GameOver = true;
                        Time.timeScale = 0.01f;
                        GameOverPanel.SetActive(true);
                        Debug.Log(mino.transform.position.y);
                    }
                }

            }

        }
    }


    //Adjust avalible slots in Grid as tetrimino gameObjects move
    public void UpdateGrid(BlockScript blocks)
    {
        for (int y=0; y < GridHeight; y++)
        {
            for(int x = 0; x < GridWidth; x++)
            {
                if(Grid [x,y] != null)
                {
                    if (Grid[x,y].parent == blocks.transform)
                    {
                        Grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in blocks.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < GridHeight -1)
            {
                Grid[(int)pos.x , (int)pos.y] = mino;
            }
        }
    }

    //Check to see if player has completed a full row
    public bool IsRowFull(int y)
    {
        //return false if there is a null transform
        for (int x = 0; x < GridWidth; x++)
        {
            if (Grid[x, y] == null)
            {
                return false;
            }
        }

    //Else row is cleared add score
        ClearedLines++;
        AddTime++;

        if (AddTime > 5)
        {
            Timer += 50;
        }
        else
        {
            switch (AddTime)
            {
                case 1:
                    Timer += 20;
                    AddTime = 0;
                    break;
                case 2:
                    Timer += 25;
                    AddTime = 0;

                    break;
                case 3:
                    Timer += 30;
                    AddTime = 0;

                    break;
                case 4:
                    Timer += 35;
                    AddTime = 0;

                    break;
            }
        }

        Debug.Log(ClearedLines);
        return true;
    }

    //Method that calls helper methods to remove rows
    public void DeleteRow()
    {
        for (int y = 0; y < GridHeight; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteMinoAt(y);
                MoveAllDown(y + 1);
                y--;

            }
        }

    }

    //Helper Method that loops to get all minos in a full row and destroy them
    public void DeleteMinoAt (int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            Destroy(Grid[x, y].gameObject);
            Grid[x, y] = null;
        }
    }


    public void MoveAllDown(int y)
    {
        for (int i = y; i < GridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    //After "x" rows are destroyed, move rows above down that many rows
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            if (Grid[x,y] != null)
            {
                Grid[x, y - 1] = Grid[x, y];
                Grid[x, y] = null;
                Grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }

    }

    public Transform GetTransform (Vector2 pos)
    {
        if (pos.y > GridHeight - 1)
        {
            return null;
        }
        else
        {
            return Grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextBlock()
    {
        GameObject nextBlock = (GameObject)Instantiate(Resources.Load(GetRandomBlock(), typeof(GameObject)), new Vector2(5, GridHeight), Quaternion.identity); 
    }

    //is mino at valid grid position
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < GridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round (Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    //switch on random int for next block to spawn
    string GetRandomBlock()
    {
        int _random = Random.Range(1, 8);
        string BlockName = "Prefabs/J-Block";

        switch (_random)
        {
            case 1:
                BlockName = "Prefabs/J-Block";
                break;
            case 2:
                BlockName = "Prefabs/L-Block";
                break;
            case 3:
                BlockName = "Prefabs/S-Block";
                break;
            case 4:
                BlockName = "Prefabs/Square-Block";
                break;
            case 5:
                BlockName = "Prefabs/Straight-Block";
                break;
            case 6:
                BlockName = "Prefabs/T-Block";
                break;
            case 7:
                BlockName = "Prefabs/Z-Block";
                break;
        }
        return BlockName;
    }

    // Counts down timer, updates UI
    IEnumerator TimerCounter()
    {
        Timer--;

        TimerText.text = Timer.ToString();
        ToClear.text = LinesToClear.ToString();
        Cleared.text = ClearedLines.ToString();
        Level.text = CurrentLvl.ToString();

        yield return new WaitForSecondsRealtime(1);
        if (Timer >= 1)
        {
            StartCoroutine(TimerCounter());
        }
        else
        {
            GameOver = true;
            Time.timeScale = 0.01f;
            GameOverPanel.SetActive(true);
        }
    }
}
