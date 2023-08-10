'
' SPDX-FileCopyrightText: 2023 Frank Schwab'
' SPDX-License-Identifier: Apache-2.0
'
' Licensed under the Apache License, Version 2.0 (the "License");
' You may not use this file except in compliance with the License.
'
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
' Author: Frank Schwab
'
' Version: 1.0.0
'
' History:
'    2017-04-24  Created
'

Option Strict On

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
   Public Function NewFromPart(Of T)(ByRef sourceArray As T(), ByVal startIndex As Integer, ByVal resultLength As Integer) As T()
      Dim copyLength As Integer = Math.Max(0, Math.Min(sourceArray.Length - startIndex, resultLength))
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
