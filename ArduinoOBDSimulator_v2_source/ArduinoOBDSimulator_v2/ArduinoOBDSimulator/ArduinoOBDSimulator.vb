Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO.Ports
Imports System.ComponentModel
Imports System.IO
Imports System.Text

Public Class ArduinoOBDSimulator
    '------------------------------------------------
    Dim myPort As Array
    Delegate Sub SetTextCallback(ByVal [text] As String) 'Added to prevent threading errors during receiveing of data
    Dim dtcSrt As String

    Dim ParentDirectory As String = System.IO.Path.GetFullPath(Application.StartupPath & "\Resources\") 'CommonAppDataPath
    Dim avrdudeProgramPath As String = ParentDirectory & "avrdude.exe"
    Dim avrdudeConfigFilePath As String = ParentDirectory & "avrdude.conf"
    Dim hexFilePath As String = ParentDirectory & "hexfile.hex"
    '------------------------------------------------
    Private Sub Form1_Load(sender As System.Object, e As EventArgs) Handles MyBase.Load

        myPort = IO.Ports.SerialPort.GetPortNames()
        ComboBox1.Items.AddRange(myPort)
        Init_Components()
        ComboBox2.SelectedIndex = 0
    End Sub
    '------------------------------------------------
    Private Sub Init_Components()

        Button2.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button5.Enabled = True
        Button6.Enabled = False
        RadioButton1.Checked = True
        RadioButton5.Checked = True

        Disable_All_TextBox_TrackBar()
        Timer1.Enabled = True

        TextBox1.Text = TrackBar1.Value
        TextBox2.Text = TrackBar2.Value
        TextBox3.Text = TrackBar3.Value
        TextBox4.Text = TrackBar4.Value
        TextBox5.Text = TrackBar5.Value
        TextBox6.Text = TrackBar6.Value
        TextBox7.Text = TrackBar7.Value
        TextBox8.Text = TrackBar8.Value
        TextBox9.Text = TrackBar9.Value
        TextBox10.Text = TrackBar10.Value
        TextBox11.Text = "100"
        RichTextBox2.Text = ""
    End Sub
    '------------------------------------------------
    Private Sub Button1_Click(sender As System.Object, e As EventArgs) Handles Button1.Click
        Dim exFlag As Integer = 0
        RichTextBox2.Text = ""
        If (myPort.Length > 0) And (ComboBox1.Text.Length > 0) And (ComboBox2.Text.Length > 0) Then
            SerialPort1.PortName = ComboBox1.Text
            SerialPort1.BaudRate = ComboBox2.Text
            Try
                SerialPort1.Open()
            Catch ex As Exception
                exFlag = 1
                Console.WriteLine("Exeption Caught: {0}", ex)
                MessageBox.Show("Failed To Open COM port" + vbCr + vbCr + ex.GetType.ToString, "Alert", MessageBoxButtons.OK)
            End Try

            If (exFlag = 0) Then
                Button1.Enabled = False
                Button2.Enabled = True
                Button3.Enabled = True
                Button4.Enabled = True
                Button5.Enabled = False
                Button6.Enabled = True
                Enable_All_TextBox_TrackBar()
            End If

        End If
    End Sub
    '------------------------------------------------
    Private Sub Button2_Click(sender As System.Object, e As EventArgs) Handles Button2.Click

        SerialPort1.Write(RichTextBox1.Text & vbCr) 'concatenate with \n
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As EventArgs) Handles Button4.Click
        SerialPort1.Dispose()
        SerialPort1.Close()
        SerialPort1.Close()
        Button1.Enabled = True
        Button2.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button5.Enabled = True
        Button6.Enabled = False

        Disable_All_TextBox_TrackBar()

    End Sub

    Private Sub SerialPort1_DataReceived(sender As System.Object, e As SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        ReceivedText(SerialPort1.ReadExisting())
    End Sub

    Private Sub ReceivedText(ByVal [text] As String) 'input from ReadExisting
        If Me.RichTextBox2.InvokeRequired Then
            Dim x As New SetTextCallback(AddressOf ReceivedText)
            Me.Invoke(x, New Object() {(text)})
        Else
            Me.RichTextBox2.AppendText(text)
            Me.RichTextBox2.SelectionStart = Me.RichTextBox2.TextLength
            Me.RichTextBox2.ScrollToCaret()
        End If

    End Sub

    Private Sub CreateAndSendFrame()
        Dim abArray(44) As Byte
        Dim Output As String

        Dim value As Integer
        value = TrackBar1.Value
        For i As Integer = 0 To 3
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar2.Value
        For i As Integer = 4 To 7
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 4))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar3.Value + 40
        For i As Integer = 8 To 11
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 8))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar4.Value
        For i As Integer = 12 To 15
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 12))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = (TrackBar5.Value * 255) / 100
        For i As Integer = 16 To 19
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 16))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar6.Value + 40
        For i As Integer = 20 To 23
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 20))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = (TrackBar7.Value * 100)
        For i As Integer = 24 To 27
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 24))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar8.Value
        For i As Integer = 28 To 31
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 28))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = (TrackBar9.Value * 255) / 100
        For i As Integer = 32 To 35
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 32))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("---")
        value = TrackBar10.Value * 4
        For i As Integer = 36 To 39
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 36))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        Console.WriteLine("DTC---")
        value = ConvertDTC()
        For i As Integer = 40 To 43
            abArray(i) = (Int(value / (2 ^ (8 * (Len(value) - 1 - i + 40))))) And ((2 ^ 8) - 1)
            Output = abArray(i).ToString("X4")
            Console.WriteLine(Output)
        Next i

        SerialPort1.Write(abArray, 0, 44)

    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        TextBox1.Text = TrackBar1.Value.ToString
    End Sub

    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar2.Scroll
        TextBox2.Text = TrackBar2.Value.ToString
    End Sub

    Private Sub TrackBar3_Scroll(sender As Object, e As EventArgs) Handles TrackBar3.Scroll
        TextBox3.Text = TrackBar3.Value.ToString
    End Sub

    Private Sub TrackBar4_Scroll(sender As Object, e As EventArgs) Handles TrackBar4.Scroll
        TextBox4.Text = TrackBar4.Value
    End Sub

    Private Sub TrackBar5_Scroll(sender As Object, e As EventArgs) Handles TrackBar5.Scroll
        TextBox5.Text = TrackBar5.Value
    End Sub

    Private Sub TrackBar6_Scroll(sender As Object, e As EventArgs) Handles TrackBar6.Scroll
        TextBox6.Text = TrackBar6.Value
    End Sub

    Private Sub TrackBar7_Scroll(sender As Object, e As EventArgs) Handles TrackBar7.Scroll
        TextBox7.Text = TrackBar7.Value
    End Sub

    Private Sub TrackBar8_Scroll(sender As Object, e As EventArgs) Handles TrackBar8.Scroll
        TextBox8.Text = TrackBar8.Value
    End Sub

    Private Sub TrackBar9_Scroll(sender As Object, e As EventArgs) Handles TrackBar9.Scroll
        TextBox9.Text = TrackBar9.Value
    End Sub

    Private Sub TrackBar10_Scroll(sender As Object, e As EventArgs) Handles TrackBar10.Scroll
        TextBox10.Text = TrackBar10.Value.ToString
    End Sub

    Private Sub TrackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseUp
        'TextBox1.Text = TrackBar1.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar2_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar2.MouseUp
        ' TextBox2.Text = TrackBar2.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar3_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar3.MouseUp
        'TextBox3.Text = TrackBar3.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar4_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar4.MouseUp
        'TextBox4.Text = TrackBar4.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar5_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar5.MouseUp
        'TextBox5.Text = TrackBar5.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar6_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar6.MouseUp
        'TextBox6.Text = TrackBar6.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar7_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar7.MouseUp
        'TextBox7.Text = TrackBar7.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar8_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar8.MouseUp
        'TextBox8.Text = TrackBar8.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar9_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar9.MouseUp
        'TextBox9.Text = TrackBar9.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TrackBar10_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar10.MouseUp
        'TextBox10.Text = TrackBar10.Value.ToString
        CreateAndSendFrame()
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox1.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox1.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox1.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox1.Text)
            If (TrackBar1.Minimum <= value) And (TrackBar1.Maximum >= value) Then
                TrackBar1.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox1.Text = TrackBar1.Minimum.ToString
                    TrackBar1.Value = TrackBar1.Minimum
                Else
                    TextBox1.Text = TrackBar1.Maximum.ToString
                    TrackBar1.Value = TrackBar1.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox1.Text.Length > 1) Then
                    TextBox1.Text = TrackBar1.Maximum.ToString
                    TrackBar1.Value = TrackBar1.Maximum
                End If
            Else
                TextBox1.Text = TrackBar1.Maximum.ToString
                TrackBar1.Value = TrackBar1.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox2.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox2.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox2.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox2.Text)
            If (TrackBar2.Minimum <= value) And (TrackBar2.Maximum >= value) Then
                TrackBar2.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox2.Text = TrackBar2.Minimum.ToString
                    TrackBar2.Value = TrackBar2.Minimum
                Else
                    TextBox2.Text = TrackBar2.Maximum.ToString
                    TrackBar2.Value = TrackBar2.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox2.Text.Length > 1) Then
                    TextBox2.Text = TrackBar2.Maximum.ToString
                    TrackBar2.Value = TrackBar2.Maximum
                End If
            Else
                TextBox2.Text = TrackBar2.Maximum.ToString
                TrackBar2.Value = TrackBar2.Maximum
            End If
        End Try

    End Sub
    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox3.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox3.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox3.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox3.Text)
            If (TrackBar3.Minimum <= value) And (TrackBar3.Maximum >= value) Then
                TrackBar3.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox3.Text = TrackBar3.Minimum.ToString
                    TrackBar3.Value = TrackBar3.Minimum
                Else
                    TextBox3.Text = TrackBar3.Maximum.ToString
                    TrackBar3.Value = TrackBar3.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox3.Text.Length > 1) Then
                    TextBox3.Text = TrackBar3.Maximum.ToString
                    TrackBar3.Value = TrackBar3.Maximum
                End If
            Else
                TextBox3.Text = TrackBar3.Maximum.ToString
                TrackBar3.Value = TrackBar3.Maximum
            End If
        End Try

    End Sub


    Private Sub TextBox4_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox4.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox4.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox4.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox4.Text)
            If (TrackBar4.Minimum <= value) And (TrackBar4.Maximum >= value) Then
                TrackBar4.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox4.Text = TrackBar4.Minimum.ToString
                    TrackBar4.Value = TrackBar4.Minimum
                Else
                    TextBox4.Text = TrackBar4.Maximum.ToString
                    TrackBar4.Value = TrackBar4.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox4.Text.Length > 1) Then
                    TextBox4.Text = TrackBar4.Maximum.ToString
                    TrackBar4.Value = TrackBar4.Maximum
                End If
            Else
                TextBox4.Text = TrackBar4.Maximum.ToString
                TrackBar4.Value = TrackBar4.Maximum
            End If
        End Try

    End Sub


    Private Sub TextBox5_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox5.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox5.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox5.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox5.Text)
            If (TrackBar5.Minimum <= value) And (TrackBar5.Maximum >= value) Then
                TrackBar5.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox5.Text = TrackBar5.Minimum.ToString
                    TrackBar5.Value = TrackBar5.Minimum
                Else
                    TextBox5.Text = TrackBar5.Maximum.ToString
                    TrackBar5.Value = TrackBar5.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox5.Text.Length > 1) Then
                    TextBox5.Text = TrackBar5.Maximum.ToString
                    TrackBar5.Value = TrackBar5.Maximum
                End If
            Else
                TextBox5.Text = TrackBar5.Maximum.ToString
                TrackBar5.Value = TrackBar5.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox6_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox6.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox6.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox6.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox6.Text)
            If (TrackBar6.Minimum <= value) And (TrackBar6.Maximum >= value) Then
                TrackBar6.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox6.Text = TrackBar6.Minimum.ToString
                    TrackBar6.Value = TrackBar6.Minimum
                Else
                    TextBox6.Text = TrackBar6.Maximum.ToString
                    TrackBar6.Value = TrackBar6.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox6.Text.Length > 1) Then
                    TextBox6.Text = TrackBar6.Maximum.ToString
                    TrackBar6.Value = TrackBar6.Maximum
                End If
            Else
                TextBox6.Text = TrackBar6.Maximum.ToString
                TrackBar6.Value = TrackBar6.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox7_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox7.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox7.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox7.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox7.Text)
            If (TrackBar7.Minimum <= value) And (TrackBar7.Maximum >= value) Then
                TrackBar7.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox7.Text = TrackBar7.Minimum.ToString
                    TrackBar7.Value = TrackBar7.Minimum
                Else
                    TextBox7.Text = TrackBar7.Maximum.ToString
                    TrackBar7.Value = TrackBar7.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox7.Text.Length > 1) Then
                    TextBox7.Text = TrackBar7.Maximum.ToString
                    TrackBar7.Value = TrackBar7.Maximum
                End If
            Else
                TextBox7.Text = TrackBar7.Maximum.ToString
                TrackBar7.Value = TrackBar7.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox8_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox8.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox8.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox8.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox8.Text)
            If (TrackBar8.Minimum <= value) And (TrackBar8.Maximum >= value) Then
                TrackBar8.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox8.Text = TrackBar8.Minimum.ToString
                    TrackBar8.Value = TrackBar8.Minimum
                Else
                    TextBox8.Text = TrackBar8.Maximum.ToString
                    TrackBar8.Value = TrackBar8.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox8.Text.Length > 1) Then
                    TextBox8.Text = TrackBar8.Maximum.ToString
                    TrackBar8.Value = TrackBar8.Maximum
                End If
            Else
                TextBox8.Text = TrackBar8.Maximum.ToString
                TrackBar8.Value = TrackBar8.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox9_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox9.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox9.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox9.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox9.Text)
            If (TrackBar9.Minimum <= value) And (TrackBar9.Maximum >= value) Then
                TrackBar9.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox9.Text = TrackBar9.Minimum.ToString
                    TrackBar9.Value = TrackBar9.Minimum
                Else
                    TextBox9.Text = TrackBar9.Maximum.ToString
                    TrackBar9.Value = TrackBar9.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox9.Text.Length > 1) Then
                    TextBox9.Text = TrackBar9.Maximum.ToString
                    TrackBar9.Value = TrackBar9.Maximum
                End If
            Else
                TextBox9.Text = TrackBar9.Maximum.ToString
                TrackBar9.Value = TrackBar9.Maximum
            End If
        End Try

    End Sub

    Private Sub TextBox10_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox10.KeyUp

        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True

        Else
            If (TextBox10.Text.Length > 1 And (e.KeyValue = 109)) Then
                TextBox10.Text = "0"
            End If
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            Dim value As Integer = CInt(TextBox10.Text)
            If (TrackBar10.Minimum <= value) And (TrackBar10.Maximum >= value) Then
                TrackBar10.Value = value
                Button3.Enabled = True
            Else
                If value < 0 Then
                    TextBox10.Text = TrackBar10.Minimum.ToString
                    TrackBar10.Value = TrackBar10.Minimum
                Else
                    TextBox10.Text = TrackBar10.Maximum.ToString
                    TrackBar10.Value = TrackBar10.Maximum
                End If
            End If

        Catch ex As Exception
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                If (TextBox10.Text.Length > 1) Then
                    TextBox10.Text = TrackBar10.Maximum.ToString
                    TrackBar10.Value = TrackBar10.Maximum
                End If
            Else
                TextBox10.Text = TrackBar10.Maximum.ToString
                TrackBar10.Value = TrackBar10.Maximum
            End If
        End Try

    End Sub
    Private Sub TextBox11_KeyPress(ByVal sender As Object, ByVal e As KeyEventArgs) Handles TextBox11.KeyUp
        If (96 <= e.KeyCode <= 105) Or ((37 <= e.KeyCode <= 40)) Then
            e.Handled = True
            Console.WriteLine("True")
        Else
            TextBox11.Text = "0"
            Console.WriteLine("False")
            Exit Sub
        End If
        Try
            If (e.KeyCode = 109) Or (e.KeyCode = 189) Then
                TextBox11.Text = "0"
            End If

            Dim value As Integer = CInt(TextBox11.Text)
        Catch ex As Exception
            TextBox11.Text = "0"
        End Try


    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        CreateAndSendFrame()
        Button3.Enabled = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Button3.Enabled Then
            If Button3.BackColor = Color.Yellow Then
                Button3.BackColor = Color.LightGray
            Else
                Button3.BackColor = Color.Yellow
            End If
        Else
            Button3.BackColor = Color.LightGray
        End If
    End Sub

    Private Function ConvertDTC() As Int32
        Dim data As Int32 = 0
        If (Button6.Text = "Clear DTC") Then
            Dim First_Byte As Int32 = 0
            If RadioButton1.Checked Then
                First_Byte = 0
            End If
            If RadioButton2.Checked Then
                First_Byte = 1 << 14
            End If
            If RadioButton3.Checked Then
                First_Byte = 2 << 14
            End If
            If RadioButton4.Checked Then
                First_Byte = 3 << 14
            End If

            If RadioButton5.Checked Then
                First_Byte = First_Byte And Not (1 << 12)
            End If
            If RadioButton6.Checked Then
                First_Byte = First_Byte Or 1 << 12
            End If

            Dim value As Int32
            Try
                value = CInt(TextBox11.Text)
            Catch ex As Exception
                value = 0
            End Try


            data = (Int(value / (2 ^ (16 * (Len(value) - 1 - 3 + 0))))) And ((2 ^ 16) - 1)
            data = (data And &HFFF) Or (First_Byte And &HF000)
        Else
            data = 0
        End If

        Return data
    End Function


    Private Sub Disable_All_TextBox_TrackBar()

        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        TextBox5.Enabled = False
        TextBox6.Enabled = False
        TextBox7.Enabled = False
        TextBox8.Enabled = False
        TextBox9.Enabled = False
        TextBox10.Enabled = False
        TextBox11.Enabled = False

        TrackBar1.Enabled = False
        TrackBar2.Enabled = False
        TrackBar3.Enabled = False
        TrackBar4.Enabled = False
        TrackBar5.Enabled = False
        TrackBar6.Enabled = False
        TrackBar7.Enabled = False
        TrackBar8.Enabled = False
        TrackBar9.Enabled = False
        TrackBar10.Enabled = False
        GroupBox1.Enabled = False
        GroupBox2.Enabled = False
    End Sub


    Private Sub Enable_All_TextBox_TrackBar()

        TextBox1.Enabled = True
        TextBox2.Enabled = True
        TextBox3.Enabled = True
        TextBox4.Enabled = True
        TextBox5.Enabled = True
        TextBox6.Enabled = True
        TextBox7.Enabled = True
        TextBox8.Enabled = True
        TextBox9.Enabled = True
        TextBox10.Enabled = True
        TextBox11.Enabled = True

        TrackBar1.Enabled = True
        TrackBar2.Enabled = True
        TrackBar3.Enabled = True
        TrackBar4.Enabled = True
        TrackBar5.Enabled = True
        TrackBar6.Enabled = True
        TrackBar7.Enabled = True
        TrackBar8.Enabled = True
        TrackBar9.Enabled = True
        TrackBar10.Enabled = True
        GroupBox1.Enabled = True
        GroupBox2.Enabled = True
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        RichTextBox2.Text = "Please Wait .........."

        '**********************Create avrdude.conf File From Res************************
        Dim path As String = ParentDirectory
        path = System.IO.Path.Combine(path, "avrdude.conf")
        If System.IO.File.Exists(path) = True Then
            System.IO.File.Delete(path)
        End If
        System.IO.File.WriteAllText(path, My.Resources.avrdude)

        '**********************Create Hex File From Res************************
        path = ParentDirectory
        path = System.IO.Path.Combine(path, "hexfile.hex")
        If System.IO.File.Exists(path) = True Then
            System.IO.File.Delete(path)
        End If

        Dim HexFileData As String = My.Resources.hexfile
        System.IO.File.WriteAllText(path, HexFileData)

        'BackgroundWorker1.RunWorkerAsync()
        Dim myProcess() As Process = System.Diagnostics.Process.GetProcessesByName("avrdude.exe")
        For Each myKill As Process In myProcess
            myKill.Kill()
        Next
        Task.Run(Sub()
                     BackgroundWorker1_DoWork()
                 End Sub)

    End Sub

    Public Sub BackgroundWorker1_DoWork()

        Dim comportString As String = Me.Invoke(Function()
                                                    If Not IsNothing(ComboBox1.SelectedItem) Then
                                                        Return ComboBox1.SelectedItem.ToString
                                                    Else
                                                        Return String.Empty
                                                    End If
                                                End Function).ToString

        Dim proc As New Process
        proc.StartInfo.FileName = avrdudeProgramPath
        proc.StartInfo.Arguments = "-C " & avrdudeConfigFilePath & " -v -patmega328p -carduino -P" & comportString & " -b115200 -D -Uflash:w:" & hexFilePath & ":i "
        proc.StartInfo.CreateNoWindow = True
        proc.StartInfo.UseShellExecute = False
        Application.DoEvents()
        proc.StartInfo.RedirectStandardError = True
        AddHandler proc.ErrorDataReceived, AddressOf Proc_OutputDataReceived

        proc.Start()

        proc.BeginErrorReadLine()

        proc.WaitForExit()
        proc.Close()
        SetText("OBD 2 Simulator 2.0" & vbCr & "Powered by Arduino and avrdude")
    End Sub
    Public Sub Proc_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        SetText(e.Data + vbCrLf)
    End Sub

    Private Sub SetText(ByVal newString As String)
        ' Calling from another thread? -> Use delegate
        If Me.RichTextBox2.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetText)
            ' Execute delegate in the UI thread, pass args as an array
            Me.Invoke(d, New Object() {newString})
        Else ' Same thread, assign string to the textbox
            Me.RichTextBox2.AppendText(newString)
            Me.RichTextBox2.SelectionStart = Me.RichTextBox2.TextLength
            Me.RichTextBox2.ScrollToCaret()
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If Button6.Text = "Set DTC" Then
            Button6.Text = "Clear DTC"
        Else
            Button6.Text = "Set DTC"
        End If
        CreateAndSendFrame()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim Form1 As Form = New Help
        Form1.Show()
    End Sub
End Class
