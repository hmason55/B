using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T> {

    public List<int> adjacent;
    public T item;

    public Node(T item)
    {
        this.item = item;
    }

    public Node()
    {
        this.item = default(T);
    }
}
