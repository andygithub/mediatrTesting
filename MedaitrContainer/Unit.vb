Public NotInheritable Class Unit
    Implements IComparable
    Public Shared ReadOnly Value As New Unit()

    Public Overrides Function GetHashCode() As Integer
        Return 0
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Return obj Is Nothing OrElse TypeOf obj Is Unit
    End Function

    Private Function IComparable_CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Return 0
    End Function
End Class