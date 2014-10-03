Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit


Public Class PublishTests
    Public Class Ping
        Implements INotification
        Public Property Message() As String

    End Class

    Public Class PongHandler
        Implements INotificationHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Public Sub Handle(message As Ping) Implements INotificationHandler(Of mediatr.Fixture.PublishTests.Ping).Handle
            _writer.WriteLine(message.Message & " Pong")
        End Sub
    End Class

    Public Class PungHandler
        Implements INotificationHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Public Sub Handle(message As Ping) Implements INotificationHandler(Of mediatr.Fixture.PublishTests.Ping).Handle
            _writer.WriteLine(message.Message & " Pung")
        End Sub
    End Class

    <Fact()> Public Sub Should_resolve_main_handler()
        Dim builder = New StringBuilder()
        Dim writer = New StringWriter(builder)

        Dim _container As IUnityContainer = AsyncPublishTests.InitContainer()
        _container.RegisterInstance(Of TextWriter)(writer)

        Dim mediator = New Mediator(_container)

        mediator.Publish(New Ping() With {.Message = "Ping"})

        Dim result = builder.ToString().Split(New Char() {vbCrLf}, StringSplitOptions.None)
        Assert.Contains("Ping Pong", result)
        Assert.Contains("" & vbLf & "Ping Pung", result)
    End Sub
End Class