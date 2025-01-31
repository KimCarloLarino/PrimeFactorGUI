' UIHelper.vb
Imports System.Drawing

Public Class UIHelper
    Private ReadOnly DefaultFontSize As Single = 24

    ' Adjusts the font size of the input textbox dynamically
    Public Sub AdjustFontSize(txtInput As TextBox)
        Dim currentFont = txtInput.Font
        Dim newSize = CalculateOptimalFontSize(txtInput.Text, txtInput.Width, currentFont)
        If newSize <> currentFont.Size Then
            txtInput.Font = New Font(currentFont.FontFamily, newSize, FontStyle.Bold)
        End If
    End Sub

    ' Throttles font size adjustment to prevent excessive calls
    Public Sub ThrottledAdjustFontSize(txtInput As TextBox)
        Static lastUpdate As DateTime = DateTime.MinValue
        If (DateTime.Now - lastUpdate).TotalMilliseconds < 100 Then Exit Sub
        lastUpdate = DateTime.Now
        AdjustFontSize(txtInput)
    End Sub

    ' Calculates the optimal font size based on textbox width
    Private Function CalculateOptimalFontSize(text As String, width As Integer, font As Font) As Single
        Using g = Graphics.FromHwnd(IntPtr.Zero)
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

    ' Updates button colors dynamically based on toggle states
    Public Sub UpdateButtonColors(btnPrime As Button, btnFactorial As Button, isPrimeToggled As Boolean, isFactorialToggled As Boolean)
        btnPrime.BackColor = If(isPrimeToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
        btnFactorial.BackColor = If(isFactorialToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
    End Sub

    ' Handles mouse hover effects on buttons
    Public Sub ButtonMouseEnter(btn As Button, isToggled As Boolean)
        If isToggled Then Exit Sub
        btn.BackColor = Color.FromArgb(45, 45, 45)
    End Sub

    ' Handles mouse leave effects on buttons
    Public Sub ButtonMouseLeave(btn As Button, isToggled As Boolean)
        If isToggled Then Exit Sub
        btn.BackColor = Color.FromArgb(59, 59, 59)
    End Sub

    ' Handles button press effect
    Public Sub ButtonMouseDown(btn As Button, isToggled As Boolean)
        If isToggled Then Exit Sub
        btn.BackColor = Color.FromArgb(30, 30, 30)
    End Sub

    ' Handles button release effect
    Public Sub ButtonMouseUp(btn As Button, isToggled As Boolean)
        If isToggled Then Exit Sub
        btn.BackColor = Color.FromArgb(45, 45, 45)
    End Sub
End Class
