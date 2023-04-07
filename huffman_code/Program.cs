using System.Text;
Dictionary<char, int> occurrences = new Dictionary<char, int>();
var huffmanCodes = new Dictionary<string, string>();
string text = File.ReadAllText("/Users/platonovskaaa/RiderProjects/huffman/huffman_code/sherlock.txt");
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
/*
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
*/

void BinaryHardcore(string inputData,Dictionary<string, string> codes)
{
    // Open the file stream for writing
    FileStream outputStream = new FileStream("/Users/platonovskaaa/RiderProjects/huffman/huffman_code/compressed.bin", FileMode.Create);

    // Create a binary writer to write the encoded bits to the file stream
    BinaryWriter writer = new BinaryWriter(outputStream);
    
    // Define a buffer to store the encoded bits before writing them to the file stream
    byte[] buffer = new byte[1];

    foreach (char c in inputData)
    {
        string code = codes[c.ToString()];
        foreach (char bit in code)
        {
            // Shift the current byte left and add the current bit
            buffer[0] <<= 1; //  shifts the bits in the buffer[0] byte one position to the left.
            if (bit == '1')
            {
                buffer[0] |= 0x01; // or = 0x01 is binary 00000001
            }
        
            // If the current byte is full, write it to the file stream
            if ((buffer[0] & 0x80) == 0x80) // 0x80 is binary 10000000
            {
                writer.Write(buffer);
                buffer[0] = 0;
            }
        }
    }

    // If there are any remaining bits in the buffer, write them to the file stream
    if ((buffer[0] & 0x80) != 0x80)
    {
        buffer[0] <<= (8 - (codes[inputData[^1].ToString()].Length % 8)); // ^1 = length-1
        writer.Write(buffer);
    }

    writer.Close();
    outputStream.Close();
}

BinaryHardcore(text, huffmanCodes);

void DecodeBinaryHardcore(string path, Dictionary<string, string> codes)
{
    // Open the file stream for reading
    FileStream inputStream = new FileStream(path, FileMode.Open);

    // Create a binary reader to read the encoded bits from the file stream
    BinaryReader reader = new BinaryReader(inputStream);

    // Define a buffer to store the bits as they are read from the file stream
    byte[] buffer = new byte[1];

    // Define a variable to keep track of the current bit position in the buffer
    int position = 0;

    // Define a variable to store the decoded text
    StringBuilder decodedText = new StringBuilder();

    // Loop through the bits in the file stream
    while (reader.BaseStream.Position < reader.BaseStream.Length)
    {
        // Read the next byte from the file stream
        buffer[0] = reader.ReadByte();

        // Loop through the bits in the byte
        for (int i = 0; i < 8; i++)
        {
            // Extract the current bit from the buffer
            bool bit = (buffer[0] & (1 << (7 - position))) != 0;

            // Shift the current bit position to the next bit in the buffer
            position++;

            // If the current bit position is at the end of the buffer, reset it to the beginning
            if (position == 8)
            {
                position = 0;
            }

            // Check if the current sequence of bits matches any of the Huffman codes
            foreach (var c in codes)
            {
                if (c.Value.Length == decodedText.Length + 1 && c.Value.StartsWith(decodedText.ToString()) && c.Value[^1] == (bit ? '1' : '0'))
                {
                    // If the current sequence of bits matches a Huffman code, add the corresponding character to the decoded text
                    decodedText.Append(c.Key);

                    // Reset the current bit position to the beginning of the buffer
                    position = 0;

                    // Exit the loop over Huffman codes
                    break;
                }
            }
        }
    }

    reader.Close();
    inputStream.Close();

    Console.WriteLine(decodedText.ToString());
}

DecodeBinaryHardcore("/Users/platonovskaaa/RiderProjects/huffman/huffman_code/compressed.bin",huffmanCodes);
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
