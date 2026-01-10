namespace MusicBox.DataStructures;


public class Node<T> // <T> significa que puede almacenar cualquier dato.
{
    // Los datos que queremos guardar
    public T Data {get; set;}
    // Apunta al nodo anterior
    public Node<T>? Previous {get; set;}
    // Apunta al siguiente nodo
    public Node<T>? Next {get; set;}

    // Constructor
    public Node(T data)
    {
        Data = data;
        Previous = null;    // Al crear, no tiene anterior
        Next = null;        // Al crear, no tiene siguiente
    }
}