using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{
    S2C_BroadcastEnterGame = 1,
	C2S_LeaveGame = 2,
	S2C_BroadcastLeaveGame = 3,
	S2C_PlayerList = 4,
	C2S_Move = 5,
	S2C_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S2C_BroadcastEnterGame : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.S2C_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S2C_BroadcastEnterGame);
        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(int);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
		count += sizeof(float);
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C2S_LeaveGame : IPacket
{
    

    public ushort Protocol { get { return (ushort)PacketID.C2S_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C2S_LeaveGame);
        count += sizeof(ushort);
        
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S2C_BroadcastLeaveGame : IPacket
{
    public int playerId;

    public ushort Protocol { get { return (ushort)PacketID.S2C_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S2C_BroadcastLeaveGame);
        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(int);
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S2C_PlayerList : IPacket
{
    public class Player
	{
	    public bool isSelf;
		public int playerId;
		public float posX;
		public float posY;
		public float posZ;
	    public void Read(ReadOnlySpan<byte> s, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(s.Slice(count, s.Length - count));
			count += sizeof(bool);
			this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
			count += sizeof(int);
			this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
			count += sizeof(float);
	    }
	    public bool Write(Span<byte> s, ref ushort count)
	    {
	        bool bSuccess = true;
	        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.isSelf);
			count += sizeof(bool);
			bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
			count += sizeof(int);
			bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
			count += sizeof(float);
			bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
			count += sizeof(float);
			bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
			count += sizeof(float);
	        return bSuccess;
	    }
	}
	public List<Player> players = new List<Player>();

    public ushort Protocol { get { return (ushort)PacketID.S2C_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(s.Slice(count , s.Length - count));
		count += sizeof(ushort);
		            
		for(int i=0;i<playerLen;++i)
		{
		    Player player = new Player();
		    player.Read(s, ref count);
		    players.Add(player);
		}
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S2C_PlayerList);
        count += sizeof(ushort);
        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(s.Slice(count , s.Length - count));
		count += sizeof(ushort);
		            
		for(int i=0;i<playerLen;++i)
		{
		    Player player = new Player();
		    player.Read(s, ref count);
		    players.Add(player);
		}
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class C2S_Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.C2S_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.C2S_Move);
        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
		count += sizeof(float);
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}
public class S2C_BroadcastMove : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.S2C_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(s.Slice(count, s.Length - count));
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(s.Slice(count, s.Length - count));
		count += sizeof(float);
    }
    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool bSuccess = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.S2C_BroadcastMove);
        count += sizeof(ushort);
        bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
		count += sizeof(int);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posX);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posY);
		count += sizeof(float);
		bSuccess &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.posZ);
		count += sizeof(float);
        bSuccess &= BitConverter.TryWriteBytes(s, count);
        if (bSuccess == false)
            return null;
        return SendBufferHelper.Close(count);
    }
}