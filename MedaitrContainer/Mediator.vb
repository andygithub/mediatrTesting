Imports System.Threading.Tasks
Imports Microsoft.Practices.Unity

'Translated from here - https://github.com/jbogard/MediatR 

Public Class Mediator
    Implements IMediator

    Private ReadOnly _container As IunityContainer

    Public Sub New(container As IUnityContainer)
        _container = container
    End Sub

    Public Function Send(Of TResponse)(request As IRequest(Of TResponse)) As TResponse Implements IMediator.Send
        Dim defaultHandler = GetHandler(request)

        Dim result As TResponse = defaultHandler.Handle(request)

        Return result
    End Function

    Public Async Function SendAsync(Of TResponse)(request As IAsyncRequest(Of TResponse)) As Task(Of TResponse) Implements IMediator.SendAsync
        Dim defaultHandler = GetHandler(request)

        Dim result As TResponse = Await defaultHandler.Handle(request)

        Return result
    End Function

    Public Sub Publish(Of TNotification As INotification)(notification As TNotification) Implements IMediator.Publish
        Dim notificationHandlers = GetNotificationHandlers(Of TNotification)()

        For Each handler In notificationHandlers
            handler.Handle(notification)
        Next

        'base interface notifications
        notificationHandlers = GetNotificationHandlers(Of INotification)()

        For Each handler In notificationHandlers
            handler.Handle(notification)
        Next
    End Sub

    Public Async Function PublishAsync(Of TNotification As IAsyncNotification)(notification As TNotification) As Task Implements IMediator.PublishAsync
        Dim notificationHandlers = GetAsyncNotificationHandlers(Of TNotification)()

        For Each handler In notificationHandlers
            Await handler.Handle(notification)
        Next

        'base interface notifications
        notificationHandlers = GetAsyncNotificationHandlers(Of IAsyncNotification)()

        For Each handler In notificationHandlers
            Await handler.Handle(notification)
        Next
    End Function

    Private Shared Function BuildException(message As Object) As InvalidOperationException
        Return New InvalidOperationException("Handler was not found for request of type " & Convert.ToString(message.[GetType]()) & "." & vbCr & vbLf & "Container or service locator not configured properly or handlers not registered with your container.")
    End Function

    Private Function GetHandler(Of TResponse)(request As IRequest(Of TResponse)) As RequestHandler(Of TResponse)
        Dim handlerType = GetType(IRequestHandler(Of ,)).MakeGenericType(request.[GetType](), GetType(TResponse))
        Dim wrapperType = GetType(RequestHandler(Of ,)).MakeGenericType(request.[GetType](), GetType(TResponse))
        Dim handler = _container.Resolve(handlerType)

        If handler Is Nothing Then
            Throw BuildException(request)
        End If

        Dim wrapperHandler = Activator.CreateInstance(wrapperType, handler)
        Return DirectCast(wrapperHandler, RequestHandler(Of TResponse))
    End Function

    Private Function GetHandler(Of TResponse)(request As IAsyncRequest(Of TResponse)) As AsyncRequestHandler(Of TResponse)
        Dim handlerType = GetType(IAsyncRequestHandler(Of ,)).MakeGenericType(request.[GetType](), GetType(TResponse))
        Dim wrapperType = GetType(AsyncRequestHandler(Of ,)).MakeGenericType(request.[GetType](), GetType(TResponse))
        Dim handler = _container.Resolve(handlerType)

        If handler Is Nothing Then
            Throw BuildException(request)
        End If

        Dim wrapperHandler = Activator.CreateInstance(wrapperType, handler)
        Return DirectCast(wrapperHandler, AsyncRequestHandler(Of TResponse))
    End Function

    Private Function GetNotificationHandlers(Of TNotification As INotification)() As IEnumerable(Of INotificationHandler(Of TNotification))
        Return _container.ResolveAll(Of INotificationHandler(Of TNotification))()
    End Function

    Private Function GetAsyncNotificationHandlers(Of TNotification As IAsyncNotification)() As IEnumerable(Of IAsyncNotificationHandler(Of TNotification))
        Return _container.ResolveAll(Of IAsyncNotificationHandler(Of TNotification))()
    End Function

    Private MustInherit Class RequestHandler(Of TResult)
        Public MustOverride Function Handle(message As IRequest(Of TResult)) As TResult
    End Class

    Private Class RequestHandler(Of TCommand As IRequest(Of TResult), TResult)
        Inherits RequestHandler(Of TResult)
        Private ReadOnly _inner As IRequestHandler(Of TCommand, TResult)

        Public Sub New(inner As IRequestHandler(Of TCommand, TResult))
            _inner = inner
        End Sub

        Public Overrides Function Handle(message As IRequest(Of TResult)) As TResult
            Return _inner.Handle(DirectCast(message, TCommand))
        End Function
    End Class

    Private MustInherit Class AsyncRequestHandler(Of TResult)
        Public MustOverride Overloads Function Handle(message As IAsyncRequest(Of TResult)) As Task(Of TResult)
    End Class

    Private Class AsyncRequestHandler(Of TCommand As IAsyncRequest(Of TResult), TResult)
        Inherits AsyncRequestHandler(Of TResult)
        Private ReadOnly _inner As IAsyncRequestHandler(Of TCommand, TResult)

        Public Sub New(inner As IAsyncRequestHandler(Of TCommand, TResult))
            _inner = inner
        End Sub

        Public Overrides Function Handle(message As IAsyncRequest(Of TResult)) As Task(Of TResult)
            Return _inner.Handle(DirectCast(message, TCommand))
        End Function
    End Class
End Class