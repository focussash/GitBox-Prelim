Public Class PHsensor

    Public Property Name As String
    Public Property Reading As Double
    Public Property PH As Double
    Public Property PlotStatus As Integer
    Public Property Timer As Integer
    Public Property AutoControlStatus As Integer
    Public Property PH_Setpoint As Double
    Public Property ExportStatus As Integer 'Not in use yet, will be used to determine whether or not a sensor's reading is exported'
    Public Property Slope As Integer 'Slope of PH sensor calibration
    Public Property Intercept As Integer 'Intercept of PH sensor calibration

End Class
