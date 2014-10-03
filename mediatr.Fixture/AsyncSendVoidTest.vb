Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit

Public Class AsyncSendVoidTests
    Public Class Ping
        Implements IAsyncRequest
        Public Property Message() As String

    End Class

    Public Class PingHandler
        Inherits AsyncRequestHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Protected Overrides Async Function HandleCore(message As Ping) As Task
            Await _writer.WriteAsync(message.Message & " Pong")
        End Function
    End Class

    <Fact()> Public Sub Should_resolve_main_void_handler()
        Dim builder = New StringBuilder()
        Dim writer = New StringWriter(builder)

        Dim _container As IUnityContainer = AsyncPublishTests.InitContainer()
        _container.RegisterInstance(Of TextWriter)(writer)

        Dim mediator = New Mediator(_container)

        Dim response = mediator.SendAsync(New Ping() With {.Message = "Ping"})

        Task.WaitAll(response)

        Assert.Equal(builder.ToString(), "Ping Pong")
    End Sub
End Class