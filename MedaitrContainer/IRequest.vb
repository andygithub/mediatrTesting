Public Interface IRequest
    Inherits IRequest(Of Unit)
End Interface

Public Interface IAsyncRequest
    Inherits IAsyncRequest(Of Unit)
End Interface

Public Interface IRequest(Of Out TResponse)
End Interface

Public Interface IAsyncRequest(Of Out TResponse)
End Interface

Public Interface INotification
End Interface

Public Interface IAsyncNotification
End Interface