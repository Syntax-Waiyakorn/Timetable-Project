﻿Imports System.Data.OleDb
Public Class TeachersTimetables
    Public Property Pass As String
    Dim conn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Application.StartupPath & "\Timetable.accdb")
    Dim dr As OleDbDataReader
    Sub LoadCbo()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim cmd As New OleDb.OleDbCommand("Select TeacherFirstName from Teachers", conn)
            cboTeachers.Items.Clear()
            dr = cmd.ExecuteReader
            While dr.Read
                cboTeachers.Items.Add(dr.GetString(0))
            End While

            Dim cmd2 As New OleDb.OleDbCommand("Select YearNumber from Years", conn)
            dr = cmd2.ExecuteReader
            While dr.Read
                txtYear.Text = CStr(dr.Item("YearNumber"))
            End While

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Sub status()
        If cboTeachers.SelectedIndex = -1 Then
            Connection_status.Text = "กรุณาเลือกครู"
            lblCurrentDay.Text = "ว่าง"
            lblCurrentPeriod.Text = "ว่าง"
            Connection_status.ForeColor = Color.Red
            lblCurrentDay.ForeColor = Color.Red
            lblCurrentPeriod.ForeColor = Color.Red
        Else
            Connection_status.Text = "แสดงข้อมูล"
            Connection_status.ForeColor = Color.Lime
        End If
    End Sub
    Sub search(Table As String, Field1 As String, txtBox As TextBox, cboBox As ComboBox)
        Try
            cboBox.Items.Clear()
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim cmd1 As New OleDb.OleDbCommand("Select " & Field1 & " from " & Table & " WHERE " & Field1 & " like '%" & txtBox.Text & "%'", conn)
            dr = cmd1.ExecuteReader
            While dr.Read
                cboBox.Items.Add(dr.Item(Field1))
            End While
            dr.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Sub InitializeTable()
        Dim lblDays() As Label = {lblDay1, lblDay2, lblDay3, lblDay4, lblDay5}
        Dim lblPeriods() As Label = {lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7, lbl8, lbl9, lbl10, lbl11}
        Dim lblTimes() As Label = {lblTime1, lblTime2, lblTime3, lblTime4, lblTime5, lblTime6, lblTime7, lblTime8, lblTime9, lblTime10, lblTime11}
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            For i As Integer = 1 To 5
                Dim cmd As New OleDb.OleDbCommand("SELECT DayName FROM Days WHERE DayNumber = @DayNumber", conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@DayNumber", i)
                dr = cmd.ExecuteReader
                While dr.Read
                    lblDays(i - 1).Text = CStr(dr.Item("DayName"))
                End While
            Next
            For i As Integer = 1 To 11
                Dim cmd As New OleDb.OleDbCommand("Select PeriodNumber, PeriodName, PeriodTimeStart, PeriodTimeEnd from Periods WHERE PeriodNumber = @PeriodNumber", conn)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@PeriodNumber", i)
                dr = cmd.ExecuteReader
                While dr.Read
                    lblPeriods(i - 1).Text = CStr(dr.Item("PeriodName"))
                    lblTimes(i - 1).Text = CStr(dr.Item("PeriodTimeStart")) + "-" + CStr(dr.Item("PeriodTimeEnd"))
                End While
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Sub DisplayTeacherTable(Teacher As String)
        Dim lblDaysPeriods() As Label = {lblD1P1, lblD1P2, lblD1P3, lblD1P4, lblD1P5, lblD1P6, lblD1P7, lblD1P8, lblD1P9, lblD1P10, lblD1P11,
                                         lblD2P1, lblD2P2, lblD2P3, lblD2P4, lblD2P5, lblD2P6, lblD2P7, lblD2P8, lblD2P9, lblD2P10, lblD2P11,
                                         lblD3P1, lblD3P2, lblD3P3, lblD3P4, lblD3P5, lblD3P6, lblD3P7, lblD3P8, lblD3P9, lblD3P10, lblD3P11,
                                         lblD4P1, lblD4P2, lblD4P3, lblD4P4, lblD4P5, lblD4P6, lblD4P7, lblD4P8, lblD4P9, lblD4P10, lblD4P11,
                                         lblD5P1, lblD5P2, lblD5P3, lblD5P4, lblD5P5, lblD5P6, lblD5P7, lblD5P8, lblD5P9, lblD5P10, lblD5P11}
        For Each PLabel As Label In lblDaysPeriods
            DisplayPeriod(Teacher, PLabel)
        Next
    End Sub
    Sub DisplayPeriod(Teacher As String, PLabel As Label)
        If PLabel.Text = "ว่าง" Then
            Try
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If
                Dim PLabelName As String = PLabel.Name
                Dim PDay As String = PLabelName.Chars(4)
                Dim PPeriod As String
                Try
                    PPeriod = PLabelName.Chars(6) & PLabelName.Chars(7)
                Catch
                    PPeriod = PLabelName.Chars(6)
                End Try
                Dim TeacherIndex As String = CStr(Teacher) & "$$" & PDay & "$$" & PPeriod
                Dim cmd As New OleDb.OleDbCommand("SELECT SubjectCode, ClassroomName, ClassroomCode FROM TimetablesQuery WHERE TeacherIndex = '" + TeacherIndex + "'", conn)
                dr = cmd.ExecuteReader
                While dr.Read
                    If PLabel.Text = "ว่าง" Then
                        PLabel.Text = CStr(dr.Item("SubjectCode")) & vbCrLf & CStr(dr.Item("ClassroomName")) & vbCrLf & CStr(dr.Item("ClassroomCode"))
                    Else
                        PLabel.Text = "ว่าง"
                    End If
                End While
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try
        Else
            PLabel.Text = "ว่าง"
        End If
    End Sub
    Sub Period_Click(sender As Label, e As EventArgs) Handles lblD1P1.Click, lblD1P2.Click, lblD1P3.Click, lblD1P4.Click, lblD1P5.Click, lblD1P6.Click, lblD1P7.Click, lblD1P8.Click, lblD1P9.Click, lblD1P10.Click, lblD1P11.Click,
                                                              lblD2P1.Click, lblD2P2.Click, lblD2P3.Click, lblD2P4.Click, lblD2P5.Click, lblD2P6.Click, lblD2P7.Click, lblD2P8.Click, lblD2P9.Click, lblD2P10.Click, lblD2P11.Click,
                                                              lblD3P1.Click, lblD3P2.Click, lblD3P3.Click, lblD3P4.Click, lblD3P5.Click, lblD3P6.Click, lblD3P7.Click, lblD3P8.Click, lblD3P9.Click, lblD3P10.Click, lblD3P11.Click,
                                                              lblD4P1.Click, lblD4P2.Click, lblD4P3.Click, lblD4P4.Click, lblD4P5.Click, lblD4P6.Click, lblD4P7.Click, lblD4P8.Click, lblD4P9.Click, lblD4P10.Click, lblD4P11.Click,
                                                              lblD5P1.Click, lblD5P2.Click, lblD5P3.Click, lblD5P4.Click, lblD5P5.Click, lblD5P6.Click, lblD5P7.Click, lblD5P8.Click, lblD5P9.Click, lblD5P10.Click, lblD5P11.Click
        If Not cboTeachers.SelectedIndex = -1 Then
            Try
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If
                Dim PLabelNameC As String = sender.Name
                Dim PDayC As String = PLabelNameC.Chars(4)
                Dim PPeriodC As String
                Try
                    PPeriodC = PLabelNameC.Chars(6) & PLabelNameC.Chars(7)
                Catch
                    PPeriodC = PLabelNameC.Chars(6)
                End Try
                lblCurrentDay.Tag = PDayC
                lblCurrentPeriod.Tag = PPeriodC
                Dim cmd1 As New OleDb.OleDbCommand("Select TOP 1 DayNumber, DayName from Days WHERE `DayNumber` like '%" & PDayC & "%' ", conn)
                Dim cmd2 As New OleDb.OleDbCommand("Select TOP 1 PeriodNumber, PeriodName  from Periods WHERE `PeriodNumber` like '%" & PPeriodC & "%' ", conn)
                dr = cmd1.ExecuteReader
                While dr.Read
                    lblCurrentDay.Text = dr.Item("DayName")
                    lblCurrentDay.ForeColor = Color.Lime
                End While
                dr = cmd2.ExecuteReader
                While dr.Read
                    lblCurrentPeriod.Text = dr.Item("PeriodName")
                    lblCurrentPeriod.ForeColor = Color.Lime
                End While
            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try
        Else
            MsgBox("เลือกครู", vbYes, "เเจ้งเตืน")
        End If
    End Sub

    Private Sub cboClassrooms_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTeachers.SelectedIndexChanged
        Dim Teacher As String = cboTeachers.Text
        DisplayTeacherTable(Teacher)
        status()
    End Sub
    Private Sub cboTeachersSubjects_SelectedIndexChanged(sender As Object, e As EventArgs)
        Me.agent.Focus()
        status()
    End Sub
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        search("Teachers", "TeacherFirstName", txtSearch, cboTeachers)
        status()
    End Sub
    Private Sub StudentTimetables_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeTable()
        txtYear.Text = Pass
        status()
    End Sub
    Private Sub StudentTimetables_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        LoadCbo()
        status()
    End Sub
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim TeachersTimetablesFrom As New TeachersTimetablesFrom
        TeachersTimetablesFrom.Show()
    End Sub
End Class