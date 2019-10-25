''' <summary>
''' Implements the FF1 algorithm as specified in NIST Special Publication 800-38G (March 2016)
''' </summary>
''' <remarks>
''' Copyright 2017, DB Systel GmbH
''' 
''' Redistribution and use in source and binary forms, with or without modification, are permitted provided
''' that the following conditions are met
''' 
''' 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
''' 
''' 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
''' the following disclaimer in the documentation and/or other materials provided with the distribution.
''' 
''' 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products
''' derived from this software without specific prior written permission.
''' 
''' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
''' BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY And FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
''' IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
''' EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT Of SUBSTITUTE GOODS OR SERVICES;
''' LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
''' IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT Of THE USE
''' OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY Of SUCH DAMAGE.
'''
''' Author: Frank Schwab, DB Systel GmbH
''' Version: 1.0.1
''' History:
'''    2017-04-24 Created.
'''    2019-10-25 Bump up minimum domain size according to NIST SP 800-38G REV. 1.
'''    
''' Usage:
''' <code>
'''      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
'''      Dim tweak() As Byte = {&amp;H39, &amp;H38, &amp;H37, &amp;H36, &amp;H35, &amp;H34, &amp;H33, &amp;H32, &amp;H31, &amp;H30}
'''      Dim key() As Byte = {&amp;H2B, &amp;H7E, &amp;H15, &amp;H16, &amp;H28, &amp;HAE, &amp;HD2, &amp;HA6, &amp;HAB, &amp;HF7, &amp;H15, &amp;H88, &amp;H9, &amp;HCF, &amp;H4F, &amp;H3C}
'''      
'''      Dim encryptedData() As UShort
'''      
'''      encryptedData = FF1.encrypt(sourceText, 10, key, tweak)
'''
'''      Dim decryptedData() As UShort
'''      
'''      decryptedData = FF1.decrypt(encryptedData, 10, key, tweak)
''' </code>
''' </remarks>
Public Class FF1
   '
   ' Private constants
   '
#Region "Private constants"
   ' Minimum domain size according to NIST SP 800-38G REV. 1
   Private Const MINIMUM_DOMAIN_SIZE As Double = 1000000
#End Region

   '
   ' Private methods
   '
