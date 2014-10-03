Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit


Public Class SendTests
    Public Class Ping
        Implements IRequest(Of Pong)
        Public Property Message() As String
    End Class

    Public Class Pong
        Public Property Message() As String
    End Class

    Public Class PingHandler
        Implements IRequestHandler(Of Ping, Pong)
        Public Function Handle(message As Ping) As Pong Implements IRequestHandler(Of mediatr.Fixture.SendTests.Ping, mediatr.Fixture.SendTests.Pong).Handle
            Return New Pong() With {.Message = message.Message & " Pong"}
        End Function
    End Class

    <Fact()> Public Sub Should_resolve_main_handler()
        Dim _container As IUnityContainer = AsyncPublishTests.InitContainer()

        Dim mediator = New Mediator(_container)

        Dim response = mediator.Send(New Ping() With {.Message = "Ping"})

        Assert.Equal(response.Message, "Ping Pong")
    End Sub
End Class