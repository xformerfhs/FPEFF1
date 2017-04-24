Imports System.Text
Imports fpeff1
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class ExceptionTests

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub RadixTooSmall()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 1

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub SourceTooSmallForRadix()
      Dim sourceText() As UShort = {0, 1, 2, 3}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 2

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub RadixTooLarge()
      Dim sourceText() As UShort = {0, 1, 2, 3}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C,
                           &HEF, &H43, &H59, &HD8, &HD5, &H80, &HAA, &H4F, &H7F, &H3, &H6D, &H6F, &H4, &HFC, &H6A, &H94}
      Dim radix As UInteger = 2 ^ 27

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub InvalidKey()
      Dim sourceText() As UShort = {0, 1, 2, 3}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub SourceIsNull()
      Dim sourceText() As UShort
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28}
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub KeyIsNull()
      Dim sourceText() As UShort = {0, 1, 2, 3}
      Dim tweak() As Byte = {&H37, &H37, &H37, &H37, &H70, &H71, &H72, &H73, &H37, &H37, &H37}
      Dim key() As Byte
      Dim radix As UInteger = 10

      Dim expectedEncryption() As UShort = {33, 28, 8, 10, 0, 10, 35, 17, 2, 10, 31, 34, 10, 21, 34, 35, 30, 32, 13}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub SourceDoesNotConformToBase()
      Dim sourceText() As UShort = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
      Dim tweak() As Byte = {&H39, &H38, &H37, &H36, &H35, &H34, &H33, &H32, &H31, &H30}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C}
      Dim radix As UInteger = 4

      Dim expectedEncryption() As UShort = {6, 1, 2, 4, 2, 0, 0, 7, 7, 3}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

   <TestMethod()>
   <ExpectedException(GetType(ArgumentException))>
   Public Sub SourceTooShort()
      Dim sourceText() As UShort = {0, 1}
      Dim tweak() As Byte = {&H39, &H38, &H37, &H36, &H35, &H34, &H33, &H32, &H31, &H30}
      Dim key() As Byte = {&H2B, &H7E, &H15, &H16, &H28, &HAE, &HD2, &HA6, &HAB, &HF7, &H15, &H88, &H9, &HCF, &H4F, &H3C}
      Dim radix As UInteger = 2

      Dim expectedEncryption() As UShort = {6, 1, 2, 4, 2, 0, 0, 7, 7, 3}

      Dim encryptedData() As UShort = FF1.encrypt(sourceText, radix, key, tweak)
   End Sub

End Class