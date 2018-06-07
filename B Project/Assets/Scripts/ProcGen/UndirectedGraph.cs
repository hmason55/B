using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndirectedGraph<T> {
    //The graph is represented via adjacency Lists
    private List<Node<T>> nodes;
	
    UndirectedGraph(int s)
    {
        nodes = new List<Node<T>>();
    }

    public Node<T> this[int v]
    {
        get
        {
            return nodes[v];
        }
        set
        {
            nodes[v] = value;
        }
    }

    public int V()
    {
        return nodes.Count;
    }

    public int E()
    {
        int count = 0;
        foreach(Node<T> node in nodes)
        {
            count += node.adjacent.Count;
        }

        return count;
    }

    public void addEdge(int v, int w)
    {
        nodes[v].adjacent.Add(w);
        nodes[w].adjacent.Add(v);
    }

    public void addVertex(Node<T> v)
    {
        nodes.Add(v);
    }

    public List<int> adjacent(int v)
    {
        return nodes[v].adjacent;
    }

    public Node<T> vertex(int i)
    {
        return nodes[i];
    }
}
