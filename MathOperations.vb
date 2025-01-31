'' MathOperations.vb
'Imports System.Numerics
'Imports System.ComponentModel

'Public Class MathOperations
'    ' Factorial cache for memoization
'    Private Shared factorialCache As New Dictionary(Of Integer, BigInteger)()

'    ' Prime calculation background worker
'    Private primeWorker As New BackgroundWorker()

'    Public Sub New()
'        InitializeFactorialCache()
'        ConfigurePrimeWorker()
'    End Sub

'    '======= PUBLIC METHODS ========
'    Public Function CalculatePrime(num As BigInteger, ByRef worker As BackgroundWorker) As String
'        If num < 2 Then Return "= Not Prime"
'        If worker.IsBusy Then worker.CancelAsync()

'        Dim result = ""
'        AddHandler worker.RunWorkerCompleted, Sub(s, e)
'                                                  If e.Error IsNot Nothing Then
'                                                      result = "= Calculation Error"
'                                                  ElseIf Not e.Cancelled Then
'                                                      result = If(CBool(e.Result), "= Prime", "= Not Prime")
'                                                  End If
'                                              End Sub

'        worker.RunWorkerAsync(num)
'        Return result
'    End Function

'    Public Function CalculateFactorial(num As Integer) As String
'        If num < 0 Then Return "= Invalid Input"
'        If num > 10000 Then Return "= Number too large"

'        Try
'            Return "= " & GetFactorial(num).ToString()
'        Catch
'            Return "= Calculation Error"
'        End Try
'    End Function

'    '======= PRIVATE METHODS ========
'    Private Sub InitializeFactorialCache()
'        factorialCache.Clear()
'        factorialCache.Add(0, 1)
'        factorialCache.Add(1, 1)
'    End Sub

'    Private Sub ConfigurePrimeWorker()
'        primeWorker.WorkerSupportsCancellation = True
'        AddHandler primeWorker.DoWork, AddressOf PrimeWorker_DoWork
'    End Sub

'    Private Sub PrimeWorker_DoWork(sender As Object, e As DoWorkEventArgs)
'        Dim num = CType(e.Argument, BigInteger)
'        e.Result = IsPrime(num)
'    End Sub

'    Private Shared Function IsPrime(num As BigInteger) As Boolean
'        If num < 2 Then Return False
'        If num = 2 OrElse num = 3 Then Return True
'        If num Mod 2 = 0 OrElse num Mod 3 = 0 Then Return False

'        Dim sqrtNum As BigInteger = BigIntegerSqrt(num)
'        For i As BigInteger = 5 To sqrtNum Step 6
'            If num Mod i = 0 OrElse num Mod (i + 2) = 0 Then Return False
'            If primeWorker.CancellationPending Then Return False
'        Next
'        Return True
'    End Function

'    Private Shared Function GetFactorial(num As Integer) As BigInteger
'        If factorialCache.ContainsKey(num) Then Return factorialCache(num)

'        Dim result As BigInteger = factorialCache.Values.Last()
'        For i As Integer = factorialCache.Keys.Max() + 1 To num
'            result *= i
'            factorialCache(i) = result
'        Next
'        Return result
'    End Function

'    Private Shared Function BigIntegerSqrt(n As BigInteger) As BigInteger
'        If n < 0 Then Throw New ArgumentException("Negative number")
'        If n < 2 Then Return n

'        Dim x As BigInteger = n
'        Dim y As BigInteger = (x + 1) >> 1
'        While y < x
'            x = y
'            y = (x + n / x) >> 1
'        End While
'        Return x
'    End Function
'End Class