#Region "Private methods"
#Region "Algorithm helper methods"
   ''' <summary>
   ''' Pseudo random function as specified in FF1, algorithm 6
   ''' </summary>
   ''' <param name="encryptor">AES-ECB-NoPadding encryptor with key already set</param>
   ''' <param name="byteArray">Data to be used in generating the PRF value</param>
   ''' <returns>Pseudo random number</returns>
   Private Shared Function pseudoRandomFunction(ByRef encryptor As Security.Cryptography.ICryptoTransform, ByRef byteArray() As Byte) As Byte()
      Dim blockCount As UInteger = byteArray.Length >> 4
      Dim startIndex As UInteger = 0

      Dim result() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

      For i As UShort = 1 To blockCount
         result = encryptor.TransformFinalBlock(result.XorValues(byteArray, startIndex), 0, result.Length)
         startIndex += result.Length
      Next

      Return result
   End Function

   ''' <summary>
   ''' Sets the constant byte array P as specified in FF1, algorithm 7 and algorithm 8
   ''' </summary>
   ''' <param name="sourceLength">Length of the source input</param>
   ''' <param name="radix">Radix of the numbers</param>
   ''' <param name="leftLength">Length of the left part of the source</param>
   ''' <param name="tweakLength">Length of the tweak</param>
   ''' <returns>The (from there on) constant array P</returns>
   Private Shared Function initializeP(ByVal sourceLength As UInteger, ByVal radix As UInteger, ByVal leftLength As UInteger, ByVal tweakLength As UInteger) As Byte()
      Dim result(0 To 15) As Byte

      result(0) = 1
      result(1) = 2
      result(2) = 1

      radix.GetBigEndianBytes(3).CopyTo(result, 3)    ' The 3rd byte is only needed for radix = 2^16

      result(6) = 10

      result(7) = CByte(leftLength Mod 256)

      sourceLength.GetBigEndianBytes.CopyTo(result, 8)

      tweakLength.GetBigEndianBytes.CopyTo(result, 12)

      Return result
   End Function

   ''' <summary>
   ''' Initializes the constant part of array Q as specified in FF1, algorithm 7 and algorithm 8
   ''' </summary>
   ''' <param name="tweak">The tweak byte array</param>
   ''' <param name="tweakLength">Length of the tweak byte array</param>
   ''' <param name="maxPartNumberByteLength">Maximum byte length of the binary numbers used in the calculation parts</param>
   ''' <returns>The array Q with the constant parts set</returns>
   Private Shared Function initializeQ(ByRef tweak() As Byte, ByVal tweakLength As UInteger, ByVal maxPartNumberByteLength As UInteger) As Byte()
      Dim padLength As UInteger = 16 - ((tweakLength + maxPartNumberByteLength + 1) Mod 16)

      Dim result(0 To tweakLength + padLength + maxPartNumberByteLength) As Byte

      Dim actIndex As UInteger = 0

      Dim i As UInteger = 0
      Do While i < tweakLength
         result(i) = tweak(i)
         actIndex = i
         i += 1
      Loop

      i = 0
      Do While i < padLength
         actIndex += 1
         result(actIndex) = 0
         i += 1
      Loop

      Return result
   End Function

   ''' <summary>
   ''' Complement array Q with round data
   ''' </summary>
   ''' <param name="q">Q array to be complemented</param>
   ''' <param name="roundNumber">Round number</param>
   ''' <param name="partAsBigInteger">Part that is used in the calculation</param>
   ''' <param name="maxPartNumberByteLength">Maximum byte length of the binary numbers used in the calculation parts</param>
   Private Shared Sub complementQ(ByRef q() As Byte, ByVal roundNumber As Byte, ByRef partAsBigInteger As Numerics.BigInteger, ByVal maxPartNumberByteLength As UInteger)
      Dim actIndex As UInteger

      actIndex = q.Length - maxPartNumberByteLength
      Array.Copy(partAsBigInteger.ToByteArray.NewWithReversedOrder(maxPartNumberByteLength), 0, q, actIndex, maxPartNumberByteLength)

      actIndex -= 1
      q(actIndex) = roundNumber
   End Sub

   ''' <summary>
   ''' Construct array S as specified in FF1, algorithm 7 and algorithm 8
   ''' </summary>
   ''' <param name="s">S array to be filled with data (Allocated and reused in the calling method for efficiency)</param>
   ''' <param name="r">Output of the pseudo random function</param>
   ''' <param name="blockCount">No. of blocks to be processed</param>
   ''' <param name="encryptor">AES-ECB-NoPadding encryptor with key already set</param>
   Private Shared Sub constructS(ByRef s() As Byte, ByRef r() As Byte, ByVal blockCount As UInteger, ByRef encryptor As Security.Cryptography.ICryptoTransform)
      Dim xorBlock() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
      Dim xorBlockLength As UInteger = xorBlock.Length
      Dim sOffset As UInteger = xorBlockLength

      Array.Copy(r, s, r.Length)

      For i As UInteger = 1 To blockCount - 1
         i.GetBigEndianBytes.CopyTo(xorBlock, 12)

         Array.Copy(encryptor.TransformFinalBlock(r.XorValues(xorBlock, 0), 0, xorBlockLength), 0, s, sOffset, xorBlockLength)

         sOffset += xorBlockLength
      Next
   End Sub

   ''' <summary>
   ''' Common initialization of the encryption/decryption machinery
   ''' </summary>
   ''' <param name="source">Input: Source of the encryption/decryption</param>
   ''' <param name="radix">Input: Radix of the numbers in the source array</param>
   ''' <param name="encryptionKey">Input: Encryption key to be used</param>
   ''' <param name="tweak">Input: Tweak to be used</param>
   ''' <param name="sourceLength">Output: Length of the Source array</param>
   ''' <param name="tweakLength">Output: Length of the Tweak array</param>
   ''' <param name="leftLength">Output: Length of the left part of the source</param>
   ''' <param name="rightLength">Output: Length of the right part of the source</param>
   ''' <param name="radixToTheLeftSize">Output: The radix power to the leftSize</param>
   ''' <param name="radixToTheRightSize">Output: The radix power to the rightSize</param>
   ''' <param name="leftPart">Output: Left part of the source</param>
   ''' <param name="rightPart">Output: Right part of the source</param>
   ''' <param name="maxPartNumberByteLength">Output: Maximum length of a binary number of one part in bytes</param>
   ''' <param name="feistelRoundOutputLength">Output: Number of bytes for arrays used in Feistel round calculations</param>
   ''' <param name="blockCount">Output: Number of blocks used in Feistel round calculations</param>
   ''' <param name="aesCipher">Output: AES-CBC-NoPadding cipher</param>
   ''' <param name="encryptor">Output: AES-ECB-NoPadding encryptor with key already set</param>
   ''' <param name="p">Output: Array P as specified in FF1, algorithm 7 and algorithm 8</param>
   ''' <param name="q">Output: Array P as specified in FF1, algorithm 7 and algorithm 8</param>
   Private Shared Sub setUpMachinery(ByRef source() As UShort,
                                     ByRef radix As UShort,
                                     ByRef encryptionKey() As Byte,
                                     ByRef tweak() As Byte,
                                     ByRef sourceLength As UInteger,
                                     ByRef tweakLength As UInteger,
                                     ByRef leftLength As UInteger,
                                     ByRef rightLength As UInteger,
                                     ByRef radixToTheLeftSize As Numerics.BigInteger,
                                     ByRef radixToTheRightSize As Numerics.BigInteger,
                                     ByRef leftPart() As UShort,
                                     ByRef rightPart() As UShort,
                                     ByRef maxPartNumberByteLength As UInteger,
                                     ByRef feistelRoundOutputLength As UInteger,
                                     ByRef blockCount As UInteger,
                                     ByRef aesCipher As Security.Cryptography.AesManaged,
                                     ByRef encryptor As Security.Cryptography.ICryptoTransform,
                                     ByRef p() As Byte,
                                     ByRef q() As Byte)
      '
      ' Initialize length fields
      '
      sourceLength = source.Length
      tweakLength = tweak.Length

      leftLength = sourceLength >> 1
      rightLength = sourceLength - leftLength

      radixToTheLeftSize = radix ^ leftLength
      radixToTheRightSize = radix ^ rightLength

      leftPart = source.NewFromPartWithCheckForRadix(0, leftLength, radix)
      rightPart = source.NewFromPartWithCheckForRadix(leftLength, rightLength, radix)

      maxPartNumberByteLength = Math.Ceiling(Math.Ceiling(rightLength * Math.Log(radix) / Math.Log(2)) * 0.125)
      feistelRoundOutputLength = 4 * Math.Ceiling(maxPartNumberByteLength * 0.25) + 4
      blockCount = Math.Ceiling(feistelRoundOutputLength * 0.0625)

      '
      ' Initialize byte arrays
      '
      p = initializeP(sourceLength, radix, leftLength, tweakLength)   ' This is a constant that is only computed once

      q = initializeQ(tweak, tweakLength, maxPartNumberByteLength) ' This part of q never changes, so it is computed only once

      '
      ' Initialize the crypto functions
      '
      aesCipher = New Security.Cryptography.AesManaged

      aesCipher.Mode = Security.Cryptography.CipherMode.ECB
      aesCipher.BlockSize = 128
      aesCipher.Padding = Security.Cryptography.PaddingMode.None

      aesCipher.Key = encryptionKey

      encryptor = aesCipher.CreateEncryptor()
   End Sub

   ''' <summary>
   ''' Calculates the binary integer y as specified in FF1 algorithms 7 and 8
   ''' </summary>
   ''' <param name="p">P array as specfied in FF1 algorithms 7 and 8</param>
   ''' <param name="q">Q array as specfied in FF1 algorithms 7 and 8</param>
   ''' <param name="s">S array as specfied in FF1 algorithms 7 and 8</param>
   ''' <param name="roundNumber">Round number</param>
   ''' <param name="radix">Radix of the numbers in the source array</param>
   ''' <param name="part">Left or right part of calculation</param>
   ''' <param name="maxPartNumberByteLength">Maximum length of a binary number of one part in bytes</param>
   ''' <param name="blockCount">Number of blocks used in Feistel round calculations</param>
   ''' <param name="feistelRoundOutputLength">Number of bytes for arrays used in Feistel round calculations</param>
   ''' <param name="encryptor">AES-ECB-NoPadding encryptor with key already set</param>
   ''' <returns>The number y as specfied in FF1 algorithms 7 and 8</returns>
   Private Shared Function calculateY(ByRef p() As Byte,
                                      ByRef q() As Byte,
                                      ByRef s() As Byte,
                                      ByVal roundNumber As Byte,
                                      ByVal radix As UShort,
                                      ByRef part() As UShort,
                                      ByVal maxPartNumberByteLength As UInteger,
                                      ByVal blockCount As UInteger,
                                      ByVal feistelRoundOutputLength As UInteger,
                                      ByRef encryptor As Security.Cryptography.ICryptoTransform) As Numerics.BigInteger
      complementQ(q, roundNumber, part.ToBigIntegerWithRadix(radix), maxPartNumberByteLength)

      constructS(s, pseudoRandomFunction(encryptor, p.Concatenate(q)), blockCount, encryptor)

      Return New Numerics.BigInteger(s.ReverseWithUnsignedExtension(feistelRoundOutputLength))
   End Function
#End Region

#Region "Check methods"
   ''' <summary>
   ''' Check encrypt/decrypt methods parameters
   ''' </summary>
   ''' <param name="source">Source array</param>
   ''' <param name="radix">Radix of the numbers in the source array</param>
   Private Shared Sub checkParameters(ByRef source() As UShort, ByRef radix As UInteger, ByRef encryptionKey() As Byte, ByRef tweak() As Byte)
      If source Is Nothing Then
         Throw New ArgumentException("Source is nothing")
      End If

      If encryptionKey Is Nothing Then
         Throw New ArgumentException("Key is nothing")
      End If

      If radix < 2 Then
         Throw New ArgumentException("Radix is less than 2")
      End If

      If radix > 2 ^ 16 Then
         Throw New ArgumentException("Radix is greater than 2^16")
      End If

      If source.LongLength >= 2L ^ 32 Then
         Throw New ArgumentException("Source length is longer than or equal to 2^32 elements")
      End If

      Dim minLength As UInteger = Math.Ceiling(Math.Log(MINIMUM_DOMAIN_SIZE) / Math.Log(radix))

      If source.Length < minLength Then
         Throw New ArgumentException("Source is too short for radix. Minimum length is " & FormatNumber(minLength, 0))
      End If

      Select Case encryptionKey.Length
         Case 16, 24, 32
         Case Else
            Throw New ArgumentException("Encryption key is not 128, 192, or 256 bits long")
      End Select

      If tweak Is Nothing Then
         tweak = New Byte() {}
      End If
   End Sub
