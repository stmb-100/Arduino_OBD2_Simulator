Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Public Class Help
    Private Sub Linklabel1_Linkclicked(ByVal Sender As System.Object, ByVal E As _
      System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        LinkLabel1.LinkVisited = True
        System.Diagnostics.Process.Start _
              ("Iexplore", "http://www.synotics.com")
        System.Diagnostics.Process.Start _
              ("Iexplore", "http://www.zenelectro.blogspot.in")
    End Sub
End Class