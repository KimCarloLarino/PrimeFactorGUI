' PrimeChecker.vb
Imports System.Numerics
Imports System.ComponentModel

' Handles prime number calculations using an optimized approach.
'Uses a BackgroundWorker for asynchronous execution.
Public Class PrimeChecker
    Private primeWorker As BackgroundWorker

    'Initializes a new instance of PrimeChecker and configures the BackgroundWorker.
    'The BackgroundWorker instance used for asynchronous prime checking.
    Public Sub New(worker As BackgroundWorker)
        primeWorker = worker
        ConfigureBackgroundWorker()
    End Sub

    'Configures the BackgroundWorker for performing prime calculations asynchronously.
    Private Sub ConfigureBackgroundWorker()
        AddHandler primeWorker.DoWork, AddressOf PrimeWorker_DoWork
        AddHandler primeWorker.RunWorkerCompleted, AddressOf PrimeWorker_Completed
        primeWorker.WorkerSupportsCancellation = True
    End Sub

    'Checks if a number is prime using an optimized algorithm.
    'The number to check for primality.
    'True if the number is prime, otherwise False.
    Public Function IsPrime(num As BigInteger) As Boolean
        If num < 2 Then Return False
        If num = 2 OrElse num = 3 Then Return True
        If num Mod 2 = 0 OrElse num Mod 3 = 0 Then Return False

        ' Compute square root manually since BigInteger does not support Math.Sqrt
        Dim sqrtNum As BigInteger = BigIntegerSqrt(num)
        For i As BigInteger = 5 To sqrtNum Step 6
            If num Mod i = 0 OrElse num Mod (i + 2) = 0 Then Return False
            If primeWorker.CancellationPending Then Return False
        Next
        Return True
    End Function

    'Computes the integer square root of a BigInteger using the Newton-Raphson method.
    'The BigInteger number for which to compute the square root.
    'The integer square root of the given number.
    'Since BigInteger does not support Math.Sqrt, we must compute it manually.

    'The Newton-Raphson method is used for efficiency.

    Private Function BigIntegerSqrt(n As BigInteger) As BigInteger
        If n < 0 Then Throw New ArgumentException("Negative number")
        If n < 2 Then Return n

        Dim x As BigInteger = n
        Dim y As BigInteger = (x + 1) >> 1 ' Initial approximation
        While y < x
            x = y
            y = (x + n / x) >> 1
        End While
        Return x
    End Function

    'Handles the BackgroundWorker execution for checking primality.
    'DoWorkEventArgs containing the number to check.
    Private Sub PrimeWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim num As BigInteger = CType(e.Argument, BigInteger)
        e.Result = IsPrime(num)
    End Sub

    'Handles completion of the prime check operation.
    Private Sub PrimeWorker_Completed(sender As Object, e As RunWorkerCompletedEventArgs)
        ' Output results could be handled externally where needed
    End Sub
End Class
