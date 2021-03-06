﻿Imports System.Windows.Forms
Public Class Form
    'Declarations'
    Dim InitializeStatus As Integer = 0
    'Pumps'
    Dim Pump1 As New Pump()
    Dim Pump2 As New Pump()
    Dim Pump3 As New Pump()
    Dim Pump4 As New Pump()
    Dim Pump5 As New Pump()
    Dim Pump6 As New Pump()
    Dim Pump7 As New Pump()
    Dim Pump8 As New Pump()
    Dim Pump9 As New Pump()
    Dim Pump10 As New Pump()

    'PH sensors'
    Dim Sensor1 As New pHsensor()
    Dim Sensor2 As New pHsensor()
    Dim Sensor3 As New pHsensor()
    Dim Sensor4 As New pHsensor()
    Dim Sensor5 As New PHsensor()
    Dim Sensor6 As New PHsensor()
    'Serial Ports'
    Dim com(4) As IO.Ports.SerialPort
    Dim portCount As Integer

    Dim SensorNumber As Integer = 1  'For choosing sensors to read'
    Dim ArduinoReading As String ' This is the immediate reading (including sensor handle) from Arduino
    Dim VoltageReading As Integer ' This is the actual voltage reading from Arduino
    Dim PH_for_export(100000, 5) As Double
    Dim tick As ULong = 0 'For timer 1'
    Dim tick2 As ULong = 0 'for timer 2'
    Dim tick3 As ULong = 0 'For timer 3'
    Dim testrun As Integer = 0 'Flag for the system actually completely running'

    Dim SerialCheck As Integer 'This handle checks whether a serial is available
    'Random stuff for troubleshooting'
    Dim a As Integer

    Private Sub Base_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox15.Text = "Not initialized!"
        InitializeStatus = 0
    End Sub

    'Output string definition: ABCD - A refers to the type (1 for pump, 2 for valve, ...) B and C refers to the device number (starting from 01)
    'D refers to the control type (1 for digital and 2 for analog for pump or flash for valve)


    'The following 4 ports handle liquid transfer between vessels and are for MEGA

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump1, Button1, TextBox1, Port1, "101")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump2, Button2, TextBox2, Port1, "102")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump3, Button3, TextBox3, Port1, "103")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump4, Button4, TextBox4, Port1, "104")
            SerialCheck = 1
        End If
    End Sub

    'The following 6 ports handle pH adjustments and are for UNO

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump5, Button5, TextBox5, Port2, "105")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button37_Click(sender As Object, e As EventArgs) Handles Button37.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump6, Button37, TextBox26, Port2, "106")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button36_Click(sender As Object, e As EventArgs) Handles Button36.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump7, Button36, TextBox25, Port2, "107")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button35_Click(sender As Object, e As EventArgs) Handles Button35.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump8, Button35, TextBox24, Port2, "108")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button34_Click(sender As Object, e As EventArgs) Handles Button34.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump9, Button34, TextBox23, Port2, "109")
            SerialCheck = 1
        End If
    End Sub

    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click
        If SerialCheck = 1 Then
            SerialCheck = 0
            Call Pump_OnOff(Pump10, Button33, TextBox22, Port2, "110")
            SerialCheck = 1
        End If
    End Sub



    Private Sub Initialization_Click(sender As Object, e As EventArgs) Handles Initialization.Click 'Initialize!'

        SerialCheck = 1
        'This is the initialization button which initializes all variables'
        Sensor1.Slope = -35
        Sensor1.Intercept = 860
        Sensor2.Slope = -34
        Sensor2.Intercept = 867
        Sensor3.Slope = -35
        Sensor3.Intercept = 860


        tick = 0
        portCount = 0

        'Initialize the serial port connection with Arduinos'
        For Each sp As String In My.Computer.Ports.SerialPortNames
            comlist.Items.Add(sp)
            Try
                com(portCount) = My.Computer.Ports.OpenSerialPort(sp)
                com(portCount).ReadTimeout = 10000
                com(portCount).Close()
            Catch
            End Try
            portCount += 1
        Next
        'I only have limited amount of port objects, so lets manually assign them'
        Try
            Port2 = My.Computer.Ports.OpenSerialPort("COM6") 'Port for pH sensors  (UNO/MEGA1)
            Port1 = My.Computer.Ports.OpenSerialPort("COM7") 'Port for pumps/gas sparger (MEGA2)

            InitializeStatus += 1 'Check if this is initialization, or update
            'Confirm that the form is initialized'
            If InitializeStatus = 1 Then
                TextBox15.Text = "Initialized!"
                Initialization.Text = "Initialized, don't click again!"
            End If
            'Prime the pumps
            Port1.WriteLine("1001")
            Port2.WriteLine("1001")
            Port1.ReadTimeout = 5000
            Port2.ReadTimeout = 5000

        Catch 'if someone clicked it twice, prompt an error
            MsgBox("Initialization Error!" & vbCrLf & "Restart the program!")
        End Try
    End Sub




    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'This timer deals with reading voltage from PH sensors and plotting them etc'
        tick += 1
        If tick > 100 Then
            tick = 0
            Try
                Port2.DiscardInBuffer() 'This is VERY important! Otherwise readings accumulate in the buffer and you dont get actual real-time reading
            Catch
            End Try
        End If
        'Update the timers
        Sensor1.Timer += 1
        Sensor2.Timer += 1
        Sensor3.Timer += 1
        Sensor4.Timer += 1
        Sensor5.Timer += 1
        If InitializeStatus = 1 Then
            If SerialCheck = 1 Then 'If serial port is not occupied
                Try
                    SerialCheck = 0 'Occupy the serial
                    If Port2.BytesToRead > 0 Then
                        ArduinoReading = Port2.ReadLine.ToString
                        If ArduinoReading IsNot Nothing Then
                            If IsNumeric(ArduinoReading) = True Then
                                Try
                                    SensorNumber = CLng(ArduinoReading.Substring(0, 1))
                                    VoltageReading = CLng(ArduinoReading.Substring(1))

                                    'MsgBox(reading) 'For debugging

                                    Select Case SensorNumber
                                        Case 1
                                            Call Plot_Chart(Sensor1, Port2, StomachPH, TextBox18, VoltageReading, Sensor1.Timer)
                                            Call Plot_Chart(Sensor1, Port2, StomachPHAuto, TextBox16, VoltageReading, Sensor1.Timer)
                                            'export function under construction
                                            PH_for_export(Sensor1.Timer - 1, 0) = PH_Calculations(VoltageReading, Sensor1) 'Save the data into the array for exportation
                                        Case 2
                                            Call Plot_Chart(Sensor2, Port2, SmallIntestinePH, TextBox19, VoltageReading, Sensor2.Timer)
                                            Call Plot_Chart(Sensor2, Port2, SmallIntestinePHAuto, TextBox17, VoltageReading, Sensor2.Timer)
                                        Case 3
                                            Call Plot_Chart(Sensor3, Port2, ColonPH, TextBox20, VoltageReading, Sensor3.Timer)
                                            Call Plot_Chart(Sensor3, Port2, ColonPHAuto, TextBox21, VoltageReading, Sensor3.Timer)
                                        Case 4
                                            Call Plot_Chart(Sensor4, Port2, Colon2PH, TextBox27, VoltageReading, Sensor4.Timer)
                                            'Call Plot_Chart(Sensor4, ReadPort1, Colon2PHAuto, TextBox27, VoltageReading,Sensor4.Timer)
                                        Case 5
                                            Call Plot_Chart(Sensor5, Port2, Colon3PH, TextBox28, VoltageReading, Sensor5.Timer)
                                            'Call Plot_Chart(Sensor3, ReadPort1, Colon3PHAuto, TextBox28, VoltageReading,Sensor5.Timer)
                                    End Select
                                Catch
                                    ArduinoReading = ""
                                    SensorNumber = 0
                                    VoltageReading = 0

                                End Try
                            End If
                        End If
                        TestTextbox.Text = ""
                    Else
                        TestTextbox.Text = "Nothing"
                    End If
                Catch
                    MsgBox("error")
                End Try
            End If
            SerialCheck = 1 'Release the serial
        End If
    End Sub



    'The following buttons handle the three charts'
    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        'Button for enabling Stomach PH monitor'
        Call Plot_ONOFF(Sensor1, Button25)
    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        'Rest the chart'
        Call Plot_Reset(Sensor1, TextBox18, StomachPH)
    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        'Button for enabling Small Intestione PH monitor'
        Call Plot_ONOFF(Sensor2, Button26)
    End Sub

    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        'Rest the chart'
        Call Plot_Reset(Sensor2, TextBox19, SmallIntestinePH)
    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        'Button for enabling Colon PH monitor'
        Call Plot_ONOFF(Sensor3, Button27)
    End Sub

    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        'Rest the chart'
        Call Plot_Reset(Sensor3, TextBox20, ColonPH)
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        'Button for enabling Stomach PH monitor for auto control'
        Call Plot_ONOFF(Sensor1, Button24)
    End Sub

    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        'Button for enabling Small Intestine PH monitor for auto control'
        Call Plot_ONOFF(Sensor2, Button31)
    End Sub

    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        'Button for enabling Colon PH monitor for auto control'
        Call Plot_ONOFF(Sensor3, Button32)
    End Sub


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub Button45_Click(sender As Object, e As EventArgs) Handles Button45.Click
        'Export the log of PH stored to an Excel'
        Call Export_to_Excel("D:\book4.xlsx", 1, Sensor1)
        Call Export_to_Excel("D:\book4.xlsx", 2, Sensor2)
        Call Export_to_Excel("D:\book4.xlsx", 3, Sensor3)
    End Sub


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Functions
    Function PH_Calculations(readings As Integer, sensor_reading As PHsensor) As Double
        'This is the function to calculate PH from sensor readings
        PH_Calculations = (readings - sensor_reading.Intercept) / sensor_reading.Slope
    End Function

    Function Pump_OnOff(input_pump As Pump, input_button As Button, output_textbox As TextBox, output_port As System.IO.Ports.SerialPort, output_text As String)
        'This function handles the On/Off of each pump'
        If (input_pump.State = 0) Then
            input_pump.State += 1
        Else
            input_pump.State *= 0
        End If

        If (input_pump.State = 1) Then
            input_pump.StateStr = "On"
        Else
            input_pump.StateStr = "Off"
        End If
        Try
            output_port.WriteLine(input_pump.State.ToString & output_text) 'The output_text is the device specific code to the serial and Arduino'
        Catch
        End Try
        output_textbox.Text = input_pump.StateStr
        Pump_OnOff = True
    End Function

    Function Plot_Chart(input_sensor As PHsensor, input_port As System.IO.Ports.SerialPort, output_chart As System.Windows.Forms.DataVisualization.Charting.Chart, output_textbox As TextBox, input_number As Long, x_value As Integer)
        'This function plots the computed reading from sensors into charts
        Dim time As Double
        time = x_value / (1000 / Timer1.Interval) 'Convert to seconds
        If input_sensor.PlotStatus = 1 Then
            input_sensor.Timer += 1
            Try
                input_sensor.Reading = PH_Calculations(input_number, input_sensor) 'Note that here Arduino directly sent a number, not a string
                output_chart.Series(0).Points.AddXY(CInt(time), input_sensor.Reading.ToString)
                'The following code automatically trim the dataset'
                'If output_chart.Series(serie_number).Points.Count = 20 Then
                'output_chart.Series(serie_number).Points.RemoveAt(0)
                'End If
                output_textbox.Text = input_sensor.Reading.ToString
            Catch
            End Try
        End If
        Plot_Chart = True
    End Function

    Function Plot_ONOFF(input_sensor As pHsensor, input_button As Button)
        If input_sensor.PlotStatus = 1 Then
            input_button.Text = "Enable"
            input_sensor.PlotStatus = 0
        Else
            input_button.Text = "Disable"
            input_sensor.PlotStatus = 1
        End If
        Plot_ONOFF = True
    End Function

    Function Plot_Reset(input_sensor As pHsensor, output_textbox As TextBox, output_chart As System.Windows.Forms.DataVisualization.Charting.Chart)
        'Rest the chart'
        input_sensor.Timer = 0
        output_chart.Series(0).Points.Clear()
        output_textbox.Text = ""
        Plot_Reset = True
    End Function


    Function Export_to_Excel(Path As String, sensor_number As Integer, source_sensor As PHsensor)
        'To export sensor data to Excel for further investigation'
        'To use this you need to create an excel file already, ideally in root folder of a disk'
        Dim xls As Microsoft.Office.Interop.Excel.Application
        Dim xlsWorkBook As Microsoft.Office.Interop.Excel.Workbook
        Dim xlsWorkSheet As Microsoft.Office.Interop.Excel.Worksheet
        Dim misValue As Object = System.Reflection.Missing.Value
        Dim i As Integer

        xls = New Microsoft.Office.Interop.Excel.Application
        xlsWorkBook = xls.Workbooks.Open(Path)
        xlsWorkSheet = xlsWorkBook.Sheets("sheet1")
        Try
            For i = 0 To source_sensor.Timer
                xlsWorkSheet.Cells(i + 1, sensor_number).value = PH_for_export(i, sensor_number - 1)
            Next
        Catch
        End Try
        xls.Workbooks.Add()
        xlsWorkBook.Save()
        xlsWorkBook.Close()
        xls.Quit()
        Export_to_Excel = True
    End Function

    'The following codes are just to try out new functionalities


    Private Sub Random_Testing_Click(sender As Object, e As EventArgs) Handles Random_Testing.Click
        Try
            Port2 = My.Computer.Ports.OpenSerialPort("COM6") 'Port for pH sensors  (UNO/MEGA1)
            Port1 = My.Computer.Ports.OpenSerialPort("COM4") 'Port for pumps/gas sparger (MEGA2)
            Port1.DiscardInBuffer()
            Port2.DiscardInBuffer()

            'Prime the pumps
        Catch 'if someone clicked it twice, prompt an error

        End Try

    End Sub


    Private Sub RandomTesting2_Click(sender As Object, e As EventArgs) Handles RandomTesting2.Click
        If Port2.BytesToRead > 0 Then
            MsgBox(Port2.ReadLine)
        Else
            MsgBox(Port2.ReadExisting)
            MsgBox("a")
        End If
    End Sub
End Class



