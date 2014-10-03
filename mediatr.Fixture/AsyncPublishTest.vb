Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit


Public Class AsyncPublishTests
    Public Class Ping
        Implements IAsyncNotification
        Public Property Message() As String
    End Class

    Public Class PongHandler
        Implements IAsyncNotificationHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Public Async Function Handle(message As Ping) As Task Implements IAsyncNotificationHandler(Of AsyncPublishTests.Ping).Handle
            Await _writer.WriteLineAsync(message.Message & " Pong")
        End Function
    End Class

    Public Class PungHandler
        Implements IAsyncNotificationHandler(Of Ping)
        Private ReadOnly _writer As TextWriter

        Public Sub New(writer As TextWriter)
            _writer = writer
        End Sub

        Public Async Function Handle(message As Ping) As Task Implements IAsyncNotificationHandler(Of AsyncPublishTests.Ping).Handle
            Await _writer.WriteLineAsync(message.Message & " Pung")
        End Function
    End Class

    <Fact()>
    Public Sub Should_resolve_main_handler()
        Dim builder = New StringBuilder()
        Dim writer = New StringWriter(builder)
        Dim _container As IUnityContainer = InitContainer()
        _container.RegisterInstance(Of TextWriter)(writer)

        Dim mediator = New Mediator(_container)

        Dim response = mediator.PublishAsync(New Ping() With {.Message = "Ping"})

        Task.WaitAll(response)

        Dim result = builder.ToString().Split(New Char() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Assert.Contains("Ping Pong", result)
        Assert.Contains("" & vbLf & "Ping Pung", result)
    End Sub

    Public Shared Function InitContainer() As IUnityContainer
        Dim _container As New UnityContainer
        _container.RegisterTypes(AllClasses.FromAssemblies(GetType(Ping).Assembly).Where(Function(x) Not x.Name.StartsWith("ExceptionTests")).Where(Function(x)
                                                                                                                                                        Dim llll = x.Name
                                                                                                                                                        Return Not x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                                                                                    End Function),
                                Function(x) WithMappings.FromAllInterfaces(x)
                                    )
        _container.RegisterTypes(AllClasses.FromAssemblies(GetType(Ping).Assembly).Where(Function(x)
                                                                                             Return x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                         End Function),
                        Function(x) WithMappings.FromAllInterfaces(x),
                        Function(x) WithName.TypeName(x)
                            )
        Return _container
    End Function
End Class
