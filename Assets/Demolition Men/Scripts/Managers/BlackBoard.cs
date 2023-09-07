using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlackBoard
{
    /// <summary>
    /// Total amount of blocks in this level.
    /// Is increased by one when a single block is split into two pieces.
    /// </summary>
    public static int AmountOfBlocks
    {
        get => amountOfBlocks; set
        {
            amountOfBlocks = value;
            DeleteLater.SetAmountOfBlocks(amountOfBlocks);
        }
    }
    private static int amountOfBlocks;
    /// <summary>
    /// Amount of blocks (fully) destroyed by the player(s).
    /// </summary>
    public static int BlocksDestroyed { get => blocksDestroyed; set => blocksDestroyed = value; }

    private static int blocksDestroyed = 0;

    /// <summary>
    /// The percentage of the blocks destroyed by the palyer.
    /// </summary>
    public static float DemolitionPercentage { get => (float)BlocksDestroyed / (float)amountOfBlocks; }

}
