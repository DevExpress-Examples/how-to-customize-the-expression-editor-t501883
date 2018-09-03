﻿Imports DevExpress.Data.Controls.ExpressionEditor
Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers
Imports DevExpress.DataAccess.ExpressionEditor
Imports DevExpress.DataAccess.UI.ExpressionEditor
Imports DevExpress.DataAccess.UI.Native.ExpressionEditor
Imports DevExpress.LookAndFeel
Imports DevExpress.XtraEditors
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq

Namespace WindowsFormsApplication1
    Partial Public Class Form1
        Inherits XtraForm

        Public Class Product
            Public Sub New(productID As Integer, productName As String, category As Category)
                Me.ProductID = productID
                Me.ProductName = productName
                Me.Category = category
            End Sub

            Public ReadOnly Property ProductID As Integer
            Public ReadOnly Property ProductName As String
            Public ReadOnly Property SupplierID As Integer?
            Public ReadOnly Property CategoryID As Integer?
            Public ReadOnly Property UnitsOnOrder As Short?
            Public ReadOnly Property Discontinued As Boolean
            Public ReadOnly Property Category As Category
        End Class

        Public Class Category
            Public Property CategoryID As Integer
            Public Property CategoryName As String
        End Class

        Public Function GetProductsList() As List(Of Product)
            Dim categoryBeverages = New Category With {.CategoryID = 1, .CategoryName = "Beverages"}
            Dim categoryConfections = New Category With {.CategoryID = 2, .CategoryName = "Condiments"}

            Return New List(Of Product) From {
                New Product(1, "Chai", categoryBeverages),
                New Product(2, "Chang", categoryBeverages),
                New Product(3, "Coffee", categoryBeverages),
                New Product(4, "Chocolade", categoryConfections),
                New Product(5, "Maxilaku", categoryConfections),
                New Product(6, "Valkoinen suklaa", categoryConfections)
            }
        End Function

        Public Sub New()
            InitializeComponent()
            Me.gridControl1.DataSource = GetProductsList()
        End Sub

        Private Sub gridView1_UnboundExpressionEditorCreated(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.UnboundExpressionEditorEventArgs) Handles gridView1.UnboundExpressionEditorCreated
            If e.ExpressionEditorContext Is Nothing Then
                Return
            End If

            ' Exclude "Now" from the list of available functions.
            Dim nowFunction = e.ExpressionEditorContext.Functions.FirstOrDefault(Function(fi) String.Equals(fi.Name, "now", StringComparison.OrdinalIgnoreCase))
            If nowFunction IsNot Nothing Then
                e.ExpressionEditorContext.Functions.Remove(nowFunction)
            End If

            ' Uncomment the following line to use a custom color provider.
            'e.ExpressionEditorContext.ColorProvider = New CustomColorProvider()

            ' Implement a custom criteria validator (e.g., to forbid using a specific function).
            e.ExpressionEditorContext.CriteriaOperatorValidatorProvider = New ValidatorProvider()

            ' Disable capitalization of function names in expressions.
            e.ExpressionEditorContext.OptionsBehavior.CapitalizeFunctionNames = False

            ' Rename the "Columns" category to "Fields".
            For Each columnInfo In e.ExpressionEditorContext.Columns
                columnInfo.Category = "Fields"
            Next columnInfo

            ' Uncomment the following line to use a custom Expression Editor view.
            'e.ExpressionEditorView = New CustomExpressionEditorView(Me.LookAndFeel, New CustomExpressionEditorControl())
        End Sub

    End Class

    Friend Class ValidatorProvider
        Implements ICriteriaOperatorValidatorProvider

#Region "Implementation of ICriteriaOperatorValidatorProvider"

        Public Function GetCriteriaOperatorValidator(ByVal context As ExpressionEditorContext) As ErrorsEvaluatorCriteriaValidator Implements ICriteriaOperatorValidatorProvider.GetCriteriaOperatorValidator
            Return New Validator(context)
        End Function

#End Region
    End Class

    Friend Class Validator
        Inherits CriteriaOperatorValidator

        Public Sub New(ByVal context As ExpressionEditorContext)
            MyBase.New(context, supportsAggregates:=True)
        End Sub

#Region "Overrides of CriteriaOperatorValidator"

        Public Overrides Sub Visit(ByVal theOperator As FunctionOperator)
            If theOperator.OperatorType = FunctionOperatorType.Now Then
                Me.errors.Add(New CriteriaValidatorError("Invalid function: now()"))
            End If
            MyBase.Visit(theOperator)
        End Sub

#End Region
    End Class


    Friend Class CustomColorProvider
        Implements IExpressionEditorColorProvider

        Public Function GetColorForElement(ByVal elementKind As ExpressionElementKind) As Color Implements IExpressionEditorColorProvider.GetColorForElement
            If elementKind = ExpressionElementKind.Column Then
                Return Color.BlueViolet
            End If

            If elementKind = ExpressionElementKind.Function Then
                Return Color.Brown
            End If

            Return Color.Azure
        End Function
    End Class

    Public Class CustomExpressionEditorView
        Inherits ExpressionEditorView

        Public Sub New(ByVal lookAndFeel As UserLookAndFeel, ByVal expressionEditor As ExpressionEditorControl)
            MyBase.New(lookAndFeel, expressionEditor)
        End Sub
    End Class

    Public Class CustomExpressionEditorControl
        Inherits ExpressionEditorControl

#Region "Overrides of ExpressionEditorControl"

        Protected Overrides Function CreateDocumentationControl() As ExpressionDocumentationControl
            Return Nothing
        End Function

#End Region
    End Class

End Namespace
