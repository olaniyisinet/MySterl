Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class iMalWebservice
	Public iMAL As String = ConfigurationManager.AppSettings("iMAL_WEB_SERVICE_URL").ToString()

	Public Function getAccountDetails(ByVal nuban As String) As String
		Dim jsonRequest = New AccountDetailsRequest With {
		.account = nuban,
		.requestCode = "102",
		.principalIdentifier = "66777",
		.referenceCode = "768667888998"
	}
		Dim json = JsonConvert.SerializeObject(jsonRequest)
		Dim responseMessage = String.Empty
		Dim url = iMAL & "AccountDetails"
		Dim apiRequest = CType(WebRequest.Create(url), HttpWebRequest)
		apiRequest.Method = "POST"
		apiRequest.ContentType = "application/json"
		apiRequest.Accept = "application/json"
		Dim data = Encoding.UTF8.GetBytes(json)
		apiRequest.ContentLength = data.Length
		Dim message As String = apiRequest.Method + url + apiRequest.ContentType

		Using stream = apiRequest.GetRequestStream()
			stream.Write(data, 0, data.Length)
		End Using

		Dim response As HttpWebResponse = Nothing

		Try
			response = CType(apiRequest.GetResponse(), HttpWebResponse)

			Using responseStream As Stream = response.GetResponseStream()

				If responseStream IsNot Nothing Then

					Using reader As StreamReader = New StreamReader(responseStream)
						responseMessage = RemoveSpecialCharacters(reader.ReadToEnd()).Trim(""""c)
					End Using
				End If
			End Using

		Catch e As Exception
			responseMessage = "{""errorMessages"":[""" & e.Message.ToString() & """],""errors"":{}}"
		End Try

		Console.WriteLine(responseMessage)
		Return responseMessage
	End Function


	Public Function RemoveSpecialCharacters(ByVal str As String) As String
		Dim sb As StringBuilder = New StringBuilder()

		For Each c As Char In str

			If (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) OrElse c = "/"c OrElse c = """"c OrElse c = ":"c OrElse c = "{"c OrElse c = "}"c OrElse c = ","c OrElse c = " "c OrElse c = "-"c OrElse c = "."c Then
				sb.Append(c)
			End If
		Next

		Return sb.ToString()
	End Function
End Class
