'
' SPDX-FileCopyrightText: 2021 DB Systel GmbH
'
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
''' Methods missing from the <c>Byte</c> implementation
''' </summary>
Module ByteExtensions
   ''' <summary>
   ''' Creates a new <c>Byte</c> array that is in reverse order of the source array
   ''' </summary>
   ''' <param name="sourceArray">Array whose elements are to be reversed</param>
   ''' <returns>New <c>Byte</c> array in reverse order</returns>
   <Extension>
   Public Function NewWithReversedOrder(ByRef sourceArray() As Byte) As Byte()
      Dim resultLength As Integer = sourceArray.Length
      Dim result As Byte() = New Byte(0 To resultLength - 1) {}

      Dim destinationIndex As Integer = resultLength

      Dim i As Integer = 0

      Do While i < resultLength
         destinationIndex -= 1
         result(destinationIndex) = sourceArray(i)
         i += 1
      Loop

      Return result
   End Function

   ''' <summary>
   ''' Creates a new <c>Byte</c> array that is in reverse order of the source array with guaranteed minimum length.
   ''' </summary>
   ''' <remarks>
   ''' If <paramref name="minLength"/> is larger than the length of the source array the resulting array is padded to the left
   ''' (with the lower indices) with <c>\&amp;H00</c> bytes.
   ''' </remarks>
   ''' <param name="sourceArray">Array whose elements are to be reversed</param>
   ''' <param name="minLength">Minimum guaranteed length of the result</param>
   ''' <returns>New <c>Byte</c> array in reverse order with guaranteed minimum length</returns>
   <Extension>
   Public Function NewWithReversedOrder(ByRef sourceArray() As Byte, ByVal minLength As Integer) As Byte()
      Dim resultLength As Integer = Math.Max(sourceArray.Length, minLength)
      Dim result As Byte() = New Byte(0 To resultLength - 1) {}

      Dim copyLength As Integer = Math.Min(resultLength, sourceArray.Length)

      Dim destinationIndex As Integer = resultLength

      Dim i As Integer = 0

      Do While i < copyLength
         destinationIndex -= 1
         result(destinationIndex) = sourceArray(i)
         i += 1
      Loop

      Return result
   End Function

   ''' <summary>
   ''' Creates a new <c>Byte</c> array with the elements in reverse order of the
   ''' <paramref name="sourceArray"/> and adds a <c>&amp;H00</c> <c>Byte</c> at the element
   ''' with the highest index to guarantee that a BigInteger that is instantiated from
   ''' the resulting <c>Byte</c> array is a positive number.
   ''' </summary>
   ''' <param name="sourceArray">Source Byte array</param>
   ''' <param name="sourceLength">Length of the source byte array</param>
   ''' <returns>New <c>Byte</c> array with reversed elements and an additional <c>&amp;H00</c> byte at the highest index.
   ''' Note that the result has one more element than specified by <paramref name="sourceLength"/></returns>
   <Extension>
   Public Function ReverseWithUnsignedExtension(ByRef sourceArray() As Byte, ByVal sourceLength As Integer) As Byte()
      Dim result As Byte() = New Byte(0 To sourceLength) {}

      Dim copyLength As Integer = Math.Min(sourceLength, sourceArray.Length)

      Dim destinationIndex As Integer = copyLength

      result(destinationIndex) = 0  ' This ensures that the result is a positive number when converted to a BigInteger

      Dim i As Integer = 0

      Do While i < copyLength
         destinationIndex -= 1
         result(destinationIndex) = sourceArray(i)
         i += 1
      Loop

      Return result
   End Function

   ''' <summary>
   ''' Creates a new <c>Byte</c> array as the XOR-ed values of two <c>Byte</c> arrays.
   ''' </summary>
   ''' <remarks>
   ''' Note that the result has the same size as the first array. If the second array is shorter than
   ''' the first array, elements with higher indices than the second array are simply copied.
   ''' If the second array is larger, only as many elements, as are in the first array will be used.
   ''' </remarks>
   ''' <param name="firstArray">First <c>Byte</c> array</param>
   ''' <param name="secondArray">Second <c>Byte</c> array</param>
   ''' <returns>New <c>Byte</c> array with the XOR-ed values of the parameter elements</returns>
   <Extension>
   Public Function XorValues(ByRef firstArray() As Byte, ByRef secondArray() As Byte) As Byte()
      Dim result As Byte() = New Byte(0 To firstArray.Length - 1) {}
      Dim copyLength As Integer = Math.Min(firstArray.Length, secondArray.Length)

      Dim i As Integer

      For i = 0 To copyLength - 1
         result(i) = firstArray(i) Xor secondArray(i)
      Next

      For i = copyLength To result.Length - 1
         result(i) = firstArray(i)
      Next

      Return result
   End Function

   ''' <summary>
   ''' Creates a new <c>Byte</c> array as the XOR-ed values of two <c>Byte</c> arrays where the elements in the
   ''' second array are used beginning from <paramref name="secondArrayStartIndex"/>.
   ''' </summary>
   ''' <remarks>
   ''' Note that the result has the same size as the first array. If the second array is shorter than
   ''' the first array elements, with higher indices than the second array are simply copied.
   ''' If the second array starting from <paramref name="secondArrayStartIndex"/> is larger, 
   ''' only as many elements, as are in the first array will be used.
   ''' </remarks>
   ''' <param name="firstArray">First <c>Byte</c> array</param>
   ''' <param name="secondArray">Second <c>Byte</c> array</param>
   ''' <param name="secondArrayStartIndex">Start index in the second array</param>
   ''' <returns>New <c>Byte</c> array with the XOR-ed values of the parameter elements</returns>
   <Extension>
   Public Function XorValues(ByRef firstArray() As Byte, ByRef secondArray() As Byte, ByVal secondArrayStartIndex As Integer) As Byte()
      Dim result As Byte() = New Byte(0 To firstArray.Length - 1) {}
      Dim copyLength As Integer = Math.Max(0, Math.Min(firstArray.Length, secondArray.Length - secondArrayStartIndex))

      Dim i As Integer = 0

      Do While i < copyLength
         result(i) = firstArray(i) Xor secondArray(secondArrayStartIndex)

         i += 1
         secondArrayStartIndex += 1
      Loop

      For i = copyLength To result.Length - 1
         result(i) = firstArray(i)
      Next

      Return result
   End Function
End Module
