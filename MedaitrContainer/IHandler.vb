Imports System.Threading.Tasks

Public Interface IRequestHandler(Of In TRequest As IRequest(Of TResponse), Out TResponse)
    Function Handle(message As TRequest) As TResponse
End Interface

Public Interface IPreRequestHandler(Of In TRequest)
    Sub Handle(request As TRequest)
End Interface

Public Interface IPostRequestHandler(Of In TRequest, In TResponse)
    Sub Handle(request As TRequest, response As TResponse)
End Interface

Public Interface IAsyncRequestHandler(Of In TRequest As IAsyncRequest(Of TResponse), TResponse)
    Function Handle(message As TRequest) As Task(Of TResponse)
End Interface

Public MustInherit Class RequestHandler(Of TMessage As IRequest)
    Implements IRequestHandler(Of TMessage, Unit)

    Public Function Handle(message As TMessage) As Unit Implements IRequestHandler(Of TMessage, Unit).Handle
        HandleCore(message)

        Return Unit.Value
    End Function

    Protected MustOverride Sub HandleCore(message As TMessage)
End Class

Public MustInherit Class AsyncRequestHandler(Of TMessage As IAsyncRequest)
    Implements IAsyncRequestHandler(Of TMessage, Unit)

    Public Async Function Handle(message As TMessage) As Task(Of Unit) Implements IAsyncRequestHandler(Of TMessage, Unit).Handle
        Await HandleCore(message)

        Return Unit.Value
    End Function

    Protected MustOverride Function HandleCore(message As TMessage) As Task
End Class

Public Interface INotificationHandler(Of In TNotification As INotification)
    Sub Handle(notification As TNotification)
End Interface

Public Interface IAsyncNotificationHandler(Of In TNotification As IAsyncNotification)
    Function Handle(notification As TNotification) As Task
End Interface