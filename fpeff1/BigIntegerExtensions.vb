﻿'
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
''' Methods missing from the <c>Numerics.BigInteger</c> implementation
''' </summary>
Module BigIntegerExtensions
   ''' <summary>
   ''' Converts a <c>BigInteger</c> to an <c>UShort</c> array that contains the digits
   ''' of the <c>BigInteger</c> in Big Endian order (i.e. most significant digit first)
   ''' </summary>
   ''' <param name="aNumber">The <c>BigInteger</c> number to be converted</param>
   ''' <param name="radix">The radix of the digits in the result</param>
   ''' <param name="resultSize">The size of the resulting array</param>
   ''' <returns><c>UShort</c> array of the digits in <paramref name="aNumber"/> with radix <paramref name="radix"/></returns>
   <Extension>
   Public Function ToBigEndianUShortArrayForRadix(ByRef aNumber As Numerics.BigInteger, ByVal radix As UInteger, ByVal resultSize As Integer) As UShort()
      Dim result(0 To resultSize - 1) As UShort

      Dim workNumber As Numerics.BigInteger = aNumber
      Dim workRadix As New Numerics.BigInteger(radix)    ' We need the radix as a BigInteger
      Dim remainder As Numerics.BigInteger = Numerics.BigInteger.Zero

      Dim i As Integer = resultSize

      '
      ' We need to specify a Byte array with 2 elements so "BitConverter.ToUInt16" will work
      '
      Dim workByteArray(0 To 1) As Byte

      Do While workNumber <> Numerics.BigInteger.Zero
         workNumber = Numerics.BigInteger.DivRem(workNumber, workRadix, remainder)

         '
         ' Reset work array
         '
         workByteArray(0) = 0
         workByteArray(1) = 0

         '
         ' Copy data from remainder (which has 1 or 2 bytes)
         '
         remainder.ToByteArray.CopyTo(workByteArray, 0)

         i -= 1
         result(i) = BitConverter.ToUInt16(workByteArray, 0)
      Loop

      Return result
   End Function

   ''' <summary>
   ''' Calculates correct modulus of two BigIntegers
   ''' </summary>
   ''' <remarks>
   ''' The "normal" <c>mod</c> operator is defined as <c>x mod m = x - m * Floor(x / m)</c>. If the dividend is negative this yields a negative number.
   ''' However, we need a result that is always 0 or positive. To that end, if a negative number is produced by the "normal" <c>mod</c> operator
   ''' the modulus is added to this result, so the the end result is positive or 0. This is a correct modulus, as well.
   ''' </remarks>
   ''' <param name="dividend">Dividend for the <c>mod</c> operator</param>
   ''' <param name="divisor">Divisor for the <c>mod</c> operator</param>
   ''' <returns><paramref name="dividend"/> <c>mod</c> <paramref name="divisor"/>, guaranteed to be not negative</returns>
   <Extension>
   Public Function CorrectModulo(ByRef dividend As Numerics.BigInteger, ByRef divisor As Numerics.BigInteger) As Numerics.BigInteger
      Dim result As Numerics.BigInteger

      result = dividend Mod divisor

      If result.Sign < 0 Then _
         result = divisor + result  ' If the result is negative, make it 0, or positive

      Return result
   End Function
End Module