#End Region
#End Region

   '
   ' Public methods
   '
#Region "Public methods"
   ''' <summary>
   ''' Encrypt data with the FF1 Format-Preserving cipher
   ''' </summary>
   ''' <param name="source">Clear data to be encrypted</param>
   ''' <param name="radix">Radix of the numbers in source</param>
   ''' <param name="encryptionKey">Encryption key</param>
   ''' <param name="tweak">Tweak</param>
   ''' <returns>Encrypted data</returns>
   Public Shared Function encrypt(ByRef source() As UShort, ByRef radix As UInteger, ByRef encryptionKey() As Byte, ByRef tweak() As Byte) As UShort()
      Dim sourceLength As UInteger
      Dim tweakLength As UInteger

      Dim leftLength As UInteger
      Dim rightSize As UInteger

      Dim radixToTheLeftSize As Numerics.BigInteger
      Dim radixToTheRightSize As Numerics.BigInteger

      Dim leftPart() As UShort
      Dim rightPart() As UShort

      Dim p() As Byte
      Dim q() As Byte

      Dim maxPartNumberByteLength As UInteger
      Dim feistelRoundOutputLength As UInteger
      Dim blockCount As UInteger

      Dim aesCipher As Security.Cryptography.AesManaged
      Dim encryptor As Security.Cryptography.ICryptoTransform

      checkParameters(source, radix, encryptionKey, tweak)

