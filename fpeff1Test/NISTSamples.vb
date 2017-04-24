Imports System.Text
Imports fpeff1
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Tests from NIST samples at http://csrc.nist.gov/groups/ST/toolkit/documents/Examples/FF1samples.pdf
''' </summary>
<TestClass()> Public Class NISTSamples

   <TestMethod()> Public Sub NISTSample1()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {2, 4, 3, 3, 4, 7, 7, 4, 8, 4}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample2()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {&H39, &H38, &H37, &H36, &H35, &H34, &H33, &H32, &H31, &H30}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {6, 1, 2, 4, 2, 0, 0, 7, 7, 3}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample3()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C}
      Dim radix As UInteger = 36

      Dim expectedEncryption() As UShort = {10, 9, 29, 31, 4, 0, 22, 21, 21, 9, 20, 13, 30, 5, 0, 9, 14, 30, 22}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample4()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {2, 8, 3, 0, 6, 6, 8, 1, 3, 2}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample5()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {&H39, &H38, &H37, &H36, &H35, &H34, &H33, &H32, &H31, &H30}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {2, 4, 9, 6, 6, 5, 5, 5, 4, 9}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample6()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F}
      Dim radix As UInteger = 36

      Dim expectedEncryption() As UShort = {33, 11, 19, 3, 20, 31, 3, 5, 19, 27, 10, 32, 33, 31, 3, 2, 34, 28, 27}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample7()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {6, 6, 5, 7, 6, 6, 7, 0, 0, 9}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample8()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {&H39, &H38, &H37, &H36, &H35, &H34, &H33, &H32, &H31, &H30}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {1, 0, 0, 1, 6, 2, 3, 4, 6, 3}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

   <TestMethod()> Public Sub NISTSample9()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 36

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)

      CollectionAssert.AreEqual(encryptedData, expectedEncryption)

      Dim decryptedData() As UShort = FF1.decrypt(encryptedData, radix, key, tweak)

      CollectionAssert.AreEqual(decryptedData, sourceText)
   End Sub

End Class