Imports MedaitrContainer
Imports System.IO
Imports System.Text
Imports Microsoft.Practices.Unity
Imports Xunit

Namespace Exception

    Public Class ExceptionTests
        Private ReadOnly _mediator As Mediator

        Public Class ExceptionTestsPing
            Implements IRequest(Of ExceptionTestsPong)
        End Class
        Public Class ExceptionTestsPong
        End Class
        Public Class ExceptionTestsVoidPing
            Implements IRequest
        End Class
        Public Class ExceptionTestsPinged
            Implements INotification
        End Class
        Public Class ExceptionTestsAsyncPing
            Implements IAsyncRequest(Of ExceptionTestsPong)
        End Class
        Public Class ExceptionTestsAsyncVoidPing
            Implements IAsyncRequest
        End Class
        Public Class ExceptionTestsAsyncPinged
            Implements IAsyncNotification
        End Class

        Public Sub New()
            'Dim container = New Container()
            'Dim serviceLocator = New StructureMapServiceLocator(container)
            'Dim serviceLocatorProvider = New ServiceLocatorProvider(Function() serviceLocator)

            Dim _container As IUnityContainer = InitContainer()

            _mediator = New Mediator(_container)
        End Sub

        <Fact()> Public Sub Should_throw_for_send()
            Assert.Throws(Of ResolutionFailedException)(Function() _mediator.Send(New ExceptionTestsPing()))
        End Sub

        <Fact()> Public Sub Should_throw_for_void_send()
            Assert.Throws(Of ResolutionFailedException)(Function() _mediator.Send(New ExceptionTestsVoidPing()))
        End Sub

        <Fact()> Public Sub Should_not_throw_for_publish()
            Assert.DoesNotThrow(Sub() _mediator.Publish(New ExceptionTestsPinged()))
        End Sub

        <Fact()> Public Sub Should_throw_for_async_send()
            Assert.Throws(Of AggregateException)(Function()
                                                     Dim response = _mediator.SendAsync(New ExceptionTestsAsyncPing())
                                                     Task.WaitAll(response)
                                                     Return response

                                                 End Function)
        End Sub

        <Fact()> Public Sub Should_throw_for_async_void_send()
            Assert.Throws(Of AggregateException)(Function()
                                                     Dim response = _mediator.SendAsync(New ExceptionTestsAsyncVoidPing())
                                                     Task.WaitAll(response)
                                                     Return response

                                                 End Function)
        End Sub

        <Fact()> Public Sub Should_not_throw_for_async_publish()
            Assert.DoesNotThrow(Function()
                                    Dim response = _mediator.PublishAsync(New ExceptionTestsAsyncPinged())
                                    Task.WaitAll(response)
                                    Return response
                                End Function)
        End Sub


    Public Shared Function InitContainer() As IUnityContainer
            Dim _container As New UnityContainer
            _container.RegisterTypes(AllClasses.FromAssemblies(GetType(ExceptionTests).Assembly).Where(Function(x) x.Name.StartsWith("ExceptionTests")).Where(Function(x)
                                                                                                                                                                  Dim llll = x.Name
                                                                                                                                                                  Return Not x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                                                                                              End Function),
                                    Function(x) WithMappings.FromAllInterfaces(x)
                                        )
            _container.RegisterTypes(AllClasses.FromAssemblies(GetType(ExceptionTests).Assembly).Where(Function(x)
                                                                                                           Return x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                                       End Function),
                            Function(x) WithMappings.FromAllInterfaces(x),
                            Function(x) WithName.TypeName(x)
                                )
            Return _container
        End Function

    End Class


End Namespace