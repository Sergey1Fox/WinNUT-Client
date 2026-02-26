' WinNUT-Client is a NUT windows client for monitoring your ups hooked up to your favorite linux server.
' Copyright (C) 2019-2021 Gawindx (Decaux Nicolas)
'
' This program is free software: you can redistribute it and/or modify it under the terms of the
' GNU General Public License as published by the Free Software Foundation, either version 3 of the
' License, or any later version.
'
' This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports AGaugeClassic

Namespace Controls

    Friend Class UPSVarGauge
        Inherits AGauge

#Region "Private Fields"

        Private ReadOnly m_value1 As Single
        Private m_value2 As Single
        Private m_value1_prev As Single

        Private ReadOnly m_MinValue As Single = 0
        Private ReadOnly m_MaxValue As Single = 100

        Private ReadOnly m_BaseArcRadius = 45
        Private ReadOnly m_BaseArcStart = 135
        Private ReadOnly m_BaseArcSweep = 270
        Private ReadOnly m_BaseArcWidth = 5

        ' Private ReadOnly m_ScaleLinesMinorTicks = 9
        Private ReadOnly m_ScaleLinesMinorInnerRadius = 42
        Private ReadOnly m_ScaleLinesMinorOuterRadius = 48
        Private ReadOnly m_ScaleLinesMinorWidth = 1

        Private ReadOnly m_ScaleLinesInterInnerRadius = 40
        Private ReadOnly m_ScaleLinesInterOuterRadius = 48
        Private ReadOnly m_ScaleLinesInterWidth = 1

        ' Private ReadOnly m_ScaleLinesMajorStepValue = 50.0F
        Private ReadOnly m_ScaleLinesMajorInnerRadius = 40
        Private ReadOnly m_ScaleLinesMajorOuterRadius = 48
        Private ReadOnly m_ScaleLinesMajorWidth = 2

        Private ReadOnly m_ScaleNumbersRadius = 60
        Private ReadOnly m_ScaleNumbersFormat As String
        Private ReadOnly m_ScaleNumbersStartScaleLine As Integer
        Private ReadOnly m_ScaleNumbersStepScaleLines = 1
        Private ReadOnly m_ScaleNumbersRotation As Integer

        Private ReadOnly m_NeedleType As NeedleType
        Private ReadOnly m_NeedleRadius = 32
        Private ReadOnly m_NeedleColor1 = AGaugeNeedleColor.Gray
        Private ReadOnly m_NeedleColor2 = Color.DimGray
        Private ReadOnly m_NeedleWidth = 2
        Private m_ShowPreviousValue As Boolean = False

        Private m_gradientType = GradientTypeEnum.RedGreen
        Private m_gradientOrientation = GradientOrientationEnum.BottomToTop
        Private m_unitvalue1 = UnitValueEnum.Volts
        Private m_unitvalue2 = UnitValueEnum.None

#End Region

#Region "Properties"

        ' Map Value1 onto Value
        <Browsable(True),
                Category("AGauge"),
                Description("First value to display.")>
        Public Property Value1 As Single
            Get
                Return Value
            End Get
            Set(value As Single)
                If MyBase.Value > MyBase.MaxValue Then
                    m_value1_prev = MyBase.MaxValue
                ElseIf MyBase.Value < MyBase.MinValue Then
                    m_value1_prev = MyBase.MinValue
                Else
                    m_value1_prev = MyBase.Value
                End If
                MyBase.Value = value
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("Second value to display.")>
        Public Property Value2 As Single
            Get
                Return m_value2
            End Get
            Set(value As Single)
                If m_value2 <> value Then
                    m_value2 = value
                    OnValueChanged(Me, Nothing)
                    Refresh()
                End If
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("UseColor For Arc Base Color.")>
        Public Property GradientType As GradientTypeEnum
            Get
                Return m_gradientType
            End Get
            Set(value As GradientTypeEnum)
                m_gradientType = value
                Refresh()
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("Orientation Of Gradient Colors.")>
        Public Property GradientOrientation As GradientOrientationEnum
            Get
                Return m_gradientOrientation
            End Get
            Set(value As GradientOrientationEnum)

                If m_gradientOrientation <> value Then
                    m_gradientOrientation = value
                    Refresh()
                End If
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("Units For Value 1")>
        Public Property UnitValue1 As UnitValueEnum
            Get
                Return m_unitvalue1
            End Get
            Set(value As UnitValueEnum)

                If m_unitvalue1 <> value Then
                    m_unitvalue1 = value
                    Refresh()
                End If
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("UseColor For Arc Base Color.")>
        Public Property UnitValue2 As UnitValueEnum
            Get
                Return m_unitvalue2
            End Get
            Set(value As UnitValueEnum)

                If m_unitvalue2 <> value Then
                    m_unitvalue2 = value
                    Refresh()
                End If
            End Set
        End Property

        <Browsable(True),
                Category("AGauge"),
                Description("Show previous value.")>
        Public Property ShowPreviousValue As Boolean
            Get
                Return m_ShowPreviousValue
            End Get
            Set(value As Boolean)
                m_ShowPreviousValue = value
                Refresh()
            End Set
        End Property

