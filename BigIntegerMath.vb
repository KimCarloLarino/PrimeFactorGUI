' BigIntegerMath.vb
Imports System.Numerics

'Provides mathematical utility functions for handling large numbers using BigInteger.
Public Class BigIntegerMath

    Public Shared Function BigIntegerSqrt(n As BigInteger) As BigInteger
        If n < 0 Then Throw New ArgumentException("Negative number")
        If n < 2 Then Return n

        Dim x As BigInteger = n
        Dim y As BigInteger = (x + 1) >> 1
        While y < x
            x = y
            y = (x + n / x) >> 1
        End While
        Return x
    End Function
End Class
