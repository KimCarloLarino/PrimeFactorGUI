Imports System.Drawing
Imports System.Numerics ' Import the BigInteger namespace

Public Class frmMain
    Private DefaultFontSize As Single = 24 ' Default font size for txtInput
    Private isPrimeToggled As Boolean = False
    Private isFactorialToggled As Boolean = False

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ActiveControl = txtInput ' Ensures txtInput is ready for typing
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold) ' Set default font size
    End Sub

    ' Event handler for number buttons
    Private Sub NumberButton_Click(sender As Object, e As EventArgs) Handles btn0.Click, btn1.Click, btn2.Click, btn3.Click, btn4.Click, btn5.Click, btn6.Click, btn7.Click, btn8.Click, btn9.Click
        Dim btn As Button = CType(sender, Button) ' Identify which button was clicked
        txtInput.Text &= btn.Text ' Append the button text (number) to the textbox
        AdjustFontSize() ' Adjust font size dynamically
        txtInput.SelectionStart = txtInput.Text.Length
        txtInput.SelectionLength = 0
        txtInput.Focus()
    End Sub

    ' Event handler for sign toggle (+/-)
    Private Sub btnSignToggle_Click(sender As Object, e As EventArgs)
        If txtInput.Text.StartsWith("-") Then
            txtInput.Text = txtInput.Text.Substring(1) ' Remove the negative sign
        ElseIf txtInput.Text.Length > 0 Then
            txtInput.Text = "-" & txtInput.Text ' Add a negative sign
        End If
        AdjustFontSize()
    End Sub

    ' Event handler for Backspace button
    Private Sub btnBackspace_Click(sender As Object, e As EventArgs) Handles btnBackspace.Click
        If txtInput.Text.Length > 0 Then
            txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1) ' Remove last character
            AdjustFontSize()
        End If
    End Sub

    ' Event handler for clear button
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtInput.Clear() ' Clear input textbox
        txtAnswer.Clear() ' Clear answer textbox
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold) ' Reset font size
    End Sub

    ' Adjust the font size dynamically based on text length
    Private Sub AdjustFontSize()
        Dim graphics As Graphics = txtInput.CreateGraphics()
        Dim textSize As SizeF = graphics.MeasureString(txtInput.Text, txtInput.Font)
        graphics.Dispose()

        Dim maxFontSize As Single = DefaultFontSize
        Dim minFontSize As Single = 10 ' Minimum font size

        ' Reduce font size if text is wider than textbox
        While textSize.Width > txtInput.Width AndAlso txtInput.Font.Size > minFontSize
            txtInput.Font = New Font(txtInput.Font.FontFamily, txtInput.Font.Size - 1, FontStyle.Bold)
            textSize = TextRenderer.MeasureText(txtInput.Text, txtInput.Font)
        End While

        ' Increase font size back if there is space
        While textSize.Width < txtInput.Width AndAlso txtInput.Font.Size < maxFontSize
            txtInput.Font = New Font(txtInput.Font.FontFamily, txtInput.Font.Size + 1, FontStyle.Bold)
            textSize = TextRenderer.MeasureText(txtInput.Text, txtInput.Font)
        End While
    End Sub

    ' Trigger font adjustment when text changes
    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        ' Only update if the input is a valid number
        Dim input As String = txtInput.Text
        If IsNumeric(input) AndAlso input.Length > 0 Then
            If isPrimeToggled Then
                CalculatePrime(input)
            ElseIf isFactorialToggled Then
                CalculateFactorial(input)
            End If
        Else
            txtAnswer.Clear() ' Clear answer if input is invalid
        End If
    End Sub

    Private Sub txtInput_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtInput.KeyPress
        ' Allow only digits (0-9), backspace, and the minus sign (for negative numbers)
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> ControlChars.Back AndAlso e.KeyChar <> "-" Then
            e.Handled = True ' Suppress the keypress
        End If

        ' Allow only one minus sign at the beginning of the input
        If e.KeyChar = "-" AndAlso (txtInput.SelectionStart <> 0 OrElse txtInput.Text.Contains("-")) Then
            e.Handled = True ' Suppress the keypress
        End If
    End Sub

    Private Sub Button_MouseEnter(sender As Object, e As EventArgs) Handles btn0.MouseEnter, btn1.MouseEnter, btn2.MouseEnter, btn3.MouseEnter, btn4.MouseEnter, btn5.MouseEnter, btn6.MouseEnter, btn7.MouseEnter, btn8.MouseEnter, btn9.MouseEnter, btnClear.MouseEnter, btnBackspace.MouseEnter
        Dim btn As Button = CType(sender, Button)
        btn.BackColor = Color.FromArgb(45, 45, 45) ' Hover effect
    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As EventArgs) Handles btn0.MouseLeave, btn1.MouseLeave, btn2.MouseLeave, btn3.MouseLeave, btn4.MouseLeave, btn5.MouseLeave, btn6.MouseLeave, btn7.MouseLeave, btn8.MouseLeave, btn9.MouseLeave, btnClear.MouseLeave, btnBackspace.MouseLeave
        Dim btn As Button = CType(sender, Button)
        btn.BackColor = Color.FromArgb(59, 59, 59) ' Revert to default color when mouse leaves
    End Sub

    Private Sub Button_MouseDown(sender As Object, e As MouseEventArgs) Handles btn0.MouseDown, btn1.MouseDown, btn2.MouseDown, btn3.MouseDown, btn4.MouseDown, btn5.MouseDown, btn6.MouseDown, btn7.MouseDown, btn8.MouseDown, btn9.MouseDown, btnClear.MouseDown, btnBackspace.MouseDown
        Dim btn As Button = CType(sender, Button)
        btn.BackColor = Color.FromArgb(30, 30, 30) ' Pressed effect
    End Sub

    Private Sub Button_MouseUp(sender As Object, e As MouseEventArgs) Handles btn0.MouseUp, btn1.MouseUp, btn2.MouseUp, btn3.MouseUp, btn4.MouseUp, btn5.MouseUp, btn6.MouseUp, btn7.MouseUp, btn8.MouseUp, btn9.MouseUp, btnClear.MouseUp, btnBackspace.MouseUp
        Dim btn As Button = CType(sender, Button)
        btn.BackColor = Color.FromArgb(45, 45, 45) ' Revert to hover color after release
    End Sub

    ' Event handler for Prime Calculation button
    Private Sub btnPrime_Click(sender As Object, e As EventArgs) Handles btnPrime.Click
        ' Deselect Factorial button if it was selected
        If isFactorialToggled Then
            isFactorialToggled = False
            txtAnswer.Clear()
            txtInput.Clear() ' Clear textboxes when switching functions
        End If

        ' Toggle the Prime button's state
        isPrimeToggled = Not isPrimeToggled
        ToggleButtonBackground()

        ' Trigger the calculation immediately if Prime is toggled
        If isPrimeToggled Then
            txtInput_TextChanged(sender, e)
        End If
        txtInput.Focus()
    End Sub

    ' Event handler for Factorial Calculation button
    Private Sub btnFactorial_Click(sender As Object, e As EventArgs) Handles btnFactorial.Click
        ' Deselect Prime button if it was selected
        If isPrimeToggled Then
            isPrimeToggled = False
            txtAnswer.Clear()
            txtInput.Clear() ' Clear textboxes when switching functions
        End If

        ' Toggle the Factorial button's state
        isFactorialToggled = Not isFactorialToggled
        ToggleButtonBackground()

        ' Trigger the calculation immediately if Factorial is toggled
        If isFactorialToggled Then
            txtInput_TextChanged(sender, e)
        End If
        txtInput.Focus()
    End Sub

    ' Toggle button background color based on selection
    Private Sub ToggleButtonBackground()
        ' Reset both buttons to their default colors
        btnPrime.BackColor = Color.FromArgb(59, 59, 59) ' Default color for Prime button
        btnFactorial.BackColor = Color.FromArgb(59, 59, 59) ' Default color for Factorial button

        ' Darken the background of the toggled button
        If isPrimeToggled Then
            btnPrime.BackColor = Color.FromArgb(30, 30, 30) ' Darkened color for Prime button
        ElseIf isFactorialToggled Then
            btnFactorial.BackColor = Color.FromArgb(30, 30, 30) ' Darkened color for Factorial button
        End If
    End Sub

    ' Function to check if a number is prime
    Private Sub CalculatePrime(input As String)
        If IsNumeric(input) Then
            Try
                Dim num As BigInteger = BigInteger.Parse(input)
                If num > 1 Then
                    If IsPrime(num) Then
                        txtAnswer.Text = "= Prime"
                    Else
                        txtAnswer.Text = "= Not Prime"
                    End If
                Else
                    txtAnswer.Text = "= Not Prime"
                End If
            Catch ex As OverflowException
                txtAnswer.Text = "= Number too large"
            End Try
        End If
    End Sub

    ' Function to calculate factorial
    Private Sub CalculateFactorial(input As String)
        If IsNumeric(input) Then
            Dim num As Integer = CInt(input)
            If num >= 0 AndAlso num <= 10000 Then
                txtAnswer.Text = "= " & Factorial(num).ToString()
            ElseIf num > 10000 Then
                txtAnswer.Text = "= Number too large"
            Else
                txtAnswer.Text = "= Invalid Input"
            End If
        End If
    End Sub

    ' Function to check if a number is prime
    Private Function IsPrime(ByVal num As BigInteger) As Boolean
        If num < 2 Then
            Return False
        End If
        For i As BigInteger = 2 To BigIntegerSqrt(num)
            If num Mod i = 0 Then
                Return False
            End If
        Next
        Return True
    End Function

    ' Function to calculate factorial
    Private Function Factorial(ByVal num As Integer) As BigInteger
        If num = 0 Then
            Return 1
        End If
        Dim result As BigInteger = 1
        For i As Integer = 1 To num
            result *= i
        Next
        Return result
    End Function

    Private Function BigIntegerSqrt(n As BigInteger) As BigInteger
        If n < 0 Then
            Throw New ArgumentException("Cannot compute square root of a negative number.")
        End If
        If n = 0 OrElse n = 1 Then
            Return n
        End If

        Dim x As BigInteger = n
        Dim y As BigInteger = (x + 1) / 2
        While y < x
            x = y
            y = (x + (n / x)) / 2
        End While
        Return x
    End Function

End Class
