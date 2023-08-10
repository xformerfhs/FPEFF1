'
' SPDX-FileCopyrightText: 2017-2023 DB Systel GmbH
' SPDX-FileCopyrightText: 2023 Frank Schwab
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
''' Methods missing from the <c>UShort</c> implementation
''' </summary>
Module UShortExtensions
   ''' <summary>
   ''' Creates a new <c>UShort</c> array as the XOR-ed values of two <c>UShort</c> arrays.
   ''' </summary>
   ''' <remarks>
   ''' Note that the result has the same size as the first array. If the second array is shorter than
   ''' the first array elements, with higher indices than the second array are simply copied.
   ''' If the second array is larger, only as many elements, as are in the first array will be used.
   ''' </remarks>
   ''' <param name="firstArray">First <c>UShort</c> array</param>
   ''' <param name="secondArray">Second <c>UShort</c> array</param>
   ''' <returns>New <c>UShort</c> array with the XOR-ed values of the parameter elements</returns>
   <Extension>
   Public Function XorValues(ByRef firstArray As UShort(), ByRef secondArray As UShort()) As UShort()
      Dim result As UShort() = New UShort(0 To firstArray.Length - 1) {}
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
   ''' Creates a new <c>UShort</c> array as the XOR-ed values of two <c>UShort</c> arrays where the elements in the
   ''' second array are used beginning from <paramref name="secondArrayStartIndex"/>.
   ''' </summary>
   ''' <remarks>
   ''' Note that the result has the same size as the first array. If the second array is shorter than
   ''' the first array elements, with higher indices than the second array are simply copied.
   ''' If the second array starting from <paramref name="secondArrayStartIndex"/> is larger, 
   ''' only as many elements, as are in the first array will be used.
   ''' </remarks>
   ''' <param name="firstArray">First <c>UShort</c> array</param>
   ''' <param name="secondArray">Second <c>UShort</c> array</param>
   ''' <param name="secondArrayStartIndex">Start index in the second array</param>
   ''' <returns>New <c>UShort</c> array with the XOR-ed values of the parameter elements</returns>
   <Extension>
   Public Function XorValues(ByRef firstArray As UShort(), ByRef secondArray As UShort(), ByVal secondArrayStartIndex As Integer) As UShort()
      Dim result As UShort() = New UShort(0 To firstArray.Length - 1) {}
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

   ''' <summary>
   ''' Converts an <c>UShort</c> array to a <c>BigInteger</c> where the array's elements represent
   ''' numbers to the radix <paramref name="radix"/>.
   ''' </summary>
   ''' <remarks>
   ''' For performance reasons here is no check whether the array elements are valid "digits".
   ''' </remarks>
   ''' <param name="anUShortArray">Array with the "digits" in Big Endian order (i.e. the most significant digit first)</param>
   ''' <param name="radix">Radix of the numbers in the array</param>
   ''' <returns><c>BigInteger</c> that is created from the "digits" in the <c>UShort</c> array</returns>
   <Extension>
   Public Function ToBigIntegerWithRadix(ByRef anUShortArray As UShort(), ByVal radix As UInteger) As Numerics.BigInteger
      Dim result As Numerics.BigInteger = Numerics.BigInteger.Zero

      For i As Integer = 0 To anUShortArray.Length - 1
         result = result * radix + anUShortArray(i)
      Next

      Return result
   End Function

   ''' <summary>
   ''' Creates a new <c>UShort</c> array from a part of another <c>UShort</c> array and checks each "digit" of the source
   ''' for validity to radix <paramref name="radix"/>
   ''' </summary>
   ''' <param name="sourceArray">Source array</param>
   ''' <param name="startIndex">Start index of the data to be copied</param>
   ''' <param name="resultLength">Result length</param>
   ''' <param name="radix">Radix of the digits</param>
   ''' <returns>New <c>UShort</c> array with the data copied from the source array as specified and each "digit" guaranteed to be valid</returns>
   <Extension>
   Public Function NewFromPartWithCheckForRadix(ByRef sourceArray As UShort(), ByVal startIndex As Integer, ByVal resultLength As Integer, ByVal radix As UInteger) As UShort()
      Dim copyLength As Integer = Math.Max(0, Math.Min(sourceArray.Length - startIndex, resultLength))
      Dim result As UShort() = New UShort(0 To resultLength - 1) {}

      Dim sourceIndex As Integer = startIndex

      Dim actDigit As UShort

      For i As Integer = 0 To copyLength - 1
         actDigit = sourceArray(sourceIndex)

         '
         ' The radix check is inlined for performance
         '
         If actDigit < radix Then
            result(i) = actDigit
            sourceIndex += 1
         Else
            Throw New ArgumentException("Digit '" & FormatNumber(actDigit, 0) & "' is too large for radix " & FormatNumber(radix, 0))
         End If
      Next

      Return result
   End Function

   ''' <summary>
   ''' Gets bytes of the <c>UShort</c> in Big Endian order
   ''' </summary>
   ''' <param name="fromInteger">Integer to be converted</param>
   ''' <returns>Bytes of <paramref name="fromInteger"/> in Big Endian order (2 Bytes)</returns>
   <Extension>
   Public Function GetBigEndianBytes(ByRef fromInteger As UShort) As Byte()
      Dim result As Byte() = New Byte(0 To 1) {}
      Dim tempArray As Byte()

      tempArray = BitConverter.GetBytes(fromInteger)

      result(0) = tempArray(1)
      result(1) = tempArray(0)

      Return result
   End Function

   ''' <summary>
   ''' Gets bytes of the <c>UShort</c> in Big Endian order
   ''' </summary>
   ''' <remarks>
   ''' This method always yields a result with a specified length, even if the
   ''' "normal" result would be shorter, or longer. If the <paramref name="resultLength"/> is larger
   ''' than the "normal" result the result is padded to the left with <c>&amp;H00</c> bytes.
   ''' </remarks>
   ''' <param name="fromInteger">Integer to be converted</param>
   ''' <param name="resultLength">Result length</param>
   ''' <returns>Bytes of <paramref name="fromInteger"/> in Big Endian order with length <paramref name="resultLength"/></returns>
   <Extension>
   Public Function GetBigEndianBytes(ByRef fromInteger As UShort, ByVal resultLength As Integer) As Byte()
      Dim result As Byte() = New Byte(0 To resultLength - 1) {}
      Dim tempArray As Byte()

      tempArray = BitConverter.GetBytes(fromInteger)

      Dim copyLength As Integer = Math.Min(2, resultLength)
      Dim tempIndex As Integer = 2

      Dim i As Integer = Math.Max(0, resultLength - copyLength)

      Do While i < resultLength
         tempIndex -= 1

         result(i) = tempArray(tempIndex)

         i += 1
      Loop

      Return result
   End Function
End Module
