using UnityEngine;
using UnityEngine.UI;

public class Buttonscript : MonoBehaviour {

    Button _button;

	void Start ()
    {
        _button = gameObject.GetComponent<Button>();
	}

    // used for mobile functionality
    #region Button Methods
    public void Rotate()
    {
        FindObjectOfType<BlockScript>().RotatePiece();
    }
    public void MoveRight()
    {
        FindObjectOfType<BlockScript>().MoveRight();
    }
    public void MoveLeft()
    {
        FindObjectOfType<BlockScript>().MoveLeft();
    }
    public void MoveDown()
    {
        FindObjectOfType<BlockScript>().MoveDown();
    }
    #endregion
}
