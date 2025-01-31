Imports System.Drawing
Imports System.Numerics
Imports System.ComponentModel

' Main form for the calculator application.
' Handles user interactions, UI updates, and delegates calculations.
Public Class frmMain
    Private DefaultFontSize As Single = 24
    Private isPrimeToggled As Boolean = False
    Private isFactorialToggled As Boolean = False
    Private lastProcessedInput As String = String.Empty
    Private primeWorker As BackgroundWorker = New BackgroundWorker()
    Private calculator As New Calculator()
    Private primeChecker As PrimeChecker
    Private uihelper As New UIHelper()

    ' Initializes the form, sets focus, and configures workers.
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ActiveControl = txtInput
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold)
        calculator.InitializeFactorialCache()
        ConfigureBackgroundWorker()
        primeChecker = New PrimeChecker(primeWorker)
    End Sub

    '=============== UI HANDLERS ===============
    'Handles numeric button clicks, appends the value to input field.
    Private Sub NumberButton_Click(sender As Object, e As EventArgs) Handles btn0.Click, btn1.Click, btn2.Click, btn3.Click, btn4.Click, btn5.Click, btn6.Click, btn7.Click, btn8.Click, btn9.Click
        Dim btn As Button = CType(sender, Button)
        txtInput.Text &= btn.Text
        uihelper.ThrottledAdjustFontSize(txtInput)
        txtInput.SelectionStart = txtInput.Text.Length
        txtInput.Focus()
    End Sub

    'Handles backspace button click to remove last character.
    Private Sub btnBackspace_Click(sender As Object, e As EventArgs) Handles btnBackspace.Click
        If txtInput.Text.Length > 0 Then
            txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1)
            uihelper.ThrottledAdjustFontSize(txtInput) ' Pass txtInput as an argument
        End If
    End Sub

    'Clears the input and output fields.
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtInput.Clear()
        txtAnswer.Clear()
        txtInput.Font = New Font(txtInput.Font.FontFamily, DefaultFontSize, FontStyle.Bold)
        txtInput.Focus()
    End Sub

    '=============== BACKGROUND WORKER CONFIGURATION ===============
    'Configures the background worker for prime number checking.
    Private Sub ConfigureBackgroundWorker()
        AddHandler primeWorker.DoWork, AddressOf PrimeWorker_DoWork
        AddHandler primeWorker.RunWorkerCompleted, AddressOf PrimeWorker_Completed
        primeWorker.WorkerSupportsCancellation = True
    End Sub

    '=============== EVENT HANDLERS ===============
    'Handles user input and triggers calculations if toggled.
    Private Sub txtInput_TextChanged(sender As Object, e As EventArgs) Handles txtInput.TextChanged
        Dim input = txtInput.Text

        ' Remove invalid characters
        If System.Text.RegularExpressions.Regex.IsMatch(txtInput.Text, "[^0-9-]") Then
            txtInput.Text = System.Text.RegularExpressions.Regex.Replace(txtInput.Text, "[^0-9-]", "")
            txtInput.SelectionStart = txtInput.Text.Length
        End If

        If String.IsNullOrEmpty(input) Then
            txtAnswer.Clear()
            Return
        End If

        If isPrimeToggled Then
            calculator.CalculatePrime(input, txtAnswer, primeWorker)
        ElseIf isFactorialToggled Then
            Dim num As Integer
            If Integer.TryParse(input, num) Then
                If num > 10000 Then
                    txtAnswer.Text = "= Number too large"
                Else
                    calculator.CalculateFactorial(input, txtAnswer)
                End If
            Else
                txtAnswer.Text = "= Invalid Input"
            End If
        End If
    End Sub

    'Handles background worker logic for prime checking.
    Private Sub PrimeWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim num As BigInteger = CType(e.Argument, BigInteger)
        e.Result = If(num < 2, False, primeChecker.IsPrime(num))  ' Use instance
    End Sub

    'Displays the result of the prime number check.
    Private Sub PrimeWorker_Completed(sender As Object, e As RunWorkerCompletedEventArgs)
        If e.Error IsNot Nothing Then
            txtAnswer.Text = "= Calculation Error"
        ElseIf Not e.Cancelled Then
            txtAnswer.Text = If(CType(e.Result, Boolean), "= Prime", "= Not Prime")
        End If
    End Sub

    '=============== BUTTON TOGGLES ===============
    'Toggles between prime and factorial calculation modes.
    Private Sub ToggleFunctionButton(ByRef currentToggle As Boolean, ByRef otherToggle As Boolean)
        If otherToggle Then
            otherToggle = False
            txtAnswer.Clear()
            txtInput.Clear()
        End If

        currentToggle = Not currentToggle
        UpdateButtonColors()
    End Sub

    'Updates button colors to reflect active states.
    Private Sub UpdateButtonColors()
        btnPrime.BackColor = If(isPrimeToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
        btnFactorial.BackColor = If(isFactorialToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
    End Sub

    'Handles Prime button click to toggle prime calculation.
    Private Sub btnPrime_Click(sender As Object, e As EventArgs) Handles btnPrime.Click
        ToggleFunctionButton(isPrimeToggled, isFactorialToggled)
        If isPrimeToggled Then txtInput_TextChanged(Nothing, Nothing)
        txtInput.Focus()
    End Sub

    ' Handles Factorial button click to toggle factorial calculation.
    Private Sub btnFactorial_Click(sender As Object, e As EventArgs) Handles btnFactorial.Click
        ToggleFunctionButton(isFactorialToggled, isPrimeToggled)
        If isFactorialToggled Then txtInput_TextChanged(Nothing, Nothing)
        txtInput.Focus()
    End Sub

End Class