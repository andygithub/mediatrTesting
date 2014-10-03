Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports MedaitrContainer
Imports Microsoft.Practices.Unity

<TestClass()> Public Class LifeCycleTest

    <TestMethod()> Public Sub LifeCycleTest()
        Dim mediator = BuildMediator()

        Runner.Run(mediator, Console.Out)
        Assert.IsTrue(True)
    End Sub

    Private Shared Function BuildMediator() As IMediator
        'Dim _genType = GetType(INotificationHandler(Of Pinged))
        'Dim val = _genType.IsAssignableFrom(GetType(PingedHandler))
        'Dim _ints = GetType(PingedHandler).GetInterfaces
        'Assert.IsTrue(GetType(PingedHandler).GetInterfaces.Any(Function(x) x.Name = GetType(INotificationHandler(Of )).Name))
        'For Each _int In _ints
        '    Dim _v = _int.Name
        '    Assert.AreEqual(GetType(INotificationHandler(Of )).Name, _int.Name)
        'Next
        Dim container = New UnityContainer()
        '.Where(Function(x) Not x.IsInterface AndAlso Not GetType(INotificationHandler(Of )).IsAssignableFrom(x)),
        'the assignable from is not working due to the generics present in the interface - http://msdn.microsoft.com/en-us/library/system.type.isassignablefrom%28v=vs.110%29.aspx
        'so pulling of the list of implemented interfaces implemented and then checking for the type name is the method that worked.
        container.RegisterTypes(AllClasses.FromAssemblies(GetType(Ping).Assembly).Where(Function(x)
                                                                                            Return Not x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                        End Function),
                                Function(x) WithMappings.FromAllInterfaces(x)
                                    )
        'container.RegisterType(Of INotificationHandler(Of INotification), GenericHandler)(GetType(GenericHandler).Name)
        'can't register multiple types back to the INotification since the derived types won't register due to does not inherit from or implement the constraint type
        'container.RegisterType(Of INotificationHandler(Of Pinged), PingedHandler)(GetType(PingedHandler).Name)
        'container.RegisterType(Of INotificationHandler(Of Pinged), PingedDubHandler)(GetType(PingedDubHandler).Name)
        container.RegisterTypes(AllClasses.FromAssemblies(GetType(Ping).Assembly).Where(Function(x)
                                                                                            Return x.GetInterfaces.Any(Function(y) y.Name = GetType(INotificationHandler(Of )).Name OrElse y.Name = GetType(IAsyncNotificationHandler(Of )).Name)
                                                                                        End Function),
                        Function(x) WithMappings.FromAllInterfaces(x),
                        Function(x) WithName.TypeName(x)
                            )
        container.RegisterTypes(AllClasses.FromAssemblies(GetType(IMediator).Assembly),
                        Function(x) WithMappings.FromAllInterfaces(x)
                            )
        container.RegisterInstance(Console.Out)
        'this registration is present so that when INotificationHandler(Of INotification) or INotificationHandler(Of Pinged) is resolved it always returns the base interface if T has it implemented.
        'Dim handlers As IEnumerable(Of Type) = AllClasses.FromLoadedAssemblies.Where(Function(t) IsAssignableToGenericType(t, GetType(INotificationHandler(Of ))) AndAlso Not t.IsInterface)
        'For Each _handler In handlers
        '    container.AddNewExtension(Of OpenGenericExtension)().Configure(Of IOpenGenericExtension).RegisterClosedImpl(GetType(INotificationHandler(Of )), _handler)
        'Next

        'these asserts are present for the registration above.
        'Assert.AreNotEqual(1, container.ResolveAll(Of INotificationHandler(Of INotification)).Count)
        'Assert.AreNotEqual(1, container.ResolveAll(Of INotificationHandler(Of Pinged)).Count)
        'Dim serviceLocator = New UnityServiceLocator(container)
        'Dim serviceLocatorProvider = New ServiceLocatorProvider(Function() serviceLocator)

        'Dim _containerT = container.ResolveAll(Of INotificationHandler(Of Pinged))()
        'For Each item In _containerT
        '    item.Handle(New Pinged)
        'Next
        'Dim _handlerT = container.ResolveAll(Of IRequestHandler(Of Pong, Ping))()
        'Dim _t = serviceLocatorProvider().GetAllInstances(Of INotificationHandler(Of Pinged))()
        'For Each item In _t
        '    item.Handle(New Pinged)
        'Next
        'Dim request As New Pong
        'Dim TResponse As New Ping
        'Dim handlerType = GetType(IRequestHandler(Of ,)).MakeGenericType(request.GetType(), TResponse.GetType)
        'Dim wrapperType = GetType(RequestHandler(Of )).MakeGenericType(request.GetType(), TResponse.GetType)
        'Dim handler = serviceLocatorProvider().GetInstance(handlerType)

        'container.RegisterInstance(serviceLocatorProvider)
        'Dim t = container.Resolve(Of IMediator)()
        Dim mediator = container.Resolve(Of IMediator)()

        Return mediator
    End Function


    Public Shared Function IsAssignableToGenericType(givenType As Type, genericType As Type) As Boolean
        Dim interfaceTypes = givenType.GetInterfaces()

        For Each it In interfaceTypes
            If it.IsGenericType AndAlso it.GetGenericTypeDefinition() = genericType Then
                Return True
            End If
        Next

        If givenType.IsGenericType AndAlso givenType.GetGenericTypeDefinition() = genericType Then
            Return True
        End If

        Dim baseType As Type = givenType.BaseType
        If baseType Is Nothing Then
            Return False
        End If

        Return IsAssignableToGenericType(baseType, genericType)
    End Function

End Class

Public Interface IOpenGenericExtension
    Inherits IUnityContainerExtensionConfigurator
    Sub RegisterClosedImpl(openGenericInterface As Type, closedType As Type)
End Interface

Public Class OpenGenericExtension
    Inherits UnityContainerExtension
    Implements IOpenGenericExtension
    Protected Overrides Sub Initialize()
    End Sub

    Public Sub RegisterClosedImpl(openGenericInterface As Type, closedType As Type) Implements IOpenGenericExtension.RegisterClosedImpl
        closedType.GetInterfaces().Where(Function(x) x.IsGenericType).
        Where(Function(x) x.GetGenericTypeDefinition() = openGenericInterface).
        ToList().
        ForEach(Function(x) Container.RegisterType(x, closedType, closedType.Name))
    End Sub
End Class
