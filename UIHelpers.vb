'' UIHelpers.vb
'Imports System.Drawing

'Public Module UIHelpers
'    ' Font adjustment logic
'    Public Sub AdjustFontSize(txtBox As TextBox, defaultSize As Single)
'        Dim currentFont = txtBox.Font
'        Dim newSize = CalculateOptimalFontSize(txtBox.Text, txtBox.Width, currentFont)

'        If newSize <> currentFont.Size Then
'            txtBox.Font = New Font(currentFont.FontFamily, newSize, FontStyle.Bold)
'        End If
'    End Sub

'    Private Function CalculateOptimalFontSize(text As String, width As Integer, font As Font) As Single
'        Using g = txtInput.CreateGraphics()
'            Dim size = g.MeasureString(text, font)
'            If size.Width <= width Then Return Math.Min(font.Size + 1, DefaultFontSize)

'            Dim minSize As Single = 10
'            Dim currentSize As Single = font.Size
'            While currentSize > minSize
'                currentSize -= 1
'                size = g.MeasureString(text, New Font(font.FontFamily, currentSize, FontStyle.Bold))
'                If size.Width <= width Then Return currentSize
'            End While
'            Return minSize
'        End Using
'    End Function

'    ' Button color management
'    Public Sub UpdateButtonColors(btnPrime As Button, btnFactorial As Button,
'                                isPrimeToggled As Boolean, isFactorialToggled As Boolean)
'        btnPrime.BackColor = If(isPrimeToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
'        btnFactorial.BackColor = If(isFactorialToggled, Color.FromArgb(30, 30, 30), Color.FromArgb(59, 59, 59))
'    End Sub
'End Module