Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit


Public Class SendVoidTests

    Public Class Ping
        Implements IRequest
        Public Property Message() As String
    End Class

    Public Class PingHandler
        Inherits RequestHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Protected Overrides Sub HandleCore(message As Ping)
            _writer.Write(message.Message & " Pong")
        End Sub
    End Class

    <Fact()> Public Sub Should_resolve_main_void_handler()
        Dim builder = New StringBuilder()
        Dim writer = New StringWriter(builder)

        Dim _container As IUnityContainer = AsyncPublishTests.InitContainer()
        _container.RegisterInstance(Of TextWriter)(writer)

        Dim mediator = New Mediator(_container)

        mediator.Send(New Ping() With {.Message = "Ping"})

        Assert.Equal(builder.ToString, "Ping Pong")
    End Sub
End Class