#Disable Warning BC42030
#Disable Warning BC42108
      setUpMachinery(source, radix, encryptionKey, tweak, sourceLength, tweakLength, leftLength, rightSize, radixToTheLeftSize, radixToTheRightSize, leftPart, rightPart, maxPartNumberByteLength, feistelRoundOutputLength, blockCount, aesCipher, encryptor, p, q)
#Enable Warning BC42030
#Enable Warning BC42108

      Dim s(0 To blockCount * 16 - 1) As Byte

      Dim moduloValue As Numerics.BigInteger

      Dim y As Numerics.BigInteger

      Dim c As Numerics.BigInteger

      Dim newPartSize As UInteger

      Dim roundNumber As Byte = 0

      Do While roundNumber < 10
         y = calculateY(p, q, s, roundNumber, radix, rightPart, maxPartNumberByteLength, blockCount, feistelRoundOutputLength, encryptor)

         If (roundNumber And 1) <> 0 Then
            moduloValue = radixToTheRightSize
            newPartSize = rightSize
         Else
            moduloValue = radixToTheLeftSize
            newPartSize = leftLength
         End If

         c = (leftPart.ToBigIntegerWithRadix(radix) + y) Mod moduloValue

         leftPart = rightPart

         rightPart = c.ToBigEndianUShortArrayForRadix(radix, newPartSize)

         roundNumber += 1
      Loop

      Return leftPart.Concatenate(rightPart)
   End Function

   ''' <summary>
   ''' Decrypt data with the FF1 Format-Preserving cipher
   ''' </summary>
   ''' <param name="source">Encrypted data to be decrypted</param>
   ''' <param name="radix">Radix of the numbers in source</param>
   ''' <param name="decryptionKey">Decryption key</param>
   ''' <param name="tweak">Tweak</param>
   ''' <returns>Decrypted data</returns>
   Public Shared Function decrypt(ByRef source() As UShort, ByRef radix As UInteger, ByRef decryptionKey() As Byte, ByRef tweak() As Byte) As UShort()
      Dim sourceLength As UInteger
      Dim tweakLength As UInteger

      Dim leftLength As UInteger
      Dim rightSize As UInteger

      Dim radixToTheLeftSize As Numerics.BigInteger
      Dim radixToTheRightSize As Numerics.BigInteger

      Dim leftPart() As UShort
      Dim rightPart() As UShort

      Dim p() As Byte
      Dim q() As Byte

      Dim maxPartNumberByteLength As UInteger
      Dim feistelRoundOutputLength As UInteger
      Dim blockCount As UInteger

      Dim aesCipher As Security.Cryptography.AesManaged
      Dim encryptor As Security.Cryptography.ICryptoTransform

