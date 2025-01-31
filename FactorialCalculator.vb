' FactorialCalculator.vb
Imports System.Numerics

'Handles the computation of factorials using memoization (caching) for efficiency.
Public Class FactorialCalculator
    ' Dictionary to store precomputed factorial values for optimization
    Private factorialCache As New Dictionary(Of Integer, BigInteger)()

    'Constructor that initializes the factorial cache.
    Public Sub New()
        InitializeFactorialCache()
    End Sub

    'Initializes the factorial cache with base values for 0! and 1!.
    Public Sub InitializeFactorialCache()
        factorialCache.Clear()
        factorialCache(0) = 1 ' 0! = 1
        factorialCache(1) = 1 ' 1! = 1
    End Sub

    'Computes the factorial of a given number using a cached approach for efficiency.
    Public Function GetFactorial(num As Integer) As BigInteger
        ' Return cached result if already computed
        If factorialCache.ContainsKey(num) Then Return factorialCache(num)

        ' Retrieve the last cached factorial value to continue calculation
        Dim result As BigInteger = factorialCache.Values.Last()
        For i As Integer = factorialCache.Keys.Max() + 1 To num
            result *= i ' Multiply previous factorial value by next number
            factorialCache(i) = result ' Store result in cache
        Next
        Return result
    End Function
End Class