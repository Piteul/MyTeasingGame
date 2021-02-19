using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the case / tile that must be moved to solve the game
/// </summary>
public class Case : MonoBehaviour
{
    //Starting index is use to know if the case is in correct position
    public int index;
    public bool isEmptyCase;

    public GameObject selectedSprite;

    //Use to better locate and manipulate the object in the grid
    public Vector2Int coordinate;

}
