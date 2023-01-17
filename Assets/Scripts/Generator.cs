using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // Sudoku Generator Prototype
    int[,] mat = new int[9, 9];

    // Start is called before the first frame update
    void Start()
    {
        FillDiagnonalBox();
        BacktrackingCell(0, 3);
        Log(mat);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int temp = mat[i, j];
                mat[i, j] = 0;
                if (!CheckAvailable(i, j, temp))
                {
                    Debug.Log("Not unique!");
                    return;
                }
                mat[i, j] = temp;
            }
        }
        RemoveElements(10);
        Log(mat);
    }

    void FillDiagnonalBox()
    {
        int[] nums = Enumerable.Range(1, 9).ToArray();
        for (int i = 0; i < 3; i++)
        {
            Shuffle(ref nums);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    mat[j + i * 3, k + i * 3] = nums[j * 3 + k];
                }
            }
        }
    }

    bool BacktrackingCell(int i, int j)
    {
        if (j >= 9)
        {
            if (i < 8)
            {
                i += 1;
                j = 0;
            }
            else
            {
                return true;
            }
        }

        if (mat[i, j] != 0)
        {
            return BacktrackingCell(i, j+1);
        }

        for (int num = 1; num <= 9; num++)
        {
            if (CheckAvailable(i, j, num))
            {
                mat[i, j] = num;
                if (BacktrackingCell(i, j + 1))
                {
                    return true;
                }
                mat[i, j] = 0;
            }
        }
        return false;
    }

    bool CheckAvailable(int i, int j, int num)
    {
        for (int k = 0; k < 9; k++)
        {
            // Horizontal
            if (mat[i, k] == num)
            {
                return false;
            }
            // Vertical
            if (mat[k, j] == num)
            {
                return false;
            }
            // Box
            if (mat[i-i%3 + k / 3, j-j%3 + k % 3] == num)
            {
                return false;
            }
        }
        return true;
    }

    void RemoveElements(int count)
    {
        int[] seq = Enumerable.Range(0, 80).ToArray();
        Shuffle(ref seq);
        for(int i = 0; i < count; i++)
        {
            mat[seq[i]/9, seq[i]%9] = 0;
        }
    }

    // Fisher-Yates shuffle algorithm
    void Shuffle<T>(ref T[] arr)
    {
        var rng = new System.Random();
        int n = arr.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (arr[k], arr[n]) = (arr[n], arr[k]);
        }
    }

    void Log(int[,] mat)
    {
        var msg = new StringBuilder();
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                msg.Append($"{mat[i, j]} ");
            }
            msg.Append('\n');
        }
        Debug.Log(msg.ToString());
    }
}
