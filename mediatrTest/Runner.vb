Imports System.IO
Imports System.Threading.Tasks
Imports MedaitrContainer

Public NotInheritable Class Runner
    Private Sub New()
    End Sub
    Public Shared Sub Run(mediator As IMediator, writer As TextWriter)

        writer.WriteLine("Sample mediator implementation using send, publish and post-request handlers in sync and async version.")
        writer.WriteLine("---------------")

        writer.WriteLine("Sending Custom Pong...")
        Dim customPong = mediator.Send(New Pong() With {.Message = "Table Miss"})
        writer.WriteLine("Received: {0}", customPong.Message)

        writer.WriteLine("Sending Ping...")
        Dim pong = mediator.Send(New Ping() With {.Message = "Ping"})
        writer.WriteLine("Received: " & Convert.ToString(pong.Message))

        writer.WriteLine("Sending Ping async...")
        Dim response = mediator.SendAsync(New PingAsync() With {.Message = "Ping"})
        Task.WaitAll(response)
        writer.WriteLine("Received async: " & Convert.ToString(response.Result.Message))

        writer.WriteLine("Publishing Pinged...")
        mediator.Publish(New Pinged())

        writer.WriteLine("Publishing Pinged async...")
        Dim publishResponse = mediator.PublishAsync(New PingedAsync())
        Task.WaitAll(publishResponse)

    End Sub
End Class

Public Class Ping
    Implements IRequest(Of Pong)

    Public Property Message() As String

End Class

Public Class Pong
    Implements IRequest(Of Ping)

    Public Property Message() As String

End Class

Public Class PingAsync
    Implements IAsyncRequest(Of Pong)

    Public Property Message() As String

End Class

Public Class Pinged
    Implements INotification

End Class

Public Class PingedAsync
    Implements IAsyncNotification

End Class

Public Class PingHandler
    Implements IRequestHandler(Of Ping, Pong)

    Public Function Handle(message As Ping) As Pong Implements IRequestHandler(Of Ping, Pong).Handle
        Return New Pong() With {.Message = Convert.ToString(message.Message) & " Pong"}
    End Function

End Class

Public Class PongHandler
    Implements IRequestHandler(Of Pong, Ping)

    Public Function Handle(message As Pong) As Ping Implements IRequestHandler(Of Pong, Ping).Handle
        Return New Ping() With {.Message = Convert.ToString(message.Message) & " Ping"}
    End Function

End Class

Public Class PingedHandler
    Implements INotificationHandler(Of Pinged)

    Private ReadOnly _writer As TextWriter

    Public Sub New(writer As TextWriter)
        _writer = writer
    End Sub

    Public Sub Handle(notification As Pinged) Implements INotificationHandler(Of Pinged).Handle
        _writer.WriteLine("Got pinged.")
    End Sub

End Class

Public Class PingedDubHandler
    Implements INotificationHandler(Of Pinged)

    Private ReadOnly _writer As TextWriter

    Public Sub New(writer As TextWriter)
        _writer = writer
    End Sub

    Public Sub Handle(notification As Pinged) Implements INotificationHandler(Of Pinged).Handle
        _writer.WriteLine("Got pinged Dup.")
    End Sub

End Class


Public Class PingAsyncHandler
    Implements IAsyncRequestHandler(Of PingAsync, Pong)

    Public Async Function Handle(message As PingAsync) As Task(Of Pong) Implements IAsyncRequestHandler(Of PingAsync, Pong).Handle
        Return Await Task.Factory.StartNew(Function() New Pong() With {.Message = message.Message + " Pong"})
    End Function

End Class

Public Class PingedAsyncHandler
    Implements IAsyncNotificationHandler(Of PingedAsync)

    Private ReadOnly _writer As TextWriter

    Public Sub New(writer As TextWriter)
        _writer = writer
    End Sub

    Public Async Function Handle(notification As PingedAsync) As Task Implements IAsyncNotificationHandler(Of PingedAsync).Handle
        Await _writer.WriteLineAsync("Got pinged async.")
    End Function

End Class

Public Class GenericHandler
    Implements INotificationHandler(Of INotification)

    Private ReadOnly _writer As TextWriter

    Public Sub New(writer As TextWriter)
        _writer = writer
    End Sub

    Public Sub Handle(notification As INotification) Implements INotificationHandler(Of INotification).Handle
        _writer.WriteLine("Got notified.")
    End Sub

End Class

