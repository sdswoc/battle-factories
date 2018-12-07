using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Packet 
{
	public List<byte> data;

	public Packet()
	{
		data = new List<byte>();
	}

	public Packet(byte[] array)
	{
		data = new List<byte>();
		for (int i = 0;i < array.Length;i++)
		{
			data.Add(array[i]);
		}
	}

	public Packet(byte b)
	{
		data = new List<byte>();
		WriteByte(b);
	}

	public byte ReadByte()
	{
		if (data.Count > 0)
		{
			byte b = data[0];
			data.RemoveAt(0);
			return b;
		}
		else
		{
			return 0;
		}
	}

	public void WriteByte(byte b)
	{
		data.Add(b);
	}

	public int ReadInt()
	{
		int result = 0;
		if (data.Count > 3)
		{
			for (int i = 0;i < 4;i++)
			{
				result |= ReadByte() << (8 * i);
			}
		}
		return result;
	}

	public void WriteInt(int i)
	{
		for (int j = 0;j < 4;j++)
		{
			WriteByte((byte)(i>>(8*j)));
		}
	}

	public void WriteString(string s)
	{
		WriteInt(s.Length);
		for (int i = 0;i < s.Length;i++)
		{
			WriteByte((byte)s[i]);
		}
	}

	public string ReadString()
	{
		int length = ReadInt();
		if (data.Count >= length)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				sb.Append((char)ReadByte());
			}
			return sb.ToString();
		}
		return "";
	}

	public void Flush()
	{
		data.Clear();
	}

}