#Disable Warning BC42030
#Disable Warning BC42108
      setUpMachinery(source, radix, decryptionKey, tweak, sourceLength, tweakLength, leftLength, rightSize, radixToTheLeftSize, radixToTheRightSize, leftPart, rightPart, maxPartNumberByteLength, feistelRoundOutputLength, blockCount, aesCipher, encryptor, p, q)
#Enable Warning BC42030
#Enable Warning BC42108

      Dim s(0 To blockCount * 16 - 1) As Byte

      Dim moduloValue As Numerics.BigInteger

      Dim y As Numerics.BigInteger

      Dim c As Numerics.BigInteger

      Dim newPartSize As UInteger

      Dim roundNumber As Byte = 10

      Do While roundNumber > 0
         roundNumber -= 1

         y = calculateY(p, q, s, roundNumber, radix, leftPart, maxPartNumberByteLength, blockCount, feistelRoundOutputLength, encryptor)

         If (roundNumber And 1) <> 0 Then
            moduloValue = radixToTheRightSize
            newPartSize = rightSize
         Else
            moduloValue = radixToTheLeftSize
            newPartSize = leftLength
         End If

         c = (rightPart.ToBigIntegerWithRadix(radix) - y).CorrectModulo(moduloValue)  ' This has to deal with negative values, so my own "mod" implementation is used

         rightPart = leftPart

         leftPart = c.ToBigEndianUShortArrayForRadix(radix, newPartSize)
      Loop

      Return leftPart.Concatenate(rightPart)
   End Function
#End Region
End Class
