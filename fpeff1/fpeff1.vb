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
' Version: 1.1.1
'
' History:
'    2017-04-24  Created.
'    2021-01-04  Fixed some SonarLint findings.
'    2021-09-21  Fixed unnecessary assignment to radix.
'

Option Strict On

''' <summary>
''' Simple demonstration program for FF1 encryption/decryption
''' </summary>
Module fpeff1

   ''' <summary>
   ''' Tries to convert a string to an UShort number and put out error message on console if that is not possible
   ''' </summary>
   ''' <param name="aNumberText">The text to convert</param>
   ''' <param name="destinationArray">The array in which to place the value</param>
   ''' <param name="arrayIndex">The index of the array element that receives the value</param>
   ''' <returns><c>True</c>, if conversion succedded, <c>False</c>, if not</returns>
   Private Function TryProcessUShort(ByRef aNumberText As String, ByRef destinationArray As UShort(), ByVal arrayIndex As Integer) As Boolean
      Dim actNumber As UShort = 0

      If UShort.TryParse(aNumberText, actNumber) Then
         destinationArray(arrayIndex) = actNumber
         Return True
      Else
         Console.Error.WriteLine("'{0}' is not a valid number between 0 an 65535", aNumberText)
         Return False
      End If
   End Function

   ''' <summary>
   ''' Converts a comma-separated list of numbers to an array of UShorts
   ''' </summary>
   ''' <param name="text">Comma-separated list</param>
   ''' <param name="destinationArray">Array that receives the converted numbers</param>
   ''' <returns><c>True</c>, if conversion succedded, <c>False</c>, if not</returns>
   Private Function CommaSeparatedListToUShortArray(ByRef text As String, ByRef destinationArray As UShort()) As Boolean
      Dim actTextIndex As Integer = 0
      Dim actNumberStart As Integer = 0

      Dim inNumber As Boolean = False

      destinationArray = New UShort(0 To text.Length >> 1) {}

      Dim actArrayIndex As Integer = 0

      For Each actChar As Char In text
         If Char.IsDigit(actChar) Then
            If Not inNumber Then
               inNumber = True
               actNumberStart = actTextIndex
            End If
         Else
            If actChar <> ","c AndAlso actChar <> " "c Then
               Console.Error.WriteLine("Invalid character '{0}' in list '{1}'", actChar, text)
               Return False
            End If

            If inNumber AndAlso TryProcessUShort(text.Substring(actNumberStart, actTextIndex - actNumberStart), destinationArray, actArrayIndex) Then
               actArrayIndex += 1

               inNumber = False
            End If
         End If

         actTextIndex += 1
      Next

      If inNumber Then
         If TryProcessUShort(text.Substring(actNumberStart, actTextIndex - actNumberStart), destinationArray, actArrayIndex) Then
            actArrayIndex += 1
         Else
            Return False
         End If
      Else
         Console.Error.WriteLine("Integer list '{0}' does not end with a number", text)
         Return False
      End If

      If destinationArray.Length <> actArrayIndex Then _
         Array.Resize(destinationArray, actArrayIndex)

      Return True
   End Function

   ''' <summary>
   ''' Converts a string of hexadecimal digits to a <c>Byte</c> array
   ''' </summary>
   ''' <param name="text">String of hexadecimal digits</param>
   ''' <param name="destinationArray">Array that receives the converted numbers</param>
   ''' <returns><c>True</c>, if conversion succedded, <c>False</c>, if not</returns>
   Private Function HexTextToByteArray(ByRef text As String, ByRef destinationArray As Byte()) As Boolean
      If (text.Length And 1) = 0 Then
         Dim actTextIndex As Integer = 0

         Dim isEven As Boolean = False

         destinationArray = New Byte(0 To (text.Length >> 1) - 1) {}

         Dim actArrayIndex As Integer = 0

         For Each actChar As Char In text
            If isEven Then
               Try
                  destinationArray(actArrayIndex) = Convert.ToByte(text.Substring(actTextIndex - 1, 2), 16)

               Catch e As Exception
                  Console.Error.WriteLine("Text '{0}' is not a valid hexadecimal number", text.Substring(actTextIndex - 1, 2), text)
                  Return False
               End Try

               actArrayIndex += 1
            End If

            actTextIndex += 1

            isEven = Not isEven
         Next

         If destinationArray.Length <> actArrayIndex Then _
            Array.Resize(destinationArray, actArrayIndex)

         Return True
      Else
         Console.Error.WriteLine("Hexadecimal string '{0}' has an odd number of characters", text)
         Return False
      End If
   End Function

   ''' <summary>
   ''' Shows the usage of the program on the console
   ''' </summary>
   Private Sub ShowUsage()
      With Console.Out
         .WriteLine()
         .WriteLine("Usage:")
         .WriteLine()
         .WriteLine("fpeff1 {encode|decode} radix source key [tweak]")
         .WriteLine()
         .WriteLine("   radix:  A number that is the radix for the source")
         .WriteLine("   source: A comma separated list of numbers with radix radix. There must be no spaces or other characters in the list")
         .WriteLine("   key:    A hex string of the key. There must be no spaces or other characters in the string")
         .WriteLine("   tweak:  A hex string of the tweak. This parameter is optional. If present, there must be no spaces or other characters in the string")
         .WriteLine()
         .WriteLine("Example:")
         .WriteLine("   fpeff1 encode 10 0,1,2,3,4,5,6,7,8,9 2B7E151628AED2A6ABF7158809CF4F3C 39383736353433323130")
      End With
   End Sub

   ''' <summary>
   ''' Parses the command line
   ''' </summary>
   ''' <remarks>
   ''' It really is a shame that MS does not have such a parser in the .Net libs
   ''' </remarks>
   Private Function SimpleCommandLineParser(ByRef commandLineArguments As String(), ByRef shouldEncrypt As Boolean, ByRef sourceText As UShort(), ByRef radix As UInteger, ByRef key As Byte(), ByRef tweak As Byte()) As Boolean
      If commandLineArguments.Length >= 4 Then
         Select Case Char.ToLower(commandLineArguments(0)(0))
            Case "e"c
               shouldEncrypt = True

            Case "d"c
               shouldEncrypt = False

            Case Else
               Console.Error.WriteLine("First argument '{0}' is neither 'encrypt', nor 'decrypt'", commandLineArguments(1))
               ShowUsage()
               Return False
         End Select

         If Not UInteger.TryParse(commandLineArguments(1), radix) Then
            Console.Error.WriteLine("Radix '{0}' is not an integer number", commandLineArguments(2))
            ShowUsage()
            Return False
         End If

         If Not CommaSeparatedListToUShortArray(commandLineArguments(2), sourceText) Then
            ShowUsage()
            Return False
         End If

         If Not HexTextToByteArray(commandLineArguments(3), key) Then
            ShowUsage()
            Return False
         End If

         If commandLineArguments.Length >= 5 Then
            If Not HexTextToByteArray(commandLineArguments(4), tweak) Then
               ShowUsage()
               Return False
            End If
         Else
            tweak = New Byte() {}
         End If
      Else
         Console.Error.WriteLine("Not enough arguments")
         ShowUsage()
         Return False
      End If

      Return True
   End Function

   ''' <summary>
   ''' Simple command line interface to call FF1.encrypt/.decrypt
   ''' </summary>
   ''' <param name="commandLineArguments">Parameters. See usage</param>
   ''' <returns><c>0</c> if encryption/decryption was successful, <c>1</c>, if a parameter was in error</returns>
   Public Function Main(ByVal commandLineArguments As String()) As Integer
      Dim sourceText As UShort()
      Dim tweak As Byte()
      Dim key As Byte()
      Dim radix As UInteger
      Dim shouldEncrypt As Boolean

      '
      ' All of the arguments will be set in SimpleCommandLineParser
      '
#Disable Warning BC42030 ' Variable is passed by reference before it has been assigned a value
      If SimpleCommandLineParser(commandLineArguments, shouldEncrypt, sourceText, radix, key, tweak) Then
#Enable Warning BC42030 ' Variable is passed by reference before it has been assigned a value
         Dim convertedText As UShort()

         Try
            If shouldEncrypt Then
               convertedText = FF1.Encrypt(sourceText, radix, key, tweak)
            Else
               convertedText = FF1.Decrypt(sourceText, radix, key, tweak)
            End If

         Catch e As Exception
            Console.Error.WriteLine(e.Message)
            ShowUsage()
            Return 1

         Finally
            Array.Clear(key, 0, key.Length)
         End Try

         Console.Out.WriteLine(String.Join(",", convertedText))

         Console.ReadKey()

         Return 0
      Else
         Return 1
      End If
   End Function
End Module
