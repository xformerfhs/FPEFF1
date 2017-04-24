Imports System.Runtime.CompilerServices

''' <summary>
''' Methods missing from the standard <c>Array</c> implementation
''' </summary>
Module ArrayExtensions
   ''' <summary>
   ''' Creates a new array from a part of another array
   ''' </summary>
   ''' <typeparam name="T">Type of the array elements</typeparam>
   ''' <param name="sourceArray">Source array of the data</param>
   ''' <param name="startIndex">Start index of the data to be copied</param>
   ''' <param name="resultLength">Length of the data to be copied</param>
   ''' <returns>New array of type <c>T</c> with the data copied from the source array as specified</returns>
   <Extension>
   Public Function NewFromPart(Of T)(ByRef sourceArray As T(), ByVal startIndex As UInteger, ByVal resultLength As UInteger) As T()
      Dim copyLength As UInteger = Math.Max(0, Math.Min(sourceArray.Length - startIndex, resultLength))
      Dim result(0 To resultLength - 1) As T

      Array.Copy(sourceArray, startIndex, result, 0, copyLength)

      Return result
   End Function

   ''' <summary>
   ''' Creates a new array as the concatenation of two arrays
   ''' </summary>
   ''' <typeparam name="T">Type of the array elements</typeparam>
   ''' <param name="firstArray">First array to be concatenated</param>
   ''' <param name="secondArray">First array to be concatenated</param>
   ''' <returns>Concatenation of the first and the second array</returns>
   <Extension>
   Public Function Concatenate(Of T)(ByRef firstArray As T(), ByRef secondArray As T()) As T()
      Dim result(0 To firstArray.Length + secondArray.Length - 1) As T

      firstArray.CopyTo(result, 0)
      secondArray.CopyTo(result, firstArray.Length)

      Return result
   End Function

End Module
