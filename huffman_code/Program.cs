Dictionary<char, int> occurrences = new Dictionary<char, int>();
var huffmanCodes = new Dictionary<string, string>();
string text = File.ReadAllText("sherlock.txt");
foreach (char c in text) {

        if (!occurrences.ContainsKey(c)) 
        {
            occurrences.Add(c, 1);
        }
        else {
            occurrences[c]++;
        }
    
}

HuffmanCode(occurrences);


void WriteDictToFile(Dictionary<string, string> codesDict, string path)
{
    using StreamWriter writer = new StreamWriter(path);
    foreach (var pair in codesDict)
    {
        var symbol = pair.Key;
        if (symbol == "\n")
        {
            symbol = "<N>" ;
        }
        else if (symbol == "\r")
        {
            symbol = "<R>" ;
        }
        writer.WriteLine("{0}: {1}", symbol, pair.Value);
    }
    writer.Close();
}

void GenerateCodes(MinHeapNode root, string str)
{
    if (root == null)
        return;

    if (root.Value != null)
    {
        huffmanCodes[root.Value] = str;
        Console.WriteLine(root.Value + ": " + str);
    }

    GenerateCodes(root.Left, str + "0");
    GenerateCodes(root.Right, str + "1");
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
    
    GenerateCodes(minHeap.Peek(), null);
}

void WriteCoddedFile()
{
    using StreamWriter writer = new StreamWriter("CodedFile.txt", true);
    {
        foreach (var letter in text)
        {
            writer.Write(huffmanCodes[letter.ToString()]);
        }
    }
}

void DecodeFile(string path)
{
    var huffCodes = new Dictionary<string, string>();
    string[] lines = File.ReadAllLines(path);
    foreach (var line in lines)
    {
        if (!line.Contains("endDict"))
        {
            var keyValue = line.Split(": ");
            var symbol = keyValue[0];
            if (symbol == "<N>")
            {
                symbol = "\n";
            }
            else if (symbol == "<R>")
            {
                symbol = "\r";
            }
            huffCodes.Add(symbol, keyValue[1]);
        }
        else
        {
            break;
        }
    }
    var codedText = lines[lines.Length - 1];
    var coddedLetter = "";
    foreach (char c in codedText) {
        
        if (huffmanCodes.ContainsValue(coddedLetter))
        {
            var letter = coddedLetter;
            var myKey = huffmanCodes.FirstOrDefault(x => x.Value == letter).Key;
            Console.Write(myKey);
            coddedLetter = "";
        }

        coddedLetter += c;
    
    }
    
}

huffmanCodes["endDict"] = "9";
WriteDictToFile(huffmanCodes, "CodedFile.txt");
WriteCoddedFile();
DecodeFile("CodedFile.txt");
Console.WriteLine();
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