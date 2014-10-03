Public Interface IMediator
    Function Send(Of TResponse)(request As IRequest(Of TResponse)) As TResponse
    Function SendAsync(Of TResponse)(request As IAsyncRequest(Of TResponse)) As Task(Of TResponse)
    Sub Publish(Of TNotification As INotification)(notification As TNotification)
    Function PublishAsync(Of TNotification As IAsyncNotification)(notification As TNotification) As Task
End Interface