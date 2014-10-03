Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit


Public Class AsyncSendTests

    Public Class Ping
        Implements IAsyncRequest(Of Pong)
        Public Property Message() As String
    End Class

    Public Class Pong
        Public Property Message() As String
    End Class

    Public Class PingHandler
        Implements IAsyncRequestHandler(Of Ping, Pong)
        Public Async Function Handle(message As Ping) As Task(Of Pong) Implements IAsyncRequestHandler(Of mediatr.Fixture.AsyncSendTests.Ping, mediatr.Fixture.AsyncSendTests.Pong).Handle
            Return Await Task.Factory.StartNew(Function() New Pong() With {.Message = message.Message & " Pong"})
        End Function
    End Class

    <Fact()> Public Sub Should_resolve_main_handler()
        Dim _container As IUnityContainer = AsyncPublishTests.InitContainer()

        Dim mediator = New Mediator(_container)

        Dim response = mediator.SendAsync(New Ping() With {.Message = "Ping"})

        Task.WaitAll(response)

        Assert.Equal(response.Result.Message, "Ping Pong")
    End Sub

End Class