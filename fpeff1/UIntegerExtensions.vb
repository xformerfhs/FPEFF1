Imports System.Runtime.CompilerServices

''' <summary>
''' Methods missing from the standard <c>UInteger</c> implementation
''' </summary>
Module UIntegerExtensions
   ''' <summary>
   ''' Gets bytes of the <c>UInteger</c> in Big Endian order
   ''' </summary>
   ''' <param name="fromInteger">Integer to be converted</param>
   ''' <returns>Bytes of <paramref name="fromInteger"/> in Big Endian order (2 Bytes)</returns>
   <Extension>
   Public Function GetBigEndianBytes(ByRef fromInteger As UInteger) As Byte()
      Dim result(0 To 3) As Byte
      Dim tempArray() As Byte

      tempArray = BitConverter.GetBytes(fromInteger)

      result(0) = tempArray(3)
      result(1) = tempArray(2)
      result(2) = tempArray(1)
      result(3) = tempArray(0)

      Return result
   End Function

   ''' <summary>
   ''' Gets bytes of the <c>UInteger</c> in Big Endian order
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
   Public Function GetBigEndianBytes(ByRef fromInteger As UInteger, ByVal resultLength As UInteger) As Byte()
      Dim result(0 To resultLength - 1) As Byte
      Dim tempArray() As Byte

      tempArray = BitConverter.GetBytes(fromInteger)

      Dim copyLength As UInteger = Math.Min(4, resultLength)
      Dim tempIndex As UInteger = copyLength

      Dim i As UInteger = Math.Max(0, resultLength - copyLength)

      Do While i < resultLength
         tempIndex -= 1

         result(i) = tempArray(tempIndex)

         i += 1
      Loop

      Return result
   End Function
End Module
