protoc.exe -I=./ --csharp_out=./ ./Protocol.proto
PAUSE
REM START ../../PacketGenerator/bin/Debug/net6.0/PacketGenerator.exe ../../PacketGenerator/PDL.xml
REM XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
REM XCOPY /Y GenPackets.cs "../../Server/Packet"
REM XCOPY /Y ClientPacketManager.cs "../../DummyClient/Packet"
REM XCOPY /Y ServerPacketManager.cs "../../Server/Packet"