using System.Runtime.InteropServices;

namespace ImplicitSpanConversions;

// This static class contains extension methods for Span<T> and ReadOnlySpan<char>.
// This demonstrates how Span types can be "extension method receivers" in C# 14.
public static class SpanExtensions
{
    // A simple extension method to count letters or digits in a ReadOnlySpan<char>.
    // This method is called directly on a string due to C# 14's implicit conversions.
    public static int CountLettersOrDigits(this ReadOnlySpan<char> span)
    {
        var count = 0;
        foreach (var c in span)
        {
            if (char.IsLetterOrDigit(c))
            {
                count++;
            }
        }
        return count;
    }
}

public class AdvancedSpanDemo
{
    // A generic method to demonstrate implicit conversion and type inference.
    // The 'where T : struct' constraint is necessary for MemoryMarshal.Cast.
    private static void ProcessData<T>(Span<T> data) where T : struct
    {
        Console.WriteLine($"Processing a Span of type: {typeof(T).Name}");
        Console.WriteLine($"Span length: {data.Length}");

        // Only process if the type is 'int' to avoid runtime errors.
        if (typeof(T) != typeof(int)) return;

        // Correct way to cast a generic Span<T> to Span<int> for in-place modification.
        var intSpan = MemoryMarshal.Cast<T, int>(data);
        for (var i = 0; i < intSpan.Length; i++)
        {
            intSpan[i]++;
        }
    }

    // A method that demonstrates composing conversions.
    // It takes a ReadOnlySpan<char>, but C# 14 allows passing a string directly.
    private static void FindFirstCapital(ReadOnlySpan<char> text)
    {
        Console.WriteLine($"\nSearching text: '{text.ToString()}'");
        var foundChar = ' ';
        foreach (var c in text)
        {
            if (!char.IsUpper(c)) continue;
            foundChar = c;
            break;
        }
        Console.WriteLine($"First capital letter found: '{foundChar}'");
    }

    public static void Main()
    {
        // === DEMO: GENERIC TYPE INFERENCE ===
        Console.WriteLine("--- DEMO: GENERIC TYPE INFERENCE ---");
        int[] numbers = [1, 2, 3];
        
        // Before C# 14, this would require explicit conversion:
        // ProcessData<int>(new Span<int>(numbers));
        
        // C# 14 automatically infers T as 'int' and converts int[] to Span<int>.
        ProcessData(numbers); 

        Console.WriteLine($"Original array after processing: [{string.Join(", ", numbers)}]");
        Console.WriteLine("Note: The array was modified through the Span, proving zero-copy access.");

        // === DEMO: COMPOSING CONVERSIONS ===
        Console.WriteLine("\n--- DEMO: COMPOSING CONVERSIONS ---");
        string message = "hello World";
        
        // Before C# 14, this required an explicit conversion:
        // FindFirstCapital(message.AsSpan());
        
        // C# 14 automatically converts the string to a ReadOnlySpan<char>.
        FindFirstCapital(message); 

        // === DEMO: EXTENSION METHOD RECEIVERS ===
        Console.WriteLine("\n--- DEMO: EXTENSION METHOD RECEIVERS ---");
        string sentence = "Hello, world!";
        
        // The compiler first implicitly converts the string to ReadOnlySpan<char>.
        // Then, it calls the extension method on that Span.
        int lettersAndDigitsCount = sentence.CountLettersOrDigits();
        
        Console.WriteLine($"The sentence is: '{sentence}'");
        Console.WriteLine($"Count of letters and digits: {lettersAndDigitsCount}"); 

        Console.WriteLine("\nDemo has finished!");
    }
}
