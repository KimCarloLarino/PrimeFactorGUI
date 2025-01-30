Imports System.Drawing
Imports System.Numerics
Imports System.ComponentModel

Public Class frmMain
    Private DefaultFontSize As Single = 24
    Private isPrimeToggled As Boolean = False
    Private isFactorialToggled As Boolean = False
    Private factorialCache As New Dictionary(Of Integer, BigInteger)()
    Private primeWorker As BackgroundWorker = New BackgroundWorker()
    Private lastProcessedInput As String = String.Empty

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ActiveControl = txtInput
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold)
        InitializeFactorialCache()
        ConfigureBackgroundWorker()
    End Sub

    '=============== UI HANDLERS ===============
    Private Sub NumberButton_Click(sender As Object, e As EventArgs) Handles btn0.Click, btn1.Click, btn2.Click, btn3.Click, btn4.Click, btn5.Click, btn6.Click, btn7.Click, btn8.Click, btn9.Click
        Dim btn As Button = CType(sender, Button)
        txtInput.Text &= btn.Text
        ThrottledAdjustFontSize()
        txtInput.SelectionStart = txtInput.Text.Length
        txtInput.Focus()
    End Sub

    Private Sub btnBackspace_Click(sender As Object, e As EventArgs) Handles btnBackspace.Click
        If txtInput.Text.Length > 0 Then
            txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1)
            ThrottledAdjustFontSize()
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtInput.Clear()
        txtAnswer.Clear()
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold)
    End Sub

    '=============== CORE FUNCTIONALITY ===============
    Private Sub CalculatePrime(input As String)
        If input = lastProcessedInput Then Exit Sub
        lastProcessedInput = input

        If Not IsNumeric(input) Then
            txtAnswer.Text = "= Invalid Input"
            Return
        End If

        Try
            Dim num As BigInteger = BigInteger.Parse(input)
            If primeWorker.IsBusy Then primeWorker.CancelAsync()
            primeWorker.RunWorkerAsync(num)
        Catch ex As OverflowException
            txtAnswer.Text = "= Number too large"
        End Try
    End Sub

    Private Sub CalculateFactorial(input As String)
        If Not IsNumeric(input) Then
            txtAnswer.Text = "= Invalid Input"
            Return
        End If

        Dim num As Integer
        If Integer.TryParse(input, num) Then
            If num >= 0 AndAlso num <= 10000 Then
                txtAnswer.Text = "= " & GetFactorial(num).ToString()
            ElseIf num > 10000 Then
                txtAnswer.Text = "= Number too large"
            Else
                txtAnswer.Text = "= Invalid Input"
            End If
        Else
            txtAnswer.Text = "= Invalid Input"
        End If
    End Sub

    '=============== OPTIMIZED ALGORITHMS ===============
    Private Function IsPrime(num As BigInteger) As Boolean
        If num < 2 Then Return False
        If num = 2 OrElse num = 3 Then Return True
        If num Mod 2 = 0 OrElse num Mod 3 = 0 Then Return False

        Dim sqrtNum As BigInteger = BigIntegerSqrt(num)
        For i As BigInteger = 5 To sqrtNum Step 6
            If num Mod i = 0 OrElse num Mod (i + 2) = 0 Then Return False
            If primeWorker.CancellationPending Then Return False
        Next
        Return True
    End Function

    Private Function GetFactorial(num As Integer) As BigInteger
        If factorialCache.ContainsKey(num) Then Return factorialCache(num)

        Dim result As BigInteger = factorialCache.Values.Last()
        For i As Integer = factorialCache.Keys.Max() + 1 To num
            result *= i
            factorialCache(i) = result
        Next
        Return result
    End Function

    '=============== HELPER METHODS ===============
    Private Sub InitializeFactorialCache()
        factorialCache.Clear()
        factorialCache.Add(0, 1)
        factorialCache.Add(1, 1)
    End Sub

    Private Sub ConfigureBackgroundWorker()
        AddHandler primeWorker.DoWork, AddressOf PrimeWorker_DoWork
        AddHandler primeWorker.RunWorkerCompleted, AddressOf PrimeWorker_Completed
        primeWorker.WorkerSupportsCancellation = True
    End Sub

    Private Sub ThrottledAdjustFontSize()
        Static lastUpdate As DateTime = DateTime.MinValue
        If (DateTime.Now - lastUpdate).TotalMilliseconds < 100 Then Exit Sub
        lastUpdate = DateTime.Now
        AdjustFontSize()
    End Sub

    Private Sub AdjustFontSize()
        Dim currentFont = txtInput.Font
        Dim newSize = CalculateOptimalFontSize(txtInput.Text, txtInput.Width, currentFont)
        If newSize <> currentFont.Size Then
            txtInput.Font = New Font(currentFont.FontFamily, newSize, FontStyle.Bold)
        End If
    End Sub

    Private Function CalculateOptimalFontSize(text As String, width As Integer, font As Font) As Single
        Using g = txtInput.CreateGraphics()
            Dim size = g.MeasureString(text, font)
            If size.Width <= width Then Return Math.Min(font.Size + 1, DefaultFontSize)

            Dim minSize As Single = 10
            Dim currentSize As Single = font.Size
            While currentSize > minSize
                currentSize -= 1
                size = g.MeasureString(text, New Font(font.FontFamily, currentSize, FontStyle.Bold))
                If size.Width <= width Then Return currentSize
            End While
            Return minSize
        End Using
    End Function

    Private Function BigIntegerSqrt(n As BigInteger) As BigInteger
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

    '=============== BACKGROUND WORKER ===============
    Private Sub PrimeWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim num As BigInteger = CType(e.Argument, BigInteger)
        e.Result = If(num < 2, False, IsPrime(num))
    End Sub

    Private Sub PrimeWorker_Completed(sender As Object, e As RunWorkerCompletedEventArgs)
        If e.Error IsNot Nothing Then
            txtAnswer.Text = "= Calculation Error"
        ElseIf Not e.Cancelled Then
            txtAnswer.Text = If(CType(e.Result, Boolean), "= Prime", "= Not Prime")
        End If
    End Sub

    '=============== EVENT HANDLERS ===============

    Private Sub txtInput_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtInput.KeyPress
        ' Allow only digits (0-9), backspace, and minus
        If Not Char.IsDigit(e.KeyChar) AndAlso
       e.KeyChar <> ControlChars.Back AndAlso
       e.KeyChar <> "-" Then
            e.Handled = True
        End If

        ' Restrict minus sign to first position
        If e.KeyChar = "-" AndAlso
       (txtInput.SelectionStart <> 0 OrElse
       txtInput.Text.Contains("-")) Then
            e.Handled = True
        End If
    End Sub
    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        Dim input = txtInput.Text
        ' Remove any invalid characters that might have been pasted
        If System.Text.RegularExpressions.Regex.IsMatch(txtInput.Text, "[^0-9-]") Then
            txtInput.Text = System.Text.RegularExpressions.Regex.Replace(txtInput.Text, "[^0-9-]", "")
            txtInput.SelectionStart = txtInput.Text.Length
        End If
        If String.IsNullOrEmpty(input) Then
            txtAnswer.Clear()
            Return
        End If

        If isPrimeToggled Then
            CalculatePrime(input)
        ElseIf isFactorialToggled Then
            CalculateFactorial(input)
        End If
    End Sub

    '=============== BUTTON TOGGLES ===============
    Private Sub ToggleFunctionButton(ByRef currentToggle As Boolean, ByRef otherToggle As Boolean)
        ' Clear previous function
        If otherToggle Then
            otherToggle = False
            txtAnswer.Clear()
            txtInput.Clear()
        End If

        ' Toggle current function
        currentToggle = Not currentToggle
        UpdateButtonColors()
    End Sub

    Private Sub UpdateButtonColors()
        btnPrime.BackColor = If(isPrimeToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
        btnFactorial.BackColor = If(isFactorialToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
    End Sub

    Private Sub btnPrime_Click(sender As Object, e As EventArgs) Handles btnPrime.Click
        ToggleFunctionButton(isPrimeToggled, isFactorialToggled)
        If isPrimeToggled Then txtInput_TextChanged(Nothing, Nothing)
        txtInput.Focus()
    End Sub

    Private Sub btnFactorial_Click(sender As Object, e As EventArgs) Handles btnFactorial.Click
        ToggleFunctionButton(isFactorialToggled, isPrimeToggled)
        If isFactorialToggled Then txtInput_TextChanged(Nothing, Nothing)
        txtInput.Focus()
    End Sub

    '=============== MOUSE EVENT HANDLERS ===============
    Private Sub Button_MouseEnter(sender As Object, e As EventArgs) Handles _
    btn0.MouseEnter, btn1.MouseEnter, btn2.MouseEnter, btn3.MouseEnter, btn4.MouseEnter,
    btn5.MouseEnter, btn6.MouseEnter, btn7.MouseEnter, btn8.MouseEnter, btn9.MouseEnter,
    btnClear.MouseEnter, btnBackspace.MouseEnter, btnPrime.MouseEnter, btnFactorial.MouseEnter

        Dim btn = CType(sender, Button)
        If ShouldIgnoreHover(btn) Then Exit Sub
        btn.BackColor = Color.FromArgb(45, 45, 45)
    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As EventArgs) Handles _
    btn0.MouseLeave, btn1.MouseLeave, btn2.MouseLeave, btn3.MouseLeave, btn4.MouseLeave,
    btn5.MouseLeave, btn6.MouseLeave, btn7.MouseLeave, btn8.MouseLeave, btn9.MouseLeave,
    btnClear.MouseLeave, btnBackspace.MouseLeave, btnPrime.MouseLeave, btnFactorial.MouseLeave

        Dim btn = CType(sender, Button)
        If ShouldIgnoreHover(btn) Then Exit Sub
        btn.BackColor = Color.FromArgb(59, 59, 59)
    End Sub

    Private Sub Button_MouseDown(sender As Object, e As MouseEventArgs) Handles _
    btn0.MouseDown, btn1.MouseDown, btn2.MouseDown, btn3.MouseDown, btn4.MouseDown,
    btn5.MouseDown, btn6.MouseDown, btn7.MouseDown, btn8.MouseDown, btn9.MouseDown,
    btnClear.MouseDown, btnBackspace.MouseDown, btnPrime.MouseDown, btnFactorial.MouseDown

        Dim btn = CType(sender, Button)
        If ShouldIgnoreHover(btn) Then Exit Sub
        btn.BackColor = Color.FromArgb(30, 30, 30)
    End Sub

    Private Sub Button_MouseUp(sender As Object, e As MouseEventArgs) Handles _
    btn0.MouseUp, btn1.MouseUp, btn2.MouseUp, btn3.MouseUp, btn4.MouseUp,
    btn5.MouseUp, btn6.MouseUp, btn7.MouseUp, btn8.MouseUp, btn9.MouseUp,
    btnClear.MouseUp, btnBackspace.MouseUp, btnPrime.MouseUp, btnFactorial.MouseUp

        Dim btn = CType(sender, Button)
        If ShouldIgnoreHover(btn) Then Exit Sub
        btn.BackColor = Color.FromArgb(45, 45, 45)
    End Sub

    Private Function ShouldIgnoreHover(btn As Button) As Boolean
        Return (btn Is btnPrime AndAlso isPrimeToggled) OrElse
           (btn Is btnFactorial AndAlso isFactorialToggled)
    End Function
End Class