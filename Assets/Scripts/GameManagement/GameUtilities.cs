using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtilities : MonoBehaviour {

    /// <summary>
    /// Swap the values of two items of the same type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elem1"></param>
    /// <param name="elem2"></param>
    public static void SwapElements<T>(ref T elem1, ref T elem2) {
        T temp = elem1;
        elem1 = elem2;
        elem2 = temp;
    }

    /// <summary>
    /// Turn a list into a Queue
    /// </summary>
    public static void EnqueueList<T>(ref List<T> listT, ref Queue<T> queueT) { 
        foreach (var elem in listT) {
            queueT.Enqueue(elem);
        }
    }

}
