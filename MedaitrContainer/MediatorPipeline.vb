Public Class MediatorPipeline(Of TRequest As IRequest(Of TResponse), TResponse)
    Implements IRequestHandler(Of TRequest, TResponse)

    Private ReadOnly _inner As IRequestHandler(Of TRequest, TResponse)
    Private ReadOnly _preRequestHandlers As IPreRequestHandler(Of TRequest)()
    Private ReadOnly _postRequestHandlers As IPostRequestHandler(Of TRequest, TResponse)()

    Public Sub New(inner As IRequestHandler(Of TRequest, TResponse), preRequestHandlers As IPreRequestHandler(Of TRequest)(), postRequestHandlers As IPostRequestHandler(Of TRequest, TResponse)())
        _inner = inner
        _preRequestHandlers = preRequestHandlers
        _postRequestHandlers = postRequestHandlers
    End Sub

    Public Function Handle(message As TRequest) As TResponse Implements IRequestHandler(Of TRequest, TResponse).Handle

        For Each preRequestHandler In _preRequestHandlers
            preRequestHandler.Handle(message)
        Next

        Dim result = _inner.Handle(message)

        For Each postRequestHandler In _postRequestHandlers
            postRequestHandler.Handle(message, result)
        Next

        Return result
    End Function
End Class