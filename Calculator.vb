Imports System.ComponentModel
Imports System.Numerics

'Handles mathematical calculations such as factorials and primes.
Public Class Calculator
    Private factorialCache As Dictionary(Of BigInteger, BigInteger) = New Dictionary(Of BigInteger, BigInteger)()

    'Initializes the factorial cache with base case values.
    Public Sub InitializeFactorialCache()
        factorialCache(0) = 1
        factorialCache(1) = 1
    End Sub

    'Calculates the factorial of a given number and updates the answer field.
    'Uses caching for efficiency.
    Public Sub CalculateFactorial(input As String, answerField As TextBox)
        Dim num As BigInteger
        If BigInteger.TryParse(input, num) AndAlso num >= 0 Then
            answerField.Text = "= " & ComputeFactorial(num).ToString()
        Else
            answerField.Text = "= Invalid Input"
        End If
    End Sub

    ' Recursively computes the factorial with memoization.
    Private Function ComputeFactorial(n As BigInteger) As BigInteger
        ' Base cases to prevent infinite recursion
        If n = 0 OrElse n = 1 Then
            Return 1
        End If

        ' Check if the factorial is already cached
        If factorialCache.ContainsKey(n) Then
            Return factorialCache(n)
        End If

        ' Compute the factorial iteratively to avoid deep recursion
        Dim result As BigInteger = 1
        For i As Integer = 2 To n
            result *= i
            If Not factorialCache.ContainsKey(i) Then
                factorialCache(i) = result ' Store computed values in cache
            End If
        Next

        Return result
    End Function


    'Initiates a prime number check using a background worker.

    Public Sub CalculatePrime(input As String, answerField As TextBox, worker As BackgroundWorker)
        Dim num As BigInteger
        If BigInteger.TryParse(input, num) AndAlso num >= 0 Then
            If Not worker.IsBusy Then
                worker.RunWorkerAsync(num)
            End If
        Else
            answerField.Text = "= Invalid Input"
        End If
    End Sub

End Class
