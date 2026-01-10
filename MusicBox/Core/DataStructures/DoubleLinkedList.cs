namespace MusicBox.DataStructures;

public class DoublyLinkedList<T>
{
    public Node<T>? Head {get; private set;}    // Primer nodo
    public Node<T>? Tail {get; private set;}    // Último nodo
    public int Count {get; private set;}        // Cuántos nodos hay

    // Añadir un nuevo nodo al final de la lista
    public void AddLast(T data)
    {
        Node<T> newNode = new(data);    // Crear nuevo nodo

        if (Head == null)               // Si la lista está VACÍA
        {
            Head = Tail = newNode;      // Este nodo es el último y el primero
        }
        else                            // Si ya hay nodos
        {
            Tail!.Next = newNode;       // El último nodo apunta al nuevo
            newNode.Previous = Tail;    // El nuevo nodo apunta al que era el último
            Tail = newNode;             // Ahora el nuevo es el último
        }

    Count++;                            // Aumentar el contador
    }

    // Recorrer la lista de INICIO a FIN
    public IEnumerable<T> TraverseForward()
    {
        Node<T>? current = Head;            // Empezar desde el inicio
        while (current != null)             // Mientras haya un nodo
        {
            yield return current.Data;      // Devolver lo que lleva este nodo
            current = current.Next;         // Ir al siguiente nodo
        }
    }

    // Recorrer la lista de FIN a INICIO
    public IEnumerable<T> TraverseBackward()
    {
        Node<T>? current = Tail;            // Empezar desde el final
        while (current != null)             // Mientras haya un nodo
        {   
            yield return current.Data;      // Devolver lo que lleva este nodo
            current = current.Previous;     // Ir al nodo anterior
        }
    }
}