/*******************************************************************************************
*
*    File: PlayerSpawnPoint.cs
*    Purpose: Represents a spawn point for the player
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public enum FacingDirection
    {
        Left, 
        Right
    }
    public Vector2 GetPosition => gameObject.transform.position;
    public FacingDirection GetFacingDirection => facingDirection;
    public int GetSpawnIndex => spawnIndex;
    [SerializeField] private int spawnIndex;
    [SerializeField] private FacingDirection facingDirection;
}