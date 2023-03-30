Dictionary<char, int> occurrences = new Dictionary<char, int>();
string text = File.ReadAllText("sherlock.txt");
foreach (char c in text) {
    if (c != ' ' && c != '\n')
    {
        if (!occurrences.ContainsKey(c)) 
        {
            occurrences.Add(c, 1);
        }
        else {
            occurrences[c]++;
        }
    }
}

HuffmanCode(occurrences);

static void PrintCodes(MinHeapNode root, string str)
{
    if (root == null)
        return;
 
    if (root.Value != null)
        Console.WriteLine(root.Value + ": " + str);
 
    PrintCodes(root.Left, str + "0");
    PrintCodes(root.Right, str + "1");
}

void HuffmanCode(Dictionary<char, int> frequencies)
{
    var minHeap = new MinHeap(frequencies.Count);
    foreach (var c in frequencies)
    {
        minHeap.Add(new MinHeapNode(c.Key.ToString(), c.Value));
    }
    
    while (!minHeap.IsEmpty())
    {
        var left = minHeap.Pop(); 
        var right = minHeap.Pop();

        var top = new MinHeapNode(null, left.Frequency + right.Frequency)
        {
            Left = left,
            Right = right
        };

        minHeap.Add(top);
    
    }
    
    PrintCodes(minHeap.Peek(), null);
}


public class MinHeapNode
{
    public readonly string Value;

    public readonly int Frequency;

    public MinHeapNode Left, Right;
    
    public MinHeapNode(string value, int frequency)
    {
        Left = Right = null;
        Value = value;
        Frequency = frequency;
    }
}


public class MinHeap
{
    private readonly MinHeapNode[] _elements;
    private int _size;

    public MinHeap(int size)
    {
        _elements = new MinHeapNode[size];
    }

    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    private MinHeapNode GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
    private MinHeapNode GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
    private MinHeapNode GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

    private void Swap(int firstIndex, int secondIndex)
    {
        (_elements[firstIndex], _elements[secondIndex]) = (_elements[secondIndex], _elements[firstIndex]);
    }

    public bool IsEmpty()
    {
        return _size == 1;
    }

    public MinHeapNode Peek()
    {
        if (_size != 0)
        {
            return _elements[0];
        }
        
        return null;
    }

    public MinHeapNode Pop()
    {
        if (_size == 0)
            return null;

        var result = _elements[0];
        _elements[0] = _elements[_size - 1];
        _size--;

        int index = 0;
        
        // rearranging the heap
        while (HasLeftChild(index))
        {
            var smallerIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && GetRightChild(index).Frequency < GetLeftChild(index).Frequency)
            {
                smallerIndex = GetRightChildIndex(index);
            }

            if (_elements[smallerIndex].Frequency >= _elements[index].Frequency)
            {
                break;
            }

            Swap(smallerIndex, index);
            index = smallerIndex;
        }

        return result;
    }

    public void Add(MinHeapNode element)
    {
        if (_size == _elements.Length)
            return;

        _elements[_size] = element;
        _size++;

        var index = _size - 1;
        while (!IsRoot(index) && _elements[index].Frequency < GetParent(index).Frequency)
        {
            var parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }
    
}