#End Region

        Public Enum GradientTypeEnum
            None
            RedGreen
        End Enum

        Public Enum GradientOrientationEnum
            TopToBottom
            BottomToTop
            RightToLeft
            LeftToRight
        End Enum

        Public Enum UnitValueEnum
            None
            Hertz
            Percent
            Volts
            Watts
            TemperatureC
            TemperatureF
        End Enum

        Public Sub New()
            MyBase.New()
            InitializeComponent()

            Size = New Size(148, 130)
        End Sub

        Overrides Sub RenderDefaultArc(graphics As Graphics)
            If m_BaseArcRadius > 0 Then
                Dim baseArcRadius As Integer = m_BaseArcRadius * centerFactor

                If m_gradientType = GradientTypeEnum.None Then
                    Using pnArc = New Pen(BaseArcColor, m_BaseArcWidth * centerFactor)
                        graphics.DrawArc(pnArc, New Rectangle(Center.X - baseArcRadius,
                                                                  Center.Y - baseArcRadius,
                                                                  2 * baseArcRadius,
                                                                  2 * baseArcRadius),
                                             m_BaseArcStart, m_BaseArcSweep)
                    End Using

                Else
                    Dim GradientP1Brush = New Point(0, (Center.X + baseArcRadius + m_BaseArcWidth + 2))
                    Dim GradientP2Brush = New Point(0, (Center.X - baseArcRadius - m_BaseArcWidth - 2))

                    Select Case m_gradientOrientation
                        Case GradientOrientationEnum.TopToBottom
                            GradientP1Brush = New Point(0, (Center.Y - baseArcRadius - m_BaseArcWidth - 2))
                            GradientP2Brush = New Point(0, (Center.Y + baseArcRadius + m_BaseArcWidth + 2))
                        Case GradientOrientationEnum.BottomToTop
                            GradientP1Brush = New Point(0, (Center.Y + baseArcRadius + m_BaseArcWidth + 2))
                            GradientP2Brush = New Point(0, (Center.Y - baseArcRadius - m_BaseArcWidth - 2))
                        Case GradientOrientationEnum.RightToLeft
                            GradientP1Brush = New Point((Center.X + baseArcRadius + m_BaseArcWidth + 2), 0)
                            GradientP2Brush = New Point((Center.X - baseArcRadius - m_BaseArcWidth - 2), 0)
                        Case GradientOrientationEnum.LeftToRight
                            GradientP1Brush = New Point((Center.X - baseArcRadius - m_BaseArcWidth - 2), 0)
                            GradientP2Brush = New Point((Center.X + baseArcRadius + m_BaseArcWidth + 2), 0)
                    End Select

                    Dim myArc1Gradient = New LinearGradientBrush(GradientP1Brush, GradientP2Brush, Color.Red, Color.Green)
                    Using pnArc = New Pen(myArc1Gradient, m_BaseArcWidth * centerFactor)
                        graphics.DrawArc(pnArc, New Rectangle(Center.X - baseArcRadius,
                                                                  Center.Y - baseArcRadius,
                                                                  2 * baseArcRadius,
                                                                  2 * baseArcRadius),
                                             m_BaseArcStart, m_BaseArcSweep)
                    End Using
                End If
            End If
        End Sub

        ''' <summary>
        ''' Override PostRender and render the value of the gauge with unit.
        ''' </summary>
        Overrides Sub PostRender(graphics As Graphics)
            Dim PenString = New Pen(Color.Black)
            Dim PenFontV1 = New Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            Dim PenFontV2 = New Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            Dim StringPen = New SolidBrush(Color.Black)
            Dim LineHeight = 15
            Dim StrPos = Center
            StrPos.Y += 5

            If UnitValue1 <> UnitValueEnum.None Then
                Dim StringToDraw = ApplyUnit(Value1, UnitValue1)
                Dim StringSize = TextRenderer.MeasureText(StringToDraw, PenFontV1)
                StrPos.Y += LineHeight
                graphics.DrawString(StringToDraw, PenFontV1, StringPen,
                                        New PointF((StrPos.X - (StringSize.Width / 2) + 5), StrPos.Y))
            End If

            If UnitValue2 <> UnitValueEnum.None Then
                Dim StringToDraw = ApplyUnit(Value2, UnitValue2)
                Dim StringSize = TextRenderer.MeasureText(StringToDraw, PenFontV2)
                StrPos.Y += LineHeight
                graphics.DrawString(StringToDraw, PenFontV2, StringPen,
                                        New PointF((StrPos.X - (StringSize.Width / 2) + 7), StrPos.Y))
            End If

            If m_ShowPreviousValue Then
                Dim offset As Double = 0
                Dim coord_start As Integer
                Dim coord_end As Integer

                Dim num As Double = Int((m_BaseArcStart + (m_value1_prev - MinValue) * m_BaseArcSweep / ValueRange) Mod 360)
                If num < 0F Then
                    num += 360.0
                End If

                Dim num2 As Double = num * Math.PI / 180.0
                Dim num3 As Integer = m_NeedleWidth * centerFactor
                Dim num4 As Integer = m_NeedleRadius * centerFactor

                Dim point1_prev = New Point(Int(Center.X + (num4 / 3) * Math.Cos(num2)), Int(Center.Y + (num4 / 3) * Math.Sin(num2)))
                Dim point2_prev = New Point(Int(Center.X + num4 * Math.Cos(num2)), Int(Center.Y + num4 * Math.Sin(num2)))

                Select Case m_gradientOrientation
                    Case GradientOrientationEnum.TopToBottom
                        coord_start = Center.Y - BaseArcRadius
                        coord_end = Center.Y + BaseArcRadius
                        offset = (point2_prev.Y - coord_start) / (coord_end - coord_start)
                    Case GradientOrientationEnum.BottomToTop
                        coord_start = Center.Y + BaseArcRadius
                        coord_end = Center.Y - BaseArcRadius
                        offset = (coord_start - point2_prev.Y) / (coord_start - coord_end)
                    Case GradientOrientationEnum.RightToLeft
                        coord_start = Center.X + BaseArcRadius
                        coord_end = Center.X - BaseArcRadius
                        offset = (coord_start - point2_prev.X) / (coord_start - coord_end)
                    Case GradientOrientationEnum.LeftToRight
                        coord_start = Center.X - BaseArcRadius
                        coord_end = Center.X + BaseArcRadius
                        offset = (point2_prev.X - coord_start) / (coord_end - coord_start)
                End Select

                Dim pen_prev = New Pen(Color.FromArgb(Int(255 * (1.0 - offset)), Int(255 * offset), 0), num3)
                graphics.DrawLine(pen_prev, point1_prev.X, point1_prev.Y, point2_prev.X, point2_prev.Y)
                'graphics.DrawLine(pen_prev, Center.X, Center.Y, point1_prev.X, point1_prev.Y)
            End If
        End Sub

        Private Function ApplyUnit(value As String, unit As UnitValueEnum)
            Dim returnStr = value

            Select Case unit
                Case UnitValueEnum.Hertz
                    returnStr &= " Hz"
                Case UnitValueEnum.Percent
                    returnStr &= " %"
                Case UnitValueEnum.Volts
                    returnStr &= " V"
                Case UnitValueEnum.Watts
                    returnStr &= " W"
                Case UnitValueEnum.TemperatureC
                    returnStr &= " °C"
                Case UnitValueEnum.TemperatureF
                    returnStr &= " F"
            End Select

            Return returnStr
        End Function

    End Class

End Namespace
