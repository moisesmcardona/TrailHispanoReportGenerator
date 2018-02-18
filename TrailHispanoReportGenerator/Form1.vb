Imports System.IO
Imports System.Net
Imports System.Text
Imports DSharpPlus
Imports MySql.Data.MySqlClient

Public Class Form1
    Private WithEvents DiscordClient As DiscordClient
    Private DiscordChannelObject As DiscordChannel
    Private WithEvents DiscordClientLogger As DebugLogger
    Private MySQLString As String = String.Empty
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Command As New MySqlCommand With {
            .Connection = New MySqlConnection(MySQLString)
        }
        Command.Parameters.Add("@date", MySqlDbType.VarChar).Value = TextBox1.Text + "%"
        Command.CommandText = "SELECT DISTINCT * FROM posts WHERE traildate LIKE @date AND voted=1 ORDER BY username ASC, link ASC"
        Command.Connection.Open()
        Dim reader As MySqlDataReader = Command.ExecuteReader
        If reader.HasRows = True Then
            Dim LogFile As New StreamWriter("report-" & TextBox1.Text.Replace("/", "-") & ".txt", False)
            Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "M/d/yyyy", Nothing)
            Dim Month As String = TextBoxDate.ToString("MM")
            Dim MonthName As String = ""
            If Month = "01" Then
                MonthName = "enero"
            ElseIf Month = "02" Then
                MonthName = "febrero"
            ElseIf Month = "03" Then
                MonthName = "marzo"
            ElseIf Month = "04" Then
                MonthName = "abril"
            ElseIf Month = "05" Then
                MonthName = "mayo"
            ElseIf Month = "06" Then
                MonthName = "junio"
            ElseIf Month = "07" Then
                MonthName = "julio"
            ElseIf Month = "08" Then
                MonthName = "agosto"
            ElseIf Month = "09" Then
                MonthName = "septiembre"
            ElseIf Month = "10" Then
                MonthName = "octubre"
            ElseIf Month = "11" Then
                MonthName = "noviembre"
            ElseIf Month = "12" Then
                MonthName = "diciembre"
            End If
            Dim Day As String = TextBoxDate.ToString("dddd")
            Dim Day2 As String = TextBoxDate.ToString("dd")
            If Day2 = "01" Then
                Day2 = "1"
            ElseIf Day2 = "02" Then
                Day2 = "2"
            ElseIf Day2 = "03" Then
                Day2 = "3"
            ElseIf Day2 = "04" Then
                Day2 = "4"
            ElseIf Day2 = "05" Then
                Day2 = "5"
            ElseIf Day2 = "06" Then
                Day2 = "6"
            ElseIf Day2 = "07" Then
                Day2 = "7"
            ElseIf Day2 = "08" Then
                Day2 = "8"
            ElseIf Day2 = "09" Then
                Day2 = "9"
            End If
            Dim DayName As String = ""
            If Day.ToLower = "monday" Then
                DayName = "Lunes"
            ElseIf Day.ToLower = "tuesday" Then
                DayName = "Martes"
            ElseIf Day.ToLower = "wednesday" Then
                DayName = "Miércoles"
            ElseIf Day.ToLower = "thursday" Then
                DayName = "Jueves"
            ElseIf Day.ToLower = "friday" Then
                DayName = "Viernes"
            ElseIf Day.ToLower = "saturday" Then
                DayName = "Sábado"
            ElseIf Day.ToLower = "sunday" Then
                DayName = "Domingo"
            End If
            Dim FullDate As String = DayName & ", " & Day2 & " de " & MonthName & " de " & TextBoxDate.ToString("yyyy")
            LogFile.WriteLine("<center>![TrailHispanoV2.png](https://steemitimages.com/DQmUppjZ6J3G5ogZPEmmmAv6TwUkr7A7mULp4jd6Sr7Q3G6/TrailHispanoV2.png)</center>")
            LogFile.WriteLine("En Trail Hispano, queremos apoyar a la comunidad hispana. Es por eso que hemos votado los siguientes posts el " & FullDate.ToLower & ":" & Environment.NewLine & Environment.NewLine)
            LogFile.WriteLine("Usuario | Post")
            LogFile.WriteLine("------- | ----")
            Dim Link As String = ""
            Dim User As String = ""
            Dim Title As String = ""
            While reader.Read
                Link = reader("link")
                User = reader("username")
                Title = GetPostTitle(Link)
                LogFile.WriteLine("@" & User & " | [" & Title.Replace("|", ":").Replace("&", "and") & "](https://steemit.com/tag/@" & Link & ")")
            End While
            LogFile.WriteLine("<center>![](https://steemitimages.com/DQmXit6wguuGaWRXDL3KJQM1Jr6UjRnZDr8n1jfg88vpmAr/image.png)</center>")
            LogFile.WriteLine("<center>¿Quieres que tus posts sean considerados para que el trail lo vote? [Sigue las siguientes instrucciones para unirte al Trail](https://steemit.com/castellano/@trailhispano/participar-trailhispano)</center>")
            LogFile.WriteLine("<center>![](https://steemitimages.com/DQmXit6wguuGaWRXDL3KJQM1Jr6UjRnZDr8n1jfg88vpmAr/image.png)</center>")
            LogFile.WriteLine("<center>Chat en Discord: [https://discord.gg/XqgGQH3](https://discord.gg/XqgGQH3)</center>")
            LogFile.WriteLine("<center>![](https://steemitimages.com/DQmXit6wguuGaWRXDL3KJQM1Jr6UjRnZDr8n1jfg88vpmAr/image.png)</center>")
            LogFile.WriteLine("<center>Otras iniciativas para ayudar a la comunidad hispana a crecer:</center>")
            LogFile.WriteLine("<center>[Click aquí para ver la lista de las inciativas](https://steem.place/Iniciativas)</center>")
            LogFile.WriteLine("<center>![](https://steemitimages.com/DQmXit6wguuGaWRXDL3KJQM1Jr6UjRnZDr8n1jfg88vpmAr/image.png)</center>")
            LogFile.WriteLine("<center>Reporte generado por el software de @moisesmcardona. [Vótalo como Witness Presionando aquí](https://v2.steemconnect.com/sign/account-witness-vote?witness=moisesmcardona&approve=1)</center>" & Environment.NewLine & Environment.NewLine)
            LogFile.Close()
            Threading.Thread.Sleep(5000)
            PublishReport(FullDate)
        Else
            MsgBox("No hay posts para la fecha especificada")
        End If
    End Sub
    Private Sub PublishReport(FullDate As String)
        Dim AccountFile As New StreamReader("account.txt")
        Dim currentline As String = ""
        Dim Account As String = ""
        Dim Key As String = ""
        While AccountFile.EndOfStream = False
            currentline = AccountFile.ReadLine
            If currentline.Contains("account") Then
                Dim GetAccount As String() = currentline.Split("=")
                Account = GetAccount(1)
            ElseIf currentline.Contains("key") Then
                Dim GetKey As String() = currentline.Split("=")
                Key = GetKey(1)
            End If
        End While
        Try
            Dim request As System.Net.WebRequest = System.Net.WebRequest.Create("https://api.steem.place/postToSteem/")
            request.Method = "POST"
            Dim postData As String = "title=Posts votados por Trail Hispano - " + FullDate + "&body=" + HttpUtility.UrlEncode(My.Computer.FileSystem.ReadAllText("report-" & TextBox1.Text.Replace("/", "-") & ".txt")) + "&author=" & Account & "&permlink=reporte-" + TextBox1.Text.Replace("/", "-") + "&tags=trailhispano,spanish,trail,reporte,castellano&pk=" & Key
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            Dim response As Net.WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()
            reader.Close()
            dataStream.Close()
            response.Close()
            If responseFromServer.Contains("ok") Then
                SendMessage(FullDate, TextBox1.Text)
                MessageBox.Show("Report Generated And posted")
            Else
                MessageBox.Show("An error occured while posting the report: " & vbCrLf & vbCrLf & responseFromServer)
            End If
        Catch ex As Exception
            MessageBox.Show("An error has occurred.")
        End Try
    End Sub

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim MySQLFile As StreamReader = New StreamReader("MySQLConfig.txt")
        Dim currentline As String = ""
        Dim MySQLServer As String = ""
        Dim MySQLUser As String = ""
        Dim MySQLPassword As String = ""
        Dim MySQLDatabase As String = ""
        While MySQLFile.EndOfStream = False
            currentline = MySQLFile.ReadLine
            If currentline.Contains("server") Then
                Dim GetServer As String() = currentline.Split("=")
                MySQLServer = GetServer(1)
            ElseIf currentline.Contains("username") Then
                Dim GetUsername As String() = currentline.Split("=")
                MySQLUser = GetUsername(1)
            ElseIf currentline.Contains("password") Then
                Dim GetPassword As String() = currentline.Split("=")
                MySQLPassword = GetPassword(1)
            ElseIf currentline.Contains("database") Then
                Dim GetDatabase As String() = currentline.Split("=")
                MySQLDatabase = GetDatabase(1)
            End If
        End While
        MySQLString = "server=" + MySQLServer + ";user=" + MySQLUser + ";database=" + MySQLDatabase + ";port=3306;password=" + MySQLPassword + ";"
        Dim dcfg As New DiscordConfig
        With dcfg
            .Token = My.Computer.FileSystem.ReadAllText("token.txt")
            .TokenType = TokenType.Bot
            .LogLevel = LogLevel.Debug
            .AutoReconnect = True
        End With
        Me.DiscordClient = New DiscordClient(dcfg)
        Me.DiscordClientLogger = Me.DiscordClient.DebugLogger
        Await Me.DiscordClient.ConnectAsync()
    End Sub
    Private Function GetPostTitle(Link As String)
        Dim GotTitle As Boolean = False
        Dim Title As String = ""
        While GotTitle = False
            Try
                Dim myWebRequest As Net.WebRequest = Net.WebRequest.Create("https://api.steem.place/getPostTitle/?p=" & Link.ToLower)
                Dim myWebResponse As Net.WebResponse = myWebRequest.GetResponse()
                Dim ReceiveStream As Stream = myWebResponse.GetResponseStream()
                Dim encode As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                Dim readStream As New StreamReader(ReceiveStream, encode)
                Title = readStream.ReadLine
                GotTitle = True
            Catch ex As Exception
                GotTitle = False
            End Try
        End While
        Return Title
    End Function
    Private Async Sub SendMessage(fulldate As String, datestring As String)
        Dim Channel As DiscordChannel = Await DiscordClient.GetChannelAsync(368583801154306058)
        Await DiscordClient.SendMessageAsync(Channel, fulldate & ": https://steemit.com/tag/@trailhispano/reporte-" & datestring.Replace("/", "-"))
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If My.Computer.FileSystem.FileExists("report-" & TextBox1.Text.Replace("/", "-") & ".txt") Then
            Dim TextBoxDate As DateTime = DateTime.ParseExact(TextBox1.Text, "M/d/yyyy", Nothing)
            Dim Month As String = TextBoxDate.ToString("MM")
            Dim MonthName As String = ""
            If Month = "01" Then
                MonthName = "enero"
            ElseIf Month = "02" Then
                MonthName = "febrero"
            ElseIf Month = "03" Then
                MonthName = "marzo"
            ElseIf Month = "04" Then
                MonthName = "abril"
            ElseIf Month = "05" Then
                MonthName = "mayo"
            ElseIf Month = "06" Then
                MonthName = "junio"
            ElseIf Month = "07" Then
                MonthName = "julio"
            ElseIf Month = "08" Then
                MonthName = "agosto"
            ElseIf Month = "09" Then
                MonthName = "septiembre"
            ElseIf Month = "10" Then
                MonthName = "octubre"
            ElseIf Month = "11" Then
                MonthName = "noviembre"
            ElseIf Month = "12" Then
                MonthName = "diciembre"
            End If
            Dim Day As String = TextBoxDate.ToString("dddd")
            Dim Day2 As String = TextBoxDate.ToString("dd")
            If Day2 = "01" Then
                Day2 = "1"
            ElseIf Day2 = "02" Then
                Day2 = "2"
            ElseIf Day2 = "03" Then
                Day2 = "3"
            ElseIf Day2 = "04" Then
                Day2 = "4"
            ElseIf Day2 = "05" Then
                Day2 = "5"
            ElseIf Day2 = "06" Then
                Day2 = "6"
            ElseIf Day2 = "07" Then
                Day2 = "7"
            ElseIf Day2 = "08" Then
                Day2 = "8"
            ElseIf Day2 = "09" Then
                Day2 = "9"
            End If
            Dim DayName As String = ""
            If Day.ToLower = "monday" Then
                DayName = "Lunes"
            ElseIf Day.ToLower = "tuesday" Then
                DayName = "Martes"
            ElseIf Day.ToLower = "wednesday" Then
                DayName = "Miércoles"
            ElseIf Day.ToLower = "thursday" Then
                DayName = "Jueves"
            ElseIf Day.ToLower = "friday" Then
                DayName = "Viernes"
            ElseIf Day.ToLower = "saturday" Then
                DayName = "Sábado"
            ElseIf Day.ToLower = "sunday" Then
                DayName = "Domingo"
            End If
            Dim FullDate As String = DayName & ", " & Day2 & " de " & MonthName & " de " & TextBoxDate.ToString("yyyy")
            PublishReport(FullDate)
        Else
            MsgBox("File does not exist")
        End If
    End Sub
End Class

